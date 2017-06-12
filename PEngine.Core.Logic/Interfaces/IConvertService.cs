using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IConvertService
  {
    bool ImportData(string contentRootPath, ref List<string> messages);
  }
}