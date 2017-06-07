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

namespace PEngine.Core.Web.Helpers
{
  public static class Html
  {
    public static HtmlString ContentHashFile(this IHtmlHelper htmlHelper, string webPath)
    {
      var urlHelper = new UrlHelper(htmlHelper.ViewContext);
      var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", webPath);
      var hashUrl = System.Net.WebUtility.UrlDecode(urlHelper.Action("GetHashedFileName", "hash", new {
        hash = hashEntry.Hash,
        filePath = hashEntry.WebPath
      }));
      return new HtmlString(hashUrl);
    }
  }
}