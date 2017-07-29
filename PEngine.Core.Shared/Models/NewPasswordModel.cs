using System;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.Models
{
  public class NewPasswordModel
  {
    public string Value { get; set; }
    public bool Reset { get; set; }
  }
}

