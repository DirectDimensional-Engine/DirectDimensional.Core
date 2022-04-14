using System;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Globalization;

namespace DirectDimensional.Core {
    /// <summary>
    /// A rectangle struct defined by a Center and extend size
    /// </summary>
    public struct Rect : IEquatable<Rect>, IFormattable {
        public Vector2 Center;
        public Vector2 Size;

        public Vector2 Min {
            get => Center - Size / 2;
            set {
                Center -= value / 2;
                Size += value;
            }
        }

        public Vector2 Max {
            get => Center + Size / 2;
            set {
                Center += value / 2;
                Size += value;
            }
        }

        public float Left {
            get => Center.X - Size.X / 2;
        }

        public float Right {
            get => Center.X + Size.X / 2;
        }

        public float Bottom {
            get => Center.Y - Size.Y / 2;
        }

        public float Top {
            get => Center.Y + Size.Y / 2;
        }

        public Rect(Vector2 center, Vector2 size) {
            Center = center;
            Size = size;
        }

        public Rect(float cx, float cy, float sx, float sy) {
            Center = new Vector2(cx, cy);
            Size = new Vector2(sx, sy);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Contains(Vector2 point) {
            var min = Min;
            var max = Max;

            return point.X >= min.X && point.X <= max.X && point.Y >= min.Y && point.Y <= max.Y;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Rect r) return false;

            return r.Center == Center && r.Size == Size;
        }

        public override string ToString() {
            return "Rect(Center: " + Center + ", Size: " + Size + ")";
        }

        public override int GetHashCode() {
            return (Center.X.GetHashCode() << 24) | (Center.Y.GetHashCode() << 16) | (Size.X.GetHashCode() << 8) | Size.Y.GetHashCode();
        }

        public bool Equals(Rect other) {
            return other.Center == Center && other.Size == Size;
        }

        public string ToString(string? format) {
            if (format == null) format = "F4";

            return "Rect(Center: " + Center.ToString(format) + ", Size: " + Size.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            if (format == null) format = "F4";
            if (formatProvider == null) formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return "Rect(Center: " + Center.ToString(format, formatProvider) + ", Size: " + Size.ToString(format, formatProvider) + ")";
        }

        public static bool operator ==(Rect left, Rect right) {
            return left.Center == right.Center && left.Size == right.Size;
        }

        public static bool operator !=(Rect left, Rect right) {
            return left.Center != right.Center || left.Size != right.Size;
        }

        public static implicit operator RectArea(Rect r) {
            return new RectArea(r.Min, r.Max);
        }
    }

    public struct RectArea : IEquatable<RectArea>, IFormattable {
        public Vector2 Min;
        public Vector2 Max;

        public Vector2 Center => (Min + Max) / 2;
        public Vector2 Size => Max - Min;

        public float Left => Min.X;
        public float Right => Max.X;
        public float Bottom => Min.Y;
        public float Top => Max.X;

        public RectArea(Vector2 min, Vector2 max) {
            Min = min;
            Max = max;
        }

        public RectArea(float minX, float minY, float maxX, float maxY) {
            Min = new Vector2(minX, minY);
            Max = new Vector2(maxX, maxY);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Contains(Vector2 point) {
            return point.X >= Min.X && point.X <= Max.X && point.Y >= Min.Y && point.Y <= Max.Y;
        }

        public override int GetHashCode() {
            return (Min.X.GetHashCode() << 24) | (Min.Y.GetHashCode() << 16) | (Max.X.GetHashCode() << 8) | Max.Y.GetHashCode();
        }

        public override string ToString() {
            return "RectArea(Min: " + Min + ", Max: " + Max + ")";
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not RectArea ra) return false;

            return ra.Min == Min && ra.Max == Max;
        }

        public bool Equals(RectArea other) {
            return other.Center == Center && other.Size == Size;
        }

        public string ToString(string? format) {
            if (format == null) format = "F4";

            return "RectArea(Min: " + Min.ToString(format) + ", Max: " + Max.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            if (format == null) format = "F4";
            if (formatProvider == null) formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return "RectArea(Min: " + Min.ToString(format, formatProvider) + ", Max: " + Max.ToString(format, formatProvider) + ")";
        }

        public static bool operator==(RectArea lhs, RectArea rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(RectArea lhs, RectArea rhs) {
            return lhs.Min != rhs.Min || lhs.Max != rhs.Max;
        }

        public static implicit operator Rect(RectArea ra) {
            return new Rect(ra.Center, ra.Size);
        }
    }
}
