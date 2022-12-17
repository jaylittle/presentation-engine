using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Web.Models
{
  public class PEngineGenericRecordModel<T> where T: ISubTitleModel
  {
    public PEngineGenericRecordModel(IServiceProvider svp, HttpContext context, bool hideSubTitle = false, T recordData = default(T), string currentSection = null, int? currentPage = null)
    {
      Settings = PEngine.Core.Shared.Settings.Current;
      State = new PEngineStateModel(svp, Settings, context, hideSubTitle, recordData, currentSection, currentPage);
      RecordData = recordData;
    }
    public PEngineStateModel State { get; set; }
    public SettingsData Settings { get; set; }
    private T _recordData;
    public T RecordData
    {
      get
      {
        return _recordData;
      }
      set
      {
        _recordData = value;
        State.UpdateData(_recordData);
      }
    }
  }
}