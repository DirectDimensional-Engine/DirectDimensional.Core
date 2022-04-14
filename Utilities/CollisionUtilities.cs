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
        public static bool PointInsideTriangle(Vector2 point, Vector2 a, Vector2 b, Vector2 c) {
            bool b1 = PointDirection(point, a, b) < 0;
            bool b2 = PointDirection(point, b, c) < 0;
            bool b3 = PointDirection(point, c, a) < 0;

            return b1 == b2 && b2 == b3;
        }
    }
}
