using System;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.JsonConverters
{
  public class DateOnlyJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof(DateTime) || objectType == typeof(DateTime?);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          DateTime parsed = DateTime.MinValue;
          if (DateTime.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        else if (reader.TokenType == JsonToken.Date)
        {
          return reader.ReadAsDateTime();
        }
        return objectType == typeof(DateTime?) ? (DateTime?)null : DateTime.MinValue;
      }

      throw new JsonSerializationException("Unexpected token type: " + reader.TokenType.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      //Do Nothing
      if (value is DateTime)
      {
        writer.WriteValue(((DateTime)value).ToString("yyyy-MM-dd"));
      }
      else if (value is Nullable<DateTime>)
      {
        if (value != null)
        {
          writer.WriteValue(((DateTime?)value).Value.ToString("yyyy-MM-dd"));
        }
        else
        {
          writer.WriteNull();
        }
      }
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return true;
      }
    }
  }
}