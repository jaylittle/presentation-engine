using System;

namespace PEngine.Core.Shared
{
  public interface ISettingsProvider
  {
    SettingsData Current { get; set; }
  }
}