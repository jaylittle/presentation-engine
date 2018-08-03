using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mime;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Claims;
using System.Threading.Tasks;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Antiforgery.Internal;
using Remotion.Linq.Clauses;

namespace PEngine.Core.Web.Security
{
  public class XSRF
  {
    private static IAntiforgeryTokenSerializer _antiforgerySerializer;
    private static IAntiforgeryTokenGenerator _antiforgeryGenerator;

    public static readonly string[] XSRF_HTTP_METHODS = { "POST", "PUT", "PATCH", "DELETE" };

    public static void Init(IAntiforgeryTokenSerializer antiforgerySerializer, IAntiforgeryTokenGenerator antiforgeryGenerator)
    {
      _antiforgerySerializer = antiforgerySerializer;
      _antiforgeryGenerator = antiforgeryGenerator;
    }

    public class XSRFCheckAttribute : TypeFilterAttribute
    {
      public XSRFCheckAttribute()
        : base(typeof(XSRFCheckActionFilter))
      {
        Arguments = new object[] { };
      }
    }

    public class XSRFCheckActionFilter : IAsyncActionFilter
    {     
      public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
      {
        if (context.Result == null)
        {
          string xsrfMessage = null;
          if (!IsNotXsrf(context, out xsrfMessage))
          {
            Console.WriteLine($"XSRF Check failed. Message: {xsrfMessage}");
            context.Result = new BadRequestResult();
            return;
          }
        }
        await next();
      }
    }

    private static bool IsNotXsrf(ActionExecutingContext context, out string message)
    {

      if (XSRF_HTTP_METHODS.Contains(context.HttpContext.Request.Method, StringComparer.OrdinalIgnoreCase))
      {
        var xsrfCookieValue = context.HttpContext.Request.Cookies[Middleware.TokenCookieMiddleware.COOKIE_XSRF_COOKIE_TOKEN];
        var xsrfFormValue = string.Empty;
        if (context.HttpContext.Request.Headers.ContainsKey(Middleware.TokenCookieMiddleware.HEADER_XSRF_FORM_TOKEN))
        {
          xsrfFormValue = context.HttpContext.Request.Headers[Middleware.TokenCookieMiddleware.HEADER_XSRF_FORM_TOKEN].First();
        }
        if (context.HttpContext.Request.Headers.ContainsKey(Middleware.TokenCookieMiddleware.HEADER_XSRF_COMBINED_TOKEN))
        {
          var xsrfCombinedHeaders = context.HttpContext.Request.Headers[Middleware.TokenCookieMiddleware.HEADER_XSRF_COMBINED_TOKEN].First().Split(':');
          if (string.IsNullOrWhiteSpace(xsrfCookieValue))
          {
            xsrfCookieValue = xsrfCombinedHeaders[0];
          }
          if (!context.HttpContext.Request.Headers.ContainsKey(Middleware.TokenCookieMiddleware.HEADER_XSRF_FORM_TOKEN) && xsrfCombinedHeaders.Length > 1)
          {
            xsrfFormValue = xsrfCombinedHeaders[1];
          }
        }
        if (!string.IsNullOrWhiteSpace(xsrfCookieValue) && !string.IsNullOrWhiteSpace(xsrfFormValue))
        {
          try
          {
            var cookieToken = _antiforgerySerializer.Deserialize(xsrfCookieValue);
            var requestToken = _antiforgerySerializer.Deserialize(xsrfFormValue);
            message = string.Empty;
            return _antiforgeryGenerator.TryValidateTokenSet(context.HttpContext, cookieToken, requestToken, out message);
          }
          catch
          {
            message = "Invalid cookie/header values";
            return false;
          }
        }
        else
        {
          message = "Missed required cookie/header values";
          return false;
        }
      }
      message = null;
      return true;
    }
  }
}