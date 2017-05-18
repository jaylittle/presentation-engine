using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

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

      host.Run();
    }
  }
}
