using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IConvertService
  {
    Task<OpResult> ImportData(string contentRootPath);
  }
}