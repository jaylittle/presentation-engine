using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IPostService
  {
    IEnumerable<PostModel> ListPosts(bool isAdmin);
    PostModel GetPostById(Guid? guid, int? legacyId, string uniqueName, bool isAdmin);
    bool UpsertPost(PostModel post, ref List<string> errors);
  }
}