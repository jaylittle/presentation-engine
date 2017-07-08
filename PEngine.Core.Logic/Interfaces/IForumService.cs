using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IForumService
  {
    Task<IEnumerable<ForumModel>> ListForums(bool isForumAdmin);
    Task<ForumModel> GetForumById(Guid? guid, string uniqueName, bool isForumAdmin);
    Task<OpResult> UpsertForum(ForumModel forum, bool importFlag = false);
    Task<IEnumerable<ForumThreadModel>> ListForumThreads(Guid? forumGuid, string forumUniqueName, bool isForumAdmin);
    Task<ForumThreadModel> GetForumThreadById(Guid? guid, string uniqueName, Guid forumUserGuid, bool isForumAdmin);
    Task<OpResult> UpsertForumThread(ForumThreadModel forumThread, Guid forumUserGuid, bool isForumAdmin, bool importFlag = false);
    Task<IEnumerable<ForumThreadPostModel>> ListForumThreadPosts(Guid? forumGuid, string forumUniqueName, Guid? forumThreadGuid, string forumThreadUniqueName, bool isForumAdmin);
    Task<IEnumerable<ForumThreadPostModel>> SearchForumThreadPosts(string[] searchTerms, bool isForumAdmin);
    Task<ForumThreadPostModel> GetForumThreadPostById(Guid guid, Guid forumUserGuid, bool isForumAdmin);
    Task<OpResult> UpsertForumThreadPost(ForumThreadPostModel forumThreadPost, Guid forumUserGuid, bool isForumAdmin, bool importFlag = false);
    Task<ForumUserModel> GetForumUserById(Guid? guid, string userId, Guid forumUserGuid, bool isForumAdmin);
    Task<OpResult> UpsertForumUser(ForumUserModel forumUser, Guid forumUserGuid, bool isForumAdmin, bool importFlag = false);
  }
}