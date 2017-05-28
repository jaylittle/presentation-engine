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

    private IArticleDal _articleDal;
    
    public ArticleService(IArticleDal articleDal)
    {
      _articleDal = articleDal;
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
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        Dictionary<string, bool> existingUniqueNames = _articleDal.ListArticles()
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
            article.AdminPass = Security.Encrypt(article.NewAdminPass.Value ?? string.Empty);
            _articleDal.InsertArticle(article);
          }
          else
          {
            if (article.NewAdminPass.Reset)
            {
              article.AdminPass = Security.Encrypt(string.Empty);
            }
            else if (!string.IsNullOrEmpty(article.NewAdminPass.Value))
            {
              article.AdminPass = Security.Encrypt(article.NewAdminPass.Value);
            }
            else
            {
              article.AdminPass = existingArticle?.AdminPass ?? string.Empty;
            }
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