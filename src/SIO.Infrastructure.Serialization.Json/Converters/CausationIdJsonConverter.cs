using System;
using Newtonsoft.Json;

namespace SIO.Infrastructure.Serialization.Json.Converters
{
    public sealed class CausationIdJsonConverter : JsonConverter<CausationId>
    {
        public override void WriteJson(JsonWriter writer, CausationId value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override CausationId ReadJson(JsonReader reader, Type objectType, CausationId existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);
            return CausationId.From(value);
        }
    }
}
