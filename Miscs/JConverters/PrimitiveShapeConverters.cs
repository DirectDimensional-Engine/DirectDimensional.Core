using Newtonsoft.Json;

namespace DirectDimensional.Core.Miscs.JConverters;

//namespace DirectDimensional.Core.Miscs.JConverters {
//    public sealed class RectConverter : JsonConverter<Rect> {
//        private static readonly Type _reqType = typeof(Rect);

//        public override Rect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
//            if (typeToConvert != _reqType) {
//                throw new NotSupportedException($"Rectangle converter cannot be used for type '{typeToConvert.FullName}'.");
//            }
//            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

//            Rect output = default;

//            while (reader.Read()) {
//                if (reader.TokenType == JsonTokenType.EndObject) return output;
//                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected a property name.");

//                switch (reader.GetString()) {
//                    case nameof(Rect.X):
//                        reader.Read();
//                        output.X = reader.GetSingle();
//                        break;
//                    case nameof(Rect.Y):
//                        reader.Read();
//                        output.Y = reader.GetSingle();
//                        break;
//                    case nameof(Rect.Width):
//                        reader.Read();
//                        output.Width = reader.GetSingle();
//                        break;
//                    case nameof(Rect.Height):
//                        reader.Read();
//                        output.Height = reader.GetSingle();
//                        break;
//                }
//            }

//            return output;
//        }
//        public override void Write(Utf8JsonWriter writer, Rect value, JsonSerializerOptions options) {
//            writer.WriteStartObject();
//            {
//                writer.WriteNumber(nameof(Rect.X), value.X);
//                writer.WriteNumber(nameof(Rect.Y), value.Y);
//                writer.WriteNumber(nameof(Rect.Width), value.Width);
//                writer.WriteNumber(nameof(Rect.Height), value.Height);
//            }
//            writer.WriteEndObject();
//        }
//    }
//    public sealed class CircleConverter : JsonConverter<Circle> {
//        private static readonly Type _reqType = typeof(Circle);

//        public override Circle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
//            if (typeToConvert != _reqType) {
//                throw new NotSupportedException($"Circle converter cannot be used for type '{typeToConvert.FullName}'.");
//            }
//            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

//            Circle output = default;

//            while (reader.Read()) {
//                if (reader.TokenType == JsonTokenType.EndObject) return output;
//                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected a property name.");

//                switch (reader.GetString()) {
//                    case nameof(Circle.X):
//                        reader.Read();
//                        output.X = reader.GetSingle();
//                        break;
//                    case nameof(Circle.Y):
//                        reader.Read();
//                        output.Y = reader.GetSingle();
//                        break;
//                    case nameof(Circle.Radius):
//                        reader.Read();
//                        output.Radius = reader.GetSingle();
//                        break;
//                }
//            }

//            return output;
//        }
//        public override void Write(Utf8JsonWriter writer, Circle value, JsonSerializerOptions options) {
//            writer.WriteStartObject();
//            {
//                writer.WriteNumber(nameof(Circle.X), value.X);
//                writer.WriteNumber(nameof(Circle.Y), value.Y);
//                writer.WriteNumber(nameof(Circle.Radius), value.Radius);
//            }
//            writer.WriteEndObject();
//        }
//    }
//    public sealed class LineConverter : JsonConverter<Line> {
//        private static readonly Type _reqType = typeof(Line);

//        public override Line Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
//            if (typeToConvert != _reqType) {
//                throw new NotSupportedException($"Line converter cannot be used for type '{typeToConvert.FullName}'.");
//            }
//            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

//            Line output = default;

//            while (reader.Read()) {
//                if (reader.TokenType == JsonTokenType.EndObject) return output;
//                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected a property name.");

//                switch (reader.GetString()) {
//                    case "sx":
//                        reader.Read();
//                        output.Start.X = reader.GetSingle();
//                        break;
//                    case "sy":
//                        reader.Read();
//                        output.Start.Y = reader.GetSingle();
//                        break;
//                    case "ex":
//                        reader.Read();
//                        output.End.X = reader.GetSingle();
//                        break;
//                    case "ey":
//                        reader.Read();
//                        output.End.Y = reader.GetSingle();
//                        break;
//                }
//            }

//            return output;
//        }
//        public override void Write(Utf8JsonWriter writer, Line value, JsonSerializerOptions options) {
//            writer.WriteStartObject();
//            {
//                writer.WriteNumber("sx", value.Start.X);
//                writer.WriteNumber("sy", value.Start.Y);
//                writer.WriteNumber("ex", value.End.X);
//                writer.WriteNumber("ey", value.End.Y);
//            }
//            writer.WriteEndObject();
//        }
//    }
//    public sealed class TriangleConverter : JsonConverter<Triangle> {
//        private static readonly Type _reqType = typeof(Triangle);

//        public override Triangle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
//            if (typeToConvert != _reqType) {
//                throw new NotSupportedException($"Triangle converter cannot be used for type '{typeToConvert.FullName}'.");
//            }
//            if (reader.TokenType != JsonTokenType.StartObject) throw new JsonException();

//            Triangle output = default;

//            while (reader.Read()) {
//                if (reader.TokenType == JsonTokenType.EndObject) return output;
//                if (reader.TokenType != JsonTokenType.PropertyName) throw new JsonException("Expected a property name.");

//                switch (reader.GetString()) {
//                    case "ax":
//                        reader.Read();
//                        output.A.X = reader.GetSingle();
//                        break;
//                    case "ay":
//                        reader.Read();
//                        output.A.Y = reader.GetSingle();
//                        break;
//                    case "bx":
//                        reader.Read();
//                        output.B.X = reader.GetSingle();
//                        break;
//                    case "by":
//                        reader.Read();
//                        output.B.Y = reader.GetSingle();
//                        break;
//                    case "cx":
//                        reader.Read();
//                        output.C.X = reader.GetSingle();
//                        break;
//                    case "cy":
//                        reader.Read();
//                        output.C.Y = reader.GetSingle();
//                        break;
//                }
//            }

//            return output;
//        }
//        public override void Write(Utf8JsonWriter writer, Triangle value, JsonSerializerOptions options) {
//            writer.WriteStartObject();
//            {
//                writer.WriteNumber("ax", value.A.X);
//                writer.WriteNumber("ay", value.A.Y);
//                writer.WriteNumber("bx", value.B.X);
//                writer.WriteNumber("by", value.B.Y);
//                writer.WriteNumber("cx", value.C.X);
//                writer.WriteNumber("cy", value.C.Y);
//            }
//            writer.WriteEndObject();
//        }
//    }
//}
