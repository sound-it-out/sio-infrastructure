using System;
using Newtonsoft.Json;
using SIO.Infrastructure.Queries;

namespace SIO.Infrastructure.Serialization.Json.Converters
{
    public sealed class NullableQueryIdJsonConverter : JsonConverter<QueryId?>
    {
        public override void WriteJson(JsonWriter writer, QueryId? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override QueryId? ReadJson(JsonReader reader, Type objectType, QueryId? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);

            if (value == null)
                return null;

            return QueryId.From(value);
        }
    }
}
