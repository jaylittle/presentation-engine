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
  public class PostsController : Controller
  {
    private IPostDal _postDal;
    private IPostService _postService;
    public PostsController(IPostDal postDal, IPostService postService)
    {
      _postDal = postDal;
      _postService = postService;
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet]
    public async Task<IEnumerable<PostModel>> Get([FromQuery]PagingModel paging = null)
    {
      var posts = await _postDal.ListPosts();
      return PagingUtils.Paginate(ref paging, posts);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpGet("{guid}")]
    public async Task<IActionResult> GetByGuid(Guid guid)
    {
      var post = await _postDal.GetPostById(guid, null, null);
      return post != null ? (IActionResult) this.Ok(post) : this.NotFound();
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPost]
    public async Task<IActionResult> InsertPost([FromBody]PostModel post)
    {
      var result = await _postService.UpsertPost(post);
      if (result.Successful)
      {
        await FeedManager.GenerateRSS();
        return this.Ok(post);
      }
      else
      {
        return this.StatusCode(400, result);
      }
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpPut]
    public async Task<IActionResult> UpdatePost([FromBody]PostModel post)
    {
      return await InsertPost(post);
    }

    [Authorize(Roles = "PEngineAdmin")]
    [HttpDelete("{guid}")]
    public async Task<IActionResult> DeletePost(Guid guid)
    {
      await _postDal.DeletePost(guid);
      await FeedManager.GenerateRSS();
      return this.Ok();
    }
  }
}
