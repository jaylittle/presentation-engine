using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IPostDal
    {
      IEnumerable<PostModel> ListPosts();
      PostModel GetPostById(Guid? guid, int? legacyId, string uniqueName);
    }
}
