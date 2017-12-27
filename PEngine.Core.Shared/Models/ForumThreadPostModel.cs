using System;
using PEngine.Core.Shared.Interfaces;

namespace PEngine.Core.Shared.Models
{
  public class ForumThreadPostModel : IGuidModel, ITimestampModel, ISubTitleModel
  {
    public Guid Guid { get; set; }
    public Guid ForumThreadGuid { get; set; }
    public string ForumThreadName { get; set; }
    public string ForumThreadUniqueName { get; set; }
    public string ForumName { get; set; }
    public string ForumUniqueName { get; set; }
    public Guid ForumUserGuid { get; set; }
    public string ForumUserId { get; set; }
    public bool VisibleFlag { get; set; }
    public bool LockFlag { get; set; }
    public string Data { get; set; }
    public string IPAddress { get; set; }
    public DateTime? CreatedUTC { get; set; }
    public DateTime? ModifiedUTC { get; set; }

    public string GetSubTitle(bool inList, string currentSection)
    {
      if (!inList)
      {
        return $"Forums - {ForumName} - Thread {ForumThreadName} - Post {Guid}";
      }
      return $"Forums - {ForumName} - Thread {ForumThreadName}";
    }
  }
}
