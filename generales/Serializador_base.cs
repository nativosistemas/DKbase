using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DKbase.generales
{
        public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
    {
        public override List<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }
            else
            {
                var singleValue = JsonSerializer.Deserialize<T>(ref reader, options);
                return new List<T> { singleValue };
            }
        }

        public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
    public static class Serializador_base
    {
        public static string SerializarAJson(this object objeto)
        {
            string jsonResultado = string.Empty;
            try
            {
                jsonResultado = JsonSerializer.Serialize(objeto);
            }
            catch (Exception ex) {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, objeto);
            }
            return jsonResultado;
        }
        public static T DeserializarJson<T>(this string jsonSerializado)
        {
            try
            {
                T obj  = JsonSerializer.Deserialize<T>(jsonSerializado);
                return obj;
            }
            catch (Exception ex)
            {
                DKbase.generales.Log.LogError(MethodBase.GetCurrentMethod(), ex, DateTime.Now, jsonSerializado);
                return default(T);
            }
        }
    }
}
