using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class PostDal : BaseDal<PostDal>, IPostDal
  {
    public async Task<IEnumerable<PostModel>> ListPosts()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryAsync<PostModel>(ReadQuery("ListPosts", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task<PostModel> GetPostById(Guid? guid, int? legacyId, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        return await ct.DbConnection.QueryFirstOrDefaultAsync<PostModel>(ReadQuery("GetPostById", ct.ProviderName), new { 
          guid, legacyId, uniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertPost(PostModel post, bool importFlag = false)
    {
      post.UpdateGuid();
      post.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertPost", ct.ProviderName), post, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdatePost(PostModel post)
    {
      post.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdatePost", ct.ProviderName), post, transaction: ct.DbTransaction);
      }
    }

    public async Task DeletePost(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.PEngine, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeletePost", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteAllPosts()
    {
      using (var ct = GetConnection(DatabaseType.PEngine, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteAllPosts", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
