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
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers
{
  [Route("api/[controller]")]
  public class QuotesController : Controller
  {
    private IQuoteDal _quoteDal;
    public QuotesController(IQuoteDal quoteDal)
    {
      _quoteDal = quoteDal;
    }

    private List<QuoteModel> _quotes;
    private List<QuoteModel> Quotes
    {
      get
      {
        if (_quotes == null)
        {
          _quotes =_quoteDal.ListQuotes().ToList();
        }
        return _quotes;
      }
    }

    [HttpGet]
    public IEnumerable<QuoteModel> Get()
    {
      return Quotes;
    }

    [HttpGet("random")]
    public string GetRandom()
    {
      var rnd = new Random();
      return Quotes[rnd.Next(0, Quotes.Count - 1)].Data;
    }
  }
}
