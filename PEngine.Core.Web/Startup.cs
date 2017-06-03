﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Data.Providers;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Web
{
  public class Startup
  {
    private static IHttpContextAccessor _httpContextAccessor;
    public static HttpContext HttpContext
    {
      get
      {
        return _httpContextAccessor.HttpContext;
      }
    }

    public static string ContentRootPath { get; private set; }

    public Startup(IHostingEnvironment env)
    {
      var builder = new ConfigurationBuilder()
        .SetBasePath(env.ContentRootPath)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
        .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add framework services.
      services.AddMvc();
      services.AddScoped<IPostDal, PostDal>();
      services.AddScoped<IArticleDal, ArticleDal>();
      services.AddScoped<IResumeDal, ResumeDal>();
      services.AddScoped<IForumDal, ForumDal>();
      services.AddScoped<IQuoteDal, QuoteDal>();
      services.AddScoped<IVersionDal, VersionDal>();
      services.AddScoped<ISettingsProvider, SettingsProvider>();
      services.AddScoped<IPostService, PostService>();
      services.AddScoped<IArticleService, ArticleService>();
      services.AddScoped<IResumeService, ResumeService>();
      services.AddScoped<IForumService, ForumService>();
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider svp)
    {
      _httpContextAccessor = svp.GetRequiredService<IHttpContextAccessor>();
      ContentRootPath = env.ContentRootPath;
      
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      PEngine.Core.Shared.Settings.Startup(env.ContentRootPath);

      //TODO Generate secret key on first run of app and store in PEngine settings
      var secretKey = PEngine.Core.Shared.Settings.Current.SecretKey.ToString();
      var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

      // Add JWT generation
      var providerOptions = new TokenProviderOptions
      {
        Audience = PEngine.Core.Shared.Settings.Current.DefaultTitle,
        Issuer = PEngine.Core.Shared.Settings.Current.DefaultTitle,
        SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
      };
      app.UseMiddleware<TokenProviderMiddleware>(Options.Create(providerOptions));

      // Add Support for JWTs passed in cookies
      var cookieOptions = new TokenCookieOptions
      {
        CookieName = "access_token"
      };
      app.UseMiddleware<TokenCookieMiddleware>(Options.Create(cookieOptions));

      // Add JWT authorization
      var tokenValidationParameters = new TokenValidationParameters
      {
        // The signing key must match!
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
    
        // Validate the JWT Issuer (iss) claim
        ValidateIssuer = true,
        ValidIssuer = PEngine.Core.Shared.Settings.Current.DefaultTitle,
    
        // Validate the JWT Audience (aud) claim
        ValidateAudience = true,
        ValidAudience = PEngine.Core.Shared.Settings.Current.DefaultTitle,
    
        // Validate the token expiry
        ValidateLifetime = true,
    
        // If you want to allow a certain amount of clock drift, set that here:
        ClockSkew = TimeSpan.Zero
      };

      app.UseJwtBearerAuthentication(new JwtBearerOptions
      {
        AutomaticAuthenticate = true,
        AutomaticChallenge = true,
        TokenValidationParameters = tokenValidationParameters
      });

      app.UseMvc();

      app.UseStaticFiles();

      PEngine.Core.Data.Database.Startup(env.ContentRootPath, new SQLiteDataProvider());
    }
  }
}
