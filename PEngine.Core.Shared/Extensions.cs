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
  }
}