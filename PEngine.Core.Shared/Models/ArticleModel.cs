using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Interfaces;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.Models
{
  public class ArticleModel : IGuidModel, ITimestampModel, IUniqueNameModel, ISubTitleModel
  {
    public Guid Guid { get; set; }
    public long? LegacyID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string ContentURL { get; set; }
    public string DefaultSection { get; set; } = string.Empty;
    public bool VisibleFlag { get; set; }
    public string UniqueName { get; set; } = string.Empty;
    public bool HideDropDownFlag { get; set; }
    public bool HideButtonsFlag { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    public List<ArticleSectionModel> Sections { get; set; } = new List<ArticleSectionModel>();

    public string GetSubTitle(bool inList, string currentSection, int? currentPage)
    {
      if (!inList)
      {
        var subTitle = $"{Name}";
        if (!string.IsNullOrWhiteSpace(currentSection))
        {
          var section = Sections.FirstOrDefault(s => s.UniqueName.Equals(currentSection, StringComparison.OrdinalIgnoreCase));
          if (section != null && !subTitle.Equals(section.Name, StringComparison.OrdinalIgnoreCase))
          {
            subTitle += $" - {section.Name}";
          }
        }
        return subTitle;
      }
      return $"{Category} Articles";
    }
  }
}
