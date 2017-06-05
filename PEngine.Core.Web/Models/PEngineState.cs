using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace PEngine.Core.Web
{
  public class PEngineState
  {
    private SettingsData _settings;
    private HttpRequest _request;
    private ArticleModel _article;
    private PostModel _post;
    private ResumeModel _resume;
    private ForumModel _forum;
    private ForumThreadModel _forumThread;
    private ForumThreadPostModel _forumThreadPost;
    public string Url { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }

    public string FullTitle
    {
      get
      {
        if (!string.IsNullOrEmpty(SubTitle))
        {
          return $"{Title} - {SubTitle}";
        }
        return Title;
      }
    }
    public List<KeyValuePair<string, string>> MenuButtons { get; set; }
    public List<KeyValuePair<string, string>> ArticleSections { get; set; }
    public bool HasAdmin { get; set; }
    public bool HasForumAdmin { get; set; }
    public string PEngineUserName { get; set; }
    public string PEngineUserType { get; set; }
    public string CurrentSection { get; set; }
    public int? CurrentPage { get; set; }
    public string Theme { get; set; }
    public string ThemePath
    {
      get
      {
        return $"themes/{Theme}/{Theme}.css";
      }
    }

    public PEngineState(SettingsData settings, HttpRequest request, object viewData = null, string currentSection = null, int? currentPage = null)
    {
      _settings = settings;
      _request = request;
      CurrentSection = currentSection;
      CurrentPage = currentPage;

      UpdateData(viewData);
    }

    public void UpdateData(object viewData)
    {
      _article = null;
      _post = null;
      _resume = null;
      _forum = null;
      _forumThread = null;
      _forumThreadPost = null;

      //Decipher view data
      if (viewData != null)
      {
        if (viewData is ArticleModel)
        {
          _article = (ArticleModel)viewData;
        }
        if (viewData is PostModel)
        {
          _post = (PostModel)viewData;
        }
        if (viewData is ResumeModel)
        {
          _resume = (ResumeModel)viewData;
        }
        if (viewData is ForumModel)
        {
          _forum = (ForumModel)viewData;
        }
        if (viewData is ForumThreadModel)
        {
          _forumThread = (ForumThreadModel)viewData;
        }
        if (viewData is ForumThreadPostModel)
        {
          _forumThreadPost = (ForumThreadPostModel)viewData;
        }
      }
      Init();
    }

    public void Init()
    {
      Url = _request.Path;
      Title = _settings.DefaultTitle;
      HasAdmin = false;
      HasForumAdmin = false;
      PEngineUserName = string.Empty;
      PEngineUserType = "Anonymous";
      Theme = !"Default".Equals(_settings.DefaultTheme, StringComparison.OrdinalIgnoreCase) ? _settings.DefaultTheme : "pengine-gray";
      
      //Process Cookies
      if (_request?.Cookies != null)
      {
        Theme = _request.Cookies.ContainsKey("theme") ? _request.Cookies["theme"] : Theme;
      }

      //Process Authentication
      if (_request?.HttpContext?.User != null && _request.HttpContext.User.Identity.IsAuthenticated)
      {
        HasAdmin = _request.HttpContext.User.IsInRole("PEngineAdmin");
        HasForumAdmin = _request.HttpContext.User.IsInRole("ForumAdmin");
        PEngineUserName = _request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
        PEngineUserType = _request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;
      }

      //Process Record
      if (_article != null)
      {
        SubTitle = $"Article {_article.Name}";
        if (!string.IsNullOrWhiteSpace(CurrentSection))
        {
          var section = _article.Sections.FirstOrDefault(s => s.UniqueName.Equals(CurrentSection, StringComparison.OrdinalIgnoreCase));
          if (section != null)
          {
            SubTitle += $" - {section.Name}";
          }
          else
          {
            CurrentSection = null;
          }
        }
        if (_post != null)
        {
          SubTitle = $"Post {_post.Name}";
        }
        if (_resume != null)
        {
          SubTitle = $"{_settings.LabelResumeButton} for {_settings.OwnerName}";
        }
        if (_forum != null)
        {
          SubTitle = $"Forum {_forum.Name}";
        }
        if (_forumThread != null)
        {
          if (CurrentPage.HasValue)
          {
            SubTitle = $"Forum {_forumThread.ForumName} - Thread {_forumThread.Name} - Page {CurrentPage.Value}";
          }
          else
          {
            SubTitle = $"Forum {_forumThread.ForumName} - Thread {_forumThread.Name}";
          }
        }
        if (_forumThreadPost != null)
        {
          SubTitle = $"Forum {_forumThreadPost.ForumName} - Thread {_forumThreadPost.ForumThreadName} - Post {_forumThreadPost.Guid}";
        }
      }
    }
  }
}