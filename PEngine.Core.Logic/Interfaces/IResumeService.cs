using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IResumeService
  {
    ResumeModel GetResume();
    bool UpsertResume(ResumeModel resume, ref List<string> errors);
  }
}