using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IForumDal : IBaseDal
    {
      Task<IEnumerable<ForumModel>> ListForums();
      Task<ForumModel> GetForumById(Guid? guid, string uniqueName);
      Task InsertForum(ForumModel forum, bool importFlag = false);
      Task UpdateForum(ForumModel forum);
      Task<IEnumerable<ForumThreadModel>> ListForumThreads(Guid? forumGuid, string forumUniqueName);
      Task<ForumThreadModel> GetForumThreadById(Guid? guid, string uniqueName);
      Task InsertForumThread(ForumThreadModel forumThread, bool importFlag = false);
      Task UpdateForumThread(ForumThreadModel forumThread);
      Task<IEnumerable<ForumThreadPostModel>> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName);
      Task<ForumThreadPostModel> GetForumThreadPostById(Guid guid);
      Task InsertForumThreadPost(ForumThreadPostModel forumThreadPost, bool importFlag = false);
      Task UpdateForumThreadPost(ForumThreadPostModel forumThreadPost);
      Task<IEnumerable<ForumUserModel>> ListForumUsers();
      Task<ForumUserModel> GetForumUserById(Guid? guid, string userId);
      Task InsertForumUser(ForumUserModel forumUser, bool importFlag = false);
      Task UpdateForumUser(ForumUserModel forumUser);
      Task DeleteAllForums();
    }
}
