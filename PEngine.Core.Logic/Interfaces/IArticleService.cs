using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IArticleService
  {
    IEnumerable<ArticleModel> ListArticles(string category, bool isAdmin);
    ArticleModel GetArticleById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin);
    bool UpsertArticle(ArticleModel article, ref List<string> errors, bool importFlag = false);
  }
}