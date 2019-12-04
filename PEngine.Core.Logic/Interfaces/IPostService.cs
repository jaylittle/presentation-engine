using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IPostService
  {
    Task<IEnumerable<PostModel>> ListPosts(bool isAdmin, bool isView = false);
    Task<IEnumerable<PostModel>> SearchPosts(string[] searchTerms, bool isAdmin);
    Task<PostModel> GetPostById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin);
    Task<OpResult> UpsertPost(PostModel post, bool importFlag = false);
  }
}