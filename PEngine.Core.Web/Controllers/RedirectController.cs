using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared;
using PEngine.Core.Web.Helpers;
using PEngine.Core.Shared.Models;

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
    [HttpHead("favicon.ico")]
    [HttpGet("favico.ico")]
    [HttpHead("favico.ico")]
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
    [HttpHead("apple-touch-icon.png")]
    [HttpGet("apple-touch-icon-precomposed.png")]
    [HttpHead("apple-touch-icon-precomposed.png")]
    public IActionResult AppleTouchIcon()
    {
      return FavIcon();
    }

    [HttpGet("wp-login.php")]
    [HttpHead("wp-login.php")]
    public IActionResult WrongCMS()
    {
      return Redirect($"{Settings.Current.BasePath}hack/detected/"
        + HackAttemptType.WrongCMS.ToString());
    }

    [HttpGet("xmlrpc.php")]
    [HttpHead("xmlrpc.php")]
    [HttpGet("admin.php")]
    [HttpHead("admin.php")]
    [HttpGet("global.asa")]
    [HttpHead("global.asa")]
    [HttpGet("global.asax")]
    [HttpHead("global.asax")]
    public IActionResult WrongTechStack()
    {
      return Redirect($"{Settings.Current.BasePath}hack/detected/"
        + HackAttemptType.WrongTechStack.ToString());
    }

    [HttpGet(".env")]
    [HttpHead(".env")]
    [HttpGet("passwd")]
    [HttpHead("passwd")]
    [HttpGet("shadow")]
    [HttpHead("shadow")]
    [HttpGet(".htaccess")]
    [HttpHead(".htaccess")]
    public IActionResult SensitiveFile()
    {
      return Redirect($"{Settings.Current.BasePath}hack/detected/"
        + HackAttemptType.SensitiveFile.ToString());
    }

    [HttpGet("/admin/{*filePath}")]
    [HttpHead("/admin/{*filePath}")]
    [HttpGet("/administrator/{*filePath}")]
    [HttpHead("/administrator/{*filePath}")]
    [HttpGet("/wp-admin/{*filePath}")]
    [HttpHead("/wp-admin/{*filePath}")]
    [HttpGet("/phpmyadmin/{*filePath}")]
    [HttpHead("/phpmyadmin/{*filePath}")]
    [HttpGet("/.git/{*filePath}")]
    [HttpHead("/.git/{*filePath}")]
    [HttpGet("/wp-includes/{*filePath}")]
    [HttpHead("/wp-includes/{*filePath}")]
    [HttpGet("/.vs-code/{*filePath}")]
    [HttpHead("/.vs-code/{*filePath}")]
    public IActionResult SensitiveFolder(string filePath)
    {
      return Redirect($"{Settings.Current.BasePath}hack/detected/"
        + HackAttemptType.SensitiveFolder.ToString());
    }
  }
}