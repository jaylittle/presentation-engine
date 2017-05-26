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
    private IForumDal _forumDal;
    public ForumService(IForumDal forumDal)
    {
      _forumDal = forumDal;
    }

    public bool UpsertForum(ForumModel forum, ref List<string> errors)
    {
      var startErrorCount = errors.Count;
      if (forum == null)
      {
        errors.Add("Forum data must be provided");
        return false;
      }
      if (string.IsNullOrWhiteSpace(forum.Name))
      {
        errors.Add("Forum Title is a required field");
      }
      if (string.IsNullOrWhiteSpace(forum.Description))
      {
        errors.Add("Forum Description is a required field");
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        forum.GenerateUniqueName();
        
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
      if (forumThread == null)
      {
        errors.Add("Forum Thread data must be provided");
        return false;
      }
      if (string.IsNullOrWhiteSpace(forumThread.Name))
      {
        errors.Add("Forum Thread Title is a required field");
      }
      if (forumThread.Guid == Guid.Empty)
      {
        if (forumThread.InitialPost == null)
        {
          errors.Add("New Forum Threads require an initial post");
        }
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      {
        forumThread.GenerateUniqueName();

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
      if (forumThreadPost == null)
      {
        errors.Add("Forum Thread Post data must be provided");
        return false;
      }
      if (string.IsNullOrWhiteSpace(forumThreadPost.Data))
      {
        errors.Add("Forum Thread Post must contain actual content");
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
      if (forumUser == null)
      {
        errors.Add("Forum User data must be provided");
        return false;
      }
      if (string.IsNullOrWhiteSpace(forumUser.UserId))
      {
        errors.Add("Forum User Id is a required field");
      }
      if (string.IsNullOrWhiteSpace(forumUser.Email))
      {
        errors.Add("Forum User Email is a required field");
      }
      if (forumUser.Guid == Guid.Empty && string.IsNullOrWhiteSpace(forumUser.NewPassword.Value))
      {
        errors.Add("Forum User Password is a required field for new users");
      }
      if (string.IsNullOrEmpty(forumUser.Comment))
      {
        errors.Add("Forum User Comment is a required field");
      }
      var retvalue = (errors == null || errors.Count == startErrorCount);
      if (retvalue)
      { 
        ForumUserModel existingForumUser = forumUser.Guid != Guid.Empty ? _forumDal.GetForumUserById(forumUser.Guid) : null;
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