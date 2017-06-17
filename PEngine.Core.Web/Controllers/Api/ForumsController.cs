using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Web.Constraints;

namespace PEngine.Core.Web.Controllers.Api
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
    public async Task<IEnumerable<ForumModel>> GetForums()
    {
      return await _forumService.ListForums(HttpContext.User.IsInRole("ForumAdmin"));
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpGet("{guid}")]
    public async Task<IActionResult> GetForumByGuid(Guid guid)
    {
      var forum = await _forumDal.GetForumById(guid, null);
      return forum != null ? (IActionResult) this.Ok(forum) : this.NotFound();
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpPost]
    public async Task<IActionResult> InsertForum([FromBody]ForumModel forum)
    {
      var result = await _forumService.UpsertForum(forum); 
      if (result.Successful)
      {
        return this.Ok(forum);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpPut]
    public async Task<IActionResult> UpdateForum([FromBody]ForumModel forum)
    {
      return await InsertForum(forum);
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpGet("{forumGuid}/threads/")]
    public async Task<IEnumerable<ForumThreadModel>> GetForumThreads(Guid forumGuid)
    {
      return await _forumService.ListForumThreads(forumGuid, null, HttpContext.User.IsInRole("ForumAdmin"));
    }

    [Authorize(Roles = "ForumUser")]
    [HttpGet("thread/{guid}")]
    public async Task<IActionResult> GetForumThreadByGuid(Guid guid)
    {
      var forumThread = await _forumDal.GetForumThreadById(guid, null);
      return forumThread != null ? (IActionResult) this.Ok(forumThread) : this.NotFound();
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPost("thread")]
    public async Task<IActionResult> InsertForumThread([FromBody]ForumThreadModel forumThread)
    {
      var result = await _forumService.UpsertForumThread(forumThread, Guid.Parse(HttpContext.User.Identity.Name),
        HttpContext.User.IsInRole("ForumAdmin"));
      if (result.Successful)
      {
        return this.Ok(forumThread);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPut("thread")]
    public async Task<IActionResult> UpdateForumThread([FromBody]ForumThreadModel forumThread)
    {
      return await InsertForumThread(forumThread);
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpGet("thread/{forumThreadGuid}/posts")]
    public async Task<IEnumerable<ForumThreadPostModel>> GetForumThreadPosts(Guid forumThreadGuid)
    {
      return await _forumService.ListForumThreadPosts(null, null, forumThreadGuid, null, HttpContext.User.IsInRole("ForumAdmin"));
    }

    [Authorize(Roles = "ForumUser")]
    [HttpGet("post/{guid}")]
    public async Task<IActionResult> GetForumThreadPostByGuid(Guid guid)
    {
      var forumThreadPost = await _forumDal.GetForumThreadPostById(guid);
      return forumThreadPost != null ? (IActionResult) this.Ok(forumThreadPost) : this.NotFound();
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPost("post")]
    public async Task<IActionResult> InsertForumThreadPost([FromBody]ForumThreadPostModel forumThreadPost)
    {
      var result = await _forumService.UpsertForumThreadPost(forumThreadPost, Guid.Parse(HttpContext.User.Identity.Name), HttpContext.User.IsInRole("ForumAdmin"));
      if (result.Successful)
      {
        return this.Ok(forumThreadPost);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPut("post")]
    public async Task<IActionResult> UpdateForumThreadPost([FromBody]ForumThreadPostModel forumThreadPost)
    {
      return await InsertForumThreadPost(forumThreadPost);
    }

    [Authorize(Roles = "ForumAdmin")]
    [HttpGet("users")]
    public async Task<IEnumerable<ForumUserModel>> GetForumUsers()
    {
      return await _forumDal.ListForumUsers();
    }

    [Authorize(Roles = "ForumUser")]
    [HttpGet("user/{guid}")]
    public async Task<IActionResult> GetForumUserByGuid(Guid guid)
    {
      var forumUser = await _forumService.GetForumUserById(guid, null, Guid.Parse(HttpContext.User.Identity.Name), HttpContext.User.IsInRole("ForumAdmin"));
      return forumUser != null ? (IActionResult) this.Ok(forumUser) : this.NotFound();
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPost("user")]
    public async Task<IActionResult> InsertForumUser([FromBody]ForumUserModel forumUser)
    {
      var result = await _forumService.UpsertForumUser(forumUser, Guid.Parse(HttpContext.User.Identity.Name), HttpContext.User.IsInRole("ForumAdmin"));
      if (result.Successful)
      {
        return this.Ok(forumUser);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "ForumUser")]
    [HttpPut("user")]
    public async Task<IActionResult> UpdateForumUser([FromBody]ForumUserModel forumUser)
    {
      return await InsertForumUser(forumUser);
    }
  }
}
