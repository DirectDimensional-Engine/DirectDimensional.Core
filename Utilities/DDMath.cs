using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace DirectDimensional.Core.Utilities {
    public enum EaseFunction {
        Linear,

        SineIn, SineOut, SineInOut,
        QuadIn, QuadOut, QuadInOut,
        CubicIn, CubicOut, CubicInOut,
        QuartIn, QuartOut, QuartInOut,
        QuintIn, QuintOut, QuintInOut,
        ExpoIn, ExpoOut, ExpoInOut,
        CircIn, CircOut, CircInOut,
        BackIn, BackOut, BackInOut,
        ElasticIn, ElasticOut, ElasticInOut,
        BounceIn, BounceOut, BounceInOut,
    }

    public static class DDMath {
        public const float Deg2Rad = MathF.PI / 180f;
        public const float Rad2Deg = 180f / MathF.PI;

        /// <summary>
        /// Wrap a float value between [<c>low</c>, <c>high</c>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Wrap(float value, float low, float high) {
            float d = high - low;
            return low + (((value - low) % d) + d) % d;
        }

        /// <summary>
        /// Wrap an integer value between [<c>low</c>, <c>high</c>)
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Wrap(int value, int low, int high) {
            int d = high - low;
            return low + (((value - low) % d) + d) % d;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Wrap(float value, float length) {
            return ((value % length) + length) % length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Wrap(int value, int length) {
            return ((value % length) + length) % length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Lerp(float a, float b, float t) {
            t = Saturate(t);
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float LerpUnclamped(float a, float b, float t) {
            return a + (b - a) * t;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float InverseLerp(float value, float a, float b) {
            return (value - a) / (b - a);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Bias(float value, float bias) {
            return value / ((1f / bias - 2f) * (1f - value) + 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Pulse(float value, float a, float b) {
            return (a <= value && value <= b) ? 1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Saw(float value, float magnitude) {
            return (value - MathF.Floor(value)) * magnitude;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Gamma(float value, float gamma) {
            return MathF.Pow(value, 1 / gamma);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Sigmoid(float x) {
            return 1 / (1 + MathF.Exp(-x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float SmoothStep(float a, float b, float x) {
            x = Math.Clamp((x - a) / (b - a), 0, 1);

            return x * x * (3 - 2 * x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Saturate(float x) {
            return Math.Clamp(x, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static double Saturate(double x) {
            return Math.Clamp(x, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Saturate(Vector2 vec) {
            return Vector2.Clamp(vec, Vector2.Zero, Vector2.One);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Saturate(Vector3 vec) {
            return Vector3.Clamp(vec, Vector3.Zero, Vector3.One);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4 Saturate(Vector4 vec) {
            return Vector4.Clamp(vec, Vector4.Zero, Vector4.One);
        }

        public static bool Approximate(float a, float b, float delta = 0.01f) {
            return MathF.Abs(a - b) <= delta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool Between(this int value, int min, int max) => min <= value && value <= max;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool Between(this float value, float min, float max) => min <= value && value <= max;
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool Between(this double value, double min, double max) => min <= value && value <= max;

        public static Vector2 MoveTowards(Vector2 from, Vector2 to, float delta) {
            Vector2 displacement = to - from;
            float dist = displacement.LengthSquared();

            if (dist <= delta * delta) return to;

            return from + displacement * delta;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Remap(float value, float inmin, float inmax, float outmin, float outmax) {
            return outmin + (value - inmin) * (outmax - outmin) / (inmax - inmin);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static int Remap(int value, int inmin, int inmax, int outmin, int outmax) {
            return (int)MathF.Round((float)outmin + (value - inmin) * (outmax - outmin) / (inmax - inmin));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 PolarToCartesian(float radian, float radius) {
            var (Sin, Cos) = MathF.SinCos(radian);

            return new Vector2(Cos * radius, Sin * radius);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static (float Angle, float Radius) CartesianToPolar(Vector2 position) {
            return (MathF.Atan2(position.Y, position.X), position.Length());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Project(Vector3 vector, Vector3 normal) {
            float sqrMag = normal.LengthSquared();
            if (sqrMag < 0.0000001f) return Vector3.Zero;

            float dot = Vector3.Dot(vector, normal);

            return dot / sqrMag * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Reflect(Vector2 vector, Vector2 normal) {
            return vector - 2f * Vector2.Dot(vector, normal) * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Reflect(Vector3 vector, Vector3 normal) {
            return vector - 2f * Vector3.Dot(vector, normal) * normal;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Mirror(Vector2 vector, Vector2 anchor) {
            return anchor + (anchor - vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 Mirror(Vector3 vector, Vector3 anchor) {
            return anchor + (anchor - vector);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static void RotateThis(this Vector2 v, float rad) {
            (float sin, float cos) = MathF.SinCos(rad);

            float tx = v.X;
            float ty = v.Y;
            v.X = (cos * tx) - (sin * ty);
            v.Y = (sin * tx) + (cos * ty);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 Rotate(this Vector2 v, float rad) {
            (float sin, float cos) = MathF.SinCos(rad);

            float tx = v.X;
            float ty = v.Y;
            return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector2 NormalizeAndLength(Vector2 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector3 NormalizeAndLength(Vector3 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static Vector4 NormalizeAndLength(Vector4 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        public static Matrix4x4 LookToLH(Vector3 position, Vector3 direction, Vector3 up) {
            Vector3 zaxis = Vector3.Normalize(direction);
            Vector3 xaxis = Vector3.Normalize(Vector3.Cross(up, zaxis));
            Vector3 yaxis = Vector3.Cross(zaxis, xaxis);

            Matrix4x4 result;

            result.M11 = xaxis.X;
            result.M12 = yaxis.X;
            result.M13 = zaxis.X;
            result.M14 = 0.0f;
            result.M21 = xaxis.Y;
            result.M22 = yaxis.Y;
            result.M23 = zaxis.Y;
            result.M24 = 0.0f;
            result.M31 = xaxis.Z;
            result.M32 = yaxis.Z;
            result.M33 = zaxis.Z;
            result.M34 = 0.0f;
            result.M41 = -Vector3.Dot(xaxis, position);
            result.M42 = -Vector3.Dot(yaxis, position);
            result.M43 = -Vector3.Dot(zaxis, position);
            result.M44 = 1.0f;

            return result;
        }

        public static Matrix4x4 PerspectiveFovLH(float fov, float aspectRatio, float near, float far) {
            float itan = 1f / MathF.Tan(fov * 0.5f);
            float plane = far / (far - near);

            Matrix4x4 ret = new();

            ret.M11 = itan / aspectRatio;
            ret.M22 = itan;
            ret.M33 = plane;
            ret.M34 = 1;
            ret.M43 = -near * plane;

            return ret;
        }

        public static Matrix4x4 OrthographicLH(float width, float height, float far, float near) {
            Matrix4x4 ret = new();

            ret.M11 = 2f / width;
            ret.M22 = 2f / height;
            ret.M33 = 1f / (far - near);
            ret.M43 = -ret.M33 * near;
            ret.M44 = 1;

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 V3(this Vector2 vec) => new(vec, 0);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 V4(this Vector2 vec) => new(vec, 0, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 V2(this Vector3 vec) => new(vec.X, vec.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector4 V4(this Vector3 vec) => new(vec, 0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 V2(this Vector4 vec) => new(vec.X, vec.Y);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 V3(this Vector4 vec) => new(vec.X, vec.Y, vec.Z);

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float ExpoDecay(float value, float t, float decay) {
            return value * MathF.Exp(-decay * t);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float LinearToGamma(float linear) {
            return MathF.Pow(linear, 1 / 2.2f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float GammaToLinear(float gamma) {
            return MathF.Pow(gamma, 2.2f);
        }

        public static float Round(float value, float snap) {
            var inv = 1 / snap;
            return MathF.Round(value * inv, MidpointRounding.AwayFromZero) / inv;
        }

        public static double Round(double value, float snap) {
            var inv = 1 / snap;
            return Math.Round(value * inv, MidpointRounding.AwayFromZero) / inv;
        }

        /// <summary>
        /// Calculate Quadratic Equation (ax^2 + bx + c = 0)
        /// </summary>
        /// <param name="a">First coefficients of x^2 (a)</param>
        /// <param name="b">Second coefficients of x (b)</param>
        /// <param name="c">Third coefficients (c)</param>
        /// <param name="x0">Result which subtract delta</param>
        /// <param name="x1">Result which add delta</param>
        /// <param name="delta">Square root of b^2 - 4ac</param>
        /// <exception cref="ArgumentOutOfRangeException">First coefficient is 0</exception>
        public static void QuadraticEquation(float a, float b, float c, out float? x0, out float? x1, out float delta) {
            if (a == 0) throw new ArgumentOutOfRangeException(nameof(a));
            delta = MathF.Sqrt(b * b - 4 * a * c);

            switch (delta) {
                case var _ when delta < 0: x0 = x1 = null; break;
                case 0:
                    x0 = x1 = -b / (2 * a); break;
                default:
                    var a2 = a * 2;
                    x0 = (-b - delta) / a2;
                    x1 = (-b + delta) / a2;
                    break;
            }
        }

        public static class Curve {
            public static class BezierQuad {
                public static Vector2 Evaluate(Vector2 start, Vector2 control, Vector2 end, float t) {
                    float _t = 1f - t;

                    return _t * _t * start + 2 * t * _t * control + t * t * end;
                }
                public static Vector3 Evaluate(Vector3 start, Vector3 control, Vector3 end, float t) {
                    float _t = 1f - t;

                    return _t * _t * start + 2 * t * _t * control + t * t * end;
                }
                public static Vector4 Evaluate(Vector4 start, Vector4 control, Vector4 end, float t) {
                    float _t = 1f - t;

                    return _t * _t * start + 2 * t * _t * control + t * t * end;
                }

                public static Vector2 Velocity(Vector2 start, Vector2 control, Vector2 end, float t) {
                    t = Saturate(t);
                    return 2f * ((1f - t) * (control - start) + t * (end - control));
                }
                public static Vector3 Velocity(Vector3 start, Vector3 control, Vector3 end, float t) {
                    t = Saturate(t);
                    return 2f * ((1f - t) * (control - start) + t * (end - control));
                }
                public static Vector4 Velocity(Vector4 start, Vector4 control, Vector4 end, float t) {
                    t = Saturate(t);
                    return 2f * ((1f - t) * (control - start) + t * (end - control));
                }

                public static float ArcLength(Vector2 from, Vector2 control, Vector2 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector2 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector2 currPos = Evaluate(from, control, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
                public static float ArcLength(Vector3 from, Vector3 control, Vector3 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector3 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector3 currPos = Evaluate(from, control, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
                public static float ArcLength(Vector4 from, Vector4 control, Vector4 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector4 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector4 currPos = Evaluate(from, control, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
            }
            public static class BezierCubic {
                public static Vector2 Evaluate(Vector2 start, Vector2 startControl, Vector2 endControl, Vector2 end, float t) {
                    float _t = 1 - t;

                    return
                        _t * _t * _t * start +
                        3 * _t * _t * t * startControl +
                        3 * _t * t * t * endControl +
                        t * t * t * end;
                }
                public static Vector3 Evaluate(Vector3 start, Vector3 startControl, Vector3 endControl, Vector3 end, float t) {
                    float _t = 1 - t;

                    return
                        _t * _t * _t * start +
                        3 * _t * _t * t * startControl +
                        3 * _t * t * t * endControl +
                        t * t * t * end;
                }
                public static Vector4 Evaluate(Vector4 start, Vector4 startControl, Vector4 endControl, Vector4 end, float t) {
                    float _t = 1 - t;

                    return
                        _t * _t * _t * start +
                        3 * _t * _t * t * startControl +
                        3 * _t * t * t * endControl +
                        t * t * t * end;
                }

                public static Vector2 Velocity(Vector2 start, Vector2 startControl, Vector2 endControl, Vector2 end, float t) {
                    t = Saturate(t);
                    float _t = 1 - t;
                    return 3 * _t * _t * (startControl - start) + 6 * _t * t * (endControl - startControl) + 3 * t * t * (end - endControl);
                }
                public static Vector3 Velocity(Vector3 start, Vector3 startControl, Vector3 endControl, Vector3 end, float t) {
                    t = Saturate(t);
                    float _t = 1 - t;
                    return 3 * _t * _t * (startControl - start) + 6 * _t * t * (endControl - startControl) + 3 * t * t * (end - endControl);
                }
                public static Vector4 Velocity(Vector4 start, Vector4 startControl, Vector4 endControl, Vector4 end, float t) {
                    t = Saturate(t);
                    float _t = 1 - t;
                    return 3 * (_t * _t * (startControl - start) + 2 * _t * t * (endControl - startControl) + t * t * (end - endControl));
                }

                public static float ArcLength(Vector2 from, Vector2 startControl, Vector2 endControl, Vector2 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector2 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector2 currPos = Evaluate(from, startControl, endControl, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
                public static float ArcLength(Vector3 from, Vector3 startControl, Vector3 endControl, Vector3 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector3 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector3 currPos = Evaluate(from, startControl, endControl, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
                public static float ArcLength(Vector4 from, Vector4 startControl, Vector4 endControl, Vector4 end, int segments) {
                    float step = 1f / segments;

                    float ret = 0;
                    float curr = 0;
                    Vector4 lastPos = from;

                    for (int i = 0; i < segments; i++) {
                        Vector4 currPos = Evaluate(from, startControl, endControl, end, curr + step);

                        ret += (currPos - lastPos).Length();

                        lastPos = currPos;

                        curr += step;
                    }

                    return ret;
                }
            }
            public static class CatmullRom {
                public static Vector2 Evaluate(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
                    var a = 2f * p1;
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
                }
                public static Vector3 Evaluate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
                    var a = 2f * p1;
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
                }
                public static Vector4 Evaluate(Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3, float t) {
                    var a = 2f * p1;
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
                }

                public static Vector2 Evaluate(ReadOnlySpan<Vector2> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Evaluate(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }
                public static Vector3 Evaluate(ReadOnlySpan<Vector3> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Evaluate(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }
                public static Vector4 Evaluate(ReadOnlySpan<Vector4> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Evaluate(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }

                public static Vector2 Velocity(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t) {
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * b + c * t + 1.5f * d * t * t;
                }
                public static Vector3 Velocity(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t) {
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * b + c * t + 1.5f * d * t * t;
                }
                public static Vector4 Velocity(Vector4 p0, Vector4 p1, Vector4 p2, Vector4 p3, float t) {
                    var b = p2 - p0;
                    var c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
                    var d = -p0 + 3f * p1 - 3f * p2 + p3;

                    t = Saturate(t);
                    return 0.5f * b + c * t + 1.5f * d * t * t;
                }

                public static Vector2 Velocity(ReadOnlySpan<Vector2> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Velocity(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }
                public static Vector3 Velocity(ReadOnlySpan<Vector3> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Velocity(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }
                public static Vector4 Velocity(ReadOnlySpan<Vector4> points, float t) {
                    if (points.Length < 4) return new(float.NaN);

                    t = Saturate(t) * (points.Length - 3);
                    var begin = Math.Min((int)MathF.Floor(t), points.Length - 4);
                    t -= begin;

                    return Velocity(points[begin], points[begin + 1], points[begin + 2], points[begin + 3], t);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static float Ease(float x, EaseFunction type) {
            return type switch {
                EaseFunction.SineIn => Easing.Sine.In(x),
                EaseFunction.SineOut => Easing.Sine.Out(x),
                EaseFunction.SineInOut => Easing.Sine.InOut(x),

                EaseFunction.QuadIn => Easing.Quad.In(x),
                EaseFunction.QuadOut => Easing.Quad.Out(x),
                EaseFunction.QuadInOut => Easing.Quad.InOut(x),

                EaseFunction.CubicIn => Easing.Cubic.In(x),
                EaseFunction.CubicOut => Easing.Cubic.Out(x),
                EaseFunction.CubicInOut => Easing.Cubic.InOut(x),

                EaseFunction.QuartIn => Easing.Quart.In(x),
                EaseFunction.QuartOut => Easing.Quart.Out(x),
                EaseFunction.QuartInOut => Easing.Quart.InOut(x),

                EaseFunction.QuintIn => Easing.Quint.In(x),
                EaseFunction.QuintOut => Easing.Quint.Out(x),
                EaseFunction.QuintInOut => Easing.Quint.InOut(x),

                EaseFunction.ExpoIn => Easing.Expo.In(x),
                EaseFunction.ExpoOut => Easing.Expo.Out(x),
                EaseFunction.ExpoInOut => Easing.Expo.InOut(x),

                EaseFunction.CircIn => Easing.Circ.In(x),
                EaseFunction.CircOut => Easing.Circ.Out(x),
                EaseFunction.CircInOut => Easing.Circ.InOut(x),

                EaseFunction.BackIn => Easing.Back.In(x),
                EaseFunction.BackOut => Easing.Back.Out(x),
                EaseFunction.BackInOut => Easing.Back.InOut(x),

                EaseFunction.ElasticIn => Easing.Elastic.In(x),
                EaseFunction.ElasticOut => Easing.Elastic.Out(x),
                EaseFunction.ElasticInOut => Easing.Elastic.InOut(x),

                EaseFunction.BounceIn => Easing.Bounce.In(x),
                EaseFunction.BounceOut => Easing.Bounce.Out(x),
                EaseFunction.BounceInOut => Easing.Bounce.InOut(x),

                _ => x,
            };
        }
        public static class Easing {
            public static class Sine {
                public static float In(float x) {
                    return 1 - MathF.Cos(MathF.PI / 2 * x);
                }

                public static float Out(float x) {
                    return MathF.Sin(MathF.PI / 2 * x);
                }

                public static float InOut(float x) {
                    return -(MathF.Cos(x * MathF.PI) - 1) / 2;
                }
            }

            public static class Quad {
                public static float In(float x) {
                    return x * x;
                }

                public static float Out(float x) {
                    return 1 - (1 - x) * (1 - x);
                }

                public static float InOut(float x) {
                    var p = -2 * x + 2;
                    return x < 0.5f ? 2 * x * x : 1 - p * p / 2;
                }
            }

            public static class Cubic {
                public static float In(float x) {
                    return x * x * x;
                }

                public static float Out(float x) {
                    float _x = 1 - x;
                    return 1 - _x * _x * _x;
                }

                public static float InOut(float x) {
                    var p = -2 * x + 2;
                    return x < 0.5f ? (4 * x * x * x) : 1 - p * p * p / 2;
                }
            }

            public static class Quart {
                public static float In(float x) {
                    return x * x * x * x;
                }

                public static float Out(float x) {
                    float _x = 1 - x;
                    return 1 - _x * _x * _x * _x;
                }

                public static float InOut(float x) {
                    var p = -2 * x + 2;
                    return x < 0.5f ? 8 * x * x * x * x : 1 - p * p * p * p / 2;
                }
            }

            public static class Quint {
                public static float In(float x) {
                    return x * x * x * x * x;
                }

                public static float Out(float x) {
                    float _x = 1 - x;
                    return 1 - _x * _x * _x * _x * _x;
                }

                public static float InOut(float x) {
                    var p = -2 * x + 2;
                    return x < 0.5f ? 16 * x * x * x * x * x : 1 - p * p * p * p * p / 2;
                }
            }

            public static class Expo {
                public static float In(float x) {
                    return x == 0 ? 0 : MathF.Pow(2, 10 * x - 10);
                }

                public static float Out(float x) {
                    return x == 1 ? 1 : 1 - MathF.Pow(2, -10 * x);
                }

                public static float InOut(float x) {
                    return x == 0 ? 0 : (x == 1 ? 1 : (x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2 : (2 - MathF.Pow(2, -20 * x + 10)) / 2));
                }
            }

            public static class Circ {
                public static float In(float x) {
                    return 1 - MathF.Sqrt(1 - x * x);
                }

                public static float Out(float x) {
                    return MathF.Sqrt(1 - (x - 1) * (x - 1));
                }

                public static float InOut(float x) {
                    if (x < 0.5f)
                        return (1 - MathF.Sqrt(1 - 4 * x * x)) / 2;

                    var p = -2 * x + 2;
                    return (MathF.Sqrt(1 - p * p) + 1) / 2;
                }
            }

            public static class Back {
                public static float In(float x) {
                    return 2.70158f * x * x * x - 1.70158f * x * x;
                }

                public static float Out(float x) {
                    float s = x - 1;
                    return 1 + 2.70158f * s * s * s + 1.70158f * s * s;
                }

                public static float InOut(float x) {
                    float c2 = 2.5949095f;

                    if (x < 0.5f)
                        return 4 * x * x * ((c2 + 1) * 2 * x - c2) / 2;

                    float s = 2 * x - 2;
                    return (s * s * ((c2 + 1) * s + c2) + 2) / 2f;
                }
            }

            public static class Elastic {
                public static float In(float x) {
                    return x == 0 ? 0 : (x == 1 ? 1 : -MathF.Pow(2, 10 * x - 10) * MathF.Sin((x * 10 - 10.75f) * 2 * MathF.PI / 3));
                }

                public static float Out(float x) {
                    return x == 0 ? 0 : (x == 1 ? 1 : MathF.Pow(2, -10 * x) * MathF.Sin((x * 10 - 0.75f) * 2 * MathF.PI / 3) + 1);
                }

                public static float InOut(float x) {
                    float c = 2 * MathF.PI / 4.5f;

                    float sin = MathF.Sin((20 * x - 11.125f) * c);
                    return x == 0 ? 0 : (x == 1 ? 1 : (x < 0.5f ? -(MathF.Pow(2, 20 * x - 10) * sin) / 2 : MathF.Pow(2, -20 * x + 10) * sin / 2 + 1));
                }
            }

            public static class Bounce {
                public static float In(float x) {
                    return 1 - Out(1 - x);
                }

                public static float Out(float x) {
                    var n1 = 7.5625f;
                    var d1 = 2.75f;

                    if (x < 1 / d1) {
                        return n1 * x * x;
                    } else if (x < 2 / d1) {
                        return n1 * (x -= 1.5f / d1) * x + 0.75f;
                    } else if (x < 2.5f / d1) {
                        return n1 * (x -= 2.25f / d1) * x + 0.9375f;
                    } else {
                        return n1 * (x -= 2.625f / d1) * x + 0.984375f;
                    }
                }

                public static float InOut(float x) {
                    return x < 0.5f ? (1 - Out(1 - 2 * x)) / 2 : (1 + Out(2 * x - 1)) / 2;
                }
            }
        }
    }
}
