using Newtonsoft.Json;

namespace DirectDimensional.Core.Miscs.JConverters {
    public sealed class ColorConverter : JsonConverter<Color> {
        internal static readonly Type __colType = typeof(Color);
        internal static readonly Type __col32Type = typeof(Color32);

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (objectType != __colType && objectType != __col32Type) throw new NotSupportedException($"ColorConverter cannot be used to deserialize '{objectType.FullName}'");
            
            var read = reader.ReadAsInt32();
            if (read is not null) {
                return new Color32(read.Value);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer) {
            writer.WriteValue(((Color32)value).Integer);
        }
    }

    public sealed class Color32Converter : JsonConverter<Color32> {
        public override Color32 ReadJson(JsonReader reader, Type objectType, Color32 existingValue, bool hasExistingValue, JsonSerializer serializer) {
            if (objectType != ColorConverter.__colType && objectType != ColorConverter.__col32Type) throw new NotSupportedException($"Color32Converter cannot be used to deserialize '{objectType.FullName}'");

            var read = reader.ReadAsInt32();
            if (read is not null) {
                return new Color32(read.Value);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, Color32 value, JsonSerializer serializer) {
            writer.WriteValue(value.Integer);
        }
    }
}
