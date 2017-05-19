using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ArticleModel : IGuidModel, ITimestampModel, IUniqueNameModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string ContentURL { get; set; }
    public string DefaultSection { get; set; }
    public bool VisibleFlag { get; set; }
    public string UniqueName { get; set; }
    public bool HideDropDownFlag { get; set; }
    public bool HideButtonsFlag { get; set; }
    public string AdminPass { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
