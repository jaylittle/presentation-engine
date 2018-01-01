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
  [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
  public class QuotesController : Controller
  {
    private IQuoteService _quoteService;
    public QuotesController(IQuoteService quoteService)
    {
      _quoteService = quoteService;
    }

    [HttpGet]
    public async Task<IEnumerable<QuoteModel>> Get([FromQuery]PagingModel paging = null)
    {
      var quotes = await _quoteService.Get();
      return PagingUtils.Paginate(ref paging, quotes);
    }

    [HttpGet("random")]
    public async Task<string> GetRandom()
    {
      return (await _quoteService.GetRandom()).Data;
    }
  }
}
