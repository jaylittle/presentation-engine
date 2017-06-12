using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
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
        .UseIISIntegration()
        .UseStartup<Startup>()
        .Build();

      var launchKestrelFlag = true;

      if (args.Any(a => a.Equals("importdata", StringComparison.OrdinalIgnoreCase)))
      {
        Console.WriteLine("Import Data: Importing from PEV4 exports as Requested");
        Console.WriteLine("-------------------------------------");
        var convertService = new ConvertService(host.Services);
        var messages = new List<string>();
        var importSuccessful = convertService.ImportData(Directory.GetCurrentDirectory(), ref messages);
        launchKestrelFlag = launchKestrelFlag && importSuccessful;
        if (importSuccessful)
        {
          Console.WriteLine("Import Data: Succeeded!");
        }
        else
        {
          Console.WriteLine("Import Data: Failed!");
        }
        if (messages.Any())
        {
          foreach (var message in messages)
          {
            Console.WriteLine($"Import Data Message: {message}");
          }
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
