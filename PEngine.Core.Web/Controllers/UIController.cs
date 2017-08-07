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
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Headers;

namespace PEngine.Core.Web.Controllers
{
  [Route("ui")]
  public class UIController : Controller
  {
    public UIController()
    {
    }

    [HttpGet("theme")]
    public IActionResult ChangeTheme([FromQuery]string selection)
    {
      var state = new PEngineStateModel(Settings.Current, HttpContext, false, false);
      if (state.ThemeList.Any(t => t.Equals(selection, StringComparison.OrdinalIgnoreCase)))
      {
        state.ThemeChange(selection);
      }
      var redirectUrl = "/";
      if (!string.IsNullOrWhiteSpace(Request.Headers[HeaderNames.Referer]))
      {
        if (!string.IsNullOrWhiteSpace(Settings.Current.ExternalBaseUrl) 
          && Request.Headers[HeaderNames.Referer].ToString().StartsWith(Settings.Current.ExternalBaseUrl, StringComparison.OrdinalIgnoreCase)
          && Request.Headers[HeaderNames.Referer].ToString().Length > Settings.Current.ExternalBaseUrl.Length)
        {
          redirectUrl = Request.Headers[HeaderNames.Referer];
          if (Settings.Current.ExternalBaseUrl.EndsWith("/"))
          {
            redirectUrl = redirectUrl.Substring(Settings.Current.ExternalBaseUrl.Length - 1);
          }
          else
          {
            redirectUrl = redirectUrl.Substring(Settings.Current.ExternalBaseUrl.Length);
          }
        }
      }
      return this.Redirect(redirectUrl);
    }
  }
}