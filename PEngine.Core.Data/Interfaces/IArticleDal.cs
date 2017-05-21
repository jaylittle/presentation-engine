using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IArticleDal
    {
      IEnumerable<ArticleModel> ListArticles();
      ArticleModel GetArticleById(Guid? guid, int? legacyId, string uniqueName);
      void InsertArticle(ArticleModel article);
      void UpdateArticle(ArticleModel article);
      void DeleteArticle(Guid guid);
      IEnumerable<ArticleSectionModel> ListArticleSections(Guid? articleGuid);
      void InsertArticleSection(ArticleSectionModel articleSection);
      void UpdateArticleSection(ArticleSectionModel articleSection);
      void DeleteArticleSection(Guid guid);
    }
}
