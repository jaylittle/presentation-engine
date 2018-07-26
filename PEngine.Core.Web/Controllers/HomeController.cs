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
  [Security.XSRF.XSRFCheck]
  [ResponseCache(CacheProfileName = "None")]
  public class HomeController : Controller
  {
    private IServiceProvider _svp;
    private IPostService _postService;
    private IQuoteService _quoteService;

    public HomeController(IServiceProvider svp, IPostService postService, IQuoteService quoteService)
    {
      _svp = svp;
      _postService = postService;
      _quoteService = quoteService;
    }

    public async Task<IActionResult> Index()
    {
      var model = new PEngineGenericListModel<PostModel>(_svp, HttpContext, true);
      model.ListData = PagingUtils.Paginate(1, model.Settings.PerPagePostFront, "CreatedUTC", false, await _postService.ListPosts(model.State.HasAdmin));
      model.State.QuoteText = (await _quoteService.GetRandom()).Data;
      return View(model);
    }
  }
}