using System;
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
  }
}