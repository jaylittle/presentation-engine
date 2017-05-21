using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class ArticleDal : BaseDal<ArticleDal>, IArticleDal
  {
    public IEnumerable<ArticleModel> ListArticles()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ArticleModel>(ReadQuery("ListArticles", ct.ProviderName));
      }
    }

    public ArticleModel GetArticleById(Guid? guid, int? legacyId, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        var output = new ConcurrentDictionary<Guid, ArticleModel>();
        ct.DbConnection.Query<ArticleModel, ArticleSectionModel, ArticleModel>(ReadQuery("GetArticleById", ct.ProviderName), (article, section) => {
          if (!output.ContainsKey(article.Guid))
          {
            while (!output.ContainsKey(article.Guid) && !output.TryAdd(article.Guid, article));
          }
          output[article.Guid].Sections.Add(section);
          return article;
        }, new {
          guid,
          legacyId,
          uniqueName
        }, splitOn: "Guid");
        foreach (var kv in output)
        {
          kv.Value.Sections = kv.Value.Sections.OrderBy(s => s.SortOrder).ToList();
        }
        return output.Select(kv => kv.Value).FirstOrDefault();
      }
    }

    public void InsertArticle(ArticleModel article)
    {
      article.UpdateGuid();
      article.UpdateTimestamps(true);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("InsertArticle", ct.ProviderName), article);
      }
    }

    public void UpdateArticle(ArticleModel article)
    {
      article.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateArticle", ct.ProviderName), article);
      }
    }

    public void DeleteArticle(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteArticle", ct.ProviderName), new {
          guid
        });
      }
    }

    public IEnumerable<ArticleSectionModel> ListArticleSections(Guid? articleGuid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return ct.DbConnection.Query<ArticleSectionModel>(ReadQuery("ListArticleSections", ct.ProviderName), new {
          articleGuid
        });
      }
    }

    public void InsertArticleSection(ArticleSectionModel articleSection)
    {
      articleSection.UpdateGuid();
      articleSection.UpdateTimestamps(true);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("InsertArticleSection", ct.ProviderName), articleSection);
      }
    }

    public void UpdateArticleSection(ArticleSectionModel articleSection)
    {
      articleSection.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateArticleSection", ct.ProviderName), articleSection);
      }
    }

    public void DeleteArticleSection(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        ct.DbConnection.Execute(ReadQuery("DeleteArticleSection", ct.ProviderName), new {
          guid
        });
      }
    }
  }
}
