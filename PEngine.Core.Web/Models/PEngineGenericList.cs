using System;
using System.Collections.Generic;
using PEngine.Core.Shared;

namespace PEngine.Core.Web.Models
{
  public class PEngineGenericList<T> where T: new()
  {
    public SettingsData Settings { get; set; }
    public IEnumerable<T> Data { get; set; }
  }
}