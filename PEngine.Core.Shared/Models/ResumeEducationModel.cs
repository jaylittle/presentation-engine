using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumeEducationModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Institute { get; set; } = string.Empty;
    public string InstituteURL { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Completed { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
