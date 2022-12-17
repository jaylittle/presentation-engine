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
  [Route("error")]
  [ResponseCache(CacheProfileName = "None")]
  [ApiExplorerSettings(IgnoreApi = true)]
  public class ErrorController : Controller
  {
    private IServiceProvider _svp;

    public ErrorController(IServiceProvider svp)
    {
      _svp = svp;
    }

    [HttpGet("{errorCode}")]
    public IActionResult Error(string errorCode)
    {
      string errorMessage = Settings.Current.ErrorMessageException;

      switch (errorCode ?? string.Empty)
      {
        case "404":
          errorMessage = Settings.Current.ErrorMessageNotFound;
          break;
        case "401":
          errorMessage = Settings.Current.ErrorMessageForbidden;
          break;
      }

      var error = new ErrorModel(Settings.Current.ErrorMessageTitle, errorMessage, errorCode);
      var model = new PEngineGenericRecordModel<ErrorModel>(_svp, HttpContext, false, error);

      return View("View", model);
    }
  }
}