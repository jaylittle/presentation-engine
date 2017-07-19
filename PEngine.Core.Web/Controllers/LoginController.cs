using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting.Internal;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Constraints;
using PEngine.Core.Web.Models;

namespace PEngine.Core.Web.Controllers
{
  [Route("log")]
  public class LoginController : Controller
  {
    public LoginController()
    {
    }

    [HttpGet("in")]
    public IActionResult Index()
    {
      return Index("PEngine");
    }

    [HttpGet("in/{userType}")]
    public IActionResult Index(string userType)
    {
      var model = new PEngineGenericRecordModel<PEngineLoginModel>(HttpContext, true);
      model.RecordData = new PEngineLoginModel();
      switch (userType.ToLower().Trim())
      {
        case "pengine":
          model.RecordData.ActionUrl = "/token/pengine";
          model.RecordData.RedirectUrl = "/";
          break;
        case "forum":
          model.RecordData.ActionUrl = "/token/forum";
          model.RecordData.RedirectUrl = "/forum";
          break;
      }
      return View(model);
    }

    [HttpGet("out")]
    public void Logout()
    {
      this.Response.Cookies.Delete(Models.PEngineStateModel.COOKIE_ACCESS_TOKEN);
      this.Response.Redirect("/");
    }
  }
}