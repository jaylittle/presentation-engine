using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;
using PEngine.Core.Web.Constraints;
using PEngine.Core.Web.Models;

namespace PEngine.Core.Web.Controllers.Api
{
  [Route("api/[controller]")]
  public class SettingsController : Controller
  {
    private ISettingsProvider _settingsProvider;
    public SettingsController(ISettingsProvider settingsProvider)
    {
      _settingsProvider = settingsProvider;
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet]
    public SettingsData Get()
    {
      return new PEngineSettingsModel(_settingsProvider.Current);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost]
    public IActionResult UpdateSettings([FromBody]PEngineSettingsModel settings)
    {
      var newSettings = settings.Update(_settingsProvider.Current);
      _settingsProvider.Current = newSettings;
      return this.Ok(new PEngineSettingsModel(newSettings));
    }
  }
}
