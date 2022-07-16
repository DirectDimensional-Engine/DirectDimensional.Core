using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using DirectDimensional.Core.Utilities;
using DirectDimensional.Core.Miscs.JConverters;
using System.Text.Json.Serialization;

namespace DirectDimensional.Core {
    //[JsonConverter(typeof(TriangleConverter))]
    public struct Triangle : IEquatable<Triangle>, IFormattable {
        public Vector2 A;
        public Vector2 B;
        public Vector2 C;

        public float Area {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => MathF.Abs(A.X * (B.Y - C.Y) + B.X * (C.Y - A.Y) + C.X * (A.Y - B.Y)) / 2;
        }

        public float Perimeter {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => (A - B).Length() + (B - C).Length() + (C - A).Length();
        }

        public Vector2 Center {
            [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => (A + B + C) / 2;
        }

        public Triangle(Vector2 a, Vector2 b, Vector2 c) {
            A = a; B = b; C = c;
        }

        public Triangle(float ax, float ay, float bx, float by, float cx, float cy) {
            A = new(ax, ay); B = new(bx, by); C = new(cx, cy);
        }

        public bool Collide(Vector2 point) {
            return CollisionUtilities.PointInsideTriangle(point, A, B, C);
        }

        public Triangle Move(Vector2 delta) {
            return new Triangle(A + delta, B + delta, C + delta);
        }

        public bool Equals(Triangle other) {
            return A == other.A && B == other.B && C == other.C;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            if (obj is not Triangle other) return false;
            return Equals(other);
        }

        public override int GetHashCode() {
            var hc = new HashCode();
            hc.Add(A);
            hc.Add(B);
            hc.Add(C);

            return hc.ToHashCode();
        }

        public override string ToString() {
            return "Triangle(" + A + ", " + B + ", " + C + ")";
        }

        public string ToString(string? format) {
            return "Triangle(" + A.ToString(format) + ", " + B.ToString(format) + ", " + C.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            return "Triangle(" + A.ToString(format, formatProvider) + ", " + B.ToString(format, formatProvider) + ", " + C.ToString(format, formatProvider) + ")";
        }

        public static bool operator==(Triangle lhs, Triangle rhs) {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(Triangle lhs, Triangle rhs) {
            return !lhs.Equals(rhs);
        }
    }
}
