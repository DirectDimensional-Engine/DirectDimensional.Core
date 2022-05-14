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
        public Vector2 Position;
        public Vector2 Size;

        public Vector2 Max {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Position + Size;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Size = value - Position;
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

            return tmax.X >= rect.Position.X && Position.X <= omax.X && tmax.Y >= rect.Position.Y && Position.Y <= omax.Y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Circle circle) {
            return circle.Collide(this);
        }

        public void Extrude(float left, float right, float top, float bottom) {
            Position -= new Vector2(left, bottom);
            Size += new Vector2(left + right, bottom + top);
        }

        /// <summary>
        /// Wrapper for <seealso cref="Extrude(float, float, float, float)"/>, each axis correspond to 1 parameter, in order of XYZW
        /// </summary>
        /// <param name="extrude"></param>
        public void Extrude(Vector4 extrude) {
            Extrude(extrude.X, extrude.Y, extrude.Z, extrude.W);
        }

        public void Extrude(float horizontal, float vertical) {
            Extrude(horizontal, horizontal, vertical, vertical);
        }

        public void Extrude(float value) {
            Extrude(value, value, value, value);
        }

        public void RoundSize() {
            Size = new Vector2(MathF.Round(Size.X), MathF.Round(Size.Y));
        }

        public void FloorSize() {
            Size = new Vector2(MathF.Floor(Size.X), MathF.Floor(Size.Y));
        }

        public void CeilSize() {
            Size = new Vector2(MathF.Ceiling(Size.X), MathF.Ceiling(Size.Y));
        }

        public void RoundPosition() {
            Position = new Vector2(MathF.Round(Position.X), MathF.Round(Position.Y));
        }

        public void FloorPosition() {
            Position = new Vector2(MathF.Floor(Position.X), MathF.Floor(Position.Y));
        }

        public void CeilPosition() {
            Position = new Vector2(MathF.Ceiling(Position.X), MathF.Ceiling(Position.Y));
        }

        public Vector2 MapCoordinate(Vector2 point) {
            return (point - Position) / Size;
        }

        public Vector2 UnmapCoordinate(Vector2 point) {
            return Position + Size * point;
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

        public static Rect operator*(Rect rect, float scale) {
            return new Rect(rect.Position, rect.Size * scale);
        }

        public static Rect operator/(Rect rect, float scale) {
            return new Rect(rect.Position, rect.Size / scale);
        }

        public static Rect operator&(Rect lhs, Rect rhs) {
            Vector2 lmax = lhs.Max, rmax = rhs.Max;

            float left = MathF.Max(lhs.X, rhs.X);
            float right = MathF.Min(lmax.X, rmax.X);

            float bottom = MathF.Max(lhs.Y, rhs.Y);
            float top = MathF.Min(lmax.Y, rmax.Y);

            if (left > right || bottom > top) {
                return default;
            }

            return new Rect(left, bottom, right - left, top - bottom);
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
