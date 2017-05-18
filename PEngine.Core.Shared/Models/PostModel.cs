using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class PostModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Title { get; set; }
    public string Data { get; set; }
    public string IconFileName { get; set; }
    public bool VisibleFlag { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
