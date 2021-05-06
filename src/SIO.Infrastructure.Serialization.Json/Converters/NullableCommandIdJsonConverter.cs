using System;
using Newtonsoft.Json;
using SIO.Infrastructure.Commands;

namespace SIO.Infrastructure.Serialization.Json.Converters
{
    public sealed class NullableCommandIdJsonConverter : JsonConverter<CommandId?>
    {
        public override void WriteJson(JsonWriter writer, CommandId? value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override CommandId? ReadJson(JsonReader reader, Type objectType, CommandId? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var value = serializer.Deserialize<string>(reader);

            if (value == null)
                return null;

            return CommandId.From(value);
        }
    }
}
