using Newtonsoft.Json;
using System.Numerics;

namespace DirectDimensional.Core.Miscs.JConverters;

public sealed unsafe class Vector2Converter : JsonConverter<Vector2> {
    private static readonly Type vec2type = typeof(Vector2);

    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != vec2type) throw new NotSupportedException($"Vector2Converter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.X = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Y = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer) {
        writer.WriteStartArray();
        {
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
        }
        writer.WriteEndArray();
    }
}
public sealed unsafe class Vector3Converter : JsonConverter<Vector3> {
    private static readonly Type vec3type = typeof(Vector3);

    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != vec3type) throw new NotSupportedException($"Vector3Converter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.X = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Y = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Z = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer) {
        writer.WriteStartArray();
        {
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            writer.WriteValue(value.Z);
        }
        writer.WriteEndArray();
    }
}
public sealed unsafe class Vector4Converter : JsonConverter<Vector4> {
    private static readonly Type vec4type = typeof(Vector4);

    public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != vec4type) throw new NotSupportedException($"Vector4Converter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.X = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Y = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Z = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.W = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer) {
        writer.WriteStartArray();
        {
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            writer.WriteValue(value.Z);
            writer.WriteValue(value.W);
        }
        writer.WriteEndArray();
    }
}
public sealed unsafe class QuaternionConverter : JsonConverter<Quaternion> {
    private static readonly Type quatType = typeof(Quaternion);

    public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != quatType) throw new NotSupportedException($"QuaternionConverter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.X = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Y = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.Z = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.W = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Quaternion value, JsonSerializer serializer) {
        writer.WriteStartArray();
        {
            writer.WriteValue(value.X);
            writer.WriteValue(value.Y);
            writer.WriteValue(value.Z);
            writer.WriteValue(value.W);
        }
        writer.WriteEndArray();
    }
}
public sealed unsafe class ComplexConverter : JsonConverter<Complex> {
    private static readonly Type compType = typeof(Complex);

    public override Complex ReadJson(JsonReader reader, Type objectType, Complex existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != compType) throw new NotSupportedException($"ComplexConverter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        double* ptr = (double*)&existingValue;

        ptr[0] = reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        ptr[1] = reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Complex value, JsonSerializer serializer) {
        writer.WriteStartArray();
        {
            writer.WriteValue(value.Real);
            writer.WriteValue(value.Imaginary);
        }
        writer.WriteEndArray();
    }
}
public sealed class BigIntegerConverter : JsonConverter<BigInteger> {
    private static readonly Type bigIntType = typeof(BigInteger);

    public override BigInteger ReadJson(JsonReader reader, Type objectType, BigInteger existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != bigIntType) throw new NotSupportedException($"BigIntegerConverter cannot be used for type '{objectType.FullName}'.");

        if (BigInteger.TryParse(reader.ReadAsString(), out var result)) {
            return result;
        }
        return default;
    }

    public override void WriteJson(JsonWriter writer, BigInteger value, JsonSerializer serializer) {
        writer.WriteValue(value.ToString());
    }
}
public sealed unsafe class Matrix3x2Converter : JsonConverter<Matrix3x2> {
    private static readonly Type mat3x2Type = typeof(Matrix3x2);

    public override Matrix3x2 ReadJson(JsonReader reader, Type objectType, Matrix3x2 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != mat3x2Type) throw new NotSupportedException($"Matrix3x2Converter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.M11 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M12 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M21 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M22 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M31 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M32 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        while (reader.Read() && reader.TokenType != JsonToken.EndArray) { }

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Matrix3x2 value, JsonSerializer serializer) {
        writer.WriteStartArray();

        writer.WriteValue(value.M11);
        writer.WriteValue(value.M12);
        writer.WriteValue(value.M21);
        writer.WriteValue(value.M22);
        writer.WriteValue(value.M31);
        writer.WriteValue(value.M32);

        writer.WriteEndArray();
    }
}
public sealed unsafe class Matrix4x4Converter : JsonConverter<Matrix4x4> {
    private static readonly Type mat4x4Type = typeof(Matrix4x4);

    public override Matrix4x4 ReadJson(JsonReader reader, Type objectType, Matrix4x4 existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != mat4x4Type) throw new NotSupportedException($"Matrix4x4Converter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartArray) throw new JsonException("Expected start array.");

        existingValue.M11 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M12 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M13 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M14 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        existingValue.M21 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M22 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M23 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M24 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        existingValue.M31 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M32 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M33 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M34 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        existingValue.M41 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M42 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M43 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;
        existingValue.M44 = (float)reader.ReadAsDouble().GetValueOrDefault();
        if (reader.TokenType == JsonToken.EndArray) return existingValue;

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Matrix4x4 value, JsonSerializer serializer) {
        writer.WriteStartArray();

        writer.WriteValue(value.M11);
        writer.WriteValue(value.M12);
        writer.WriteValue(value.M13);
        writer.WriteValue(value.M14);
        writer.WriteValue(value.M21);
        writer.WriteValue(value.M22);
        writer.WriteValue(value.M23);
        writer.WriteValue(value.M24);
        writer.WriteValue(value.M31);
        writer.WriteValue(value.M32);
        writer.WriteValue(value.M33);
        writer.WriteValue(value.M34);
        writer.WriteValue(value.M41);
        writer.WriteValue(value.M42);
        writer.WriteValue(value.M43);
        writer.WriteValue(value.M44);

        writer.WriteEndArray();
    }
}
public sealed class PlaneConverter : JsonConverter<Plane> {
    private static readonly Type _planeType = typeof(Plane);

    public override Plane ReadJson(JsonReader reader, Type objectType, Plane existingValue, bool hasExistingValue, JsonSerializer serializer) {
        if (objectType != _planeType) throw new NotSupportedException($"PlaneConverter cannot be used for type '{objectType.FullName}'.");
        if (reader.TokenType != JsonToken.StartObject) throw new JsonException("Expected start object.");

        reader.Read();
        existingValue.D = (float)reader.ReadAsDouble().GetValueOrDefault();

        reader.Read();
        reader.Read();
        existingValue.Normal = serializer.Deserialize<Vector3>(reader);

        return existingValue;
    }

    public override void WriteJson(JsonWriter writer, Plane value, JsonSerializer serializer) {
        writer.WriteStartObject();

        writer.WritePropertyName(nameof(Plane.D));
        writer.WriteValue(value.D);

        writer.WritePropertyName(nameof(Plane.Normal));
        serializer.Serialize(writer, value.Normal);

        writer.WriteEndObject();
    }
}