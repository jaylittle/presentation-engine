using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers
{
    [Route("api/[controller]")]
    public class PostsController : Controller
    {
        private IPostDal _postDal;
        public PostsController(IPostDal postDal)
        {
          _postDal = postDal;
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
    }
}
