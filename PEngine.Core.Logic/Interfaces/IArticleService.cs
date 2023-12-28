using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IArticleService
  {
    Task<IEnumerable<ArticleModel>> ListArticles(string category, bool isAdmin, bool isLockedDown);
    Task<IEnumerable<ArticleModel>> ListArticlesWithSections(string category, bool isAdmin, bool isLockedDown);
    Task<(IEnumerable<ArticleModel> exact, IEnumerable<ArticleModel> fuzzy)> SearchArticles(string searchQuery, string[] searchTerms, bool isAdmin, bool isLockedDown);
    Task<ArticleModel> GetArticleById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin, bool isLockedDown);
    Task<OpResult> UpsertArticle(ArticleModel article, bool importFlag = false);
  }
}