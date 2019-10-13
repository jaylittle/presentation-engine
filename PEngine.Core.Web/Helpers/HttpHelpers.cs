using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PEngine.Core.Shared;

namespace PEngine.Core.Web.Helpers
{
  public class HttpHelpers
  {
    public static CookieOptions GetCookieOptions(HttpContext context)
    {
      var secureFlag = Settings.Current.ExternalBaseUrl.StartsWith("https", StringComparison.OrdinalIgnoreCase)
        || context.Request.Protocol.StartsWith("https", StringComparison.OrdinalIgnoreCase);
      var cookieOptions = new CookieOptions()
      {
        HttpOnly = true,
        Secure = secureFlag,
      };
      if (!string.IsNullOrWhiteSpace(Settings.Current.CookieDomain))
      {
        cookieOptions.Domain = Settings.Current.CookieDomain;
      }
      if (!string.IsNullOrWhiteSpace(Settings.Current.CookiePath))
      {
        cookieOptions.Path = Settings.Current.CookiePath;
      }
      return cookieOptions;
    }
  }
}