using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;

namespace PEngine.Core.Logic.Interfaces
{
  public interface IForumService
  {
    bool UpsertForum(ForumModel forum, ref List<string> errors);
    bool UpsertForumThread(ForumThreadModel forumThread, ref List<string> errors);
    bool UpsertForumThreadPost(ForumThreadPostModel forumThreadPost, ref List<string> errors);
    bool UpsertForumUser(ForumUserModel forumUser, ref List<string> errors);
  }
}