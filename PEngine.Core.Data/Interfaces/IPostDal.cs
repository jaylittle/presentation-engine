using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IPostDal : IBaseDal
    {
      Task<IEnumerable<PostModel>> ListPosts();
      Task<PostModel> GetPostById(Guid? guid, int? legacyId, string uniqueName);
      Task InsertPost(PostModel post, bool importFlag = false);
      Task UpdatePost(PostModel post);
      Task DeletePost(Guid guid);
      Task DeleteAllPosts();
    }
}
