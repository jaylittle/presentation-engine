using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ForumThreadModel : IGuidModel, ITimestampModel, IUniqueNameModel
  {
    public Guid Guid { get; set; }
    public Guid ForumGuid { get; set; }
    public string ForumName { get; set; }
    public string ForumUniqueName { get; set; }
    public Guid ForumUserGuid { get; set; }
    public string ForumUserName { get; set; }
    public bool VisibleFlag { get; set; }
    public bool LockFlag { get; set; }
    public string Name { get; set; }
    public string UniqueName { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }
  }
}
