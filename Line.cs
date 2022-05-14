using System;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Globalization;
using DirectDimensional.Core.Utilities;
using System.Runtime.CompilerServices;

namespace DirectDimensional.Core {
    public struct Line : IEquatable<Line>, IFormattable {
        public Vector2 Start;
        public Vector2 End;

        public float Angle {
            get {
                var d = End - Start;
                return MathF.Atan2(d.Y, d.X);
            }
        }

        /// <summary>
        /// Normal vector of the line. Counter-clockwised, unnormalized.
        /// </summary>
        public Vector2 CCWNormal {
            get {
                var d = End - Start;
                return new(d.Y, -d.X);
            }
        }

        /// <summary>
        /// Normal vector of the line. Clockwised, unnormalized.
        /// </summary>
        public Vector2 CWNormal {
            get {
                var d = End - Start;
                return new(-d.Y, d.X);
            }
        }

        public float Length => Displacement.Length();
        public float LengthSquare => Displacement.LengthSquared();

        public Vector2 Displacement {
            [method: MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
            get => End - Start;
        }

        public Line(float ex, float ey) : this(0, 0, ex, ey) { }
        public Line(float sx, float sy, float ex, float ey) {
            Start = new(sx, sy);
            End = new(ex, ey);
        }
        public Line(Vector2 end) : this(default, end) { }
        public Line(Vector2 start, Vector2 end) {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Get a point in the line by calculating linear interpolation
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public Vector2 At(float t) {
            return Start + (End - Start) * t;
        }

        public bool Collide(Vector2 point, float displacement = 0.01f) {
            return DDMath.Approximate((End - Start).Length(), (End - point).Length() + (Start - point).Length(), displacement);
        }

        public bool CollideSqr(Vector2 point, float displacement = 0.01f) {
            return DDMath.Approximate((End - Start).LengthSquared(), (End - point).LengthSquared() + (Start - point).LengthSquared(), displacement);
        }

        public bool Collide(Line other) {
            float uA = ((other.End.X - other.Start.X) * (Start.Y - other.Start.Y) - (other.End.Y - other.Start.Y) * (Start.X - other.Start.X)) / ((other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y));

            if (uA >= 0 && uA <= 1) {
                float uB = ((End.X - Start.X) * (Start.Y - other.Start.Y) - (End.Y - Start.Y) * (Start.X - other.Start.X)) / ((other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y));

                if (uB >= 0 && uB <= 1) {
                    return true;
                }
            }

            return false;
        }

        public bool Collide(Line other, out Vector2 point) {
            float uA = ((other.End.X - other.Start.X) * (Start.Y - other.Start.Y) - (other.End.Y - other.Start.Y) * (Start.X - other.Start.X)) / ((other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y));

            if (uA >= 0 && uA <= 1) {
                float uB = ((End.X - Start.X) * (Start.Y - other.Start.Y) - (End.Y - Start.Y) * (Start.X - other.Start.X)) / ((other.End.Y - other.Start.Y) * (End.X - Start.X) - (other.End.X - other.Start.X) * (End.Y - Start.Y));

                if (uB >= 0 && uB <= 1) {
                    point = Vector2.Lerp(Start, End, uA);
                    return true;
                }
            }

            point = default;
            return false;
        }

        public bool Collide(Rect rect) {
            if (Collide(new Line(rect.Position, rect.Position + new Vector2(rect.Size.X, 0)))) return true;
            if (Collide(new Line(rect.Position, rect.Position + new Vector2(0, rect.Size.Y)))) return true;
            if (Collide(new Line(rect.Position + new Vector2(0, rect.Size.Y), rect.Position + rect.Size))) return true;
            if (Collide(new Line(rect.Position + rect.Size, rect.Position + new Vector2(rect.Size.X, 0)))) return true;

            return false;
        }

        public bool Collide(Rect rect, out Vector2 point) {
            if (Collide(new Line(rect.Position, rect.Position + new Vector2(rect.Size.X, 0)), out point)) return true;
            if (Collide(new Line(rect.Position, rect.Position + new Vector2(0, rect.Size.Y)), out point)) return true;
            if (Collide(new Line(rect.Position + new Vector2(0, rect.Size.Y), rect.Position + rect.Size), out point)) return true;
            if (Collide(new Line(rect.Position + rect.Size, rect.Position + new Vector2(rect.Size.X, 0)), out point)) return true;

            point = default;
            return false;
        }

        public bool Equals(Line line) {
            return line.Start == Start && line.End == End;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) {
            return base.Equals(obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(Start, End);
        }

        public override string ToString() {
            return "Line(" + Start + " -> " + End + ")";
        }

        public string ToString(string? format) {
            if (format == null) format = "F4";
            return "Line(" + Start.ToString(format) + " -> " + End.ToString(format) + ")";
        }

        public string ToString(string? format, IFormatProvider? formatProvider) {
            if (format == null) format = "F4";
            if (formatProvider == null) formatProvider = CultureInfo.InvariantCulture.NumberFormat;

            return "Line(" + Start.ToString(format, formatProvider) + " -> " + End.ToString(format, formatProvider) + ")";
        }

        public static bool operator ==(Line left, Line right) {
            return left.Equals(right);
        }

        public static bool operator !=(Line left, Line right) {
            return !(left == right);
        }

        public static Line operator-(Line line) {
            return new Line(line.End, line.Start);
        }

        public static Line operator+(Line line, Vector2 translate) {
            return new Line(line.Start + translate, line.End + translate);
        }

        public static Line operator -(Line line, Vector2 translate) {
            return new Line(line.Start - translate, line.End - translate);
        }

        public static Line CreateHalfExtend(Vector2 center, Vector2 direction, float length) {
            var r = direction * length / 2;
            return new Line(center - r, center + r);
        }
    }
}
