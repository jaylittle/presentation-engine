using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared;
using PEngine.Core.Web.Helpers;

namespace PEngine.Core.Web.Controllers
{
  [Route("")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class RedirectController : Controller
  {
    private IServiceProvider _svp;

    public RedirectController(IServiceProvider svp)
    {
      _svp = svp;
    }

    [HttpGet("favicon.ico")]
    public IActionResult FavIcon()
    {
      if (!string.IsNullOrWhiteSpace(Settings.Current.FavIcon))
      {
        var filePath = System.Net.WebUtility.UrlDecode(Rendering.FavIconPath);
        if (!string.IsNullOrWhiteSpace(filePath))
        {
          var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", filePath, Helpers.Html.GetAbsoluteHashPath, true).Result;
          if (hashEntry != null)
          {
            return RedirectPermanent(Html.GetAbsoluteHashPath(hashEntry.Hash, hashEntry.WebPath));
          }
        }
      }
      return NotFound();
    }

    [HttpGet("apple-touch-icon.png")]
    public IActionResult AppleTouchIcon()
    {
      return FavIcon();
    }
  }
}