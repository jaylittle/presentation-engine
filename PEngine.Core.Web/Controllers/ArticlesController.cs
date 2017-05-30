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
        public IEnumerable<ArticleModel> Get()
        {
          return _articleDal.ListArticles();
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
          var article = _articleDal.GetArticleById(guid, null, null);
          return article != null ? (IActionResult) this.Ok(article) : this.NotFound();
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpPost]
        public IActionResult InsertArticle([FromBody]ArticleModel article)
        {
          var errors = new List<string>();
          if (_articleService.UpsertArticle(article, ref errors))
          {
            return this.Ok(article);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpPut]
        public IActionResult UpdateArticle([FromBody]ArticleModel article)
        {
          return InsertArticle(article);
        }

        [Authorize(Roles = "PEngineAdmin")]
        [HttpDelete("{guid}")]
        public IActionResult DeleteArticle(Guid guid)
        {
          var errors = new List<string>();
          _articleDal.DeleteArticle(guid);
          return this.Ok();
        }
    }
}
