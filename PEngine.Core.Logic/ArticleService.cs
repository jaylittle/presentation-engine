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
    private IArticleDal _articleDal;
    public ArticleService(IArticleDal articleDal)
    {
      _articleDal = articleDal;
    }

    public bool UpsertArticle(ArticleModel article, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      if (article == null)
      {
        errors.Add("Article data must be provided");
        return false;
      }
      if (string.IsNullOrWhiteSpace(article.Name))
      {
        errors.Add("Article Title is a required field");
      }
      if (string.IsNullOrEmpty(article.Description))
      {
        errors.Add("Article Description is a required field");
      }
      if (string.IsNullOrEmpty(article.Category))
      {
        errors.Add("Article Category is a required field");
      }
      if (article.Sections == null || !article.Sections.Any())
      {
        errors.Add("Article requires at least one section");
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        ArticleModel existingArticle = article.Guid != Guid.Empty ? _articleDal.GetArticleById(article.Guid, null, null) : null;
        article.GenerateUniqueName();
        var existingSectionGuids = (article.Guid == Guid.Empty ? new List<Guid>() : _articleDal.ListArticleSections(article.Guid).Select(a => a.Guid).ToList());

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
          foreach (var section in article.Sections)
          {
            section.ArticleGuid = article.Guid;
            section.GenerateUniqueName();
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