using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Data
{
  public class ForumDal : BaseDal<ForumDal>, IForumDal
  {
    public IEnumerable<ForumModel> ListForums()
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.Query<ForumModel>(ReadQuery("ListForums", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public ForumModel GetForumById(Guid? guid, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.QueryFirstOrDefault<ForumModel>(ReadQuery("GetForumById", ct.ProviderName), new {
          guid, uniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public void InsertForum(ForumModel forum)
    {
      forum.UpdateGuid();
      forum.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertForum", ct.ProviderName), forum, transaction: ct.DbTransaction);
      }
    }

    public void UpdateForum(ForumModel forum)
    {
      forum.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateForum", ct.ProviderName), forum, transaction: ct.DbTransaction);
      }
    }

    public IEnumerable<ForumThreadModel> ListForumThreads(Guid? forumGuid, string forumUniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.Query<ForumThreadModel>(ReadQuery("ListForumThreads", ct.ProviderName), new {
            forumGuid, forumUniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public ForumThreadModel GetForumThreadById(Guid? guid, string uniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.QueryFirstOrDefault<ForumThreadModel>(ReadQuery("GetForumThreadById", ct.ProviderName), new {
          guid, uniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public void InsertForumThread(ForumThreadModel forumThread)
    {
      forumThread.UpdateGuid();
      forumThread.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertForumThread", ct.ProviderName), forumThread, transaction: ct.DbTransaction);
      }
    }

    public void UpdateForumThread(ForumThreadModel forumThread)
    {
      forumThread.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateForumThread", ct.ProviderName), forumThread, transaction: ct.DbTransaction);
      }
    }

    public IEnumerable<ForumThreadPostModel> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.Query<ForumThreadPostModel>(ReadQuery("ListForumThreadPosts", ct.ProviderName), new {
            forumGuid, forumUniqueName, forumThreadGuid, forumThreadUniqueName
        }, transaction: ct.DbTransaction);
      }
    }

    public ForumThreadPostModel GetForumThreadPostById(Guid guid)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.QueryFirstOrDefault<ForumThreadPostModel>(ReadQuery("GetForumThreadPostById", ct.ProviderName), new {
          guid
        }, transaction: ct.DbTransaction);
      }
    }

    public void InsertForumThreadPost(ForumThreadPostModel forumThreadPost)
    {
      forumThreadPost.UpdateGuid();
      forumThreadPost.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertForumThreadPost", ct.ProviderName), forumThreadPost, transaction: ct.DbTransaction);
      }
    }

    public void UpdateForumThreadPost(ForumThreadPostModel forumThreadPost)
    {
      forumThreadPost.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateForumThreadPost", ct.ProviderName), forumThreadPost, transaction: ct.DbTransaction);
      }
    }

    public IEnumerable<ForumUserModel> ListForumUsers()
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.Query<ForumUserModel>(ReadQuery("ListForumUsers", ct.ProviderName), transaction: ct.DbTransaction);
      }
    }

    public ForumUserModel GetForumUserById(Guid? guid, string userId)
    {
      using (var ct = GetConnection(DatabaseType.Forum, true))
      {
        return ct.DbConnection.QueryFirstOrDefault<ForumUserModel>(ReadQuery("GetForumUserById", ct.ProviderName), new {
          guid, userId
        }, transaction: ct.DbTransaction);
      }
    }

    public void InsertForumUser(ForumUserModel forumUser)
    {
      forumUser.UpdateGuid();
      forumUser.UpdateTimestamps(true);
      
      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("InsertForumUser", ct.ProviderName), param: forumUser, transaction: ct.DbTransaction);
      }
    }

    public void UpdateForumUser(ForumUserModel forumUser)
    {
      forumUser.UpdateTimestamps(false);

      using (var ct = GetConnection(DatabaseType.Forum, false))
      {
        ct.DbConnection.Execute(ReadQuery("UpdateForumUser", ct.ProviderName), param: forumUser, transaction: ct.DbTransaction);
      }
    }
  }
}
