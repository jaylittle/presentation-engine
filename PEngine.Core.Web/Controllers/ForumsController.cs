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
    public class ForumsController : Controller
    {
        private IForumDal _forumDal;
        private IForumService _forumService;

        public ForumsController(IForumDal forumDal, IForumService forumService)
        {
          _forumDal = forumDal;
          _forumService = forumService;
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet]
        public IEnumerable<ForumModel> GetForums()
        {
          return _forumDal.ListForums();
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("{guid}")]
        public IActionResult GetForumByGuid(Guid guid)
        {
          var forum = _forumDal.GetForumById(guid, null);
          return forum != null ? (IActionResult) this.Ok(forum) : this.NotFound();
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpPost]
        public IActionResult InsertForum([FromBody]ForumModel forum)
        {
          var errors = new List<string>();
          if (_forumService.UpsertForum(forum, ref errors))
          {
            return this.Ok(forum);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpPut]
        public IActionResult UpdateForum([FromBody]ForumModel forum)
        {
          return InsertForum(forum);
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("{forumGuid}/threads/")]
        public IEnumerable<ForumThreadModel> GetForumThreads(Guid forumGuid)
        {
          return _forumDal.ListForumThreads(forumGuid, null);
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("thread/{guid}")]
        public IActionResult GetForumThreadByGuid(Guid guid)
        {
          var forumThread = _forumDal.GetForumThreadById(guid, null);
          return forumThread != null ? (IActionResult) this.Ok(forumThread) : this.NotFound();
        }

        [Authorize(Roles = "ForumUser")]
        [HttpPost("thread")]
        public IActionResult InsertForumThread([FromBody]ForumThreadModel forumThread)
        {
          var errors = new List<string>();
          if (_forumService.UpsertForumThread(forumThread, ref errors))
          {
            return this.Ok(forumThread);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "ForumUser")]
        [HttpPut("thread")]
        public IActionResult UpdateForumThread([FromBody]ForumThreadModel forumThread)
        {
          return InsertForumThread(forumThread);
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("thread/{forumThreadGuid}/posts")]
        public IEnumerable<ForumThreadPostModel> GetForumThreadPosts(Guid forumThreadGuid)
        {
          return _forumDal.ListForumThreadPosts(null, null, forumThreadGuid, null);
        }

        [Authorize(Roles = "ForumUser")]
        [HttpGet("post/{guid}")]
        public IActionResult GetForumThreadPostByGuid(Guid guid)
        {
          var forumThreadPost = _forumDal.GetForumThreadPostById(guid);
          return forumThreadPost != null ? (IActionResult) this.Ok(forumThreadPost) : this.NotFound();
        }

        [Authorize(Roles = "ForumUser")]
        [HttpPost("post")]
        public IActionResult InsertForumThreadPost([FromBody]ForumThreadPostModel forumThreadPost)
        {
          var errors = new List<string>();
          if (_forumService.UpsertForumThreadPost(forumThreadPost, ref errors))
          {
            return this.Ok(forumThreadPost);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "ForumUser")]
        [HttpPut("post")]
        public IActionResult UpdateForumThreadPost([FromBody]ForumThreadPostModel forumThreadPost)
        {
          return InsertForumThreadPost(forumThreadPost);
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("users")]
        public IEnumerable<ForumUserModel> GetForumUsers()
        {
          return _forumDal.ListForumUsers();
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpGet("user/{guid}")]
        public IActionResult GetForumUserByGuid(Guid guid)
        {
          var forumUser = _forumDal.GetForumUserById(guid, null);
          return forumUser != null ? (IActionResult) this.Ok(forumUser) : this.NotFound();
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpPost("user")]
        public IActionResult InsertForumUser([FromBody]ForumUserModel forumUser)
        {
          var errors = new List<string>();
          if (_forumService.UpsertForumUser(forumUser, ref errors))
          {
            return this.Ok(forumUser);
          }
          else
          {
            return this.StatusCode(400, new { errors });
          }
        }

        [Authorize(Roles = "ForumAdmin")]
        [HttpPut("user")]
        public IActionResult UpdateForumUser([FromBody]ForumUserModel forumUser)
        {
          return InsertForumUser(forumUser);
        }
    }
}
