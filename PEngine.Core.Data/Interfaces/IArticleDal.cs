using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IArticleDal : IBaseDal
    {
      IEnumerable<ArticleModel> ListArticles(string category);
      ArticleModel GetArticleById(Guid? guid, int? legacyId, string uniqueName);
      void InsertArticle(ArticleModel article, bool importFlag = false);
      void UpdateArticle(ArticleModel article);
      void DeleteArticle(Guid guid);
      IEnumerable<ArticleSectionModel> ListArticleSections(Guid? articleGuid);
      void InsertArticleSection(ArticleSectionModel articleSection, bool importFlag = false);
      void UpdateArticleSection(ArticleSectionModel articleSection);
      void DeleteArticleSection(Guid guid);
      void DeleteAllArticles();
    }
}
