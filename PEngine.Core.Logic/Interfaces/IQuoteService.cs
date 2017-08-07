using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IQuoteService
  {
    Task<IEnumerable<QuoteModel>> Get();
    Task<QuoteModel> GetRandom();
  }
}