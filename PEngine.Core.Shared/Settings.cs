using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PEngine.Core.Shared
{
  public class SettingsData
  {
    // app_owner_name,
    public string OwnerName { get; set; } = "PEngine User";
    // app_owner_email,
    public string OwnerEmail { get; set; } = "pengineuser@pengine.com";
    // app_default_title,
    public string DefaultTitle { get; set; } = "Presentation Engine 5.0";
    // app_default_theme,
    public string DefaultTheme { get; set; } = "pengine-fresh-dark";
    // app_logo_frontpage,
    public string LogoFrontPage { get; set; } = "pengine_logo5.png";
    // app_recpage_news_summary,
    public int PerPagePostArchived { get; set; } = 10;
    // app_recpage_news,
    public int PerPagePostFront { get; set; } = 5;
    // app_recpage_search_results,
    public int PerPageSearchResults { get; set; } = 20;
    // app_recpage_rss,
    public int PerPageRSS { get; set; } = 20;
    // app_timelimit_forum_edit,
    public int TimeLimitForumPostEdit { get; set; } = 30;
    // app_timelimit_forum_login,
    public int TimeLimitForumToken { get; set; } = 30;
    // app_timelimit_admin_login,
    public int TimeLimitAdminToken { get; set; } = 30;
    // app_exclude_resume,
    public bool DisableResume { get; set; } = false;
    // app_exclude_theme,
    public bool DisableThemeSelection { get; set; } = false;
    // app_exclude_quotes,
    public bool DisableQuotes { get; set; } = false;
    // app_exclude_search,
    public bool DisableSearch { get; set; } = false;
    // app_exclude_forum,
    [JsonIgnore]
    public bool DisableForum { get; set; } = true;
    // app_exclude_rss,
    public bool DisableRSS { get; set; } = false;
    public string LabelArchivedPostsButton { get; set; } = "Archived Posts";
    // app_label_home,
    public string LabelHomeButton { get; set; } = "Home";
    // app_label_theme,
    public string LabelThemeButton { get; set; } = "Theme";
    // app_label_resume,
    public string LabelResumeButton { get; set; } = "Resume";
    // app_label_admin,
    public string LabelAdminLoginButton { get; set; } = "Admin";
    // app_label_admin2,
    public string LabelAdminLogoffButton { get; set; } = "Standard";
    // app_label_print,
    public string LabelPrintButton { get; set; } = "Print";
    // app_label_quote,
    public string LabelQuoteButton { get; set; } = "Quote";
    // app_label_clippy_button,
    public string LabelClippyButton { get; set; } = "Help!";
    public string LabelBackToHomeButton { get; set; } = "Back To Home";
    public string LabelReadTheRestButton { get; set; } = "Read the Rest";
    // app_pass_admin,
    public virtual string PasswordAdmin { get; set; } = string.Empty;
    public string ExternalBaseUrl { get; set; } = "http://localhost:5000/";
    public string TwitterCardSite { get; set; } = string.Empty;
    public virtual Guid SecretKey { get; set; } = Guid.NewGuid();
    public string UserNameAdmin { get; set; } = "PEngineAdmin";
    public string CookieDomain { get; set; } = string.Empty;
    public string BasePath { get; set; } = "/";
    public int CacheControlSeconds { get; set; } = 86400;
    public string HiddenThemeList { get; set; }
    public string FavIcon { get; set; } = "pengine_icon5.png";
    public string AdditionalHeaders { get; set; } = string.Empty;
    public string ErrorMessageTitle { get; set; } = "Well This Is Embarassing...";
    public string ErrorMessageNotFound { get; set; } = "Knock, Knock.\n\nWho is there?\n\nNobody.\n\nNobody Who?\n\nNobody At All.";
    public string ErrorMessageForbidden { get; set; } = "What's The Password?\n\nBBBBBBZZZZZZTTTTTT!\n\nWRONG!";
    public string ErrorMessageException { get; set; } = "That Was A Fine Example Of Unexcepted Behavior!\n\nDiagnosis: Your Request Is Dead Jim.";
    public bool SummaryModeFront { get; set; } = false;

    [JsonIgnore]
    public string CookiePath
    {
      get
      {
        if (string.IsNullOrWhiteSpace(BasePath))
        {
          return string.Empty;
        }
        return BasePath.TrimEnd('/');
      }
    }
    
    public bool IsThemeHidden(string theme)
    {
      return (HiddenThemeList ?? string.Empty).Split(',')
        .Any(ht => !string.IsNullOrWhiteSpace(ht) && string.Equals(ht, theme, StringComparison.OrdinalIgnoreCase));
    }
  }

  public class SettingsProvider : ISettingsProvider
  {
    public SettingsData Current
    {
      get
      {
        return Settings.Current;
      }
      set
      {
        Settings.Current = value;
      }
    }
  }

  public class Settings
  {
    public static void Startup(string contentRootPath)
    {
      ContentRootPath = contentRootPath;
      var current = Current;
      if (_defaultSettings)
      {
        Console.WriteLine("Default Settings detected - writing new settings file to disk");
        Current = _current;
      }
    }

    private static bool _defaultSettings = true;
    private static SettingsData _current;
    public static SettingsData Current
    {
      get
      {
        if (_current == null)
        {
          if (System.IO.File.Exists(SettingsFilePath))
          {
            try
            {
              _current = JsonConvert.DeserializeObject<SettingsData>(System.IO.File.ReadAllText(SettingsFilePath));
              _defaultSettings = false;
            }
            catch (Exception ex)
            {
              //JSON file is fucked - log to console and move the fuck on
              Console.WriteLine($"JSON Settings Read Failed with Error: {ex.Message}");
              _current = null;
            }
          }
          if (_current == null)
          {
            _defaultSettings = true;
            _current = new SettingsData();
          }        
        }
        return _current;
      }
      set
      {
        if (System.IO.File.Exists(SettingsFilePath))
        {
          System.IO.File.Delete(SettingsFilePath);
        }
        System.IO.File.WriteAllText(SettingsFilePath, JsonConvert.SerializeObject(_current, new JsonSerializerSettings(){
          ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(),
          Formatting = Formatting.Indented
        }));
        _defaultSettings = false;
      }
    }

    private static string SettingsFilePath
    {
      get 
      {
        return $"{ContentRootPath}pengine.settings.json";
      }
    }

    private static string _contentRootPath;
    public static string ContentRootPath
    {
      get
      {
        return _contentRootPath;
      }
      private set 
      {
        _contentRootPath = value + (value.EndsWith(System.IO.Path.DirectorySeparatorChar.ToString()) ? string.Empty : System.IO.Path.DirectorySeparatorChar.ToString());
      }
    }
  }
}