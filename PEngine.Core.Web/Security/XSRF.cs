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
using Microsoft.Extensions.Logging;

namespace PEngine.Core.Web.Security
{
  public class XSRF
  {
    public static readonly string[] XSRF_HTTP_METHODS = { "POST", "PUT", "PATCH", "DELETE" };

    private static ILoggerFactory _logFactory;

    public static void Startup(ILoggerFactory logFactory)
    {
      _logFactory = logFactory;
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
        var logger = _logFactory.CreateLogger<XSRF>();

        if (context.Result == null)
        {
          string xsrfMessage = null;
          if (!IsNotXsrf(context, out xsrfMessage))
          {
            logger.LogError($"XSRF Check failed for user with error: {xsrfMessage}");
            context.Result = new JsonResult(new List<string>(new string[] { "XSRF Security Check Failed. Refresh this browser tab and try again." })) {
              StatusCode = 400
            };
          }
        }
        await next();
      }
    }

    private static bool IsNotXsrf(ActionExecutingContext context, out string message)
    {
      var logger = _logFactory.CreateLogger<XSRF>();
      
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
          if (!ValidateXsrfToken(context.HttpContext, xsrfCookieValue, xsrfFormValue))
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

    private static string GetXsrfBaseValue(string userName, string sessionId)
    {
      if (!string.IsNullOrWhiteSpace(userName))
      {
        return $"{userName}|{sessionId}";
      }
      throw new Exception("XSRF Logic requires at least a proper User Id!");
    }

    public static string GetXsrfCookieValue(string userName, string sessionId)
    {
      return GetMD5Hash(GetXsrfBaseValue(userName, sessionId));
    }

    public static string GetXsrfFormValue(string userName, string sessionId)
    {
      return GetMD5Hash(Settings.Current.SecretKey + "|" + GetXsrfBaseValue(userName, sessionId));
    }

    public static (string cookieValue, string formValue) GetXsrfValues(string userName, string sessionId)
    {
      return (GetXsrfCookieValue(userName, sessionId), GetXsrfFormValue(userName, sessionId));
    }

    private static bool ValidateXsrfToken(HttpContext context, string submittedXsrfCookieValue, string submittedXsrfFormValue)
    {
      string userName = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
      string userType = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;
      string sessionId = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineSessionId"))?.Value;
      var correctValues = GetXsrfValues(userName, sessionId);
      return (correctValues.cookieValue.Equals(submittedXsrfCookieValue) && correctValues.formValue.Equals(submittedXsrfFormValue));
    }

    private static string GetMD5Hash(string originalValue)
    {
      using (var md5 = System.Security.Cryptography.MD5.Create())
      {
        var byteHash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(originalValue));
        var textHash = new System.Text.StringBuilder();
        for (int byteHashPtr = 0; byteHashPtr < byteHash.Length; byteHashPtr++)
        {
          textHash.Append(byteHash[byteHashPtr].ToString("x2"));
        }
        return textHash.ToString();
      }
    }
  }
}