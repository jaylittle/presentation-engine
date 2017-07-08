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
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Routing;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Web.Helpers
{
  public static class Html
  {
    public static HtmlString ContentHashFile(this IHtmlHelper htmlHelper, string webPath)
    {
      var urlHelper = new UrlHelper(htmlHelper.ViewContext);
      var hashEntry = ContentHash.GetContentHashEntryForFile(Startup.ContentRootPath, "wwwroot", webPath).Result;
      var hashUrl = System.Net.WebUtility.UrlDecode(urlHelper.Action("GetHashedFileName", "hash", new
      {
        hash = hashEntry.Hash,
        filePath = hashEntry.WebPath
      }));
      return new HtmlString(hashUrl);
    }

    public static IHtmlContent PEPager(this IHtmlHelper htmlHelper, PagingModel paging, object htmlAttributes, string action, object routeValues = null)
    {
      var linkRd = routeValues != null ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
      linkRd.Add("sortField", paging.SortField);
      linkRd.Add("sortAscending", paging.SortAscending);
      return htmlHelper.PEPager(paging.Start, paging.Count, paging.Total, linkRd, htmlAttributes, action);
    }

    private static RouteValueDictionary CopyRVD(RouteValueDictionary source)
    {
      var retvalue = new RouteValueDictionary();
      foreach (var item in source)
      {
        retvalue.Add(item.Key, item.Value);
      }
      return retvalue;
    }

    private static IHtmlContent PEPager(this IHtmlHelper htmlHelper, int start, int count, int total, RouteValueDictionary routeValues, object htmlAttributes, string action)
    {
      HtmlContentBuilder retvalue = new HtmlContentBuilder();
      if (start > 1)
      {
        RouteValueDictionary linkRd = CopyRVD(routeValues);
        linkRd.Add("start", 1);
        linkRd.Add("count", count);
        retvalue.AppendHtml(htmlHelper.ActionLink("[Start]", action, linkRd, new RouteValueDictionary(htmlAttributes)));
        retvalue.AppendHtml("&nbsp;");
        linkRd = CopyRVD(routeValues);
        linkRd.Add("start", start - count);
        linkRd.Add("count", count);
        retvalue.AppendHtml(htmlHelper.ActionLink("[Prev]", action, linkRd, new RouteValueDictionary(htmlAttributes)));
      }
      else
      {
        if (total > count)
        {
          retvalue.AppendHtml("[Start]&nbsp;[Prev]");
        }
      }
      if (total > count)
      {
        retvalue.AppendHtml(string.Format("&nbsp;<span>{0} to {1} of {2}</span>&nbsp;", start, total < start + count ? total : start + count - 1, total));
      }
      else
      {
        retvalue.AppendHtml(string.Format("&nbsp;<span>{0} of {0}&nbsp;", total));
      }

      if (total > (start + count) - 1)
      {
        RouteValueDictionary linkRd = CopyRVD(routeValues);
        linkRd.Add("start", start + count);
        linkRd.Add("count", count);
        retvalue.AppendHtml(htmlHelper.ActionLink("[Next]", action, linkRd, new RouteValueDictionary(htmlAttributes)));
        retvalue.AppendHtml("&nbsp;");
        linkRd = CopyRVD(routeValues);
        linkRd.Add("start", (total - (total % count)) + 1);
        linkRd.Add("count", count);
        retvalue.AppendHtml(htmlHelper.ActionLink("[End]", action, linkRd, new RouteValueDictionary(htmlAttributes)));
      }
      else
      {
        if (total > count)
        {
          retvalue.AppendHtml("[Next]&nbsp;[End]");
        }
      }
      return retvalue;
    }

    public static IHtmlContent PEPagerLinkWithText(this IHtmlHelper htmlHelper, string linkText, string actionName, string fieldName, PagingModel paging, object routeValues = null)
    {
      var retvalue = new HtmlContentBuilder();
      var linkRd = routeValues != null ? new RouteValueDictionary(routeValues) : new RouteValueDictionary();
      linkRd.Add("sortField", fieldName);
      linkRd.Add("sortAscending", paging.SortField == fieldName ? !paging.SortAscending : true);
      retvalue.AppendHtml(htmlHelper.PEPagerLink(linkText, actionName, 1, paging.Count, linkRd));
      retvalue.AppendHtml(paging.SortField == fieldName ? paging.SortAscending ? "[A]" : "[D]" : string.Empty);
      return retvalue;
    }

    public static IHtmlContent PEPagerLink(this IHtmlHelper htmlHelper, string linkText, string actionName, int start, int count, object routeValues = null)
    {
      RouteValueDictionary linkRd = new RouteValueDictionary(routeValues);
      return htmlHelper.PEPagerLink(linkText, actionName, start, count, linkRd);
    }

    public static IHtmlContent PEPagerLink(this IHtmlHelper htmlHelper, string linkText, string actionName, int start, int count, RouteValueDictionary linkRd)
    {
      linkRd.Add("start", start);
      linkRd.Add("count", count);
      return htmlHelper.ActionLink(linkText, actionName, linkRd);
    }
  }
}