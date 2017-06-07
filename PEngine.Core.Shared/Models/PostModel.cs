using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class PostModel : IGuidModel, ITimestampModel, IUniqueNameModel, ISubTitleModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
    public string IconFileName { get; set; }
    public bool VisibleFlag { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    public int CreatedMonth
    {
      get
      {
        return CreatedUTC?.Month ?? DateTime.UtcNow.Month;
      }
    }

    public int CreatedYear
    {
      get
      {
        return CreatedUTC?.Year ?? DateTime.UtcNow.Year;
      }
    }

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      if (!inList)
      {
        return $"{Name}";
      }
      if (currentPage.HasValue)
      {
        return $"Archived Posts - Page {currentPage.Value}";
      }
      else
      {
        return $"Archived Posts";
      }
    }
  }
}
