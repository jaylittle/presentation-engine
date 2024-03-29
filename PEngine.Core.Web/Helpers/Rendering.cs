using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using PEngine.Core.Shared;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace PEngine.Core.Web.Helpers
{
  public static class Rendering
  {
    private static readonly char[] _bannedChars = { '/', '\\', '?', '!', ';', ':', '\"', '\'', '(', ')', '&', '$', '%', '#', '@', '*', '|', ',', '-' };

    public static string FormatDate(object dateobj)
    {
      return FormatDate(dateobj, false);
    }

    public static string FormatDate(object dateobj, bool skipPresent)
    {
      return FormatDate(dateobj, false, false);
    }

    public static string FormatDateForJavascript(DateTime? dt)
    {
      return dt.HasValue ? FormatDateForJavascript(dt.Value) : string.Empty;
    }

    public static string FormatDateForJavascript(DateTime dt)
    {
      var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return Convert.ToInt64((dt - epoch).TotalSeconds * 1000).ToString();
    }

    public static string FormatDate(object dateobj, bool skipPresent, bool skipTime)
    {
      DateTime tempobj = DateTime.MinValue;
      if (dateobj is DateTime)
      {
        tempobj = (DateTime)dateobj;
      }
      else if (dateobj is string)
      {
        tempobj = DateTime.Parse((string)dateobj);
      }
      if (tempobj != DateTime.MinValue)
      {
        if (tempobj <= DateTime.UtcNow || skipPresent)
        {
          return tempobj.ToString("yyyy/M/d") + ((!skipTime && tempobj.TimeOfDay.TotalSeconds > 0) ? " " + tempobj.ToString("h:mm tt") : string.Empty);
        }
        else
        {
          return "Present";
        }
      }
      else
      {
        return string.Empty;
      }
    }

    public static string GetAbsolutePath(string relativePath)
    {
      return System.IO.Path.Combine(Settings.Current.BasePath, relativePath);
    }

    public static string MarkupSubheader(string text)
    {
      return MarkupSubheader(text, true);
    }

    public static string MarkupSubheader(string text, bool anchorFlag)
    {
      return MarkupSubheader(text, anchorFlag, anchorFlag ? text.Replace(" ", string.Empty) : string.Empty);
    }

    public static string MarkupSubheader(string text, bool anchorFlag, string anchorName)
    {
      return (anchorFlag ? $"<a name=\"{anchorName}\"></a>" : string.Empty)
        + $"<div class=\"sub-header\">{text}</div>";
    }

    public static string MarkupIcon(string url)
    {
      return !string.IsNullOrEmpty(url)
        ? string.Format($"<img src=\"images/icons/{url}\" class=\"post-icon\" alt=\"Post Icon\" />")
        : string.Empty;
    }

    public static string MarkupMenuButton(string text, string url, string linkAttributes)
    {
      linkAttributes = !string.IsNullOrWhiteSpace(linkAttributes) ? $" {linkAttributes}" : string.Empty;
      if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(url))
      {
        if (Shared.Helpers.IsUrlAbsolute(url))
        {
          return $"<a class=\"menu-button\" href=\"{url}\" target=\"_blank\"{linkAttributes}>{text}</a>";
        }
        else
        {
          return $"<a class=\"menu-button\" href=\"{Rendering.GetAbsolutePath(url)}\"{linkAttributes}>{text}</a>";
        }
      }
      else
      {
        return "<div class=\"menu-separator\"></div>";
      }
    }

    public static string MarkupArticle(string secdata, bool forum, bool frontPage = false)
    {
      var pipelineBuilder = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions();
      if (forum)
      {
        pipelineBuilder.DisableHtml();
      }
      var pipeline = pipelineBuilder.Build();

      //Parse and Render Markup while applying special optimizations
      //Shamelessly ripped off from https://github.com/lunet-io/markdig/issues/293#issuecomment-456376415
      var document = Markdown.Parse(secdata, pipeline);
      foreach (var descendant in document.Descendants())
      {
        string url = null;
        bool isImage = false;
        if (descendant is AutolinkInline autoLink) 
        {
          url = autoLink.Url;
        }
        else if (descendant is LinkInline linkInline)
        {
          url = linkInline.Url;
          isImage = linkInline.IsImage;
          if (isImage && !Shared.Helpers.IsUrlAbsolute(url))
          {
            var imageUrl = url.TrimStart('/');
            var imageHashEntry = ContentHash.GetContentHashEntryForFile(SystemInfoHelpers.ContentRootPath, new string[] { "wwwoverlay", "wwwroot" }, imageUrl, Html.GetAbsoluteHashPath).Result;
            if (imageHashEntry != null)
            {
              linkInline.Url = url = Html.GetAbsoluteHashPath(imageHashEntry.Hash, imageHashEntry.WebPath);
            }
          }
        }
        if (!isImage && Shared.Helpers.IsUrlAbsolute(url))
        {
          descendant.GetAttributes().AddPropertyIfNotExist("target", "_blank");
        }
      }
      using (var writer = new StringWriter())
      {
        var renderer = new HtmlRenderer(writer);
        pipeline.Setup(renderer);
        renderer.Render(document);

        //Return finalized HTML
        if (!frontPage || !Settings.Current.SummaryModeFront)
        {
          return writer.ToString();
        }
        return writer.ToString().DataTruncate(-3);
      }
    }

    public static bool SummaryModeFront
    {
      get
      {
        return Settings.Current.SummaryModeFront;
      }
    }

    public static string LogoPath
    {
      get
      {
        return $"images/system/{Settings.Current.LogoFrontPage}";
      }
    }

    public static bool LogoEnabled
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(Settings.Current.LogoFrontPage))
        {
          var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, new string[] { "wwwoverlay", "wwwroot" }, LogoPath, null).Result;
          return hashEntry != null;
        }
        return false;
      }
    }

    public static bool IconEnabled
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(Settings.Current.FavIcon))
        {
          var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, new string[] { "wwwoverlay", "wwwroot" }, LogoPath, null).Result;
          return hashEntry != null;
        }
        return false;
      }
    }

    public static string FavIconPath
    {
      get
      {
        return $"images/system/{Settings.Current.FavIcon}";
      }
    }

    public static bool FavIconEnabled
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(Settings.Current.FavIcon))
        {
          var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, new string[] { "wwwoverlay", "wwwroot" }, FavIconPath, null).Result;
          return hashEntry != null;
        }
        return false;
      }
    }

    public static bool AdditionalHeadersEnabled
    {
      get
      {
        return !string.IsNullOrWhiteSpace(Settings.Current.AdditionalHeaders);
      }
    }

    public static string DataTruncate(string data, int length = 75)
    {
      return data.DataTruncate(length);
    }

    public static string DataRenderAndTruncate(string data, int length = 75)
    {
      var pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions();
        
      return DataTruncate(Regex.Replace(Markdown.ToHtml(data, pipeline.Build()), @"<(.|\n)*?>", string.Empty), length);
    }

    public static IEnumerable<string> ThemeList
    {
      get
      {
        var themePaths = new string[] { 
          System.IO.Path.Combine(Startup.ContentRootPath, $"wwwoverlay{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}"),
          System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}")
        };

        var themeDirs = new List<string>();

        foreach(var themePath in themePaths)
        {
          if (System.IO.Directory.Exists(themePath))
          {
            themeDirs.AddRange(
              new DirectoryInfo(themePath).GetDirectories()
                .Where(d => System.IO.File.Exists($"{d.FullName}{Path.DirectorySeparatorChar}{d.Name}.css"))
                .Select(d => d.Name)
                .Where(t => !Settings.Current.IsThemeHidden(t))
            );
          }
        }

        themeDirs = themeDirs
          .OrderBy(d => d)
          .Distinct()
          .ToList();
        
        return themeDirs.Any() ? themeDirs : new List<string>();
      }
    }

    public static IEnumerable<string> IconList
    {
      get
      {
        var iconPaths = new string[] {
          System.IO.Path.Combine(Startup.ContentRootPath, $"wwwoverlay{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}icons{Path.DirectorySeparatorChar}"),
          System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}icons{Path.DirectorySeparatorChar}")
        };

        var iconFiles = new List<string>();

        foreach(var iconPath in iconPaths)
        {
          if (System.IO.Directory.Exists(iconPath))
          {
            iconFiles.AddRange(
              new DirectoryInfo(iconPath).GetFiles()
                .Where(f => !string.Equals(f.Name, "placeholder.txt", StringComparison.OrdinalIgnoreCase))
                .Select(f => f.Name)
            );
          }
        }

        iconFiles = iconFiles
          .OrderBy(f => f)
          .Distinct()
          .ToList();
        
        return iconFiles.Any() ? iconFiles : new List<string>();
      }
    }
  }
}