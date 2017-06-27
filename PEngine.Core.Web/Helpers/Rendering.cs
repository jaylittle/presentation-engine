using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using PEngine.Core.Shared;
using Markdig;

namespace PEngine.Core.Web.Helpers
{
  public static class Rendering
  {
    private static readonly char[] _bannedChars = { '/', '\\', '?', '!', ';', ':', '\"', '\'', '(', ')', '&', '$', '%', '#', '@', '*', '|', ',', '-' };

    private static readonly Dictionary<string, string> _eliteWords = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      { "cool", "k3wl" },
      { "dude", "d00d" },
      { "dudes", "d00dz" },
      { "hacker", "hax0r" },
      { "hacked", "hax0red" },
      { "mp3s", "mp3z" },
      { "rock", "r0x0r" },
      { "rocks", "r0x0rez" },
      { "you", "j00" },
      { "elite", "l33t|31337" },
      { "the", "teh|the" },
      { "own", "pwn|0wnzor" },
      { "porn", "porn|pr0n" }
    };

    private static readonly Dictionary<string, string> _eliteChars = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
      { "a", "@:4" },
      { "b", "b:8" },
      { "d", "d:|)" },
      { "e", "e:3" },
      { "f", "f:ph" },
      { "g", "g:9" },
      { "h", "h:|-|" },
      { "i", "i:1" },
      { "k", "k:|&lt;" },
      { "m", "m:|\\\\\\/|" },
      { "n", "n:|\\\\|" },
      { "o", "o:0" },
      { "s", "$:5" },
      { "t", "t:+" },
      { "v", "v:\\\\\\/" },
      { "w", "w:\\\\\\/\\\\\\/" },
      { "x", "x:&gt;&lt;" }
    };

    public static string FormatDate(object dateobj)
    {
      return FormatDate(dateobj, false);
    }

    public static string FormatDate(object dateobj, bool skipPresent)
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
          return tempobj.ToString("MM/dd/yyyy") + (tempobj.TimeOfDay.TotalSeconds > 0 ? " " + tempobj.ToString("HH:mm:ss") : string.Empty);
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

    public static string MarkupSubheader(string text, bool eliteFlag)
    {
      return MarkupSubheader(text, true, eliteFlag);
    }

    public static string MarkupSubheader(string text, bool anchorFlag, bool eliteFlag)
    {
      return MarkupSubheader(text, anchorFlag, anchorFlag ? text.Replace(" ", string.Empty) : string.Empty, eliteFlag);
    }

    public static string MarkupSubheader(string text, bool anchorFlag, string anchorName, bool eliteFlag)
    {
      return (anchorFlag ? $"<a name=\"{anchorName}\"></a>" : string.Empty)
        + $"<div class=\"sub-header\">{EliteConvert(text, eliteFlag)}</div>";
    }

    public static string MarkupIcon(string url, bool eliteFlag)
    {
      return !string.IsNullOrEmpty(url)
        ? string.Format($"<img src=\"images/icons/{url}\" class=\"post-icon\" alt=\"Post Icon\" />")
        : string.Empty;
    }

    public static string MarkupMenuButton(string text, string url, bool eliteFlag)
    {
      if (!string.IsNullOrWhiteSpace(text) && !string.IsNullOrWhiteSpace(url))
      {
        return $"<a class=\"menu-button\" href=\"{url}\">{EliteConvert(text, eliteFlag)}</a>";
      }
      else
      {
        return "<div class=\"menu-separator\"></div>";
      }
    }

    public static string MarkupArticle(string secdata, bool forum)
    {
      var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions();
      if (forum)
      {
        pipeline.DisableHtml();
      }
      return Markdown.ToHtml(secdata, pipeline.Build());
    }

    public static string EliteConvert(string origText, bool eliteFlag)
    {
      if (!eliteFlag)
      {
        return origText;
      }

      StringBuilder retvalue = new StringBuilder();
      System.Random Randomizer = new Random(DateTime.Now.Millisecond);
      string[] words = origText.ToLower().Split(' ');
      for (int wordptr = 0; wordptr < words.Length; wordptr++)
      {
        string cword = words[wordptr];
        string newword = string.Empty;
        switch (cword)
        {
          case "am":
            if ((wordptr < words.Length - 1) && (words[wordptr + 1] == "good"))
            {
              cword = "ownz0r";
              wordptr++;
            }
            break;
          case "is":
            if ((wordptr < words.Length - 1) && (words[wordptr + 1] == "good"))
            {
              cword = "ownz0rz";
              wordptr++;
            }
            break;
          default:
            if (_eliteWords.ContainsKey(words[wordptr]))
            {
              string[] tword = _eliteWords[words[wordptr]].Split('|');
              cword = tword[Randomizer.Next(0, tword.Length - 1)];
            }
            break;
        }
        for (int charptr = 0; charptr < cword.Length; charptr++)
        {
          string curchar = cword[charptr].ToString();
          if (_eliteChars.ContainsKey(curchar))
          {
            string[] tchar = _eliteChars[curchar].Split(':');
            curchar = tchar[Randomizer.Next(0, tchar.Length - 1)];
          }
          if (Randomizer.Next(0, 1) == 1)
          {
            curchar = curchar.ToUpper();
          }
          newword += curchar;
        }
        retvalue.Append(newword + " ");
      }
      return retvalue.ToString();
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
        return !string.IsNullOrWhiteSpace(Settings.Current.LogoFrontPage);
      }
    }

    public static string DataTruncate(string data)
    {
      return DataTruncate(data, 75);
    }

    public static string DataTruncate(string data, int length)
    {
      data = data
        .Replace("[", string.Empty)
        .Replace("]", string.Empty)
        .Replace("<", string.Empty)
        .Replace(">", string.Empty);

      if (length > 0)
      {
        if (data.Length > length)
        {
          return data.Substring(0, length) + "...";
        }
      }
      else
      {
        string[] delimiters = { Environment.NewLine, "\n", "<br>", "[br]" };
        int strptr = -1;
        int dptr = 0;
        while (strptr < 0 && dptr < delimiters.Length)
        {
          strptr = data.IndexOf(delimiters[dptr]);
          dptr++;
        }
        if (strptr >= 0)
        {
          return data.Substring(0, strptr);
        }
      }
      return data;
    }

    public static IEnumerable<string> ThemeList
    {
      get
      {
        var themePath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}themes{Path.DirectorySeparatorChar}");
        if (System.IO.Directory.Exists(themePath))
        {
          return new DirectoryInfo(themePath).GetDirectories()
            .Where(d => System.IO.File.Exists($"{d.FullName}{Path.DirectorySeparatorChar}{d.Name}.css"))
            .Select(d => d.Name);
        }
        return new List<string>();
      }
    }

    public static IEnumerable<string> IconList
    {
      get
      {
        var iconPath = System.IO.Path.Combine(Startup.ContentRootPath, $"wwwroot{Path.DirectorySeparatorChar}images{Path.DirectorySeparatorChar}icons{Path.DirectorySeparatorChar}");
        if (System.IO.Directory.Exists(iconPath))
        {
          return new DirectoryInfo(iconPath).GetFiles()
            .Select(f => f.Name);
        }
        return new List<string>();
      }
    }
  }
}