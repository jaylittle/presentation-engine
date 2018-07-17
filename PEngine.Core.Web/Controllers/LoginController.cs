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
using Microsoft.AspNetCore.Http;

namespace PEngine.Core.Web.Controllers
{
  [Route("log")]
  [ResponseCache(CacheProfileName = "None")]
  public class LoginController : Controller
  {
    private IServiceProvider _svp;

    public LoginController(IServiceProvider svp)
    {
      _svp = svp;
    }

    [HttpGet("in")]
    public IActionResult Index([FromQuery]bool authFailed)
    {
      return Index("PEngine", authFailed);
    }

    [HttpGet("in/{userType}")]
    public IActionResult Index(string userType, [FromQuery]bool authFailed)
    {
      var model = new PEngineGenericRecordModel<PEngineLoginModel>(_svp, HttpContext, true);
      model.RecordData = new PEngineLoginModel();
      model.RecordData.AuthFailed = authFailed;
      switch (userType.ToLower().Trim())
      {
        case "pengine":
          model.RecordData.ActionUrl = $"{Settings.Current.BasePath}token/pengine";
          model.RecordData.SuccessUrl = Settings.Current.BasePath;
          model.RecordData.FailUrl = $"{Settings.Current.BasePath}log/in/pengine";
          break;
        case "forum":
          model.RecordData.ActionUrl = $"{Settings.Current.BasePath}token/forum";
          model.RecordData.SuccessUrl = $"{Settings.Current.BasePath}forum";
          model.RecordData.FailUrl = $"{Settings.Current.BasePath}log/in/forum";
          break;
      }
      return View(model);
    }

    [HttpGet("out")]
    public void Logout()
    {
      var cookieOptions = new CookieOptions();
      if (!string.IsNullOrWhiteSpace(Settings.Current.CookieDomain))
      {
        cookieOptions.Domain = Settings.Current.CookieDomain;
      }
      if (!string.IsNullOrWhiteSpace(Settings.Current.CookiePath))
      {
        cookieOptions.Path = Settings.Current.CookiePath;
      }
      this.Response.Cookies.Delete(Models.PEngineStateModel.COOKIE_ACCESS_TOKEN, cookieOptions);
      this.Response.Redirect(Settings.Current.BasePath);
    }
  }
}