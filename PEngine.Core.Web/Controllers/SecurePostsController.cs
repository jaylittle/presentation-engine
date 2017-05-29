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
using Microsoft.AspNetCore.Authorization;

namespace PEngine.Core.Web.Controllers
{
    [Route("api/[controller]")]
    public class SecurePostsController : Controller
    {
        private IPostDal _postDal;
        private IPostService _postService;
        public SecurePostsController(IPostDal postDal, IPostService postService)
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
          var post = _postDal.GetPostById(guid, null, null);
          return post != null ? (IActionResult) this.Ok(post) : this.NotFound();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult InsertPost([FromBody]PostModel post)
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

        [Authorize(Roles = "ForumAdmin")]
        [HttpPut]
        public IActionResult UpdatePost([FromBody]PostModel post)
        {
          return InsertPost(post);
        }

        [HttpDelete("{guid}")]
        public IActionResult DeletePost(Guid guid)
        {
          var errors = new List<string>();
          _postDal.DeletePost(guid);
          return this.Ok();
        }
    }
}
