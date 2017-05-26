using System;
using PEngine.Core.Shared.Interfaces;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.Models
{
  public class ForumUserModel : IGuidModel, ITimestampModel
  {
    public Guid Guid { get; set; }
    public string UserId { get; set; }
    public bool AdminFlag { get; set; }
    public bool BanFlag { get; set; }
    public string Email { get; set; }
    public string Website { get; set; }
    public string Comment { get; set; }
    public string LastIPAddress { get; set; }
    public DateTime? LastLogon { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    [JsonIgnore]
    public string Password { get; set; }
    public NewPasswordModel NewPassword { get; set; } = new NewPasswordModel();
  }
}
