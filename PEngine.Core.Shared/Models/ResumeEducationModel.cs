using System;
using Newtonsoft.Json;
using PEngine.Core.Shared.Interfaces;
using PEngine.Core.Shared.JsonConverters;

namespace PEngine.Core.Shared.Models
{
  public class ResumeEducationModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public long? LegacyID { get; set; }
    public string Institute { get; set; } = string.Empty;
    public string InstituteURL { get; set; } = string.Empty;
    public string Program { get; set; } = string.Empty;

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime? Started { get; set; }

    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime? Completed { get; set; }

    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
