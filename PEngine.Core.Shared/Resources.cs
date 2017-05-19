using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.IO;
using System.Xml.Linq;
using System.Reflection;

namespace PEngine.Core.Shared
{
  public class Resources
  {
    private static ConcurrentDictionary<string, byte[]> _resourceCache = new ConcurrentDictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);

    public static byte[] ReadBinaryResource<TService>(string resourceName)
    {
      return GetResourceBytes<TService>(resourceName);
    }

    public static XDocument ReadXmlResource<TService>(string resourceName)
    {
      using (MemoryStream resourceData = GetResourceStream<TService>(resourceName))
      {
        return XDocument.Load(resourceData);
      }
    }

    public static IEnumerable<string> ReadMultiLineResource<TService>(string resourceName)
    {
      using (MemoryStream resourceData = GetResourceStream<TService>(resourceName))
      {
        return ReadAllLinesFromStream(resourceData);
      }
    }

    public static string ReadTextResource<TService>(string resourceName)
    {
      using (MemoryStream resourceData = GetResourceStream<TService>(resourceName))
      {
        return ReadAllTextFromStream(resourceData);
      }
    }

    public static string ReadQueryResource<TService>(string queryName)
    {
      using (MemoryStream resourceData = GetResourceStream<TService>(queryName + ".sql"))
      {
        var lines = ReadAllLinesFromStream(resourceData);
        return string.Join(System.Environment.NewLine, lines);
      }
    }

    private static MemoryStream GetResourceStream<TService>(string resourceName)
    {
      return new MemoryStream(GetResourceBytes<TService>(resourceName));
    }

    private static byte[] GetResourceBytes<TService>(string resourceName)
    {
      Type type = typeof(TService);

      string actualResourceName = $"{type.Namespace}.{type.Name}.{resourceName}";

      if (!_resourceCache.ContainsKey(actualResourceName))
      {
        using (Stream stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(actualResourceName))
        {
          using (var result = new MemoryStream())
          {
            stream.CopyTo(result);
            var byteArray = result.ToArray();
            while(!_resourceCache.ContainsKey(actualResourceName) && !_resourceCache.TryAdd(actualResourceName, byteArray));
            return byteArray;
          }
        }
      }
      else
      {
        return _resourceCache[actualResourceName];
      }
    }

    private static IEnumerable<string> ReadAllLinesFromStream(MemoryStream stream)
    {
      using (StreamReader reader = new StreamReader(stream))
      {
        string line;

        while ((line = reader.ReadLine()) != null)
        {
          yield return line;
        }
      }
    }

    private static string ReadAllTextFromStream(MemoryStream stream)
    {
      using (StreamReader reader = new StreamReader(stream))
      {
        return reader.ReadToEnd();
      }
    }
  }
}