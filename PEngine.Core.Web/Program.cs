using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PEngine.Core.Logic;
using Serilog;
using Serilog.Events;
using Microsoft.Extensions.Hosting;

namespace PEngine.Core.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine($"Entry Assembly Location: {System.Reflection.Assembly.GetEntryAssembly().Location}");
      
      var isDevMode = (args != null && args.Contains("--dev", StringComparer.OrdinalIgnoreCase));
      var contentRoot = Directory.GetCurrentDirectory();
      var logRoot = contentRoot + (contentRoot.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())
          ? string.Empty
          : System.IO.Path.DirectorySeparatorChar.ToString())
            + "logs" + System.IO.Path.DirectorySeparatorChar.ToString();

      if (!System.IO.Directory.Exists(logRoot))
      {
        System.IO.Directory.CreateDirectory(logRoot);
      }

      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      var logConfig = new LoggerConfiguration();
      if (environment == Environments.Development)
      {
        logConfig.MinimumLevel.Debug();
      }
      else
      {
        logConfig.MinimumLevel.Information();
      }
      var logger = logConfig.MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.File($"{logRoot}{System.IO.Path.DirectorySeparatorChar}PEngineLog-.txt", rollingInterval: RollingInterval.Day);
      
      if (isDevMode)
      {
        logger.WriteTo.Console();
      }

      Log.Logger = logger.CreateLogger();

      Log.Information("Configuring Host");
      IHost host = null;
      try
      {
        host = new HostBuilder()
          .UseContentRoot(contentRoot)
          .ConfigureAppConfiguration((hostingContext, config) =>
          {
            var env = hostingContext.HostingEnvironment;
            Console.WriteLine($"Hosting Environment: {env.EnvironmentName}");
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", 
                      optional: true, reloadOnChange: false);
            config.AddEnvironmentVariables();
          })
          .ConfigureWebHostDefaults(webHostBuilder => {
            if (isDevMode)
            {
              webHostBuilder = webHostBuilder
                .UseKestrel()
                .UseUrls("http://*:5000");
            }
            else
            {
              webHostBuilder = webHostBuilder
                .UseIISIntegration()
                .UseUrls("http://*:80");;
            }

            webHostBuilder
              .UseStartup<Startup>()
              .UseDefaultServiceProvider(options => options.ValidateScopes = false);
          })
          .UseSerilog()
          .Build();
      }
      catch (Exception ex)
      {
        throw new Exception("Host Configuration Failed!", ex);
      }

      var launchKestrelFlag = true;

      if (args.Any(a => a.Equals("importdata", StringComparison.OrdinalIgnoreCase)))
      {
        Console.WriteLine("Import Data: Importing from PEV4 exports as Requested");
        Console.WriteLine("-------------------------------------");
        var convertService = new ConvertService(host.Services);
        var importResults = convertService.ImportData(Directory.GetCurrentDirectory()).Result;
        launchKestrelFlag = launchKestrelFlag && importResults.Successful;
        if (importResults.Successful)
        {
          Console.WriteLine("Import Data: Succeeded!");
        }
        else
        {
          Console.WriteLine("Import Data: Failed!");
        }
        Console.WriteLine("-------------------------------------");
      }

      if (launchKestrelFlag)
      {
        host.Run();
      }
    }
  }
}
