using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class QuoteModel : IGuidModel
  {
    public Guid Guid { get; set; }
    public int? LegacyID { get; set; }
    public string Data { get; set; }
  }
}
