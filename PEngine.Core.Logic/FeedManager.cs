using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Markdig;

namespace PEngine.Core.Logic
{
  public static class FeedManager
  {
    private static string RSSFilePath
    {
      get 
      {
        return $"{ContentRootPath}rss.xml";
      }
    }

    private static string _contentRootPath;
    public static string ContentRootPath
    {
      get
      {
        return _contentRootPath;
      }
      private set 
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? string.Empty : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }

    private static IPostService _postService = null;

    public static async Task Startup(string contentRootPath, IPostService postService)
    {
      _postService = postService;
      ContentRootPath = contentRootPath;
      await GenerateRSS();
    }

    public static async Task GenerateRSS(int? count = null)
    {
      count = count ?? Settings.Current.PerPageRSS;
      if (!Settings.Current.DisableRSS)
      {
        if (System.IO.File.Exists(RSSFilePath))
        {
          System.IO.File.Delete(RSSFilePath);
        }
        string title = Settings.Current.DefaultTitle;
        string logo = Settings.Current.LogoFrontPage;
        string location = Settings.Current.ExternalBaseUrl.TrimEnd('/');
        StringBuilder frameXml = new StringBuilder();
        System.Xml.XmlDocument rssfeed = new System.Xml.XmlDocument();

        frameXml.Append("<rss version=\"2.0\">");
        frameXml.Append("<channel>");
        frameXml.Append($"<title>{title}</title>");
        frameXml.Append($"<link>{location}</link>");
        frameXml.Append($"<description>{title}</description>");
        frameXml.Append($"<lastBuildDate>{string.Format("{0:R}", DateTime.Now)}</lastBuildDate>");
        frameXml.Append("<language>en-us</language>");
        if (!string.IsNullOrEmpty(logo))
        {
          frameXml.Append("<image>");
          frameXml.Append($"<title>{title}</title>");
          frameXml.Append($"<link>{location}</link>");
          frameXml.Append($"<url>{location}/images/system/{logo}</url>");
          frameXml.Append("</image>");
        }
        frameXml.Append("</channel>");
        frameXml.Append("</rss>");
        rssfeed.LoadXml(frameXml.ToString());
        var rssChannel = rssfeed.SelectSingleNode("/rss/channel");

        var posts = (await _postService.ListPosts(false, Settings.Current.IsLockedDown))
          .OrderByDescending(p => p.CreatedUTC)
          .Take(count.Value)
          .ToList();
        if ((posts != null) && (posts.Count > 0))
        {
          var postXml = new StringBuilder();
          foreach (var post in posts)
          {
            var postDate = string.Format("{0:R}", post.CreatedUTC.Value);
            var postData = RenderForFeed(post.Data);

            postXml.Append("<item>");
            postXml.Append($"<title>{System.Net.WebUtility.HtmlEncode(post.Name)}</title>");
            postXml.Append($"<link>{location}/post/view/{post.CreatedYear}/{post.CreatedMonth}/{post.UniqueName}</link>");
            postXml.Append($"<pubDate>{postDate}</pubDate>");
            postXml.Append($"<description>{System.Net.WebUtility.HtmlEncode(postData.DataTruncate(-1))}</description>");
            postXml.Append("</item>");
          }
          rssChannel.InnerXml += postXml.ToString();

          using (var feedStream = System.IO.File.OpenWrite(RSSFilePath))
          {
            rssfeed.Save(feedStream);
          }
        }
      }
    }

    private static string RenderForFeed(string data)
    {
      var pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions();
        
      return Regex.Replace(Markdown.ToHtml(data, pipeline.Build()), @"<(.|\n)*?>", string.Empty);
    }

    public static async Task<string> GetRSSXml()
    {
      if (!System.IO.File.Exists(RSSFilePath))
      {
        await GenerateRSS();
      }
      return System.IO.File.ReadAllText(RSSFilePath);
    }
  }
}