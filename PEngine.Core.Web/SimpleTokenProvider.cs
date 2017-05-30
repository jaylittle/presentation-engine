using System;
using System.Linq;
using System.Collections.Generic;
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
using Newtonsoft.Json;

namespace PEngine.Core.Web
{
  public class TokenProviderOptions
  {
    public string PEnginePath { get; set; } = "/token/pengine";
    public string ForumPath { get; set; } = "/token/forum";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public SigningCredentials SigningCredentials { get; set; }
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
        && !context.Request.Path.Equals(_options.ForumPath, StringComparison.OrdinalIgnoreCase))
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
      
      context.Response.StatusCode = 400;
      return context.Response.WriteAsync("Bad request.");
    }

    private async Task ValidatePEngineUser(HttpContext context)
    {
      var password = (string)context.Request.Form["password"] ?? string.Empty;
      var username = string.Empty;
      var roleClaims = new List<string>();
      if (Security.EncryptAndCompare(password, Settings.Current.PasswordGod))
      {
        roleClaims.Add("PEngineGod");
        roleClaims.Add("PEngineAdmin");
        username = "PEngineGod";
      }
      else if (Security.EncryptAndCompare(password, Settings.Current.PasswordAdmin))
      {
        roleClaims.Add("PEngineAdmin");
        username = "PEngineAdmin";
      }

      ClaimsIdentity identity = null;
      if (!string.IsNullOrEmpty(username))
      {
        identity = await GetIdentity(username, roleClaims.ToArray());
      }
      await GenerateToken(context, identity, username, Settings.Current.TimeLimitAdminToken);
    }

    private async Task ValidateForumUser(HttpContext context)
    {
      var username = (string)context.Request.Form["username"] ?? string.Empty;
      var password = (string)context.Request.Form["password"] ?? string.Empty;
      var roleClaims = new List<string>();

      ClaimsIdentity identity = null;
      var forumUser = _forumDal.GetForumUserById(null, username);
      if (forumUser != null && Security.EncryptAndCompare(password, forumUser.Password))
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
        identity = await GetIdentity(username, roleClaims.ToArray());
      }
      await GenerateToken(context, identity, username, Settings.Current.TimeLimitForumToken);
    }

    private async Task GenerateToken(HttpContext context, ClaimsIdentity identity, string username, int expirationMinutes)
    {
      if (identity == null)
      {
        context.Response.StatusCode = 401;
        await context.Response.WriteAsync("Invalid username or password.");
        return;
      }
    
      var now = DateTime.UtcNow;
  
      // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
      // You can add other claims here, if you want:
      var claims = new Claim[]
      {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
      };
    
      // Create the JWT and write it to a string
      var jwt = new JwtSecurityToken(
        issuer: _options.Issuer,
        audience: _options.Audience,
        claims: claims.Union(identity.Claims),
        notBefore: now,
        expires: now.AddMinutes(expirationMinutes),
        signingCredentials: _options.SigningCredentials);
      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    
      var response = new
      {
        access_token = encodedJwt,
        expires_in = (int)expirationMinutes * 60
      };
  
      // Serialize and return the response
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
    }

    private Task<ClaimsIdentity> GetIdentity(string username, string[] roleClaims)
    {
      var identity = new System.Security.Principal.GenericIdentity(username, "Token");
      var claims = roleClaims.Select(rc => new Claim(identity.RoleClaimType, rc)).ToArray();
      return Task.FromResult(new ClaimsIdentity(identity, claims));
    }
  }
}
