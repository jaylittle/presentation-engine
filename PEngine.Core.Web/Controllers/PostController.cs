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
  [Route("post")]
  [ResponseCache(CacheProfileName = "None")]
  public class PostController : Controller
  {
    private IServiceProvider _svp;
    private IPostDal _postDal;
    private IPostService _postService;

    public PostController(IServiceProvider svp, IPostDal postDal, IPostService postService)
    {
      _svp = svp;
      _postDal = postDal;
      _postService = postService;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List([FromQuery]PagingModel paging = null)
    {
      var model = new PEngineGenericListModel<PostModel>(_svp, HttpContext, false);
      if (paging != null)
      {
        paging.Count = paging.Count > 0 ? paging.Count : model.Settings.PerPagePostArchived;
        if (string.IsNullOrEmpty(paging.SortField))
        {
          paging.SortField = "CreatedUTC";
          paging.SortAscending = false;
        }
      }
      model.ListData = PagingUtils.Paginate<PostModel>(ref paging, await _postService.ListPosts(model.State.HasAdmin));
      model.Paging = paging;
      return View(model);
    }

    [HttpGet("view/{year}/{month}/{uniqueName}")]
    public async Task<IActionResult> View(int year, int month, string uniqueName)
    {
      var model = new PEngineGenericRecordModel<PostModel>(_svp, HttpContext, false);
      model.RecordData = (await _postService.ListPosts(model.State.HasAdmin)).FirstOrDefault(p => p.CreatedUTC.HasValue && p.CreatedYear == year && p.CreatedMonth == month && p.UniqueName.Equals(uniqueName, StringComparison.OrdinalIgnoreCase));
      return View(model);
    }
  }
}