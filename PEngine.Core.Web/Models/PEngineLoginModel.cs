using PEngine.Core.Shared;

namespace PEngine.Core.Web.Models
{
  public class PEngineLoginModel : ISubTitleModel
  {
    public string ActionUrl { get; set; }
    public string RedirectUrl { get; set; }

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      return "Login";
    }
  }
}