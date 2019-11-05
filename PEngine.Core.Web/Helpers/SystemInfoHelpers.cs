using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PEngine.Core.Web.Helpers
{
  public class SystemInfoHelpers
  {
    public static void Init(string contentRootPath)
    {
      ContentRootPath = contentRootPath;
      var versionPath = $"{ContentRootPath}version.txt";
      Version = "pengine-core-unknown-debug";
      if (System.IO.File.Exists(versionPath))
      {
        using (var reader = System.IO.File.Open(versionPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        using (var textReader = new StreamReader(reader))
        {
          if (!textReader.EndOfStream)
          {
            var version = textReader.ReadLine();
            if (!string.IsNullOrWhiteSpace(version))
            {
              Version = version;
            }
          }
        }
      }
    }

    public static string Version { get; private set; }

    private static string _contentRootPath;

    public static string ContentRootPath
    {
      get { return _contentRootPath; }
      private set
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString())
          ? string.Empty
          : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }
  }
}