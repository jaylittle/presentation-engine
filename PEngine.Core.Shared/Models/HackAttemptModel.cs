using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Interfaces;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.Models
{
  public enum HackAttemptType
  {
    Generic,
    WrongCMS,
    WrongTechStack,
    WrongLoginPage,
    SensitiveFile,
    SensitiveFolder,
  }

  public class HackAttemptModel : ISubTitleModel
  {
    public string Title { get; set; }
    public string Data { get; set; }
    public HackAttemptType Type { get; set; }

    public HackAttemptModel(string title, string data, HackAttemptType type)
    {
      Title = title;
      Data = data;
      Type = type;
    }

    public string GetSubTitle(bool inList, string currentSection)
    {
      if (Type != HackAttemptType.Generic)
      {
        return $"Hack Attempt Detected - {Type}";
      }
      return "Hack Attempt Detected";
    }
  }
}
