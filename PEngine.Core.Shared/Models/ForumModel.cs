using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ForumModel : IGuidModel, ITimestampModel, IUniqueNameModel, ISubTitleModel
  {
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool VisibleFlag { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      if (!inList)
      {
        return $"Forums - {Name}";
      }
      return $"Forums";
    }
  }
}
