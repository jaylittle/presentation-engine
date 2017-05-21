using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
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
        article.GenerateUniqueName();
        var existingSectionGuids = new List<Guid>();
        if (article.Guid == Guid.Empty)
        {
          //TODO Obviously this field should not store plaintext password data
          //This is just a placeholder until I get off my ass and research .NET
          //Core Cryptography calls
          article.AdminPass = article.AdminPass ?? string.Empty;
          _articleDal.InsertArticle(article);
        }
        else
        {
          //TODO Obviously this field should not store plaintext password data
          //This is just a placeholder until I get off my ass and research .NET
          //Core Cryptography calls
          article.AdminPass = article.AdminPass ?? string.Empty;
          _articleDal.UpdateArticle(article);
          existingSectionGuids = _articleDal.ListArticleSections(article.Guid).Select(a => a.Guid).ToList();
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
      }
      return retvalue;
    }
  }
}