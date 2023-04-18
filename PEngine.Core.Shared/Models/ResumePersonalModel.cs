using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ResumePersonalModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public long? LegacyID { get; set; }
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

    private List<string> _displayLabels = new List<string> {
      "Email", "Web", "Phone", "Fax", "Address", "Address 2", "City", "State", "Zip"
    };

    public List<KeyValuePair<string, string>> DisplayProperties
    {
      get
      {
        var displayValues = new List<string> {
          Email, WebsiteURL, Phone, Fax, Address1, Address2, City, State, Zip
        };

        return displayValues
          .Select((displayValue, index) => new KeyValuePair<string, string>(_displayLabels[index], displayValue))
          .Where(dv => !string.IsNullOrWhiteSpace(dv.Value))
          .ToList();
      }
    }
  }
}
