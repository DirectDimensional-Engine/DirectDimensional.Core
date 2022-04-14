using System;
using System.Numerics;

namespace DirectDimensional.Core {
    public static class Mouse {
        private static int _currX, _currY, _lastX, _lastY;

        private static int _wheelSign;

        private static bool _currLM, _lastLM;
        private static bool _currRM, _lastRM;
        private static bool _currMM, _lastMM;

        private static bool _currInsideWnd, _lastInsideWnd;

        internal static void UpdateLastStates() {
            _lastX = _currX;
            _lastY = _currY;

            _lastLM = _currLM;
            _lastRM = _currRM;
            _lastMM = _currMM;

            _lastInsideWnd = _currInsideWnd;

            _wheelSign = 0;
        }

        internal static void RegisterMouseMove(int newX, int newY) {
            _currX = newX;
            _currY = newY;
        }

        internal static void RegisterMouseWheel(int sign) {
            _wheelSign = sign;
        }

        internal static void RegisterLMouseDown() {
            _currLM = true;
        }

        internal static void RegisterLMouseUp() {
            _currLM = false;
        }

        internal static void RegisterRMouseDown() {
            _currRM = true;
        }

        internal static void RegisterRMouseUp() {
            _currRM = false;
        }

        internal static void RegisterMMouseDown() {
            _currMM = true;
        }

        internal static void RegisterMMouseUp() {
            _currMM = false;
        }

        internal static void RegisterMouseEnterWnd() {
            _currInsideWnd = true;
        }

        internal static void RegisterMouseLeaveWnd() {
            _currInsideWnd = false;
        }

        public static (int X, int Y) MousePosition2 => (_currX, _currY);
        public static Vector2 MousePosition => new(_currX, _currY);

        public static int MouseWheel => _wheelSign;
        public static bool InsideWindow => _currInsideWnd;

        public static bool LeftPressed => _currLM && !_lastLM;
        public static bool LeftReleased => !_currLM && _lastLM;
        public static bool LeftHeld => _currLM;

        public static bool RightPressed => _currRM && !_lastRM;
        public static bool RightReleased => !_currRM && _lastRM;
        public static bool RightHeld => _currRM;

        public static bool MiddlePressed => _currMM && !_lastMM;
        public static bool MiddleReleased => !_currMM && _lastMM;
        public static bool MiddleHeld => _currMM;

        public static (int X, int Y) MouseMoveDelta2 => (_currX - _lastX, _currY - _lastY);
        public static Vector2 MouseMoveDelta => new(_currX - _lastX, _currY - _lastY);
    }
}
