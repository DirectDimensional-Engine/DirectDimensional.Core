using System;
using System.Collections.Specialized;
using System.Collections;

namespace DirectDimensional.Core {
    public static class Keyboard {
        private static BitArray _current, _last;

        static Keyboard() {
            _current = new(256);
            _last = new(256);
        }

        internal static void UpdateLastStates() {
            _last = (BitArray)_current.Clone();
        }

        internal static void RegisterKeyDown(byte key) {
            _current[key] = true;
        }

        internal static void RegisterKeyRelease(byte key) {
            _current[key] = false;
        }

        public static bool KeyPressed(InputCode code) => _current[(byte)code] && !_last[(byte)code];
        public static bool KeyHeld(InputCode code) => _current[(byte)code];
        public static bool KeyReleased(InputCode code) => !_current[(byte)code] && _last[(byte)code];
    }
}
