using System;

namespace PEngine.Core.Shared.Interfaces
{
  public interface ITimestampModel
  {
    DateTime? CreatedUTC
    {
      get;
      set;
    }
    DateTime? ModifiedUTC
    {
      get;
      set;
    }
  }
}