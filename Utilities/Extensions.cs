using System;
using System.Runtime.CompilerServices;

namespace DirectDimensional.Core.Utilities {
    public static class Extensions {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Pressed(this KeyboardCode code) {
            return Keyboard.Pressed(code);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Holding(this KeyboardCode code) {
            return Keyboard.Holding(code);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Released(this KeyboardCode code) {
            return Keyboard.Released(code);
        }
    }
}
