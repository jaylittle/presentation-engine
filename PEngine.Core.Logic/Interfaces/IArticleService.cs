using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IArticleService
  {
    bool UpsertArticle(ArticleModel article, ref List<string> errors);
  }
}