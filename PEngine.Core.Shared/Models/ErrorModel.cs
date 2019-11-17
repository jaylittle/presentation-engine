using System;
using System.Linq;
using System.Collections.Generic;
using PEngine.Core.Shared.Interfaces;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.Models
{
  public class ErrorModel : ISubTitleModel
  {
    public string Title { get; set; }
    public string Data { get; set; }
    public string Code { get; set; }

    public ErrorModel(string title, string data, string code)
    {
      Title = title;
      Data = data;
      Code = code;
    }

    public string GetSubTitle(bool inList, string currentSection)
    {
      if (!string.IsNullOrEmpty(Code))
      {
        return $"Error - {Code}";
      }
      return "Error";
    }
  }
}
