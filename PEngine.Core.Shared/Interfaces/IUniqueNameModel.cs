using System;

namespace PEngine.Core.Shared.Interfaces
{
  public interface IUniqueNameModel
  {
    string Name { get; set; }
    string UniqueName { get; set; }
  }
}