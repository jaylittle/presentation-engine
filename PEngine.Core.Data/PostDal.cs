using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class PostDal : BaseDal<PostDal>, IPostDal
  {
    public IEnumerable<PostModel> ListPosts()
    {
      using (var ct = Database.OpenConnection(Database.DatabaseType.PEngine))
      {
        return ct.DbConnection.Query<PostModel>(ReadQuery("ListPosts"));
      }
    }

    public PostModel GetPostById(Guid? guid, int? legacyId, string uniqueName)
    {
      using (var ct = Database.OpenConnection(Database.DatabaseType.PEngine))
      {
        return ct.DbConnection.QueryFirst<PostModel>(ReadQuery("GetPostById"), new { 
          guid, legacyId, uniqueName
        });
      }
    }

    public void InsertPost(PostModel post)
    {
      post.UpdateGuid();
      post.UpdateTimestamps(true);
      
      using (var ct = Database.OpenConnection(Database.DatabaseType.PEngine))
      {
        ct.DbConnection.Execute(ReadQuery("InsertPost"), post);
      }
    }

    public void UpdatePost(PostModel post)
    {
      post.UpdateGuid();
      post.UpdateTimestamps(false);

      using (var ct = Database.OpenConnection(Database.DatabaseType.PEngine))
      {
        ct.DbConnection.Execute(ReadQuery("UpdatePost"), post);
      }
    }
  }
}
