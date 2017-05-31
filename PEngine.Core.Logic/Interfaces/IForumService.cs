using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IForumService
  {
    IEnumerable<ForumModel> ListForums(bool isForumAdmin);
    bool UpsertForum(ForumModel forum, ref List<string> errors);
    IEnumerable<ForumThreadModel> ListForumThreads(Guid? forumGuid, string forumUniqueName, bool isForumAdmin);
    ForumThreadModel GetForumThreadById(Guid? guid, string uniqueName, Guid forumUserGuid, bool isForumAdmin);
    bool UpsertForumThread(ForumThreadModel forumThread, Guid forumUserGuid, bool isForumAdmin, ref List<string> errors);
    IEnumerable<ForumThreadPostModel> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName, bool isForumAdmin);
    ForumThreadPostModel GetForumThreadPostById(Guid guid, Guid forumUserGuid, bool isForumAdmin);
    bool UpsertForumThreadPost(ForumThreadPostModel forumThreadPost, Guid forumUserGuid, bool isForumAdmin, ref List<string> errors);
    ForumUserModel GetForumUserById(Guid? guid, string userId, Guid forumUserGuid, bool isForumAdmin);
    bool UpsertForumUser(ForumUserModel forumUser, Guid forumUserGuid, bool isForumAdmin, ref List<string> errors);
  }
}