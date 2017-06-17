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
using PEngine.Core.Shared;
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers.Api
{
  [Route("api/[controller]")]
  public class QuotesController : Controller
  {
    private IQuoteDal _quoteDal;
    public QuotesController(IQuoteDal quoteDal)
    {
      _quoteDal = quoteDal;
      _quotes = _quoteDal.ListQuotes().Result.ToList();
    }

    private static List<QuoteModel> _quotes;
    private async Task<List<QuoteModel>> GetQuotes()
    {
      if (_quotes == null)
      {
        _quotes = (await _quoteDal.ListQuotes()).ToList();
      }
      return _quotes;
    }

    [HttpGet]
    public async Task<IEnumerable<QuoteModel>> Get([FromQuery]PagingModel paging = null)
    {
      var quotes = await GetQuotes();
      return PagingUtils.Paginate(paging, quotes);
    }

    [HttpGet("random")]
    public async Task<string> GetRandom()
    {
      var rnd = new Random();
      var quotes= await GetQuotes();
      return quotes[rnd.Next(0, quotes.Count - 1)].Data;
    }
  }
}
