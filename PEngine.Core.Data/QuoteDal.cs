using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class QuoteDal : BaseDal<QuoteDal>, IQuoteDal
  {
    public IEnumerable<QuoteModel> ListQuotes()
    {
      using (var ct = GetConnection(DatabaseType.Misc, true))
      {
        return ct.DbConnection.Query<QuoteModel>(ReadQuery("ListQuotes", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
