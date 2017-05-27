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
    public const string POST_ERROR_TITLE_IS_REQUIRED = "Post Title is a required field";
    public const string POST_ERROR_CONTENT_IS_REQUIRD = "Post Content is a required field";

    private IPostDal _postDal;

    public PostService(IPostDal postDal)
    {
      _postDal = postDal;
    }

    public bool UpsertPost(PostModel post, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      if (post == null)
      {
        errors.Add(POST_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (string.IsNullOrWhiteSpace(post.Name))
      {
        errors.Add(POST_ERROR_TITLE_IS_REQUIRED);
      }
      if (string.IsNullOrEmpty(post.Data))
      {
        errors.Add(POST_ERROR_CONTENT_IS_REQUIRD);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        post.GenerateUniqueName();
        
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