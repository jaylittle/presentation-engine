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

namespace PEngine.Core.Web
{
  public class Program
  {
    public static void Main(string[] args)
    {
      Console.WriteLine($"Entry Assembly Location: {System.Reflection.Assembly.GetEntryAssembly().Location}");
      
      var host = new WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((hostingContext, config) =>
        {
            var env = hostingContext.HostingEnvironment;
            Console.WriteLine($"Hosting Environment: {env.EnvironmentName}");
            config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                  .AddJsonFile($"appsettings.{env.EnvironmentName}.json", 
                      optional: true, reloadOnChange: true);
            config.AddEnvironmentVariables();
        })
        .UseIISIntegration()
        .UseStartup<Startup>()
        .ConfigureLogging((hostingContext, logging) => {
          logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
          logging.AddConsole();
          logging.AddDebug();
        })
        .Build();

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
