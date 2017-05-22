using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumeObjectiveModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Data { get; set; } = string.Empty;
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
