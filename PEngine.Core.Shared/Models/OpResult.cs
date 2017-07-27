using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
      LogGeneric(text, OpResultMessageType.Error);
    }

    public void LogInfo(string text)
    {
      LogGeneric(text, OpResultMessageType.Info);
    }

    public void LogGeneric(string text, OpResultMessageType type)
    {
      var logMessage = new OpResultMessage(text, type);
      LogMessages.Add(logMessage);
      
      if (OutputToConsole)
      {
        Console.WriteLine(logMessage); 
      }
    }

    public void Inhale(OpResult subResults)
    {
      LogMessages.AddRange((subResults.LogMessages));
      if (OutputToConsole && !subResults.OutputToConsole)
      {
        foreach (var logMessage in subResults.LogMessages)
        {
          Console.WriteLine(logMessage);
        }
      }
    }

    [JsonIgnore]
    public bool OutputToConsole { get; set; }

    public OpResult()
    {
      
    }

    public OpResult(bool outputToConsole)
    {
      OutputToConsole = outputToConsole;
    }
  }

  public class OpResultMessage
  {
    [JsonConverter(typeof(StringEnumConverter))]
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

    public override string ToString()
    {
      return $"{Type}: {Text}";
    }
  }

  public enum OpResultMessageType
  {
    Info,
    Error
  }
}