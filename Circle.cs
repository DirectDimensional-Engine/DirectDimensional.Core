using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Globalization;
using System.Runtime.CompilerServices;
using DirectDimensional.Core.Miscs.JConverters;

namespace DirectDimensional.Core {
    //[JsonConverter(typeof(CircleConverter))]
    public struct Circle : IEquatable<Circle>, IFormattable {
        public Vector2 Center;
        public float Radius;

        public float X {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Center.X;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Center.X = value;
        }
        public float Y {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Center.Y;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Center.Y = value;
        }

        public float Diameter {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Radius * 2;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Radius = value / 2;
        }

        public float Area {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MathF.PI * Radius * Radius;
        }

        public float Perimeter {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MathF.PI * 2 * Radius;
        }

        public Circle(float radius) : this(default, radius) { }

        public Circle(Vector2 center, float radius) {
            Center = center;
            Radius = radius;
        }

        public Circle(float x, float y, float radius) {
            Center = new(x, y);
            Radius = radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool CollideSqr(Vector2 point) {
            var dist = (point - Center).LengthSquared();
            return dist < Radius * Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Vector2 point) {
            return (point - Center).Length() < Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Circle other) {
            return (Center - other.Center).Length() < Radius + other.Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool CollideSqr(Circle other) {
            float totalRadius = Radius + other.Radius;
            return (Center - other.Center).LengthSquared() < totalRadius * totalRadius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool Collide(Rect rect) {
            return (Center - Vector2.Clamp(Center, rect.Position, rect.Position + rect.Size)).Length() < Radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public bool CollideSqr(Rect rect) {
            return (Center - Vector2.Clamp(Center, rect.Position, rect.Position + rect.Size)).LengthSquared() < Radius * Radius;
        }

        public Rect BoundingBox {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get {
                return new Rect(Center - new Vector2(Radius), new Vector2(Diameter));
            }
        }

        public bool Equals(Circle other) {
            return other.Radius == Radius && other.Center == Center;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Center, Radius);
        }

        public override string ToString() {
            return "Circle(Center: " + Center + ", Radius: " + Radius + ")"; 
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Circle c) return false;
            return c.Radius == Radius && c.Center == Center;
        }

        public string ToString(string? format) {
            if (format == null) format = "F4";

            return "Rect(Center: " + Center.ToString(format) + ", Size: " + Radius.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            if (format == null) format = "F4";
            if (formatProvider == null) formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return "Rect(Center: " + Center.ToString(format, formatProvider) + ", Size: " + Radius.ToString(format, formatProvider) + ")";
        }

        public static bool operator==(Circle lhs, Circle rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator!=(Circle lhs, Circle rhs) {
            return !(lhs == rhs);
        }
    }
}
