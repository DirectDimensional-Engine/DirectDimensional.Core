using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DirectDimensional.Core.Utilities {
    public static unsafe class CollisionUtilities {
        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static int PointDirection(Vector2 point, Vector2 a, Vector2 b) {
            return MathF.Sign((point.X - b.X) * (a.Y - b.Y) - (a.X - b.X) * (point.Y - b.Y));
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool PointCircleCollision(Vector2 point, Vector2 center, float radius) {
            return (point - center).Length() <= radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool PointCircleSqrDistCollision(Vector2 point, Vector2 center, float radius) {
            return (point - center).LengthSquared() <= radius * radius;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool CircleCircleCollision(Vector2 center1, float radius1, Vector2 center2, float radius2) {
            return (center1 - center2).Length() <= radius1 + radius2;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool CircleCircleSqrDistCollision(Vector2 center1, float radius1, Vector2 center2, float radius2) {
            float r = radius1 + radius2;
            return (center1 - center2).LengthSquared() <= r * r;
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool PointInsideTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c) {
            bool b2 = PointDirection(point, b, c) < 0;

            return (PointDirection(point, a, b) < 0) == b2 && b2 == (PointDirection(point, c, a) < 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveOptimization | MethodImplOptions.AggressiveInlining)]
        public static bool PointInsidePolygon(Vector2 point, ReadOnlySpan<Vector2> vertices) {
            bool res = false;
            
            for (int curr = 0; curr < vertices.Length; curr++) {
                int next = curr + 1;
                if (next >= vertices.Length) next = 0;

                Vector2 vcurr = vertices[curr];
                Vector2 vnext = vertices[next];

                if ((vcurr.Y > point.Y) != (vnext.Y > point.Y)) {
                    if (point.X < (vnext.X - vcurr.X) * (point.Y - vcurr.Y) / (vnext.Y - vcurr.Y) + vcurr.X) {
                        res ^= true;
                    }
                }
            }

            return res;
        }
    }
}
