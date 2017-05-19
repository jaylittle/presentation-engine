using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private IPostDal _postDal;
        private IPostService _postService;
        public PostsController(IPostDal postDal, IPostService postService)
        {
          _postDal = postDal;
          _postService = postService;
        }

        [HttpGet]
        public IEnumerable<PostModel> Get()
        {
          return _postDal.ListPosts();
        }

        [HttpGet("{guid}")]
        public IActionResult GetByGuid(Guid guid)
        {
          return this.Ok(_postDal.GetPostById(guid, null, null));
        }

        [HttpPost]
        public IActionResult UpsertPost([FromBody]PostModel post)
        {
          var errors = new List<string>();
          if (_postService.UpsertPost(post, ref errors))
          {
            return this.Ok(post);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }
    }
}
