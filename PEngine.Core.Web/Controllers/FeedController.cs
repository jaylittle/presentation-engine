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
  [Security.XSRF.XSRFCheck]
  [Route("feed")]
  [ResponseCache(CacheProfileName = "None")]
  public class FeedController : Controller
  {
    public FeedController()
    {
    }

    [HttpGet("rss")]
    public async Task<IActionResult> RSS()
    {
      return this.Content(await FeedManager.GetRSSXml(), "application/rss+xml");
    }
  }
}