using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumeWorkHistoryModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public Int32? LegacyID { get; set; }
    public string Employer { get; set; } = string.Empty;
    public string EmployerURL { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;
    public DateTime? Started { get; set; }
    public DateTime? Completed { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
