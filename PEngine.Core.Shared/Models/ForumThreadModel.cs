using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ForumThreadModel : IGuidModel, ITimestampModel, IUniqueNameModel, ISubTitleModel
  {
    public Guid Guid { get; set; }
    public Guid ForumGuid { get; set; }
    public string ForumName { get; set; }
    public string ForumUniqueName { get; set; }
    public Guid ForumUserGuid { get; set; }
    public string ForumUserId { get; set; }
    public bool VisibleFlag { get; set; }
    public bool LockFlag { get; set; }
    public string Name { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
    public ForumThreadPostModel InitialPost { get; set; }

    public string GetSubTitle(bool inList, string currentSection)
    {
      if (!inList)
      {
        return $"Forums - {ForumName} - Thread {Name}";
      }
      return $"Forums - {ForumName}";
    }
  }
}
