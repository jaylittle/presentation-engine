using System;
using System.Linq;
using System.Collections.Generic;
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

    public IEnumerable<ArticleModel> ListArticles(string category, bool isAdmin)
    {
      return _articleDal.ListArticles(category).Where(a => isAdmin || a.VisibleFlag);
    }

    public ArticleModel GetArticleById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin)
    {
      var article = _articleDal.GetArticleById(guid, legacyId, uniqueName);
      return (article == null || isAdmin || article.VisibleFlag) ? article : null;
    }

    public bool UpsertArticle(ArticleModel article, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ArticleModel existingArticle = null;

      if (article == null)
      {
        errors.Add(ARTICLE_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (article.Guid != Guid.Empty)
      {
        existingArticle = _articleDal.GetArticleById(article.Guid, null, null);
        if (existingArticle == null)
        {
          errors.Add(ARTICLE_ERROR_INVALID_RECORD);
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
        errors.Add(ARTICLE_ERROR_TITLE_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(article.Description))
      {
        errors.Add(ARTICLE_ERROR_DESCRIPTION_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(article.Category))
      {
        errors.Add(ARTICLE_ERROR_CATEGORY_IS_REQUIRED);
      }
      if (!string.IsNullOrEmpty(article.DefaultSection) && article.Sections != null 
        && !article.Sections.Any(s => s.Name != null && s.Name.Equals(article.DefaultSection, StringComparison.OrdinalIgnoreCase)))
      {
        errors.Add(ARTICLE_ERROR_DEFAULT_SECTION_INVALID);
      }
      if (article.Sections != null)
      {
        var counter = 0;
        foreach (var section in article.Sections)
        {
          counter++;
          if (string.IsNullOrEmpty(section.Name))
          {
            errors.Add(string.Format(SECTION_ERROR_NAME_IS_REQUIRED, counter));
          }
          if (string.IsNullOrEmpty(section.Data))
          {
            errors.Add(string.Format(SECTION_ERROR_CONTENT_IS_REQUIRED, counter));
          }
        }
      }
      if ((article.Sections == null || article.Sections.Count <= 0) && string.IsNullOrEmpty(article.ContentURL))
      {
        errors.Add(ARTICLE_ERROR_EITHER_URL_OR_SECTION_REQUIRED);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        Dictionary<string, bool> existingUniqueNames = _articleDal.ListArticles(null)
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
          if (article.Guid == Guid.Empty)
          {
            _articleDal.InsertArticle(article);
          }
          else
          {
            _articleDal.UpdateArticle(article);
          }
          if (article.Sections != null)
          {
            var previousSectionUniqueNames = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
            foreach (var section in article.Sections)
            {
              if (section.Guid != Guid.Empty && !existingSectionGuids.Contains(section.Guid))
              {
                section.Guid = Guid.Empty;
              }
              section.ArticleGuid = article.Guid;
              section.GenerateUniqueName(previousSectionUniqueNames);
              previousSectionUniqueNames.Add(section.UniqueName, true);

              if (section.Guid == Guid.Empty || !existingSectionGuids.Contains(section.Guid))
              {
                _articleDal.InsertArticleSection(section);
              }
              else
              {
                _articleDal.UpdateArticleSection(section);
              }
              existingSectionGuids.Remove(section.Guid);
            }
          }
          foreach (var sectionGuidToDelete in existingSectionGuids)
          {
            _articleDal.DeleteArticleSection(sectionGuidToDelete);
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