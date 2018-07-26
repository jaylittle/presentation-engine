using System;
using System.Reflection;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using Newtonsoft.Json;

namespace PEngine.Core.Web.Models
{
  public class PEngineSettingsModel : SettingsData
  {
    public PEngineSettingsModel()
    {

    }
    public PEngineSettingsModel(SettingsData settings)
    {
      foreach (PropertyInfo prop in settings.GetType().GetProperties())
      {
        if (prop.GetMethod != null && prop.SetMethod != null)
        {
          prop.SetValue(this, prop.GetValue(settings, null), null);
        }
      }
    }

    public SettingsData Update(SettingsData current)
    {
      foreach (PropertyInfo prop in current.GetType().GetProperties())
      {
        if (prop.SetMethod != null && prop.GetMethod != null)
        {
          switch (prop.Name)
          {
            case "PasswordAdmin":
              string newPassword = null;
              if (NewPasswordAdmin.Reset)
              {
                newPassword = string.Empty;
              }
              else if (!string.IsNullOrWhiteSpace(NewPasswordAdmin.Value))
              {
                newPassword = NewPasswordAdmin.Value;
              }
              if (newPassword != null)
              {
                prop.SetValue(current, PEngine.Core.Shared.Security.Hash(newPassword));
              }
              break;
            case "SecretKey":
              //Skip it - never set this
              break;
            default:
              prop.SetValue(current, GetType().GetProperty(prop.Name).GetValue(this, null));
              break;
          }
          GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(current, null), null);
        }
      }
      return current;
    }

    [JsonIgnore]
    public override string PasswordAdmin { get; set; }

    [JsonIgnore]
    public override Guid SecretKey { get; set; }

    public NewPasswordModel NewPasswordAdmin { get; set; } = new NewPasswordModel();
  }
}