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
    Task<IEnumerable<PostModel>> ListPosts(bool isAdmin, bool isLockedDown, bool isView = false);
    Task<(IEnumerable<PostModel> exact, IEnumerable<PostModel> fuzzy)> SearchPosts(string searchQuery, string[] searchTerms, bool isAdmin, bool isLockedDown);
    Task<PostModel> GetPostById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin, bool isLockedDown);
    Task<OpResult> UpsertPost(PostModel post, bool importFlag = false);
  }
}