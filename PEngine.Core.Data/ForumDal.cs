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
  public class ForumDal : BaseDal<ForumDal>, IForumDal
  {
    public async Task<IEnumerable<ForumModel>> ListForums()
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryAsync<ForumModel>(ReadQuery("ListForums", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task<ForumModel> GetForumById(Guid? guid, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryFirstOrDefaultAsync<ForumModel>(ReadQuery("GetForumById", ct.ProviderName), new {
          guid, uniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertForum(ForumModel forum, bool importFlag = false)
    {
      forum.UpdateGuid();
      forum.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertForum", ct.ProviderName), forum, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateForum(ForumModel forum)
    {
      forum.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateForum", ct.ProviderName), forum, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ForumThreadModel>> ListForumThreads(Guid? forumGuid, string forumUniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryAsync<ForumThreadModel>(ReadQuery("ListForumThreads", ct.ProviderName), new {
            forumGuid, forumUniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<ForumThreadModel> GetForumThreadById(Guid? guid, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryFirstOrDefaultAsync<ForumThreadModel>(ReadQuery("GetForumThreadById", ct.ProviderName), new {
          guid, uniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertForumThread(ForumThreadModel forumThread, bool importFlag = false)
    {
      forumThread.UpdateGuid();
      forumThread.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertForumThread", ct.ProviderName), forumThread, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateForumThread(ForumThreadModel forumThread)
    {
      forumThread.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateForumThread", ct.ProviderName), forumThread, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ForumThreadPostModel>> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryAsync<ForumThreadPostModel>(ReadQuery("ListForumThreadPosts", ct.ProviderName), new {
            forumGuid, forumUniqueName, forumThreadGuid, forumThreadUniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task<ForumThreadPostModel> GetForumThreadPostById(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryFirstOrDefaultAsync<ForumThreadPostModel>(ReadQuery("GetForumThreadPostById", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertForumThreadPost(ForumThreadPostModel forumThreadPost, bool importFlag = false)
    {
      forumThreadPost.UpdateGuid();
      forumThreadPost.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertForumThreadPost", ct.ProviderName), forumThreadPost, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateForumThreadPost(ForumThreadPostModel forumThreadPost)
    {
      forumThreadPost.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateForumThreadPost", ct.ProviderName), forumThreadPost, transaction: ct.DbTransaction);
      }
    }

    public async Task<IEnumerable<ForumUserModel>> ListForumUsers()
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryAsync<ForumUserModel>(ReadQuery("ListForumUsers", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public async Task<ForumUserModel> GetForumUserById(Guid? guid, string userId)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return await ct.DbConnection.QueryFirstOrDefaultAsync<ForumUserModel>(ReadQuery("GetForumUserById", ct.ProviderName), new {
          guid, userId
        }, transaction: ct.DbTransaction);
      }
    }

    public async Task InsertForumUser(ForumUserModel forumUser, bool importFlag = false)
    {
      forumUser.UpdateGuid();
      forumUser.UpdateTimestamps(true, importFlag);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("InsertForumUser", ct.ProviderName), param: forumUser, transaction: ct.DbTransaction);
      }
    }

    public async Task UpdateForumUser(ForumUserModel forumUser)
    {
      forumUser.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("UpdateForumUser", ct.ProviderName), param: forumUser, transaction: ct.DbTransaction);
      }
    }

    public async Task DeleteAllForums()
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        await ct.DbConnection.ExecuteAsync(ReadQuery("DeleteAllForums", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }
  }
}
