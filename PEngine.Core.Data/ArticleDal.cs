using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class ArticleDal : BaseDal<ArticleDal>, IArticleDal
  {
    public async Task<IEnumerable<ArticleModel>> ListArticles(string category)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ArticleModel>(ReadQuery("ListArticles", ct.ProviderName), new { category }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ArticleModel>> ListArticlesWithSections(string category)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        var output = new ConcurrentDictionary<Guid, ArticleModel>();
        await ct.DbConnection.QueryAsync<ArticleModel, ArticleSectionModel, ArticleModel>(ReadQuery("ListArticlesWithSections", ct.ProviderName), (article, section) => {
          if (!output.ContainsKey(article.Guid))
          {
            while (!output.ContainsKey(article.Guid) && !output.TryAdd(article.Guid, article));
          }
          output[article.Guid].Sections.Add(section);
          return article;
        }, new {
          category
        }, splitOn: "Guid", transaction: ct.DbTransaction);
        foreach (var kv in output)
        {
          kv.Value.Sections = kv.Value.Sections.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToList();
        }
        return output.Select(kv => kv.Value);
      }
    }

    public async Task<ArticleModel> GetArticleById(Guid? guid, int? legacyId, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        var output = new ConcurrentDictionary<Guid, ArticleModel>();
        await ct.DbConnection.QueryAsync<ArticleModel, ArticleSectionModel, ArticleModel>(ReadQuery("GetArticleById", ct.ProviderName), (article, section) => {
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
        }, splitOn: "Guid", transaction: ct.DbTransaction);
        foreach (var kv in output)
        {
          kv.Value.Sections = kv.Value.Sections.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToList();
        }
        return output.Select(kv => kv.Value).FirstOrDefault();
      }
    }

    public async Task InsertArticle(ArticleModel article, bool importFlag = false)
    {
      article.UpdateGuid();
      article.UpdateTimestamps(true, importFlag);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertArticle", ct.ProviderName), article, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateArticle(ArticleModel article)
    {
      article.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateArticle", ct.ProviderName), article, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteArticle(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteArticle", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ArticleSectionModel>> ListArticleSections(Guid? articleGuid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<ArticleSectionModel>(ReadQuery("ListArticleSections", ct.ProviderName), new {
          articleGuid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertArticleSection(ArticleSectionModel articleSection, bool importFlag = false)
    {
      articleSection.UpdateGuid();
      articleSection.UpdateTimestamps(true, importFlag);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertArticleSection", ct.ProviderName), articleSection, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateArticleSection(ArticleSectionModel articleSection)
    {
      articleSection.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateArticleSection", ct.ProviderName), articleSection, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteArticleSection(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteArticleSection", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteAllArticles()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteAllArticles", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
