using System;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Globalization;
using DirectDimensional.Core.Miscs.JConverters;
using System.Text.Json.Serialization;

namespace DirectDimensional.Core {
    /// <summary>
    /// A rectangle struct defined by a Center and extend size
    /// </summary>
    //[JsonConverter(typeof(RectConverter))]
    public struct Rect : IEquatable<Rect>, IFormattable {
        public Vector2 Position;
        public Vector2 Size;

        public Vector2 Max {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position + Size;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size = value - Position;
        }

        public float MaxX {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.X + Size.X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size.X = value - Position.X;
        }

        public float MaxY {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.Y + Size.Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size.Y = value - Position.Y;
        }

        public float X {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Position.X = value;
        }

        public float Y {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Position.Y = value;
        }

        public float Width {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size.X = value;
        }

        public float Height {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size.Y = value;
        }

        public Vector2 Center {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position + Size / 2;
        }

        public float CenterX {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.X + Size.X / 2;
        }
        public float CenterY {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position.Y + Size.Y / 2;
        }

        public bool IsSquare {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.X == Size.Y;
        }

        public float Area {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.X * Size.Y;
        }

        public float Perimeter {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.X + Size.X + Size.Y + Size.Y;
        }

        public float DiagonalLength {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Size.Length();
        }

        public Rect(Vector2 position, Vector2 size) {
            Position = position;
            Size = size;
        }

        public Rect(Vector2 position, float size) {
            Position = position;
            Size = new Vector2(size, size);
        }

        public Rect(float x, float y, float sx, float sy) {
            Position = new Vector2(x, y);
            Size = new Vector2(sx, sy);
        }

        public Rect(float x, float y, float s) {
            Position = new Vector2(x, y);
            Size = new Vector2(s, s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Vector2 point) {
            return point.X >= Position.X && point.X < Position.X + Size.X && point.Y >= Position.Y && point.Y < Position.Y + Size.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Rect rect) {
            Vector2 tmax = Position + Size, omax = rect.Position + rect.Size;

            return tmax.X > rect.Position.X && Position.X < omax.X && tmax.Y > rect.Position.Y && Position.Y < omax.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Circle circle) {
            return circle.Collide(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Rect Extrude(float left, float right, float top, float bottom) {
            return new Rect(Position - new Vector2(left, bottom), Size + new Vector2(left + right, bottom + top));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Rect Extrude(float horizontal, float vertical) {
            return Extrude(horizontal, horizontal, vertical, vertical);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public Rect Extrude(float value) {
            return new Rect(Position - new Vector2(value), Size + new Vector2(value * 2));
        }

        public bool IntersectRect(Rect other, out Rect intersect) {
            var max = Vector2.Max(Position, other.Position);
            var min = Vector2.Min(Max, other.Max);

            if (max.X > min.X || max.Y > min.Y) {
                intersect = default;
                return false;
            }

            intersect = new(max, min - max);
            return true;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Rect r) return false;

            return r.Position == Position && r.Size == Size;
        }

        public override string ToString() {
            return "Rect(Position: " + Position + ", Size: " + Size + ")";
        }

        public override int GetHashCode() {
            return HashCode.Combine(Position, Size);
        }

        public bool Equals(Rect other) {
            return other.Position == Position && other.Size == Size;
        }

        public string ToString(string? format) {
            if (format == null) format = "F4";

            return "Rect(Position: " + Position.ToString(format) + ", Size: " + Size.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            if (format == null) format = "F4";
            if (formatProvider == null) formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return "Rect(Position: " + Position.ToString(format, formatProvider) + ", Size: " + Size.ToString(format, formatProvider) + ")";
        }

        public static bool operator==(Rect left, Rect right) {
            return left.Equals(right);
        }

        public static bool operator!=(Rect left, Rect right) {
            return !(left == right);
        }

        public bool HasInvalidSize => Size.X <= 0 || Size.Y <= 0;

        public static Rect FromMinMax(float minX, float minY, float maxX, float maxY) {
            return new Rect(minX, minY, maxX - minX, maxY - minY);
        }

        public static Rect FromMinMax(Vector2 min, Vector2 max) {
            return new Rect(min, max - min);
        }

        public static Rect FromCenter(Vector2 center, Vector2 size) {
            return new Rect(center - size / 2, size);
        }
    }
}
