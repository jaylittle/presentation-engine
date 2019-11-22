using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Linq;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared
{
  public static class Extensions
  {
    public static void UpdateTimestamps(this ITimestampModel record, bool newRecord = false, bool importFlag = false)
    {
      if (!importFlag)
      {
        var currentTime = DateTime.UtcNow;
        if (newRecord)
        {
          record.CreatedUTC = currentTime;
        }
        record.ModifiedUTC = currentTime;
      }
    }

    public static void UpdateGuid(this IGuidModel record)
    {
      if (Guid.Empty.Equals(record.Guid))
      {
        record.Guid = Guid.NewGuid();
      }
    }

    public static void GenerateUniqueName(this IUniqueNameModel record, Dictionary<string, bool> existingUniqueNames)
    {
      if (string.IsNullOrWhiteSpace(record.UniqueName) && !string.IsNullOrWhiteSpace(record.Name))
      {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        var name = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(record.Name));
        var sb = new StringBuilder(Regex.Replace(name, @"[^\w ]", "").Trim());
        sb.Replace(" ", "-");
        sb.Replace("--", "-");
        var baseUniqueName = sb.ToString().ToLower();
        var uniqueNameCounter = 0;
        var currentUniqueName = baseUniqueName;
        while (existingUniqueNames != null && existingUniqueNames.ContainsKey(currentUniqueName))
        {
          uniqueNameCounter++;
          currentUniqueName = $"{baseUniqueName}{uniqueNameCounter}";
        }
        record.UniqueName = currentUniqueName;
      }
    }

    public static string GetChildElementValue(this XElement parentElement, string childElementName, bool convertEmptyToNull = false)
    {
      var retvalue = parentElement.Descendants().FirstOrDefault(d => d.Name.LocalName.Equals(childElementName, StringComparison.OrdinalIgnoreCase))?.Value;
      return (convertEmptyToNull && retvalue != null && retvalue == string.Empty) ? null : retvalue;
    }

    public static string DataTruncate(this string data, int length = 75)
    {
      if (length > 0)
      {
        if (data.Length > length)
        {
          return data.Substring(0, length);
        }
      }
      else
      {
        string[] delimiters = { Environment.NewLine, "\n", "<br>", "</p>" };
        int strptr = -1;
        int lastptr = -1;
        int cycle = 0;
        int maxCycle = length * -1;
        int dptr = 0;
        while (cycle < maxCycle)
        {
          while (strptr <= lastptr + 1 && dptr < delimiters.Length)
          {
            strptr = data.IndexOf(delimiters[dptr], lastptr + 1);
            dptr++;
          }
          if (strptr > lastptr)
          {
            lastptr = strptr;
          }
          else
          {
            strptr = lastptr;
          }
          cycle++;
        }
        if (strptr >= 0)
        {
          return data.Substring(0, strptr);
        }
      }
      return data;
    }

    public static bool IsUrlAbsolute(this string url)
    {
      return Helpers.IsUrlAbsolute(url);
    }

    //Shamelessly borrowed from: https://stackoverflow.com/questions/43021/how-do-you-get-the-index-of-the-current-iteration-of-a-foreach-loop#comment79585489_39997157
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
    {
      return self?.Select((item, index) => (item, index)) ?? new List<(T, int)>();
    }
  }
}