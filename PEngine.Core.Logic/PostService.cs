using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Logic
{
  public class PostService : IPostService
  {
    public const string POST_ERROR_DATA_MUST_BE_PROVIDED = "Post data must be provided";
    public const string POST_ERROR_INVALID_RECORD = "Post Guid refers to an invalid record";
    public const string POST_ERROR_TITLE_IS_REQUIRED = "Post Title is a required field";
    public const string POST_ERROR_CONTENT_IS_REQUIRED = "Post Content is a required field";

    private IPostDal _postDal;

    public PostService(IPostDal postDal)
    {
      _postDal = postDal;
    }

    public IEnumerable<PostModel> ListPosts(bool isAdmin)
    {
      return _postDal.ListPosts().Where(p => isAdmin || p.VisibleFlag);
    }

    public PostModel GetPostById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin)
    {
      var post = _postDal.GetPostById(guid, legacyId, uniqueName);
      return (post == null || isAdmin || post.VisibleFlag) ? post : null;
    }

    public bool UpsertPost(PostModel post, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      PostModel existingPost = null; 
      if (post == null)
      {
        errors.Add(POST_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (post.Guid != Guid.Empty)
      {
        existingPost = _postDal.GetPostById(post.Guid, null, null);
        if (existingPost == null)
        {
          errors.Add(POST_ERROR_INVALID_RECORD);
        }
        else
        {
          post.UniqueName = existingPost.UniqueName;
          post.CreatedUTC = existingPost.CreatedUTC;
          post.ModifiedUTC = existingPost.ModifiedUTC;
        }
      }
      if (string.IsNullOrWhiteSpace(post.Name))
      {
        errors.Add(POST_ERROR_TITLE_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(post.Data))
      {
        errors.Add(POST_ERROR_CONTENT_IS_REQUIRED);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        var createMonth = post.CreatedUTC.HasValue ? post.CreatedUTC.Value.Month : DateTime.UtcNow.Month;
        var createYear = post.CreatedUTC.HasValue ? post.CreatedUTC.Value.Year : DateTime.UtcNow.Year;
        var existingPostUniqueNames = _postDal.ListPosts()
          .Where(p => p.CreatedUTC.HasValue && p.CreatedUTC.Value.Month == createMonth && p.CreatedUTC.Value.Year == createYear)
          .ToDictionary(p => p.UniqueName, p => true, StringComparer.OrdinalIgnoreCase);
        post.GenerateUniqueName(existingPostUniqueNames);
        
        if (post.Guid == Guid.Empty)
        {
          _postDal.InsertPost(post);
        }
        else
        {
          _postDal.UpdatePost(post);
        }
      }
      return retvalue;
    }
  }
}