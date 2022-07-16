using System.Globalization;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using DirectDimensional.Core.Utilities;
using DirectDimensional.Core.Miscs.JConverters;
using Newtonsoft.Json;

namespace DirectDimensional.Core {
    [JsonConverter(typeof(ColorConverter))]
    public struct Color : IEquatable<Color>, IFormattable {
        public float R;
        public float G;
        public float B;
        public float A;

        public float Grayscale => 0.299f * R + 0.587f * G + 0.114f * B;
        public Color GrayscaleColor {
            get {
                var gs = Grayscale;
                return new(gs, gs, gs, A);
            }
        }
        public Color Gamma2Linear => new(DDMath.GammaToLinear(R), DDMath.GammaToLinear(G), DDMath.GammaToLinear(B), DDMath.GammaToLinear(A));
        public Color Linear2Gamma => new(DDMath.LinearToGamma(R), DDMath.LinearToGamma(G), DDMath.LinearToGamma(B), DDMath.LinearToGamma(A));

        public Color(float r, float g, float b) {
            R = r; G = g; B = b; A = 1;
        }

        public Color(float r, float g, float b, float a) {
            R = r; G = g; B = b; A = a;
        }

        public Color WithRed(float r) => new(r, G, B, A);
        public Color WithGreen(float g) => new(R, g, B, A);
        public Color WithBlue(float b) => new(R, G, b, A);
        public Color WithAlpha(float a) => new(R, G, B, a);

        /// <summary>
        /// Return new color with alpha multiply by given multiplier
        /// </summary>
        /// <param name="multiplier">Fade amount</param>
        public Color FadeMultiply(float multiplier) => new(R, G, B, A * multiplier);

        public static Color Lerp(Color left, Color right, float t) {
            t = DDMath.Saturate(t);
            return new Color(DDMath.LerpUnclamped(left.R, right.R, t), DDMath.LerpUnclamped(left.G, right.G, t), DDMath.LerpUnclamped(left.B, right.B, t), DDMath.LerpUnclamped(left.A, right.A, t));
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
            return HashCode.Combine(R, G, B, A);
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Color other) return false;

            return Equals(other);
        }

        public string ToString(string? format, IFormatProvider? provider) {
            if (string.IsNullOrEmpty(format)) format = "F";
            if (provider == null) provider = CultureInfo.CurrentCulture;

            return format.ToUpperInvariant()[0] switch {
                'F' or 'E' or 'N' or 'G' or 'P' or 'R' => "Color(R: " + R.ToString(format, provider) + ", G: " + G.ToString(format, provider) + ", B: " + B.ToString(format, provider) + ", A: " + A.ToString(format, provider) + ")",
                'X' => "Color(R: " + ((byte)(R * 255)).ToString(format, provider) + ", G: " + ((byte)(G * 255)).ToString(format, provider) + ", B: " + ((byte)(B * 255)).ToString(format, provider) + ", A: " + ((byte)(A * 255)).ToString(format, provider) + ")",
                _ => throw new FormatException(string.Format("The {0} format string is not supported.", format)),
            };
        }

        public static bool operator ==(Color left, Color right) {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right) {
            return !(left == right);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    [JsonConverter(typeof(Color32Converter))]
    public struct Color32 : IEquatable<Color32>, IFormattable {
        public static readonly Color32 Transparent = new(0);

        public static readonly Color32 White            = new(0xFF, 0xFF, 0xFF);
        public static readonly Color32 Black            = new(0x00, 0x00, 0x00);
        public static readonly Color32 Red              = new(0xFF, 0x00, 0x00);
        public static readonly Color32 Green            = new(0x00, 0xFF, 0x00);
        public static readonly Color32 Blue             = new(0x00, 0x00, 0xFF);

        public static readonly Color32 AliceBlue = new(240, 248, 255);
        public static readonly Color32 AntiqueWhite = new(250, 235, 215);
        public static readonly Color32 Aqua = new(0, 255, 255);
        public static readonly Color32 AquaMarine = new(127, 255, 212);
        public static readonly Color32 Azure = new(240, 255, 255);
        public static readonly Color32 Beige = new(245, 245, 220);
        public static readonly Color32 Bisque = new(255, 228, 196);
        public static readonly Color32 BlanchedAlmond = new(255, 235, 205);
        public static readonly Color32 BlueViolet = new(138, 43, 226);
        public static readonly Color32 Brown = new(165, 42, 42);
        public static readonly Color32 BurlyWood = new(222, 184, 135);
        public static readonly Color32 CadetBlue = new(95, 158, 160);
        public static readonly Color32 Chartreuse = new(127, 255, 0);
        public static readonly Color32 Chocolate = new(210, 105, 30);
        public static readonly Color32 Coral = new(255, 127, 80);
        public static readonly Color32 CornflowerBlue = new(100, 149, 237);
        public static readonly Color32 Cornsilk = new(255, 248, 220);
        public static readonly Color32 Crimson = new(220, 20, 60);
        public static readonly Color32 Cyan = new(0, 255, 255);
        public static readonly Color32 DarkBlue = new(0, 0, 139);
        public static readonly Color32 DarkCyan = new(0, 139, 139);
        public static readonly Color32 DarkGoldenRod = new(184, 134, 11);
        public static readonly Color32 DarkGray = new(169, 169, 169);
        public static readonly Color32 DarkGreen = new(0, 100, 0);
        public static readonly Color32 DarkKhaki = new(189, 183, 107);
        public static readonly Color32 DarkMagenta = new(139, 0, 139);
        public static readonly Color32 DarkOliveGreen = new(85, 107, 47);
        public static readonly Color32 DarkOrange = new(255, 140, 0);
        public static readonly Color32 DarkOrchid = new(153, 50, 204);
        public static readonly Color32 DarkRed = new(139, 0, 0);
        public static readonly Color32 DarkSalmon = new(233, 150, 122);
        public static readonly Color32 DarkSeaGreen = new(143, 188, 143);
        public static readonly Color32 DarkSlateBlue = new(72, 61, 139);
        public static readonly Color32 DarkSlateGray = new(47, 79, 79);
        public static readonly Color32 DarkTurquoise = new(0, 206, 209);
        public static readonly Color32 DarkViolet = new(148, 0, 211);
        public static readonly Color32 DeepPink = new(255, 20, 147);
        public static readonly Color32 DeepSkyBlue = new(0, 191, 255);
        public static readonly Color32 DimGray = new(105, 105, 105);
        public static readonly Color32 DodgerBlue = new(30, 144, 255);
        public static readonly Color32 FireBrick = new(178, 34, 34);
        public static readonly Color32 FloralWhite = new(255, 250, 240);
        public static readonly Color32 ForestGreen = new(34, 139, 34);
        public static readonly Color32 Fuchsia = new(255, 0, 255);
        public static readonly Color32 Gainsboro = new(220, 220, 220);
        public static readonly Color32 GhostWhite = new(248, 248, 255);
        public static readonly Color32 Gold = new(255, 215, 0);
        public static readonly Color32 GoldenRod = new(218, 165, 32);
        public static readonly Color32 Gray = new(128, 128, 128);
        public static readonly Color32 GreenYellow = new(173, 255, 47);
        public static readonly Color32 Honeydew = new(240, 255, 240);
        public static readonly Color32 HotPink = new(255, 105, 180);
        public static readonly Color32 IndianRed = new(205, 92, 92);
        public static readonly Color32 Indigo = new(75, 0, 130);
        public static readonly Color32 Ivory = new(255, 255, 240);
        public static readonly Color32 Khaki = new(240, 230, 140);
        public static readonly Color32 Lavender = new(230, 230, 250);
        public static readonly Color32 LavenderBlush = new(255, 240, 245);
        public static readonly Color32 LawnGreen = new(124, 252, 0);
        public static readonly Color32 LemonChiffon = new(255, 250, 205);
        public static readonly Color32 LightBlue = new(173, 216, 230);
        public static readonly Color32 LightCoral = new(240, 128, 128);
        public static readonly Color32 LightCyan = new(224, 255, 255);
        public static readonly Color32 LightGoldenRodYellow = new(250, 250, 210);
        public static readonly Color32 LightGray = new(211, 211, 211);
        public static readonly Color32 LightGreen = new(144, 238, 144);
        public static readonly Color32 LightPink = new(255, 182, 193);
        public static readonly Color32 LightSalmon = new(255, 160, 122);
        public static readonly Color32 LightSeaGreen = new(32, 178, 170);
        public static readonly Color32 LightSkyBlue = new(135, 206, 250);
        public static readonly Color32 LightSlateGray = new(119, 136, 153);
        public static readonly Color32 LightSteelBlue = new(176, 196, 222);
        public static readonly Color32 LightYellow = new(255, 255, 224);
        public static readonly Color32 LimeGreen = new(50, 205, 50);
        public static readonly Color32 Linen = new(250, 240, 230);
        public static readonly Color32 Magenta = new(255, 0, 255);
        public static readonly Color32 Maroon = new(128, 0, 0);
        public static readonly Color32 MediumAquaMarine = new(102, 205, 170);
        public static readonly Color32 MediumBlue = new(0, 0, 205);
        public static readonly Color32 MediumOrchid = new(186, 85, 211);
        public static readonly Color32 MediumPurple = new(147, 112, 219);
        public static readonly Color32 MediumSeaGreen = new(60, 179, 113);
        public static readonly Color32 MediumSlateBlue = new(123, 104, 238);
        public static readonly Color32 MediumSpringGreen = new(0, 250, 154);
        public static readonly Color32 MediumTurquoise = new(72, 209, 204);
        public static readonly Color32 MediumVioletRed = new(199, 21, 133);
        public static readonly Color32 MidnightBlue = new(25, 25, 112);
        public static readonly Color32 MintCream = new(245, 255, 250);
        public static readonly Color32 MistyRose = new(255, 228, 225);
        public static readonly Color32 Moccasin = new(255, 228, 181);
        public static readonly Color32 NavajoWhite = new(255, 222, 173);
        public static readonly Color32 Navy = new(0, 0, 128);
        public static readonly Color32 OldLave = new(253, 245, 230);
        public static readonly Color32 Olive = new(128, 128, 0);
        public static readonly Color32 OliveDrab = new(107, 142, 35);
        public static readonly Color32 Orange = new(255, 165, 0);
        public static readonly Color32 OrangeRed = new(255, 69, 0);
        public static readonly Color32 Orchid = new(218, 112, 214);
        public static readonly Color32 PaleGoldenRod = new(238, 232, 170);
        public static readonly Color32 PaleGreen = new(152, 251, 152);
        public static readonly Color32 PaleTurquoise = new(175, 238, 238);
        public static readonly Color32 PaleVioletRed = new(219, 112, 147);
        public static readonly Color32 PapayaWhip = new(255, 239, 213);
        public static readonly Color32 PeachPuff = new(255, 218, 185);
        public static readonly Color32 Peru = new(205, 133, 63);
        public static readonly Color32 Pink = new(255, 192, 203);
        public static readonly Color32 Plum = new(221, 160, 221);
        public static readonly Color32 PowderBlue = new(176, 224, 230);
        public static readonly Color32 Purple = new(128, 0, 128);
        public static readonly Color32 RosyBrown = new(188, 143, 143);
        public static readonly Color32 RoyalBlue = new(65, 105, 225);
        public static readonly Color32 SaddleBrown = new(139, 69, 19);
        public static readonly Color32 Salmon = new(250, 128, 114);
        public static readonly Color32 SandyBrown = new(244, 164, 96);
        public static readonly Color32 SeaGreen = new(46, 139, 87);
        public static readonly Color32 SeaShell = new(255, 245, 238);
        public static readonly Color32 Sienna = new(160, 82, 45);
        public static readonly Color32 Silver = new(192, 192, 192);
        public static readonly Color32 SkyBlue = new(135, 206, 235);
        public static readonly Color32 SlateBlue = new(106, 90, 205);
        public static readonly Color32 SlateGray = new(112, 128, 144);
        public static readonly Color32 Snow = new(255, 250, 250);
        public static readonly Color32 SpringGreen = new(0, 255, 127);
        public static readonly Color32 SteelBlue = new(70, 130, 180);
        public static readonly Color32 Tan = new(210, 180, 140);
        public static readonly Color32 Teal = new(0, 128, 128);
        public static readonly Color32 Thistle = new(216, 191, 216);
        public static readonly Color32 Tomato = new(255, 99, 71);
        public static readonly Color32 Turquoise = new(64, 224, 208);
        public static readonly Color32 Violet = new(238, 130, 238);
        public static readonly Color32 Wheat = new(245, 222, 179);
        public static readonly Color32 WhiteSmoke = new(245, 245, 245);
        public static readonly Color32 Yellow = new(255, 255, 0);
        public static readonly Color32 YellowGreen = new(154, 205, 50);

        [field: FieldOffset(0)]
        public byte R;
        [field: FieldOffset(1)]
        public byte G;
        [field: FieldOffset(2)]
        public byte B;
        [field: FieldOffset(3)]
        public byte A;

        [field: FieldOffset(0)]
        public int Integer;
        [field: FieldOffset(0)]
        public uint Unsigned;

        public byte Grayscale {
            get {
                // Compiler will optimize the division
                return (byte)Math.Round((0.299f / 255f * R + 0.587f / 255f * G + 0.114f / 255f * B) * 255);
            }
        }
        public Color32 GrayscaleColor {
            get {
                var gs = Grayscale;
                return new Color32(gs, gs, gs, A);
            }
        }

        public Color32(int integer) {
            R = G = B = A = 0;
            Unsigned = 0;
            Integer = integer;
        }
        public Color32(uint unsigned) {
            R = G = B = A = 0;
            Integer = 0;
            Unsigned = unsigned;
        }

        public Color32(byte gray) : this(gray, gray, gray, 0xFF) { }

        public Color32(byte r, byte g, byte b) {
            Integer = 0;
            Unsigned = 0;

            R = r; G = g; B = b; A = 255;
        }

        public Color32(byte r, byte g, byte b, byte a) {
            Integer = 0;
            Unsigned = 0;

            R = r; G = g; B = b; A = a;
        }

        public Color32 WithRed(byte r) => new(r, G, B, A);
        public Color32 WithGreen(byte g) => new(R, g, B, A);
        public Color32 WithBlue(byte b) => new(R, G, b, A);
        public Color32 WithAlpha(byte a) => new(R, G, B, a);

        /// <summary>
        /// Return new color with alpha multiply by given multiplier
        /// </summary>
        /// <param name="multiplier">Fade amount</param>
        public Color32 FadeMultiply(float multiplier) => new(R, G, B, (byte)(A * multiplier));

        public static Color32 Lerp(Color32 left, Color32 right, float t) {
            t = DDMath.Saturate(t);

            var r = (byte)MathF.Round(DDMath.Lerp(left.R, right.R, t));
            var g = (byte)MathF.Round(DDMath.Lerp(left.G, right.G, t));
            var b = (byte)MathF.Round(DDMath.Lerp(left.B, right.B, t));
            var a = (byte)MathF.Round(DDMath.Lerp(left.A, right.A, t));

            return new Color32(r, g, b, a);
        }

        public static implicit operator Color(Color32 color) {
            return new Color(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        }

        public bool Equals(Color32 other) {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static unsafe bool operator ==(Color32 left, Color32 right) {
            return *(int*)&left.R == *(int*)&right.R;
        }

        public static unsafe bool operator !=(Color32 left, Color32 right) {
            return *(int*)&left.R != *(int*)&right.R;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Color32 color) return false;

            return this == color;
        }

        /// <summary>
        /// <para>Parse Color string in form of HTML string into instance of Color32.</para>
        /// <br>Note:</br>
        /// <br>1. Input string should be RRGGBB or RRGGBBAA</br>
        /// <br>2. Input string can contains hash character at the beginning.</br>
        /// <br>3. In form of XX, output will have RGB set to that value.</br>
        /// </summary>
        /// <param name="span">String input</param>
        /// <param name="output">Parsed result</param>
        /// <returns>Whether parsing operation successfully</returns>
        public static bool TryParse(ReadOnlySpan<char> span, out Color32 output) {
            output = default;
            if (span.IsEmpty) return false;

            // Optional hash at the beginning
            if (span[0] == '#') span = span[1..];

            switch (span.Length) {
                case 2:
                    if (byte.TryParse(span, NumberStyles.HexNumber, null, out var i8)) {
                        output = new Color32(i8);
                        return true;
                    }
                    return false;

                case 6:
                    if (!byte.TryParse(span[0..2], NumberStyles.HexNumber, null, out output.R)) {
                        output = default;
                        return false;
                    }
                    if (!byte.TryParse(span[2..4], NumberStyles.HexNumber, null, out output.G)) {
                        output = default;
                        return false;
                    }
                    if (!byte.TryParse(span[4..6], NumberStyles.HexNumber, null, out output.B)) {
                        output = default;
                        return false;
                    }

                    output.A = 255;
                    return true;

                case 8:
                    if (!byte.TryParse(span[0..2], NumberStyles.HexNumber, null, out output.R)) {
                        output = default;
                        return false;
                    }
                    if (!byte.TryParse(span[2..4], NumberStyles.HexNumber, null, out output.G)) {
                        output = default;
                        return false;
                    }
                    if (!byte.TryParse(span[4..6], NumberStyles.HexNumber, null, out output.B)) {
                        output = default;
                        return false;
                    }
                    if (!byte.TryParse(span[6..8], NumberStyles.HexNumber, null, out output.A)) {
                        output = default;
                        return false;
                    }
                    return true;
            }

            return false;
        }

        public override int GetHashCode() {
            return (R << 24) | (G << 16) | (B << 8) | A;
        }

        public string ToString(string? format, IFormatProvider? provider) {
            if (string.IsNullOrEmpty(format)) format = "D";
            if (provider == null) provider = CultureInfo.CurrentCulture;

            return format.ToUpperInvariant()[0] switch {
                'X' => "Color32(R: " + R.ToString("X2", provider) + ", G: " + G.ToString("X2", provider) + ", B: " + B.ToString("X2", provider) + ", A: " + A.ToString("X2", provider) + ")",
                _ => "Color32(R: " + R.ToString(format, provider) + ", G: " + G.ToString(format, provider) + ", B: " + B.ToString(format, provider) + ", A: " + A.ToString(format, provider) + ")",
            };
        }

        public override string ToString() {
            return "Color32(R: " + R.ToString("X2") + ", G: " + G.ToString("X2") + ", B: " + B.ToString("X2") + ", A: " + A.ToString("X2") + ")";
        }
    }
}
