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

    public static string MarkupArticle(string secdata, bool forum, bool eliteFlag)
    {
      return MarkupArticle(secdata, forum, 0, eliteFlag);
    }

    public static string MarkupArticle(string secdata, bool forum, int articleid, bool eliteFlag)
    {
      int lpos = 0;
      string tag = string.Empty;
      string tagname = string.Empty;
      string tagdata = string.Empty;
      int tagspace = 0;
      string[] tagelements = { };
      bool rawhtmlflag = false;
      int rawhtmlstart = 0;
      int rawhtmlend = 0;
      string outdata = string.Empty;
      string[] resforumtags = {"SCRIPT", "/SCRIPT", "IFRAME", "/IFRAME", "EMBED", "BLINK"
      , "TR", "TD", "TABLE", "/TR", "/TD", "/TABLE", "FRAMESET", "/FRAMESET"};
      bool restagflag = false;
      StringBuilder outputhtml = new StringBuilder();
      //Filter for obfusacated tags if forum flag is true
      //Remove HTML Content if Forum Flag is true
      if (forum)
      {
        while (secdata.IndexOf("[" + Environment.NewLine) >= 0)
        {
          secdata.Replace("[" + Environment.NewLine, "[ ");
        }
        while (secdata.IndexOf("<" + Environment.NewLine) >= 0)
        {
          secdata.Replace("<" + Environment.NewLine, "< ");
        }
        while (secdata.IndexOf("[ ") >= 0)
        {
          secdata.Replace("[ ", "[");
        }
        while (secdata.IndexOf("< ") >= 0)
        {
          secdata.Replace("< ", "<");
        }
        for (int cpos = secdata.IndexOf("<"); cpos >= 0; cpos = secdata.IndexOf("<", cpos + 1))
        {
          lpos = secdata.IndexOf(">", cpos + 1);
          if (lpos >= 0)
          {
            secdata = secdata.Substring(0, cpos) + secdata.Substring(lpos, secdata.Length - lpos);
          }
        }
      }
      lpos = -1;
      for (int cpos = secdata.IndexOf("["); cpos >= 0; cpos = secdata.IndexOf("[", lpos + 1))
      {
        if (!rawhtmlflag)
        {
          outdata = secdata.Substring(lpos + 1, cpos - (lpos + 1));
          outdata = EliteConvert(outdata, eliteFlag);
          for (int eptr = 0; eptr < Environment.NewLine.Length; eptr++)
          {
            if (outdata.IndexOf(Environment.NewLine[eptr]) >= 0)
            {
              outdata = outdata.Replace(Environment.NewLine[eptr].ToString(), "<br/>" + Environment.NewLine);
              eptr = Environment.NewLine.Length;
            }
          }
          outputhtml.Append(outdata);
        }
        lpos = secdata.IndexOf("]", cpos + 1);
        if (lpos > cpos)
        {
          tag = secdata.Substring(cpos + 1, lpos - (cpos + 1));
          tagspace = tag.IndexOf(" ");
          if (tagspace >= 0)
          {
            tagname = tag.Substring(0, tagspace).ToUpper();
            tagdata = tag.Substring(tagspace + 1, tag.Length - (tagspace + 1));
          }
          else
          {
            tagname = tag.ToUpper();
          }
          if ((!rawhtmlflag) || (tagname == "/RAWHTML"))
          {
            switch (tagname)
            {
              case "CENTER":
                outputhtml.Append("<p style=\"text-align: center\">");
                break;
              case "/CENTER":
                outputhtml.Append("</p>");
                break;
              case "IMAGE":
                if ((tagdata.ToUpper().IndexOf("HTTP") >= 0)
                  || (tagdata.Substring(0, 2) == "./") || (tagdata.Substring(0, 1) == "/"))
                {
                  outputhtml.Append($"<img src=\"{tagdata}\" alt=\"outside image\" />");
                }
                else
                {
                  outputhtml.Append($"<img src=\"images/articles/{tagdata}\" alt=\"article image\" />");
                }
                break;
              case "SUBHEADER":
                if (!forum)
                {
                  outputhtml.Append(MarkupSubheader(tagdata, eliteFlag));
                }
                break;
              case "LINK":
                tagelements = tagdata.Split(' ');
                string url = tagelements[0];
                outputhtml.Append($"<a href=\"{url}\">");
                if (tagelements.Length > 1)
                {
                  for (int teptr = 1; teptr < tagelements.Length; teptr++)
                  {
                    if (teptr > 1)
                    {
                      outputhtml.Append(" ");
                    }
                    outputhtml.Append(tagelements[teptr]);
                  }
                }
                else
                {
                  outputhtml.Append(tagelements[0]);
                }
                outputhtml.Append("</a>");
                break;
              case "ICON":
                outputhtml.Append(MarkupIcon($"images/icons/{tagdata}", eliteFlag));
                break;
              case "SYSTEMIMAGE":
                outputhtml.Append($"<img src=\"images/system/{tagdata}\" alt=\"system image\" />");
                break;
              case "RAWHTML":
                rawhtmlflag = true;
                rawhtmlstart = cpos + 9;
                rawhtmlend = rawhtmlstart;
                break;
              case "/RAWHTML":
                rawhtmlflag = false;
                rawhtmlend = cpos;
                outputhtml.Append(secdata.Substring(rawhtmlstart, rawhtmlend - rawhtmlstart));
                break;
              case "QUOTE":
                outputhtml.Append("<blockquote>");
                break;
              case "/QUOTE":
                outputhtml.Append("</blockquote>");
                break;
              default:
                restagflag = false;
                if (forum)
                {
                  for (int fptr = 0; fptr < resforumtags.Length; fptr++)
                  {
                    if (resforumtags[fptr].ToUpper() == tagname)
                    {
                      restagflag = true;
                    }
                  }
                }
                if (!restagflag)
                {
                  outputhtml.Append($"<{tag}>");
                }
                break;
            }
          }
        }
      }
      if (lpos >= -1)
      {
        outdata = secdata.Substring(lpos + 1, secdata.Length - (lpos + 1));
        if (!rawhtmlflag)
        {
          outdata = EliteConvert(outdata, eliteFlag);
          for (int eptr = 0; eptr < Environment.NewLine.Length; eptr++)
          {
            if (outdata.IndexOf(Environment.NewLine[eptr]) >= 0)
            {
              outdata = outdata.Replace(Environment.NewLine[eptr].ToString(), "<br/>" + Environment.NewLine);
              eptr = Environment.NewLine.Length;
            }
          }
          outputhtml.Append(outdata);
        }
        else
        {
          outputhtml.Append(outdata);
        }
      }
      if (outputhtml.Length <= 0)
      {
        outputhtml.Append("There was no data to convert.");
      }
      return outputhtml.ToString();
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