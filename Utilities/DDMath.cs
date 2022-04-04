using System;
using System.Runtime.CompilerServices;
using System.Numerics;

namespace DirectDimensional.Core.Utilities {
    public static class DDMath {
        public const float Deg2Rad = MathF.PI / 180f;
        public const float Rad2Deg = 180f / MathF.PI;

        /// <summary>
        /// Wrap a float value between [<c>low</c>, <c>high</c>)
        /// </summary>
        public static float Wrap(float value, float low, float high) {
            float d = high - low;

            return low + (((value - low) % d) + d) % d;
        }

        /// <summary>
        /// Wrap an integer value between [<c>low</c>, <c>high</c>)
        /// </summary>
        public static int Wrap(int value, int low, int high) {
            int d = high - low;

            return low + (((value - low) % d) + d) % d;
        }

        public static float Wrap(float value, float length) {
            return ((value % length) + length) % length;
        }

        public static int Wrap(int value, int length) {
            return ((value % length) + length) % length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Lerp(float a, float b, float t) {
            return a + (b - a) * t;
        }

        public static Vector2 Curve(in Vector2 start, in Vector2 control, in Vector2 end, float t) {
            t = Saturate(t);
            float _t = 1f - t;

            return _t * _t * start + 2 * t * _t * control + t * t * end;
        }
        public static Vector3 Curve(in Vector3 start, in Vector3 control, in Vector3 end, float t) {
            t = Saturate(t);
            float _t = 1f - t;

            return _t * _t * start + 2 * t * _t * control + t * t * end;
        }
        public static Vector2 Curve(in Vector2 start, in Vector2 startC, in Vector2 endC, in Vector2 end, float t) {
            t = Saturate(t);
            float _t = 1 - t;

            return
                _t * _t * _t * start +
                3 * _t * _t * t * startC +
                3 * _t * t * t * endC +
                t * t * t * end;
        }
        public static Vector3 Curve(in Vector3 start, in Vector3 startC, in Vector3 endC, in Vector3 end, float t) {
            t = Saturate(t);
            float _t = 1 - t;

            return
                _t * _t * _t * start +
                3 * _t * _t * t * startC +
                3 * _t * t * t * endC +
                t * t * t * end;
        }

        public static Vector2 CurveDerivative(in Vector2 start, in Vector2 control, in Vector2 end, float t) {
            t = Saturate(t);
            return 2f * (1f - t) * (control - start) + 2f * t * (end - control);
        }
        public static Vector3 CurveDerivative(in Vector3 start, in Vector3 control, in Vector3 end, float t) {
            t = Saturate(t);
            return 2f * (1f - t) * (control - start) + 2f * t * (end - control);
        }
        public static Vector2 CurveDerivative(in Vector2 start, in Vector2 startC, in Vector2 endC, in Vector2 end, float t) {
            t = Saturate(t);
            float _t = 1 - t;

            return 3 * _t * _t * (startC - start) + 6 * _t * t * (endC - startC) + 3 * t * t * (end - endC);
        }
        public static Vector3 CurveDerivative(in Vector3 start, in Vector3 startC, in Vector3 endC, in Vector3 end, float t) {
            t = Saturate(t);
            float _t = 1 - t;

            return 3 * _t * _t * (startC - start) + 6 * _t * t * (endC - startC) + 3 * t * t * (end - endC);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float InverseLerp(float value, float a, float b) {
            if (a == b) return 0;

            return (value - a) / (b - a);
        }

        public static float Bias(float value, float bias) {
            return value / ((1f / bias - 2f) * (1f - value) + 1f);
        }

        public static int Pulse(float value, float a, float b) {
            return (a <= value && value <= b) ? 1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saw(float value, float magnitude) {
            return (value - MathF.Floor(value)) * magnitude;
        }

        public static float Gamma(float value, float gamma) {
            return MathF.Pow(value, 1 / gamma);
        }

        public static float Sigmoid(float x) {
            return 1 / (1 + MathF.Exp(-x));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float SmoothStep(float a, float b, float x) {
            x = Math.Clamp((x - a) / (b - a), 0, 1);

            return x * x * (3 - 2 * x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Saturate(float x) {
            return Math.Clamp(x, 0, 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Saturate(double x) {
            return Math.Clamp(x, 0, 1);
        }

        public static bool Approximate(float a, float b, float delta = 0.01f) {
            return MathF.Abs(a - b) <= delta;
        }

        public static Vector2 MoveTowards(in Vector2 from, in Vector2 to, float delta) {
            Vector2 displacement = to - from;
            float dist = displacement.Length();

            if (dist <= delta) return to;

            return from + displacement * delta;
        }

        public static float ArcLength(in Vector2 from, in Vector2 control, in Vector2 end, int segments) {
            float step = 1f / segments;

            float ret = 0;
            float curr = 0;
            Vector2 lastPos = from;

            for (int i = 0; i < segments; i++) {
                Vector2 currPos = Curve(from, control, end, curr + step);

                ret += (currPos - lastPos).Length();

                lastPos = currPos;

                curr += step;
            }

            return ret;
        }

        public static float ArcLength(in Vector2 from, in Vector2 controlS, in Vector2 controlE, in Vector2 end, int segments) {
            float step = 1f / segments;

            float ret = 0;
            float curr = 0;
            Vector2 lastPos = from;

            for (int i = 0; i < segments; i++) {
                Vector2 currPos = Curve(from, controlS, controlE, end, curr + step);

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
                Vector3 currPos = Curve(from, control, end, curr + step);

                ret += (currPos - lastPos).Length();

                lastPos = currPos;
                curr += step;
            }

            return ret;
        }

        public static float ArcLength(in Vector3 from, in Vector3 controlS, in Vector3 controlE, in Vector3 end, int segments) {
            float step = 1f / segments;

            float ret = 0;
            float curr = 0;
            Vector3 lastPos = from;

            for (int i = 0; i < segments; i++) {
                Vector3 currPos = Curve(from, controlS, controlE, end, curr + step);

                ret += (currPos - lastPos).Length();

                lastPos = currPos;

                curr += step;
            }

            return ret;
        }

        public static float NormalizeUnclamped(float value, float min, float max) {
            return (value - min) / (max - min);
        }

        public static float Normalize(float value, float min, float max) {
            value = Math.Clamp(value, min, max);
            return (value - min) / (max - min);
        }

        public static float Remap(float value, float inmin, float inmax, float outmin, float outmax) {
            return outmin + (value - inmin) * (outmax - outmin) / (inmax - inmin);
        }

        public static Vector2 PolarToCartesian(float radian, float radius) {
            var (Sin, Cos) = MathF.SinCos(radian);

            return new Vector2(Cos * radius, Sin * radius);
        }

        public static (float Angle, float Radius) CartesianToPolar(Vector2 position) {
            return (MathF.Atan2(position.Y, position.X), position.Length());
        }

        public static Vector3 Project(in Vector3 vector, in Vector3 normal) {
            float sqrMag = normal.LengthSquared();
            if (sqrMag < 0.0000001f) return Vector3.Zero;

            float dot = Vector3.Dot(vector, normal);

            return dot / sqrMag * normal;
        }

        public static Vector2 Reflect(in Vector2 vector, in Vector2 normal) {
            return vector - 2f * Vector2.Dot(vector, normal) * normal;
        }

        public static Vector3 Reflect(in Vector3 vector, in Vector3 normal) {
            return vector - 2f * Vector3.Dot(vector, normal) * normal;
        }

        public static Vector2 Mirror(in Vector2 vector, in Vector2 anchor) {
            return anchor + (anchor - vector);
        }

        public static Vector3 Mirror(in Vector3 vector, in Vector3 anchor) {
            return anchor + (anchor - vector);
        }

        public static Vector2 NormalizeAndLength(in Vector2 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        public static Vector3 NormalizeAndLength(in Vector3 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        public static Vector4 NormalizeAndLength(in Vector4 vector, out float length) {
            length = vector.Length();
            return vector / length;
        }

        public static Matrix4x4 LookToLH(in Vector3 position, in Vector3 direction, in Vector3 up) {
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
    }
}
