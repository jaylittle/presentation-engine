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
  [ResponseCache(CacheProfileName = "None")]
  public class ArticlesController : Controller
  {
    private IArticleDal _articleDal;
    private IArticleService _articleService;
    public ArticlesController(IArticleDal articleDal, IArticleService articleService)
    {
      _articleDal = articleDal;
      _articleService = articleService;
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet]
    public async Task<IEnumerable<ArticleModel>> Get([FromQuery]PagingModel paging = null)
    {
      var articles = await _articleDal.ListArticles(null);
      return PagingUtils.Paginate(ref paging, articles);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet("/category/{category}")]
    public async Task<IEnumerable<ArticleModel>> Get(string category, [FromQuery]PagingModel paging = null)
    {
      var articles = await _articleDal.ListArticles(category);
      return PagingUtils.Paginate(ref paging, articles);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet("{guid}")]
    public async Task<IActionResult> GetByGuid(Guid guid)
    {
      var article = await _articleDal.GetArticleById(guid, null, null);
      return article != null ? (IActionResult) this.Ok(article) : this.NotFound();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost]
    public async Task<IActionResult> InsertArticle([FromBody]ArticleModel article)
    {
      var result = await _articleService.UpsertArticle(article);
      if (result.Successful)
      {
        return this.Ok(article);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPut]
    public async Task<IActionResult> UpdateArticle([FromBody]ArticleModel article)
    {
      return await InsertArticle(article);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeleteArticle(Guid guid)
    {
      await _articleDal.DeleteArticle(guid);
      return this.Ok();
    }
  }
}
