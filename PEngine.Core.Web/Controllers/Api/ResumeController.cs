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
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers.Api
{
  [Route("api/[controller]")]
  public class ResumeController : Controller
  {
    private IResumeService _resumeService;
    public ResumeController(IResumeService resumeService)
    {
      _resumeService = resumeService;
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet]
    public async Task<ResumeModel> Get()
    {
      return await _resumeService.GetResume();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost]
    public async Task<IActionResult> InsertResume([FromBody]ResumeModel resume)
    {
      var result = await _resumeService.UpsertResume(resume);
      if (result.Successful)
      {
        return this.Ok(resume);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPut]
    public async Task<IActionResult> UpdateResume([FromBody]ResumeModel resume)
    {
      return await InsertResume(resume);
    }
  }
}
