using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using PEngine.Core.Shared;

namespace PEngine.Core.Web
{
  public static class HtmlHelpers
  {
    public static HtmlString ContentHashFile(this IHtmlHelper HtmlHelper, string webPath)
    {
      var cacheEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", webPath);
      return new HtmlString($"hash/{cacheEntry.Hash}/{cacheEntry.WebPath}");
    }
  }
}