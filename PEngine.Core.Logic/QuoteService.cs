using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Logic
{
  public class QuoteService : IQuoteService
  {
    private IQuoteDal _quoteDal;
    
    public QuoteService(IQuoteDal quoteDal)
    {
      _quoteDal = quoteDal;
    }

    public async Task<IEnumerable<QuoteModel>> Get()
    {
      return await GetQuotes();
    }

    public async Task<QuoteModel> GetRandom()
    {
      var rnd = new Random();
      var quotes= await GetQuotes();
      return quotes[rnd.Next(0, quotes.Count - 1)];
    }

    private static List<QuoteModel> _quotes = new List<QuoteModel>();
    private static bool _quotesLoaded = false;
    private async Task<List<QuoteModel>> GetQuotes()
    {
      if (!_quotesLoaded)
      {
        var newQuotes = (await _quoteDal.ListQuotes());
        lock (_quotes)
        {
          if (!_quotesLoaded)
          {
            _quotesLoaded = true;
            _quotes = newQuotes.ToList();
          }
        }
      }
      return _quotes;
    }
  }
}