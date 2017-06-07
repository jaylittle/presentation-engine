using System;

namespace PEngine.Core.Shared
{
  public interface ISubTitleModel
  {
    string GetSubTitle(bool inList, string currentSection, int? currentPage);
  }
}