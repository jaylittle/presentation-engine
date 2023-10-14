using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;

namespace PEngine.Core.Web.Middleware
{
  public class TokenProviderOptions
  {
    public string PEnginePath { get; set; } = "/token/pengine";
    public string RefreshPath { get; set; } = "/token/refresh";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public SigningCredentials SigningCredentials { get; set; }
  }

  public class TokenCookieMiddleware
  {
    public const string COOKIE_ACCESS_TOKEN = "access-token";
    public const string COOKIE_XSRF_COOKIE_TOKEN = "xsrf-cookie-token";
    public const string HEADER_XSRF_FORM_TOKEN = "xsrf-form-token";
    public const string HEADER_XSRF_COMBINED_TOKEN = "xsrf-combined-token";
    private readonly RequestDelegate _next;

    public TokenCookieMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
      var cookie = context.Request.Cookies[COOKIE_ACCESS_TOKEN];
      if (!string.IsNullOrWhiteSpace(cookie) && !context.Request.Headers.ContainsKey("Authorization"))
      {
        context.Request.Headers.Append("Authorization", $"Bearer {cookie}");
      }

      await _next.Invoke(context);
    }

    public static bool HasJwtCookie(HttpContext context)
    {
      return context.Request.Cookies.Any(c => c.Key.Equals(TokenCookieMiddleware.COOKIE_ACCESS_TOKEN, StringComparison.OrdinalIgnoreCase));
    }

    public static bool HasXsrfCookie(HttpContext context)
    {
      return context.Request.Cookies.Any(c => c.Key.Equals(TokenCookieMiddleware.COOKIE_XSRF_COOKIE_TOKEN, StringComparison.OrdinalIgnoreCase));
    }

    public static void AddJwtCookie(HttpContext context, string encodedJwt)
    {
      var cookieOptions = Helpers.HttpHelpers.GetCookieOptions(context);
      context.Response.Cookies.Append(TokenCookieMiddleware.COOKIE_ACCESS_TOKEN, encodedJwt, cookieOptions);
    }

    public static bool RemoveJwtCookie(HttpContext httpContext)
    {
      if (httpContext.Request.Cookies.ContainsKey(Middleware.TokenCookieMiddleware.COOKIE_ACCESS_TOKEN))
      {
        var cookieOptions = Helpers.HttpHelpers.GetCookieOptions(httpContext);
        httpContext.Response.Cookies.Delete(Middleware.TokenCookieMiddleware.COOKIE_ACCESS_TOKEN, cookieOptions);
        return true;
      }
      return false;
    }

    public static void AddXsrfCookie(HttpContext context, string xsrfCookieValue)
    {
      var cookieOptions = Helpers.HttpHelpers.GetCookieOptions(context);
      context.Response.Cookies.Append(TokenCookieMiddleware.COOKIE_XSRF_COOKIE_TOKEN, xsrfCookieValue, cookieOptions);
    }

    public static bool RemoveXsrfCookie(HttpContext httpContext)
    {
      if (httpContext.Request.Cookies.ContainsKey(Middleware.TokenCookieMiddleware.COOKIE_XSRF_COOKIE_TOKEN))
      {
        var cookieOptions = Helpers.HttpHelpers.GetCookieOptions(httpContext);
        httpContext.Response.Cookies.Delete(Middleware.TokenCookieMiddleware.COOKIE_XSRF_COOKIE_TOKEN, cookieOptions);
        return true;
      }
      return false;
    }
  }

  public class TokenReplayCache : ITokenReplayCache
  {
    public static TokenReplayCache Instance = new TokenReplayCache();
    private ConcurrentDictionary<string, DateTime> _tokens = new ConcurrentDictionary<string, DateTime>();
    public bool TryAdd(string securityToken, DateTime expiresOn)
    {
      return true;
    }

    public bool TryFind(string securityToken)
    {
      return _tokens.ContainsKey(securityToken);
    }

    public void Expire(string securityToken)
    {
      while (!_tokens.ContainsKey(securityToken) && !_tokens.TryAdd(securityToken, DateTime.UtcNow.AddMinutes(-1))) { }
    }
  }

  public class TokenProviderMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly TokenProviderOptions _options;
    public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options)
    {
      _next = next;
      _options = options.Value;
    }

    public Task Invoke(HttpContext context)
    {
      // If the request path doesn't match, skip
      if (!context.Request.Path.Equals(_options.PEnginePath, StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.Equals(_options.RefreshPath, StringComparison.OrdinalIgnoreCase))
      {
        return _next(context);
      }

      if (context.Request.Method.Equals("POST") && context.Request.HasFormContentType)
      {
        if (context.Request.Path.Equals(_options.PEnginePath, StringComparison.OrdinalIgnoreCase))
        {
          return ValidatePEngineUser(context);
        }
      }
      if (context.Request.Method.Equals("GET") 
        && context.Request.Path.Equals(_options.RefreshPath, StringComparison.OrdinalIgnoreCase))
      {
        return RefreshToken(context);
      }
      
      context.Response.StatusCode = 400;
      return context.Response.WriteAsync("Bad request.");
    }

    private async Task ValidatePEngineUser(HttpContext context)
    {
      var userName = (string)context.Request.Form["username"] ?? string.Empty;
      var password = (string)context.Request.Form["password"] ?? string.Empty;
      var successUrl = context.Request.Form.ContainsKey("successUrl") ? (string)context.Request.Form["successUrl"] : (string)null;
      var failUrl = context.Request.Form.ContainsKey("failUrl") ? (string)context.Request.Form["failUrl"] : (string)null;
      await ValidatePEngineUser(context, userName, password, null, successUrl, failUrl, false);
    }

    private async Task RefreshToken(HttpContext context)
    {
      bool isTokenValid = false;
      if (context.Request?.HttpContext?.User != null && context.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        string userName = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
        string userType = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;
        string sessionId = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineSessionId"))?.Value;
        string token = context.Request.Headers.FirstOrDefault(h => h.Key.Equals("Authorization"))
          .Value.FirstOrDefault()?.Replace("Bearer ", string.Empty);

        if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(userType) 
          && !string.IsNullOrWhiteSpace(token))
        {
          TokenReplayCache.Instance.Expire(token);

          switch (userType)
          {
            case "PEngine":
              isTokenValid = true;
              await ValidatePEngineUser(context, userName, null, sessionId, null, null, true);
              break;
          }
        }
      }
      if (!isTokenValid)
      {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid token - unable to refresh");
      }
    }

    private async Task ValidatePEngineUser(HttpContext context, string userName, string password, string sessionId, string successUrl, string failUrl, bool refreshFlag)
    {
      var userId = string.Empty;
      sessionId = !string.IsNullOrWhiteSpace(sessionId) ? sessionId : Guid.NewGuid().ToString();
      var roleClaims = new List<string>();
      var operation = refreshFlag ? "Refresh" : "Login";
      var remoteIP = context.Request.Headers.ContainsKey("X-Forwarded-For") ? context.Request.Headers["X-Forwarded-For"].First().Split(',').First() : context.Connection.RemoteIpAddress.ToString();
      if (Settings.Current.UserNameAdmin.Equals(userName, StringComparison.OrdinalIgnoreCase) 
        && (refreshFlag || PEngine.Core.Shared.Security.HashAndCompare(password, Settings.Current.PasswordAdmin)))
      {
        roleClaims.Add("PEngineAdmin");
        userId = Settings.Current.UserNameAdmin;
        Console.WriteLine($"{operation} succeeded for {userName} from {remoteIP}");
      }
      else
      {
        Console.WriteLine($"{operation} failed for {userName} from {remoteIP}");
      }

      ClaimsIdentity identity = null;
      if (!string.IsNullOrEmpty(userId))
      {
        identity = await GetIdentity(userId, userId, "PEngine", sessionId, roleClaims.ToArray());
      }
      await GenerateToken(context, identity, userId, sessionId, Settings.Current.TimeLimitAdminToken, successUrl, failUrl, refreshFlag);
    }

    private async Task GenerateToken(HttpContext context, ClaimsIdentity identity, string userId, string sessionId, int expirationMinutes, string successUrl = null, string failUrl = null, bool refreshFlag = false)
    {
      if (identity == null)
      {
        if (string.IsNullOrWhiteSpace(failUrl))
        {
          context.Response.StatusCode = 401;
          await context.Response.WriteAsync("Invalid username or password.");
          return;
        }
        else
        {
          if (!failUrl.Contains("?"))
          {
            context.Response.Redirect($"{failUrl}?authFailed=true");
          }
          else
          {
            context.Response.Redirect($"{failUrl}&authFailed=true");
          }
          return;
        }
      }
    
      var now = DateTime.UtcNow;
      var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
  
      // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
      // You can add other claims here, if you want:
      var claims = new Claim[]
      {
        new Claim(ClaimTypes.NameIdentifier, userId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
      };
    
      // Create the JWT and write it to a string
      var jwt = new JwtSecurityToken(
        issuer: _options.Issuer,
        audience: _options.Audience,
        claims: claims.Union(identity.Claims),
        notBefore: now,
        expires: expires,
        signingCredentials: _options.SigningCredentials);

      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
      var tempIdentity = new ClaimsIdentity(jwt.Claims, "Token");
      context.User = new ClaimsPrincipal(tempIdentity);
      var xsrfTokens = Security.XSRF.GetXsrfValues(userId, sessionId);

      if (string.IsNullOrWhiteSpace(successUrl))
      {
        if (TokenCookieMiddleware.HasJwtCookie(context))
        {
          TokenCookieMiddleware.AddXsrfCookie(context, xsrfTokens.cookieValue);
          TokenCookieMiddleware.AddJwtCookie(context, encodedJwt);
        }

        var response = new
        {
          access_token = encodedJwt,
          token_type = "Bearer",
          expires = (DateTime)expires,
          expires_in = (int)expirationMinutes * 60,
          expires_in_milliseconds = (long)expirationMinutes * 60 * 1000,
          xsrf_combined_token = !refreshFlag ? $"{xsrfTokens.cookieValue}:{xsrfTokens.formValue}" : null
        };
  
        // Serialize and return the response
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented })); 
      }
      else
      {
        TokenCookieMiddleware.AddXsrfCookie(context, xsrfTokens.cookieValue);
        TokenCookieMiddleware.AddJwtCookie(context, encodedJwt);
        context.Response.Redirect(successUrl);
      }
    }

    private Task<ClaimsIdentity> GetIdentity(string userId, string userName, string userType, string sessionId, string[] roleClaims)
    {
      var identity = new System.Security.Principal.GenericIdentity(userId, "Token");
      var claims = roleClaims.Select(rc => new Claim(identity.RoleClaimType, rc)).ToList();
      claims.Add(new Claim("PEngineUserName", userName));
      claims.Add(new Claim("PEngineUserType", userType));
      claims.Add(new Claim("PEngineSessionId", sessionId));
      return Task.FromResult(new ClaimsIdentity(identity, claims));
    }
  }
}
