using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IQuoteDal : IBaseDal
    {
      IEnumerable<QuoteModel> ListQuotes();
    }
}
