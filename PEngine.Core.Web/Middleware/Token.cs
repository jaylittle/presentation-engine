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
using Microsoft.AspNetCore.Http.Authentication;
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
    public string ForumPath { get; set; } = "/token/forum";
    public string RefreshPath { get; set; } = "/token/refresh";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public SigningCredentials SigningCredentials { get; set; }
  }

  public class TokenCookieOptions
  {
    public string CookieName { get; set; } = "access_token";
  }

  public class TokenCookieMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly TokenCookieOptions _options;

    public TokenCookieMiddleware(RequestDelegate next, IOptions<TokenCookieOptions> options)
    {
      _next = next;
      _options = options.Value;
    }

    public async Task Invoke(HttpContext context)
    {
      var cookie = context.Request.Cookies[_options.CookieName];
      if (!string.IsNullOrWhiteSpace(cookie) && !context.Request.Headers.ContainsKey("Authorization"))
      {
        context.Request.Headers.Append("Authorization", $"Bearer {cookie}");
      }

      await _next.Invoke(context);
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
    private IForumDal _forumDal;

    public TokenProviderMiddleware(RequestDelegate next, IOptions<TokenProviderOptions> options, IForumDal forumDal)
    {
      _next = next;
      _options = options.Value;
      _forumDal = forumDal;
    }

    public Task Invoke(HttpContext context)
    {
      // If the request path doesn't match, skip
      if (!context.Request.Path.Equals(_options.PEnginePath, StringComparison.OrdinalIgnoreCase)
        && !context.Request.Path.Equals(_options.ForumPath, StringComparison.OrdinalIgnoreCase)
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
        if (context.Request.Path.Equals(_options.ForumPath, StringComparison.OrdinalIgnoreCase))
        {
          return ValidateForumUser(context);
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
      await ValidatePEngineUser(context, userName, password, successUrl, failUrl, false);
    }

    private async Task RefreshToken(HttpContext context)
    {
      bool isTokenValid = false;
      if (context.Request?.HttpContext?.User != null && context.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        string userName = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
        string userType = context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;;
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
              await ValidatePEngineUser(context, userName, null, null, null, true);
              break;
            case "Forum":
              isTokenValid = true;
              await ValidateForumUser(context, userName, null, null, null, true);
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

    private async Task ValidatePEngineUser(HttpContext context, string userName, string password, string successUrl, string failUrl, bool refreshFlag)
    {
      var userId = string.Empty;
      var roleClaims = new List<string>();
      if (Settings.Current.UserNameAdmin.Equals(userName, StringComparison.OrdinalIgnoreCase) 
        && (refreshFlag || Security.HashAndCompare(password, Settings.Current.PasswordAdmin)))
      {
        roleClaims.Add("PEngineAdmin");
        userId = Settings.Current.UserNameAdmin;
      }

      ClaimsIdentity identity = null;
      if (!string.IsNullOrEmpty(userId))
      {
        identity = await GetIdentity(userId, userId, "PEngine", roleClaims.ToArray());
      }
      await GenerateToken(context, identity, userId, Settings.Current.TimeLimitAdminToken, successUrl, failUrl, refreshFlag);
    }

    private async Task ValidateForumUser(HttpContext context)
    {
      var userName = (string)context.Request.Form["username"] ?? string.Empty;
      var password = (string)context.Request.Form["password"] ?? string.Empty;
      var successUrl = context.Request.Form.ContainsKey("successUrl") ? (string)context.Request.Form["successUrl"] : (string)null;
      var failUrl = context.Request.Form.ContainsKey("failUrl") ? (string)context.Request.Form["failUrl"] : (string)null;
      await ValidateForumUser(context, userName, password, successUrl, failUrl, false);
    }

    private async Task ValidateForumUser(HttpContext context, string userName, string password, string successUrl, string failUrl, bool refreshFlag)
    {
      var roleClaims = new List<string>();
      ClaimsIdentity identity = null;
      var forumUser = await _forumDal.GetForumUserById(null, userName);
      var forumEnabled = !Settings.Current.DisableForum;
      if (forumEnabled && forumUser != null && (refreshFlag || Security.HashAndCompare(password, forumUser.Password)))
      {
        if (!forumUser.BanFlag)
        {
          roleClaims.Add("ForumUser");
          if (forumUser.AdminFlag)
          {
            roleClaims.Add("ForumAdmin");
          }
        }
        else
        {
          roleClaims.Add("ForumBanned");
        }
        identity = await GetIdentity(forumUser.Guid.ToString(), userName, "Forum", roleClaims.ToArray());
      }
      await GenerateToken(context, identity, forumUser?.Guid.ToString(), Settings.Current.TimeLimitForumToken, successUrl, failUrl, refreshFlag);
    }

    private async Task GenerateToken(HttpContext context, ClaimsIdentity identity, string userId, int expirationMinutes, string successUrl = null, string failUrl = null, bool refreshFlag = false)
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
          context.Response.Redirect($"{failUrl}?authFailed=true");
          return;
        }
      }
    
      var now = DateTime.UtcNow;
      var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);
  
      // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
      // You can add other claims here, if you want:
      var claims = new Claim[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
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

      if (string.IsNullOrWhiteSpace(successUrl))
      {
        if (HasJwtCookie(context))
        {
          AddJwtCookie(context, encodedJwt);
        }

        var response = new
        {
          access_token = encodedJwt,
          token_type = "Bearer",
          expires = (DateTime)expires,
          expires_in = (int)expirationMinutes * 60,
          expires_in_milliseconds = (long)expirationMinutes * 60 * 1000
        };
  
        // Serialize and return the response
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented })); 
      }
      else
      {
        AddJwtCookie(context, encodedJwt);
        context.Response.Redirect(successUrl);
      }
    }

    private bool HasJwtCookie(HttpContext context)
    {
      return context.Request.Cookies.Any(c => c.Key.Equals(Models.PEngineStateModel.COOKIE_ACCESS_TOKEN, StringComparison.OrdinalIgnoreCase));
    }

    private void AddJwtCookie(HttpContext context, string encodedJwt)
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
      context.Response.Cookies.Append(Models.PEngineStateModel.COOKIE_ACCESS_TOKEN, encodedJwt, cookieOptions);
    }

    private Task<ClaimsIdentity> GetIdentity(string userId, string userName, string userType, string[] roleClaims)
    {
      var identity = new System.Security.Principal.GenericIdentity(userId, "Token");
      var claims = roleClaims.Select(rc => new Claim(identity.RoleClaimType, rc)).ToList();
      claims.Add(new Claim("PEngineUserName", userName));
      claims.Add(new Claim("PEngineUserType", userType));
      return Task.FromResult(new ClaimsIdentity(identity, claims));
    }
  }
}
