using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Web.Models
{
  public class PEngineGenericListModel<T> 
  {
    public PEngineGenericListModel(HttpRequest request, IEnumerable<T> listData = null)
    {
      Settings = PEngine.Core.Shared.Settings.Current;
      State = new PEngineState(Settings, request, listData);
      ListData = listData;
    }
    public PEngineState State { get; set; }
    public SettingsData Settings { get; set; }
    private IEnumerable<T> _listData;
    public IEnumerable<T> ListData
    {
      get
      {
        return _listData;
      }
      set
      {
        _listData = value;
        State.UpdateData(_listData);
      }
    }
  }
}