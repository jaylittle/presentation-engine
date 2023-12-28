using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared;
using PEngine.Core.Shared.Models;
using PEngine.Core.Data.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PEngine.Core.Web.Models
{
  public enum PEnginePage
  {
    None,
    Home,
    Posts,
    Articles,
    Post,
    Article,
    Resume,
    Login,
    Search,
    Error,
    NotFound,
    Forbidden,
    HackAttempt
  }
  public class PEngineStateModel
  {
    public const string COOKIE_THEME = "theme";
    public const string COOKIE_ACCESS_TOKEN = "access_token";

    private SettingsData _settings;
    private HttpContext _context;
    private ISubTitleModel _viewDataRecord;
    private IEnumerable<ISubTitleModel> _viewDataList;
    private bool _viewDataInList;
    private IServiceProvider _svp;
    private ILoggerFactory _logFactory;

    public string Url { get; set; }
    public string Title { get; set; }
    public string SubTitle { get; set; }
    [JsonIgnore]
    public bool HideSubTitle { get; set; }
    public string FullTitle
    {
      get
      {
        if (!HideSubTitle && !string.IsNullOrEmpty(SubTitle) && !Title.Equals(SubTitle, StringComparison.OrdinalIgnoreCase))
        {
          return $"{Title} - {SubTitle}";
        }
        return Title;
      }
    }
    [JsonIgnore]
    public List<PEngineMenuButtonModel> TopMenuButtons { get; set; }
    [JsonIgnore]
    public List<PEngineMenuButtonModel> SubMenuButtons { get; set; }
    [JsonIgnore]
    public Dictionary<string, string> Links { get; set; }

    public bool HasAdmin { get; set; }
    public bool HasForumAdmin { get; set; }
    public string PEngineUserName { get; set; }
    public string PEngineUserType { get; set; }
    public DateTime? TokenExpires { get; set; }
    public long? TokenExpiresMilliseconds { get; set; }
    public string CurrentSection { get; set; }
    public string Theme { get; set; }
    public string ThemePath
    {
      get
      {
        return $"themes/{Theme}/{Theme}.css";
      }
    }
    [JsonIgnore]
    public string SummaryTitle { get; set; }
    [JsonIgnore]
    public string SummaryDescription { get; set; }
    [JsonIgnore]
    public string SummaryUrl { get; set; }
    [JsonIgnore]
    public string SummaryImage { get; set; }
    [JsonIgnore]
    public string SummarySite { get; set; }
    [JsonIgnore]
    public bool HasSummary { get; set; }
    [JsonIgnore]
    public string Description { get; set; }

    public string LoginUrl
    {
      get
      {
        if (PEngineUserType.Equals("Anonymous", StringComparison.OrdinalIgnoreCase))
        {
          return "log/in/pengine";
        }
        return "log/out";
      }
    }

    public string LoginText
    {
      get
      {
        if (PEngineUserType.Equals("Anonymous", StringComparison.OrdinalIgnoreCase))
        {
          return "Login";
        }
        return "Logout";
      }
    }

    public IEnumerable<string> ThemeList
    {
      get
      {
        return Helpers.Rendering.ThemeList;
      }
    }

    public IEnumerable<string> IconList
    {
      get
      {
        return Helpers.Rendering.IconList;
      }
    }

    public string QuoteText { get; set; }

    public string XSRFToken { get; set; }

    public string Version { get; set; }
    
    public bool IsLockedDown
    {
      get
      {
        return _settings?.IsLockedDown ?? false;
      }
    }

    public PEnginePage Page { get; set; }
    public string PageLanguageCode { get; set; }

    public PEngineStateModel(IServiceProvider svp, SettingsData settings, HttpContext context, PEnginePage page, bool hideSubTitle = false)
    {
      _settings = settings;
      _context = context;
      HideSubTitle = hideSubTitle;
      CurrentSection = null;
      Page = page;
      _svp = svp;

      Init();
    }

    public PEngineStateModel(IServiceProvider svp, SettingsData settings, HttpContext context, PEnginePage page, bool hideSubTitle = false, ISubTitleModel viewDataRecord = null, string currentSection = null, int? currentPage = null)
    {
      _settings = settings;
      _context = context;
      HideSubTitle = hideSubTitle;
      CurrentSection = currentSection;
      Page = page;
      _svp = svp;

      UpdateData(viewDataRecord);
    }

    public PEngineStateModel(IServiceProvider svp, SettingsData settings, HttpContext context, PEnginePage page, bool hideSubTitle = false, IEnumerable<ISubTitleModel> viewDataList = null, string currentSection = null, int? currentPage = null)
    {
      _settings = settings;
      _context = context;
      HideSubTitle = hideSubTitle;
      CurrentSection = currentSection;
      Page = page;
      _svp = svp;

      UpdateData(viewDataList);
    }

    public void UpdateData(ISubTitleModel viewDataRecord)
    {
      _viewDataRecord = viewDataRecord;
      _viewDataList = null;
      _viewDataInList = false;

      Init();
    }

    public void UpdateData(IEnumerable<ISubTitleModel> viewDataList)
    {
      _viewDataList = viewDataList;
      _viewDataRecord = _viewDataList?.FirstOrDefault();
      _viewDataInList = true;

      Init();
    }

    public void Init()
    {
      Url = _context.Request.Path;
      Title = _settings.DefaultTitle;
      Description = _settings.DefaultDescription;
      HasAdmin = false;
      HasForumAdmin = false;
      PEngineUserName = string.Empty;
      PEngineUserType = "Anonymous";
      Version = Helpers.SystemInfoHelpers.Version;

      ILogger logger = null;
      if (_svp != null)
      {
        _logFactory = _svp.GetRequiredService<ILoggerFactory>();
        logger = _logFactory.CreateLogger<PEngineStateModel>();
      }
      
      Theme = string.Empty;
      var themeList = Helpers.Rendering.ThemeList;
      if (themeList.Any())
      {
        if (!themeList.Any(t => t.Equals(_settings.DefaultTheme, StringComparison.OrdinalIgnoreCase)))
        {
          Theme = themeList.First();
        }
        else
        {
          Theme = _settings.DefaultTheme;
        }
      }
      
      //Process Cookies
      if (_context.Request?.Cookies != null)
      {
        var requestedTheme = _context.Request.Cookies.ContainsKey(COOKIE_THEME) ? _context.Request.Cookies[COOKIE_THEME] : Theme;
        if (themeList.Any(t => t.Equals(requestedTheme, StringComparison.OrdinalIgnoreCase)))
        {
          Theme = requestedTheme;
        }
      }

      //Process Authentication
      if (_context.Request?.HttpContext?.User != null && _context.Request.HttpContext.User.Identity.IsAuthenticated)
      {
        HasAdmin = _context.Request.HttpContext.User.IsInRole("PEngineAdmin");
        HasForumAdmin = _context.Request.HttpContext.User.IsInRole("ForumAdmin");
        PEngineUserName = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserName"))?.Value;
        PEngineUserType = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineUserType"))?.Value;
        string sessionId = _context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("PEngineSessionId"))?.Value;
        var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TokenExpires = epoch.AddSeconds(double.Parse(_context.Request.HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("exp"))?.Value));
        TokenExpiresMilliseconds = (long)TokenExpires.Value.Subtract(DateTime.UtcNow).TotalMilliseconds;
        XSRFToken = Security.XSRF.GetXsrfValues(PEngineUserName, sessionId).formValue;
      }

      //Process Record
      SubTitle = null;
      TopMenuButtons = new List<PEngineMenuButtonModel>();
      SubMenuButtons = new List<PEngineMenuButtonModel>();
      Links = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      TopMenuButtons.Add(new PEngineMenuButtonModel(_settings.LabelHomeButton, _settings.BasePath));
      if (!_settings.DisableResume)
      {
        TopMenuButtons.Add(new PEngineMenuButtonModel(_settings.LabelResumeButton, "resume"));
      }

      var articleDal = _svp.GetRequiredService<IArticleDal>();
      var articleCategories = articleDal.ListArticles(null).Result
        .Where(a => 
          HasAdmin ||
          (!_settings.IsLockedDown && a.VisibleFlag && !a.NoIndexFlag) ||
          (_settings.IsLockedDown && a.LockDownVisibleFlag && !a.NoIndexFlag)
        )
        .GroupBy(a => $"{a.Category}|{a.ContentURL}", StringComparer.OrdinalIgnoreCase)
        .OrderBy(g => g.Key)
        .Select(g => new {
          key = g.Key,
          firstArticle = g.First(),
          count = g.Count()
        });

      foreach (var articleCategory in articleCategories)
      {
        var categoryElements = articleCategory.key.Split('|');
        var categoryUrl = $"article/category/{categoryElements[0]}";
        var categoryLinkAttributes = string.Empty;

        //Override Category URL if appropriate
        if (!HasAdmin)
        {
          if (!string.IsNullOrWhiteSpace(categoryElements[1]))
          {
            categoryUrl = categoryElements[1];
            categoryLinkAttributes = articleCategory.firstArticle.ContentLinkAttributes ?? string.Empty;
          }
          else if (articleCategory.count == 1)
          {
            categoryUrl = $"article/view/{articleCategory.firstArticle.UniqueName}";
          }
        }

        TopMenuButtons.Add(new PEngineMenuButtonModel(categoryElements[0], categoryUrl, categoryLinkAttributes));
      }

      Uri requestUri = null;
      string displayUrl = _context.Request.GetDisplayUrl();
      if (Uri.TryCreate(displayUrl, UriKind.Absolute, out requestUri))
      {
        SummaryUrl = $"{_settings.ExternalBaseUrl.TrimEnd('/')}{requestUri.PathAndQuery}";
      }
      else
      {
        if (logger != null)
        {
          logger.LogError($"AspNetCost GetDisplayUrl returned invalid Uri: {displayUrl}");
        }
        SummaryUrl = string.Empty;
      }
      SummaryTitle = string.Empty;
      SummaryDescription = string.Empty;
      SummarySite = _settings.DefaultTitle;
      SummaryImage = $"{_settings.ExternalBaseUrl.TrimEnd('/')}/images/system/{_settings.LogoFrontPage}";

      if (_viewDataRecord != null)
      {
        SubTitle = _viewDataRecord.GetSubTitle(_viewDataInList, CurrentSection);
        
        if (!_viewDataInList && _viewDataRecord is ArticleModel)
        {
          var articleData = (ArticleModel)_viewDataRecord;
          if (string.IsNullOrWhiteSpace(articleData.ContentURL))
          {
            var displayedSection = (CurrentSection ?? articleData.DefaultSection);
            var currentSectionData = (displayedSection != null) 
              ? articleData.Sections.FirstOrDefault(s => s.UniqueName.Equals(displayedSection, StringComparison.OrdinalIgnoreCase))
              : null;
            var articleSections = articleData.Sections.OrderBy(s => s.SortOrder).ThenBy(s => s.Name).ToList();
            currentSectionData = currentSectionData ?? articleSections.First();
            CurrentSection = currentSectionData.UniqueName;
            SubTitle = _viewDataRecord.GetSubTitle(_viewDataInList, CurrentSection);
            for (var sectionPtr = 0; sectionPtr < articleSections.Count; sectionPtr++)
            {
              var section = articleSections[sectionPtr];
              var sectionUrl = $"article/view/{articleData.UniqueName}/{section.UniqueName}";
              if (section == currentSectionData)
              {
                if (sectionPtr > 0)
                {
                  Links.Add("Previous", $"article/view/{articleData.UniqueName}/{articleSections[sectionPtr - 1].UniqueName}");
                }
                if (sectionPtr < articleSections.Count -1)
                {
                  Links.Add("Next", $"article/view/{articleData.UniqueName}/{articleSections[sectionPtr + 1].UniqueName}");
                }
              }
              if (!articleData.HideButtonsFlag)
              {
                SubMenuButtons.Add(new PEngineMenuButtonModel(section.Name, sectionUrl));
              }
            }
            SummaryTitle = !HideSubTitle ? SubTitle : _settings.DefaultTitle;
            SummaryDescription = Helpers.Rendering.DataTruncate(!string.IsNullOrWhiteSpace(articleData.Description) ? articleData.Description : (currentSectionData?.Data ?? string.Empty), -1);
          }
        }

        if (!_viewDataInList && _viewDataRecord is PostModel)
        {
          var postData = (PostModel)_viewDataRecord;
          SummaryTitle = !HideSubTitle ? SubTitle : _settings.DefaultTitle;
          SummaryDescription = Helpers.Rendering.DataRenderAndTruncate(postData.Data, -1);
        }

        if (!_viewDataInList && _viewDataRecord is ResumeModel)
        {
          var resumeData = (ResumeModel)_viewDataRecord;
          SummaryTitle = !HideSubTitle ? SubTitle : _settings.DefaultTitle;
          SummaryDescription = resumeData.Objectives?.FirstOrDefault()?.Data ?? string.Empty;
        }
      }
      if (string.IsNullOrWhiteSpace(SummaryTitle))
      {
        switch (Page)
        {
          case PEnginePage.Home:
            SummaryTitle = _settings.LabelHomeButton;
            break;
          case PEnginePage.Articles:
            SummaryTitle = "Articles";
            if (_viewDataInList && _viewDataRecord is ArticleModel)
            {
              var articleCategory = ((ArticleModel)_viewDataRecord)?.Category;
              if (!string.IsNullOrWhiteSpace(articleCategory))
              {
                SummaryTitle = articleCategory + " " + SummaryTitle;
              }
            }
            break;
          case PEnginePage.Posts:
            SummaryTitle = _settings.LabelArchivedPostsButton;
            break;
          case PEnginePage.Search:
            SummaryTitle = "Search Results";
            break;
          case PEnginePage.Login:
            SummaryTitle = "Login";
            break;
          case PEnginePage.NotFound:
            SummaryTitle = "Not Found";
            break;
          case PEnginePage.Error:
            SummaryTitle = "Error";
            break;
          case PEnginePage.Forbidden:
            SummaryTitle = "Forbidden";
            break;
          case PEnginePage.HackAttempt:
            SummaryTitle = "Hack Attempt";
            break;
        }
        if (!HideSubTitle && string.IsNullOrWhiteSpace(SubTitle))
        {
          SubTitle = SummaryTitle;
        }
      }

      //Override Default Description with Summary Description if Available
      if (!string.IsNullOrWhiteSpace(SummaryDescription))
      {
        Description = SummaryDescription;
      }
      else
      {
        //If Summary Title has been set without a description
        //use Default Description as Summary Description
        if (!string.IsNullOrWhiteSpace(SummaryTitle))
        {
          SummaryDescription = _settings.DefaultDescription;
        }
      }

      HasSummary = !String.IsNullOrWhiteSpace(SummaryTitle);
      PageLanguageCode = _settings.PageLanguageCode;
    }

    public void ThemeChange(string requestedTheme)
    {
      if (_context.Request != null)
      {
        var themeList = Helpers.Rendering.ThemeList;
        if (themeList.Any(t => t.Equals(requestedTheme, StringComparison.OrdinalIgnoreCase)))
        {
          Theme = requestedTheme;

          var cookieOptions = Helpers.HttpHelpers.GetCookieOptions(_context);
          cookieOptions.Expires = DateTime.UtcNow.AddYears(10);

          _context.Response.Cookies.Append(COOKIE_THEME, requestedTheme, cookieOptions);
        }
      }
    }

    public string Json()
    {
      return JsonConvert.SerializeObject(this, new JsonSerializerSettings 
      { 
        ContractResolver = new CamelCasePropertyNamesContractResolver() 
      });
    }
  }
}