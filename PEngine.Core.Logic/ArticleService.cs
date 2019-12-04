using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Logic
{
  public class ArticleService : IArticleService
  {
    public const string ARTICLE_ERROR_DATA_MUST_BE_PROVIDED = "Article data must be provided";
    public const string ARTICLE_ERROR_INVALID_RECORD = "Article Guid refers to an invalid record";
    public const string ARTICLE_ERROR_TITLE_IS_REQUIRED = "Article Title is a required field";
    public const string ARTICLE_ERROR_DESCRIPTION_IS_REQUIRED = "Article Description is a required field";
    public const string ARTICLE_ERROR_CATEGORY_IS_REQUIRED = "Article Category is a required field";
    public const string ARTICLE_ERROR_DEFAULT_SECTION_INVALID = "Article Default Section refers to an invalid section";
    public const string ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED = "Article requires either a URL or section(s) to be provided";
    public const string SECTION_ERROR_NAME_IS_REQUIRED = "Article Section #{0}: Name must be provided";
    public const string SECTION_ERROR_CONTENT_IS_REQUIRED = "Article Section #{0}: Content must be provided";
    private IArticleDal _articleDal;
    
    public ArticleService(IArticleDal articleDal)
    {
      _articleDal = articleDal;
    }

    public async Task<IEnumerable<ArticleModel>> ListArticles(string category, bool isAdmin)
    {
      return (await _articleDal.ListArticles(category)).Where(a => isAdmin || (a.VisibleFlag && !a.NoIndexFlag));
    }

    public async Task<IEnumerable<ArticleModel>> ListArticlesWithSections(string category, bool isAdmin)
    {
      return (await _articleDal.ListArticlesWithSections(category)).Where(a => isAdmin || (a.VisibleFlag && !a.NoIndexFlag));
    }

    public async Task<IEnumerable<ArticleModel>> SearchArticles(string[] searchTerms, bool isAdmin)
    {
      var matchingArticles = await _articleDal.ListArticlesWithSections(null);
      return matchingArticles
        .Where(a => isAdmin || (a.VisibleFlag && !a.NoIndexFlag))
        .Where(a => searchTerms.All(st => 
          a.Category?.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0 ||
          a.Name?.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0 ||
          a.Description?.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0 ||
          a.ContentURL?.IndexOf(st, StringComparison.OrdinalIgnoreCase) >=0 ||
          a.Sections.Any(s =>
            s.Data.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0 ||
            s.Name.IndexOf(st, StringComparison.OrdinalIgnoreCase) >= 0
          )
        ));
    }

    public async Task<ArticleModel> GetArticleById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin)
    {
      var article = await _articleDal.GetArticleById(guid, legacyId, uniqueName);
      return (article == null || isAdmin || article.VisibleFlag) ? article : null;
    }

    public async Task<OpResult> UpsertArticle(ArticleModel article, bool importFlag = false)
    {
      var retvalue = new OpResult();
      ArticleModel existingArticle = null;

      if (article == null)
      {
        retvalue.LogError(ARTICLE_ERROR_DATA_MUST_BE_PROVIDED);
        return retvalue;
      }
      if (!importFlag && article.Guid != Guid.Empty)
      {
        existingArticle = await _articleDal.GetArticleById(article.Guid, null, null);
        if (existingArticle == null)
        {
          retvalue.LogError(ARTICLE_ERROR_INVALID_RECORD);
        }
        else
        {
          article.UniqueName = existingArticle.UniqueName;
          article.CreatedUTC = existingArticle.CreatedUTC;
          article.ModifiedUTC = existingArticle.ModifiedUTC;
        }
      }
      if (string.IsNullOrWhiteSpace(article.Name))
      {
        retvalue.LogError(ARTICLE_ERROR_TITLE_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(article.Description))
      {
        retvalue.LogError(ARTICLE_ERROR_DESCRIPTION_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(article.Category))
      {
        retvalue.LogError(ARTICLE_ERROR_CATEGORY_IS_REQUIRED);
      }
      if (!string.IsNullOrEmpty(article.DefaultSection) && article.Sections != null 
        && !article.Sections.Any(s => s.Name != null && s.Name.Equals(article.DefaultSection, StringComparison.OrdinalIgnoreCase)))
      {
        retvalue.LogError(ARTICLE_ERROR_DEFAULT_SECTION_INVALID);
      }
      if (article.Sections != null)
      {
        var counter = 0;
        foreach (var section in article.Sections)
        {
          counter++;
          if (string.IsNullOrEmpty(section.Name))
          {
            retvalue.LogError(string.Format(SECTION_ERROR_NAME_IS_REQUIRED, counter));
          }
          if (string.IsNullOrEmpty(section.Data))
          {
            retvalue.LogError(string.Format(SECTION_ERROR_CONTENT_IS_REQUIRED, counter));
          }
        }
      }
      if ((article.Sections == null || article.Sections.Count <= 0) && string.IsNullOrEmpty(article.ContentURL))
      {
        retvalue.LogError(ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED);
      }
      if (retvalue.Successful)
      {
        Dictionary<string, bool> existingUniqueNames = (await _articleDal.ListArticles(null))
          .ToDictionary(a => a.UniqueName, a => true, StringComparer.OrdinalIgnoreCase);
        article.GenerateUniqueName(existingUniqueNames);

        var existingSectionGuids = new List<Guid>();
        if (existingArticle != null && existingArticle.Sections != null)
        {
          existingSectionGuids = existingArticle.Sections.Select(s => s.Guid).ToList();
        }
        _articleDal.AddTransaction(DatabaseType.PEngine, Database.OpenTransaction(DatabaseType.PEngine, false));
        try
        {
          if (importFlag || article.Guid == Guid.Empty)
          {
            await _articleDal.InsertArticle(article, importFlag);
          }
          else
          {
            await _articleDal.UpdateArticle(article);
          }
          if (article.Sections != null)
          {
            var previousSectionUniqueNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var section in article.Sections)
            {
              if (!importFlag && section.Guid != Guid.Empty && !existingSectionGuids.Contains(section.Guid))
              {
                section.Guid = Guid.Empty;
              }
              section.ArticleGuid = article.Guid;
              section.GenerateUniqueName(previousSectionUniqueNames);
              previousSectionUniqueNames.Add(section.UniqueName, true);

              if (importFlag || section.Guid == Guid.Empty || !existingSectionGuids.Contains(section.Guid))
              {
                await _articleDal.InsertArticleSection(section, importFlag);
              }
              else
              {
                await _articleDal.UpdateArticleSection(section);
              }
              existingSectionGuids.Remove(section.Guid);
            }
          }
          foreach (var sectionGuidToDelete in existingSectionGuids)
          {
            await _articleDal.DeleteArticleSection(sectionGuidToDelete);
          }
          _articleDal.CommitTransaction(DatabaseType.PEngine);
        }
        catch (Exception ex)
        {
          _articleDal.RollBackTransaction(DatabaseType.PEngine);
          throw new Exception("Article Transaction Failed", ex);
        }
      }
      return retvalue;
    }
  }
}