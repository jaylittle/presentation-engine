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
  [ResponseCache(CacheProfileName = "None")]
  public class ResumeController : Controller
  {
    private IServiceProvider _svp;
    private IResumeDal _resumeDal;
    private IResumeService _resumeService;

    public ResumeController(IServiceProvider svp, IResumeDal resumeDal, IResumeService resumeService)
    {
      _svp = svp;
      _resumeDal = resumeDal;
      _resumeService = resumeService;
    }

    public async Task<IActionResult> Index()
    {
      var model = new PEngineGenericRecordModel<ResumeModel>(_svp, HttpContext, false);
      if (!Settings.Current.DisableResume)
      {
        model.RecordData = await _resumeService.GetResume();
        if (model.RecordData != null)
        {
          return View(model);
        }
      }
      return model.State.HasAdmin ? (IActionResult)this.Redirect(Settings.Current.BasePath) : this.NotFound();
    }
  }
}