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
  [Route("feed")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class FeedController : Controller
  {
    public FeedController()
    {
    }

    [HttpGet("rss")]
    [HttpHead("rss")]
    public async Task<IActionResult> RSS()
    {
      if (!Settings.Current.DisableRSS)
      {
        return this.Content(await FeedManager.GetRSSXml(), "application/rss+xml");
      }
      else
      {
        return this.NotFound();
      }
    }
  }
}