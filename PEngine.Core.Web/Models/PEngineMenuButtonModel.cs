using System;

namespace PEngine.Core.Web.Models
{
  public class PEngineMenuButtonModel
  {
    public string Text { get; set; }
    public string Url { get; set; }
    public string Attributes { get; set; } = string.Empty;

    public PEngineMenuButtonModel(string text, string url, string attributes = null)
    {
      Text = text;
      Url = url;
      Attributes = attributes ?? string.Empty;
    }
  }
}