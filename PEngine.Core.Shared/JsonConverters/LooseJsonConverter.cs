using System;
using Newtonsoft.Json;

namespace PEngine.Core.Shared.JsonConverters
{
  public class LooseJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return (objectType == typeof(int) || objectType == typeof(int?))
        || (objectType == typeof(decimal) || objectType == typeof(decimal?))
        || objectType == typeof(double) || objectType == typeof(double?)
        || objectType == typeof(bool) || objectType == typeof(bool?);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (objectType == typeof(Guid) || objectType == typeof(Guid?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          Guid parsed = Guid.Empty;
          if (Guid.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        return objectType == typeof(Guid?) ? (Guid?)null : Guid.Empty;
      }
      else if (objectType == typeof(int) || objectType == typeof(int?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          int parsed = 0;
          if (int.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
        {
          return Convert.ToInt32(reader.Value);
        }
        return objectType == typeof(int?) ? (int?)null : int.MinValue;
      }
      else if (objectType == typeof(decimal) || objectType == typeof(decimal?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          decimal parsed = 0;
          if (decimal.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
        {
          return Convert.ToDecimal(reader.Value);
        }
        return objectType == typeof(decimal?) ? (decimal?)null : decimal.MinValue;
      }
      else if (objectType == typeof(double) || objectType == typeof(double?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          double parsed = 0;
          if (double.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
        {
          return Convert.ToDouble(reader.Value);
        }
        return objectType == typeof(double?) ? (double?)null : double.MinValue;
      }
      else if (objectType == typeof(bool) || objectType == typeof(bool?))
      {
        if (reader.TokenType == JsonToken.String)
        {
          bool parsed = false;
          if (bool.TryParse((string)reader.Value, out parsed))
          {
            return parsed;
          }
        }
        else if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
        {
          return ((double)reader.Value) != 0;
        }
        else if (reader.TokenType == JsonToken.Boolean)
        {
          return Convert.ToBoolean(reader.Value);
        }
        return objectType == typeof(bool?) ? (bool?)null : false;
      }

      throw new JsonSerializationException("Unexpected token type: " + reader.TokenType.ToString());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      //Do Nothing
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
        return false;
      }
    }
  }
}