using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static (int X, int Y) MousePosition => (_currX, _currY);

        public static int MouseWheel => _wheelSign;
        public static bool InsideWindow => _currInsideWnd;
    }
}
