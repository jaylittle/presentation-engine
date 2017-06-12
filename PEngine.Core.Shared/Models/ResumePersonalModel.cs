using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumePersonalModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public Int32? LegacyID { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Address1 { get; set; } = string.Empty;
    public string Address2 { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Fax { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WebsiteURL { get; set; } = string.Empty;
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
