using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IQuoteDal : IBaseDal
    {
      Task<IEnumerable<QuoteModel>> ListQuotes();
    }
}
