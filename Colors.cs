using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace DirectDimensional.Core {
    public struct Color : IEquatable<Color>, IFormattable {
        public float R { get; set; }
        public float G { get; set; }
        public float B { get; set; }
        public float A { get; set; }

        public Color(float r, float g, float b) {
            R = r; G = g; B = b; A = 1;
        }

        public Color(float r, float g, float b, float a) {
            R = r; G = g; B = b; A = a;
        }

        public static implicit operator Color32(Color color) {
            return new Color32((byte)Math.Round(color.R * 255f), (byte)Math.Round(color.G * 255f), (byte)Math.Round(color.B * 255f), (byte)Math.Round(color.A * 255f));
        }

        public bool Equals(Color other) {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public override string ToString() {
            return "Color(R: " + R + ", G: " + G + ", B: " + B + ", A: " + A + ")";
        }

        public override int GetHashCode() {
            return R.GetHashCode() ^ (G.GetHashCode() << 2) ^ (B.GetHashCode() >> 3) ^ (A.GetHashCode() << 1);
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Color other) return false;

            return Equals(other);
        }

        public string ToString(string? format, IFormatProvider? provider) {
            if (string.IsNullOrEmpty(format)) format = "F";
            if (provider == null) provider = CultureInfo.CurrentCulture;

            switch (format.ToUpperInvariant()[0]) {
                case 'F':
                case 'E':
                case 'N':
                case 'G':
                case 'P':
                case 'R':
                    return "Color(R: " + R.ToString(format, provider) + ", G: " + G.ToString(format, provider) + ", B: " + B.ToString(format, provider) + ", A: " + A.ToString(format, provider) + ")";

                case 'X':
                    return "Color(R: " + ((byte)(R * 255)).ToString(format, provider) + ", G: " + ((byte)(G * 255)).ToString(format, provider) + ", B: " + ((byte)(B * 255)).ToString(format, provider) + ", A: " + ((byte)(A * 255)).ToString(format, provider) + ")";

                default:
                    throw new FormatException(string.Format("The {0} format string is not supported.", format));
            }
        }

        public static bool operator ==(Color left, Color right) {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right) {
            return !(left == right);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct Color32 : IEquatable<Color32>, IFormattable {
        public static readonly Color32 White = new(0xFFFFFFFF);
        public static readonly Color32 Black = new(0xFF000000);
        public static readonly Color32 Red = new(0xFF0000FF);
        public static readonly Color32 Green = new(0xFF00FF00);
        public static readonly Color32 Blue = new(0xFFFF0000);
        public static readonly Color32 Transparent = new(0);

        [field: FieldOffset(0)]
        public int Integer { get; set; }
        [field: FieldOffset(0)]
        public uint Unsigned { get; set; }

        [field: FieldOffset(0)]
        public byte R { get; set; }
        [field: FieldOffset(1)]
        public byte G { get; set; }
        [field: FieldOffset(2)]
        public byte B { get; set; }
        [field: FieldOffset(3)]
        public byte A { get; set; }

        public Color32(byte r, byte g, byte b) {
            Integer = 0;
            Unsigned = 0u;

            R = r; G = g; B = b; A = 255;
        }

        public Color32(byte r, byte g, byte b, byte a) {
            Integer = 0;
            Unsigned = 0u;

            R = r; G = g; B = b; A = a;
        }

        public Color32(int integer) {
            R = 0; G = 0; B = 0; A = 0;
            Unsigned = 0u;

            Integer = integer;
        }

        public Color32(uint unsigned) {
            R = 0; G = 0; B = 0; A = 0;
            Integer = 0;

            Unsigned = unsigned;
        }

        public static implicit operator Color(Color32 color) {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public bool Equals(Color32 other) {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Color32 left, Color32 right) {
            return left.Integer == right.Integer;
        }

        public static bool operator !=(Color32 left, Color32 right) {
            return left.Integer != right.Integer;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Color32 color) return false;

            return this == color;
        }

        public override int GetHashCode() {
            return Integer.GetHashCode();
        }

        public string ToString(string? format, IFormatProvider? provider) {
            if (string.IsNullOrEmpty(format)) format = "D";
            if (provider == null) provider = CultureInfo.CurrentCulture;

            switch (format.ToUpperInvariant()[0]) {
                default:
                    return "Color32(R: " + R.ToString(format, provider) + ", G: " + G.ToString(format, provider) + ", B: " + B.ToString(format, provider) + ", A: " + A.ToString(format, provider) + ")";

                case 'X':
                    return "Color32(R: " + R.ToString("X2", provider) + ", G: " + G.ToString("X2", provider) + ", B: " + B.ToString("X2", provider) + ", A: " + A.ToString("X2", provider) + ")";
            }
        }

        public override string ToString() {
            return "Color32(R: " + R.ToString("X2") + ", G: " + G.ToString("X2") + ", B: " + B.ToString("X2") + ", A: " + A.ToString("X2") + ")";
        }
    }
}
