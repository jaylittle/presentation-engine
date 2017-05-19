using System;
using System.Text;
using System.Text.RegularExpressions;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared
{
  public static class Extensions
  {
    public static void UpdateTimestamps(this ITimestampModel record, bool newRecord = false)
    {
      var currentTime = DateTime.UtcNow;
      if (newRecord)
      {
        record.CreatedUTC = currentTime;
      }
      if (!record.ModifiedUTC.HasValue)
      {
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

    public static string GenerateUniqueName(this IUniqueNameModel record)
    {
      if (string.IsNullOrWhiteSpace(record.UniqueName) && !string.IsNullOrWhiteSpace(record.Name))
      {
        var name = Encoding.ASCII.GetString(Encoding.GetEncoding("Cyrillic").GetBytes(record.Name));
        var sb = new StringBuilder(Regex.Replace(name, @"[^\w ]", "").Trim());
        sb.Replace(" ", "-");
        sb.Replace("--", "-");
        return sb.ToString().ToLower();
      }
      return null;
    }
  }
}