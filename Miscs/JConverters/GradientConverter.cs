using Newtonsoft.Json;

namespace DirectDimensional.Core.Miscs.JConverters {
    public sealed class GradientKeyConverter : JsonConverter<GradientKey> {
        private static readonly Type _gradientKeyType = typeof(GradientKey);

        public override GradientKey ReadJson(JsonReader reader, Type objectType, GradientKey existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (objectType != _gradientKeyType) throw new NotSupportedException($"GradientKeyConverter cannot be used to deserialize '{objectType.FullName}'");
            if (reader.TokenType != JsonToken.StartObject) throw new JsonException("Expected start object.");

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) return existingValue;
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonException("Expected property name");

                switch ((string)reader.Value!) {
                    case nameof(GradientKey.Color):
                        existingValue.Color = serializer.Deserialize<Color32>(reader);
                        break;

                    case nameof(GradientKey.Mode):
                        existingValue.Mode = (GradientColorMode)reader.ReadAsInt32().GetValueOrDefault();
                        break;

                    case nameof(GradientKey.Position):
                        existingValue.Position = (ushort)reader.ReadAsInt32().GetValueOrDefault();
                        break;
                }
            }

            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, GradientKey value, JsonSerializer serializer) {
            writer.WriteStartObject();

            writer.WritePropertyName(nameof(GradientKey.Color));
            serializer.Serialize(writer, value.Color);

            writer.WritePropertyName(nameof(GradientKey.Mode));
            writer.WriteValue((int)value.Mode);

            writer.WritePropertyName(nameof(GradientKey.Position));
            writer.WriteValue(value.Position);

            writer.WriteEndObject();
        }
    }

    public sealed class GradientConverter : JsonConverter<Gradient> {
        private static readonly Type _gradientType = typeof(Gradient);

        public override Gradient? ReadJson(JsonReader reader, Type objectType, Gradient? existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (objectType != _gradientType) throw new NotSupportedException($"GradientConverter cannot be used for type '{objectType.FullName}'.");
            if (reader.TokenType != JsonToken.StartObject) throw new JsonException("Expected start object token.");

            Gradient output = new();

            while (reader.Read()) {
                if (reader.TokenType == JsonToken.EndObject) return output;
                if (reader.TokenType != JsonToken.PropertyName) throw new JsonException("Expected a property name.");

                switch ((string)reader.Value!) {
                    case "Keys":
                        reader.Read();
                        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected an array represent gradient keys.");

                        output.AssignKeys((ReadOnlySpan<GradientKey>)serializer.Deserialize<GradientKey[]>(reader));
                        break;

                    case nameof(Gradient.Wrapping):
                        output.Wrapping = reader.ReadAsBoolean().GetValueOrDefault();
                        break;
                }
            }

            return output;
        }

        public override void WriteJson(JsonWriter writer, Gradient? value, JsonSerializer serializer) {
            writer.WriteStartObject();

            writer.WritePropertyName("Keys");

            if (value is not null) {
                serializer.Serialize(writer, value.Keys);
            }

            writer.WritePropertyName(nameof(Gradient.Wrapping));
            writer.WriteValue(value is not null && value.Wrapping);

            writer.WriteEndObject();
        }
    }
}