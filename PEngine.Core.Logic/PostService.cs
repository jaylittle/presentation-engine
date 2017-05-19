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
    private IPostDal _postDal;
    public PostService(IPostDal postDal)
    {
      _postDal = postDal;
    }

    public bool UpsertPost(PostModel post, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      if (string.IsNullOrWhiteSpace(post.Name))
      {
        errors.Add("Post Title is a required field");
      }
      if (string.IsNullOrEmpty(post.Data))
      {
        errors.Add("Post Content is a required field");
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