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
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Headers;

namespace PEngine.Core.Web.Controllers
{
  [Route("ui")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class UIController : Controller
  {
    private IServiceProvider _svp;
    
    public UIController(IServiceProvider svp)
    {
      _svp = svp;
    }

    [HttpGet("theme")]
    public IActionResult ChangeTheme([FromQuery]string selection)
    {
      var state = new PEngineStateModel(_svp, Settings.Current, HttpContext, false, false);
      if (!Settings.Current.DisableThemeSelection)
      {
        if (state.ThemeList.Any(t => t.Equals(selection, StringComparison.OrdinalIgnoreCase)))
        {
          state.ThemeChange(selection);
        }
        var redirectUrl = PEngine.Core.Shared.Settings.Current.BasePath;
        if (!string.IsNullOrWhiteSpace(Request.Headers[HeaderNames.Referer]))
        {
          redirectUrl = Request.Headers[HeaderNames.Referer];
        }
        return this.Redirect(redirectUrl);
      }
      else
      {
        return this.NotFound();
      }
    }
  }
}