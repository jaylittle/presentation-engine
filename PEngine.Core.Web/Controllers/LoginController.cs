using System;
using System.Collections.Generic;
using System.Dynamic;
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
using PEngine.Core.Web.Constraints;
using PEngine.Core.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Encodings.Web;

namespace PEngine.Core.Web.Controllers
{
  [Security.XSRF.XSRFCheck]
  [Route("log")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class LoginController : Controller
  {
    private IServiceProvider _svp;
    private IHttpContextAccessor _httpAccessor;

    public LoginController(IServiceProvider svp,  IHttpContextAccessor httpAccessor)
    {
      _svp = svp;
      _httpAccessor = httpAccessor;
    }

    [HttpGet("in")]
    [HttpHead("in")]
    public IActionResult Index([FromQuery]bool authFailed = false, [FromQuery]string previousUrl = null)
    {
      return Index("PEngine", authFailed, previousUrl);
    }

    [HttpGet("in/{userType}")]
    [HttpHead("in/{userType}")]
    public IActionResult Index(string userType, [FromQuery]bool authFailed = false, [FromQuery]string previousUrl = null)
    {
      Middleware.TokenCookieMiddleware.RemoveJwtCookie(_httpAccessor.HttpContext);
      Middleware.TokenCookieMiddleware.RemoveXsrfCookie(_httpAccessor.HttpContext);

      string refererUrl = HttpContext.Request.GetTypedHeaders().Referer.ToString();

      //Nuke Invalid Previous URL values
      if (string.IsNullOrWhiteSpace(previousUrl)
        || !previousUrl.StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase))
      {
        previousUrl = null;
      }

      //If Valid Previous Url not provided, fill in referer (unless it is not valud)
      if (string.IsNullOrEmpty(previousUrl) && !string.IsNullOrWhiteSpace(refererUrl)
        && refererUrl.StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase))
      {
        previousUrl = refererUrl;
      }

      var model = new PEngineGenericRecordModel<PEngineLoginModel>(_svp, HttpContext, PEnginePage.Login, false);
      model.RecordData = new PEngineLoginModel();
      model.RecordData.AuthFailed = authFailed;
      switch (userType.ToLower().Trim())
      {
        case "pengine":
          model.RecordData.ActionUrl = $"{Settings.Current.BasePath}token/pengine";
          model.RecordData.SuccessUrl = previousUrl ?? Settings.Current.BasePath;
          model.RecordData.FailUrl = $"{Settings.Current.BasePath}log/in/pengine";
          if (!string.IsNullOrWhiteSpace(previousUrl))
          {
            model.RecordData.FailUrl += "?previousUrl=" + System.Net.WebUtility.UrlEncode(previousUrl);
          }
          break;
        case "forum":
          model.RecordData.ActionUrl = $"{Settings.Current.BasePath}token/forum";
          model.RecordData.SuccessUrl = previousUrl ?? $"{Settings.Current.BasePath}forum";
          model.RecordData.FailUrl = $"{Settings.Current.BasePath}log/in/forum";
          if (!string.IsNullOrWhiteSpace(previousUrl))
          {
            model.RecordData.FailUrl += "?previousUrl=" + System.Net.WebUtility.UrlEncode(previousUrl);
          }
          break;
      }
      return View(model);
    }

    [HttpGet("out")]
    [HttpHead("out")]
    public void Logout([FromQuery]string previousUrl = null)
    {
      Middleware.TokenCookieMiddleware.RemoveJwtCookie(_httpAccessor.HttpContext);
      Middleware.TokenCookieMiddleware.RemoveXsrfCookie(_httpAccessor.HttpContext);

      string refererUrl = HttpContext.Request.GetTypedHeaders().Referer.ToString();

      //Nuke Invalid Previous URL values
      if (string.IsNullOrWhiteSpace(previousUrl)
        || !previousUrl.StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase))
      {
        previousUrl = null;
      }

      //If Valid Previous Url not provided, fill in referer (unless it is not valud)
      if (string.IsNullOrEmpty(previousUrl) && !string.IsNullOrWhiteSpace(refererUrl)
        && refererUrl.StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase))
      {
        previousUrl = refererUrl;
      }

      this.Response.Redirect(previousUrl ?? Settings.Current.BasePath);
    }
  }
}