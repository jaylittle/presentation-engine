using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Data.Providers;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;

namespace PEngine.Core.Web
{
  public class Startup
  {
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
      services.AddScoped<IVersionDal, VersionDal>();
      services.AddScoped<IPostService, PostService>();
      services.AddScoped<IArticleService, ArticleService>();
      services.AddScoped<IResumeService, ResumeService>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddConsole(Configuration.GetSection("Logging"));
      loggerFactory.AddDebug();

      app.UseMvc();

      PEngine.Core.Data.Database.Startup(env.ContentRootPath, new SQLiteDataProvider());
    }
  }
}
