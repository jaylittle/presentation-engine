using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class QuoteDal : BaseDal<QuoteDal>, IQuoteDal
  {
    public async Task<IEnumerable<QuoteModel>> ListQuotes()
    {
      using (var ct = GetConnection(DatabaseType.Misc, true))
      {
        return await ct.DbConnection.QueryAsync<QuoteModel>(ReadQuery("ListQuotes", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
