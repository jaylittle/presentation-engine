using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IArticleDal : IBaseDal
    {
      Task<IEnumerable<ArticleModel>> ListArticles(string category);
      Task<ArticleModel> GetArticleById(Guid? guid, int? legacyId, string uniqueName);
      Task InsertArticle(ArticleModel article, bool importFlag = false);
      Task UpdateArticle(ArticleModel article);
      Task DeleteArticle(Guid guid);
      Task<IEnumerable<ArticleSectionModel>> ListArticleSections(Guid? articleGuid);
      Task InsertArticleSection(ArticleSectionModel articleSection, bool importFlag = false);
      Task UpdateArticleSection(ArticleSectionModel articleSection);
      Task DeleteArticleSection(Guid guid);
      Task DeleteAllArticles();
    }
}
