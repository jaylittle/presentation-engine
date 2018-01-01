using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Models;

namespace PEngine.Core.Web.Controllers
{
  [Route("search")]
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class SearchController : Controller
  {
    private IForumService _forumService;
    private IPostService _postService;
    private IArticleService _articleService;

    public SearchController(IForumService forumService, IPostService postService, IArticleService articleService)
    {
      _forumService = forumService;
      _postService = postService;
      _articleService = articleService;
    }

    public async Task<IActionResult> Index([FromQuery]string query, [FromQuery]PagingModel paging = null)
    {
      var model = new PEngineGenericListModel<PEngineSearchResultModel>(HttpContext, false);
      model.State.CurrentSection = query;
      
      var results = new List<PEngineSearchResultModel>();
      string[] searchTerms = !string.IsNullOrWhiteSpace(query) ? query.Split(' ') : new string[] {};
      
      results.AddRange((await _articleService.SearchArticles(searchTerms, model.State.HasAdmin))
        .Select(a => new PEngineSearchResultModel(a)));

      results.AddRange((await _postService.SearchPosts(searchTerms, model.State.HasAdmin))
        .Select(p => new PEngineSearchResultModel(p)));
      
      if (!Settings.Current.DisableForum)
      {
        results.AddRange((await _forumService.SearchForumThreadPosts(searchTerms, model.State.HasForumAdmin))
          .Select(ftp => new PEngineSearchResultModel(ftp)));
      }

      if (paging != null)
      {
        paging.Count = paging.Count > 0 ? paging.Count : model.Settings.PerPageSearchResults;
        if (string.IsNullOrEmpty(paging.SortField))
        {
          paging.SortField = "CreatedUTC";
          paging.SortAscending = false;
        }
      }
      model.ListData = PagingUtils.Paginate<PEngineSearchResultModel>(ref paging, results);
      model.Paging = paging;
      return View(model);
    }
  }
}