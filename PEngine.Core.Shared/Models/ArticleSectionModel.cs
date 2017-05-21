using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ArticleSectionModel : IGuidModel, ITimestampModel, IUniqueNameModel
  {
    public Guid Guid { get; set; }
    public Guid ArticleGuid { get; set; }
    public string Name { get; set; }
    public string Data { get; set; }
    public int SortOrder { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
