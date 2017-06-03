using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using PEngine.Core.Shared;

namespace PEngine.Core.Web
{
  public static class HtmlHelpers
  {
    public static HtmlString ContentHashFile(this IHtmlHelper htmlHelper, string webPath)
    {
      var urlHelper = new UrlHelper(htmlHelper.ViewContext);
      var cacheEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", webPath);
      var hashUrl = System.Net.WebUtility.UrlDecode(urlHelper.Action("GetHashedFileName", "hash", new {
        hash = cacheEntry.Hash,
        filePath = cacheEntry.WebPath
      }));
      return new HtmlString(hashUrl);
    }
  }
}