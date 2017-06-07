using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Interfaces;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Web.Models
{
  public class PEngineGenericListModel<T> where T: ISubTitleModel
  {
    public PEngineGenericListModel(HttpContext context, bool hideSubTitle = false, bool isForum = false, IEnumerable<T> listData = null, string currentSection = null, int? currentPage = null)
    {
      Settings = PEngine.Core.Shared.Settings.Current;
      State = new PEngineStateModel(Settings, context, hideSubTitle, isForum, (IEnumerable<ISubTitleModel>)listData, currentSection, currentPage);
      ListData = listData;
    }
    public PEngineStateModel State { get; set; }
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
        State.UpdateData((IEnumerable<ISubTitleModel>)_listData);
      }
    }
  }
}