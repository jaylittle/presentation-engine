using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IPostDal : IBaseDal
    {
      IEnumerable<PostModel> ListPosts();
      PostModel GetPostById(Guid? guid, int? legacyId, string uniqueName);
      void InsertPost(PostModel post, bool importFlag = false);
      void UpdatePost(PostModel post);
      void DeletePost(Guid guid);
      void DeleteAllPosts();
    }
}
