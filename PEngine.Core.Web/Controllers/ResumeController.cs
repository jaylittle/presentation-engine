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

namespace PEngine.Core.Web.Controllers
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
        public ResumeModel Get()
        {
          return _resumeService.GetResume();
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpPost]
        public IActionResult InsertResume([FromBody]ResumeModel resume)
        {
          var errors = new List<string>();
          if (_resumeService.UpsertResume(resume, ref errors))
          {
            return this.Ok(resume);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpPut]
        public IActionResult UpdateResume([FromBody]ResumeModel resume)
        {
          return InsertResume(resume);
        }
    }
}
