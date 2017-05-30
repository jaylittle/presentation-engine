using System;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;

namespace PEngine.Core.Data.Interfaces
{
    public interface IForumDal : IBaseDal
    {
      IEnumerable<ForumModel> ListForums();
      ForumModel GetForumById(Guid? guid, string uniqueName);
      void InsertForum(ForumModel forum);
      void UpdateForum(ForumModel forum);
      IEnumerable<ForumThreadModel> ListForumThreads(Guid? forumGuid, string forumUniqueName);
      ForumThreadModel GetForumThreadById(Guid? guid, string uniqueName);
      void InsertForumThread(ForumThreadModel forumThread);
      void UpdateForumThread(ForumThreadModel forumThread);
      IEnumerable<ForumThreadPostModel> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName);
      ForumThreadPostModel GetForumThreadPostById(Guid guid);
      void InsertForumThreadPost(ForumThreadPostModel forumThreadPost);
      void UpdateForumThreadPost(ForumThreadPostModel forumThreadPost);
      IEnumerable<ForumUserModel> ListForumUsers();
      ForumUserModel GetForumUserById(Guid? guid, string userId);
      void InsertForumUser(ForumUserModel forumUser);
      void UpdateForumUser(ForumUserModel forumUser);
    }
}
