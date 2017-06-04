using System;
using System.Collections.Generic;
using System.Linq;
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
  public class HomeController : Controller
  {
    private IPostDal _postDal;
    private IPostService _postService;

    public HomeController(IPostDal postDal, IPostService postService)
    {
      _postDal = postDal;
      _postService = postService;
    }

    public IActionResult Index()
    {
      var model = new PEngineGenericList<PostModel>()
      {
        Data = _postService.ListPosts(Request.HttpContext.User.IsInRole("PEngineAdmin")),
        Settings = Settings.Current
      };
      return View(model);
    }
  }
}