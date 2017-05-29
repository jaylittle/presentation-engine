using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
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
    public string Path { get; set; } = "/token";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public TimeSpan Expiration { get; set; } = TimeSpan.FromMinutes(15);
    public SigningCredentials SigningCredentials { get; set; }
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
      if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
      {
        return _next(context);
      }

      // Request must be POST with Content-Type: application/x-www-form-urlencoded
      if (!context.Request.Method.Equals("POST")
        || !context.Request.HasFormContentType)
      {
        context.Response.StatusCode = 400;
        return context.Response.WriteAsync("Bad request.");
      }

      return GenerateToken(context);
    }

    private async Task GenerateToken(HttpContext context)
    {
      var username = context.Request.Form["username"];
      var password = context.Request.Form["password"];
  
      var identity = await GetIdentity(username, password);
      if (identity == null)
      {
        context.Response.StatusCode = 400;
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
        expires: now.Add(_options.Expiration),
        signingCredentials: _options.SigningCredentials);
      var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
    
      var response = new
      {
        access_token = encodedJwt,
        expires_in = (int)_options.Expiration.TotalSeconds
      };
  
      // Serialize and return the response
      context.Response.ContentType = "application/json";
      await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
    }

    private Task<ClaimsIdentity> GetIdentity(string username, string password)
    {
      // DON'T do this in production, obviously!
      if (username == "admin" && password == "pengine")
      {
        var identity = new System.Security.Principal.GenericIdentity(username, "Token");
        return Task.FromResult(new ClaimsIdentity(identity, new Claim[] 
        {
          new Claim(identity.RoleClaimType, "Admin")
        }));
      }
  
      // Credentials are invalid, or account doesn't exist
      return Task.FromResult<ClaimsIdentity>(null);
    }
  }
}
