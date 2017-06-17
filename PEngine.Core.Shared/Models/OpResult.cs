using System;
using System.Collections.Generic;
using System.Linq;

namespace PEngine.Core.Shared.Models
{
  public class OpResult
  {
    public List<OpResultMessage> LogMessages { get; set; } = new List<OpResultMessage>();

    public bool Successful
    {
      get { return LogMessages.All(lm => lm.Type != OpResultMessageType.Error); }
    }
    
    public void LogError(string text)
    {
      LogMessages.Add(new OpResultMessage(text, OpResultMessageType.Error));
    }

    public void LogInfo(string text)
    {
      LogMessages.Add(new OpResultMessage(text, OpResultMessageType.Info));
    }

    public void Inhale(OpResult subResults)
    {
      LogMessages.AddRange((subResults.LogMessages));
    }
  }

  public class OpResultMessage
  {
    public OpResultMessageType Type { get; set; }
    public string Text { get; set; }

    public OpResultMessage(string text) : this(text, OpResultMessageType.Error)
    {
    }

    public OpResultMessage(string text, OpResultMessageType type)
    {
      Text = text;
      Type = type;
    }    
  }

  public enum OpResultMessageType
  {
    Info,
    Error
  }
}