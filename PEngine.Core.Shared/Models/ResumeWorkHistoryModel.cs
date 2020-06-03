using System;
using Newtonsoft.Json;
using PEngine.Core.Shared.Interfaces;
using PEngine.Core.Shared.JsonConverters;

namespace PEngine.Core.Shared.Models
{
  public class ResumeWorkHistoryModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public long? LegacyID { get; set; }
    public string Employer { get; set; } = string.Empty;
    public string EmployerURL { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public string JobDescription { get; set; } = string.Empty;

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime? Started { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime? Completed { get; set; }

    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
