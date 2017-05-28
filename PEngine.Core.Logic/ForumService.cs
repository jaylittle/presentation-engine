using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data;
using PEngine.Core.Data.Interfaces;
using PEngine.Core.Logic.Interfaces;
using PEngine.Core.Shared;

namespace PEngine.Core.Logic
{
  public class ForumService : IForumService
  {
    public const string FORUM_ERROR_DATA_MUST_BE_PROVIDED = "Forum data must be provided";
    public const string FORUM_ERROR_INVALID_RECORD = "Forum Guid refers to an invalid record";
    public const string FORUM_ERROR_TITLE_IS_REQUIRED = "Forum Title is a required field";
    public const string FORUM_ERROR_DESCRIPTION_IS_REQUIRED = "Forum Description is a required field";
    public const string THREAD_ERROR_DATA_MUST_BE_PROVIDED = "Forum Thread data must be provided";
    public const string THREAD_ERROR_INVALID_RECORD = "Forum Thread Guid refers to an invalid record";
    public const string THREAD_ERROR_TITLE_IS_REQUIRED = "Forum Thread Title is a required field";
    public const string THREAD_ERROR_INITIAL_POST_IS_REQUIRED = "New Forum Threads require an initial post";
    public const string POST_ERROR_DATA_MUST_BE_PROVIDED = "Forum Thread Post data must be provided";
    public const string POST_ERROR_INVALID_RECORD = "Form Thread Post Guid refers to an invalid record";
    public const string POST_ERROR_CONTENT_IS_REQUIRED = "Forum Thread Post content is required";
    public const string USER_ERROR_DATA_MUST_BE_PROVIDED = "Forum User data must be provided";
    public const string USER_ERROR_INVALID_RECORD = "Forum User Guid refers to an invalid record";
    public const string USER_ERROR_USER_ID_IS_REQUIRED = "Forum User Id is a required field";
    public const string USER_ERROR_EMAIL_IS_REQUIRED = "Forum User Email is a required field";
    public const string USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS = "Forum User Password is a required field for new users";
    public const string USER_ERROR_COMMENT_IS_REQUIRED = "Forum User Comment is a required field";

    private IForumDal _forumDal;
    
    public ForumService(IForumDal forumDal)
    {
      _forumDal = forumDal;
    }

    public bool UpsertForum(ForumModel forum, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ForumModel existingForum = null;
      if (forum == null)
      {
        errors.Add(FORUM_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (forum.Guid != Guid.Empty)
      {
        existingForum = _forumDal.GetForumById(forum.Guid, null);
        if (existingForum == null)
        {
          errors.Add(FORUM_ERROR_INVALID_RECORD);
        }
        else
        {
          forum.UniqueName = existingForum.UniqueName;
          forum.CreatedUTC = existingForum.CreatedUTC;
          forum.ModifiedUTC = existingForum.ModifiedUTC;
        }
      }
      if (string.IsNullOrWhiteSpace(forum.Name))
      {
        errors.Add(FORUM_ERROR_TITLE_IS_REQUIRED);
      }
      if (string.IsNullOrWhiteSpace(forum.Description))
      {
        errors.Add(FORUM_ERROR_DESCRIPTION_IS_REQUIRED);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        var existingForumUniqueNames = _forumDal.ListForums().ToDictionary(f => f.UniqueName, f => true, StringComparer.OrdinalIgnoreCase);
        forum.GenerateUniqueName(existingForumUniqueNames);
        
        if (forum.Guid == Guid.Empty)
        {
          _forumDal.InsertForum(forum);
        }
        else
        {
          _forumDal.UpdateForum(forum);
        }
      }
      return retvalue;
    }
    
    public bool UpsertForumThread(ForumThreadModel forumThread, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ForumThreadModel existingForumThread = null;
      if (forumThread == null)
      {
        errors.Add(THREAD_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (forumThread.Guid != Guid.Empty)
      {
        existingForumThread = _forumDal.GetForumThreadById(forumThread.Guid, null);
        if (existingForumThread == null)
        {
          errors.Add(THREAD_ERROR_INVALID_RECORD);
        }
        else
        {
          forumThread.UniqueName = existingForumThread.UniqueName;
          forumThread.CreatedUTC = existingForumThread.CreatedUTC;
          forumThread.ModifiedUTC = existingForumThread.ModifiedUTC;
        }
      }
      if (string.IsNullOrWhiteSpace(forumThread.Name))
      {
        errors.Add(THREAD_ERROR_TITLE_IS_REQUIRED);
      }
      if (forumThread.Guid == Guid.Empty)
      {
        if (forumThread.InitialPost == null)
        {
          errors.Add(THREAD_ERROR_INITIAL_POST_IS_REQUIRED);
        }
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        var existingForumThreadUniqueNames = _forumDal.ListForumThreads(forumThread.ForumGuid, null)
          .ToDictionary(ft => ft.UniqueName, ft => true, StringComparer.OrdinalIgnoreCase);
        forumThread.GenerateUniqueName(existingForumThreadUniqueNames);

        _forumDal.OpenTransaction(DatabaseType.Forum, false); 
        try
        {
          if (forumThread.Guid == Guid.Empty)
          {
            _forumDal.InsertForumThread(forumThread);
          }
          else
          {
            _forumDal.UpdateForumThread(forumThread);
          }
          if (forumThread.InitialPost != null)
          {
            forumThread.InitialPost.ForumThreadGuid = forumThread.Guid;
            UpsertForumThreadPost(forumThread.InitialPost, ref errors);
          }
          _forumDal.CommitTransaction(DatabaseType.Forum);
        }
        catch (Exception ex)
        {
          _forumDal.RollBackTransaction(DatabaseType.Forum);
          throw new Exception("Forum Thread Transaction Rolled Back Due to Previous Errors", ex);
        }
      }
      return retvalue;
    }

    public bool UpsertForumThreadPost(ForumThreadPostModel forumThreadPost, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ForumThreadPostModel existingForumThreadPost = null;
      if (forumThreadPost == null)
      {
        errors.Add(POST_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (forumThreadPost.Guid != Guid.Empty)
      {
        existingForumThreadPost = _forumDal.GetForumThreadPostById(forumThreadPost.Guid);
        if (existingForumThreadPost == null)
        {
          errors.Add(POST_ERROR_INVALID_RECORD);
        }
        else
        {
          forumThreadPost.CreatedUTC = existingForumThreadPost.CreatedUTC;
          forumThreadPost.ModifiedUTC = existingForumThreadPost.ModifiedUTC;
        }
      }
      if (string.IsNullOrWhiteSpace(forumThreadPost.Data))
      {
        errors.Add(POST_ERROR_CONTENT_IS_REQUIRED);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      { 
        if (forumThreadPost.Guid == Guid.Empty)
        {
          _forumDal.InsertForumThreadPost(forumThreadPost);
        }
        else
        {
          _forumDal.UpdateForumThreadPost(forumThreadPost);
        }
      }
      return retvalue;
    }

    public bool UpsertForumUser(ForumUserModel forumUser, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      ForumUserModel existingForumUser = null;
      if (forumUser == null)
      {
        errors.Add(USER_ERROR_DATA_MUST_BE_PROVIDED);
        return false;
      }
      if (forumUser.Guid != Guid.Empty)
      {
        existingForumUser = _forumDal.GetForumUserById(forumUser.Guid);
        if (existingForumUser == null)
        {
          errors.Add(USER_ERROR_INVALID_RECORD);
        }
        else
        {
          forumUser.CreatedUTC = existingForumUser.CreatedUTC;
          forumUser.ModifiedUTC = existingForumUser.ModifiedUTC;
          forumUser.LastIPAddress = existingForumUser.LastIPAddress;
          forumUser.LastLogon = existingForumUser.LastLogon;
        }
      }
      if (string.IsNullOrWhiteSpace(forumUser.UserId))
      {
        errors.Add(USER_ERROR_USER_ID_IS_REQUIRED);
      }
      if (string.IsNullOrWhiteSpace(forumUser.Email))
      {
        errors.Add(USER_ERROR_EMAIL_IS_REQUIRED);
      }
      if (forumUser.Guid == Guid.Empty && string.IsNullOrWhiteSpace(forumUser.NewPassword.Value))
      {
        errors.Add(USER_ERROR_PASSWORD_IS_REQUIRED_FOR_NEW_USERS);
      }
      if (string.IsNullOrEmpty(forumUser.Comment))
      {
        errors.Add(USER_ERROR_COMMENT_IS_REQUIRED);
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      { 
        if (forumUser.Guid == Guid.Empty)
        {
          forumUser.Password = Security.Encrypt(forumUser.NewPassword.Value);
          _forumDal.InsertForumUser(forumUser);
        }
        else
        {
          if (forumUser.NewPassword.Reset)
          {
            forumUser.Password = Security.Encrypt(string.Empty);
          }
          else if (!string.IsNullOrEmpty(forumUser.NewPassword.Value))
          {
            forumUser.Password = Security.Encrypt(forumUser.NewPassword.Value);
          }
          else
          {
            forumUser.Password = existingForumUser?.Password ?? string.Empty;
          }
          _forumDal.UpdateForumUser(forumUser);
        }
      }
      return retvalue;
    }
  }
}