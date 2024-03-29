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
    [HttpHead("{errorCode}")]
    public IActionResult Error(string errorCode)
    {
      string errorMessage = Settings.Current.ErrorMessageException;
      PEnginePage page = PEnginePage.Error;

      switch (errorCode ?? string.Empty)
      {
        case "404":
          errorMessage = Settings.Current.ErrorMessageNotFound;
          page = PEnginePage.NotFound;
          break;
        case "401":
          errorMessage = Settings.Current.ErrorMessageForbidden;
          page = PEnginePage.Forbidden;
          break;
      }

      var error = new ErrorModel(Settings.Current.ErrorMessageTitle, errorMessage, errorCode);
      var model = new PEngineGenericRecordModel<ErrorModel>(_svp, HttpContext, page, false, error);

      return View("View", model);
    }
  }
}