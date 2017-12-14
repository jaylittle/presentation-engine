using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumeSkillModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public long? LegacyID { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Hint { get; set; } = string.Empty;
    public int Order { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
