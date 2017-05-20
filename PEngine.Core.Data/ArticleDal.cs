using System;
using System.Collections.Generic;
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
  }
}
