using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Models;

namespace PEngine.Core.Web.Controllers
{
  [Route("hack")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class HackAttemptController : Controller
  {
    private IServiceProvider _svp;

    public HackAttemptController(IServiceProvider svp)
    {
      _svp = svp;
    }

    [HttpGet("detected/{hackTypeString}")]
    [HttpHead("detected{hackTypeString}")]
    public IActionResult HackDetected(string hackTypeString)
    {
      HackAttemptType hackType = HackAttemptType.Generic;
      Enum.TryParse<HackAttemptType>(hackTypeString, true, out hackType);
      string hackAttemptMessage = Settings.Current.HackAttemptMessage;
      PEnginePage page = PEnginePage.HackAttempt;

      var hackAttempt = new HackAttemptModel(Settings.Current.HackAttemptTitle, hackAttemptMessage, hackType);
      var model = new PEngineGenericRecordModel<HackAttemptModel>(_svp, HttpContext, page, false, hackAttempt);

      return View("View", model);
    }

    
  }
}