using System.Numerics;
using DirectDimensional.Bindings.WinAPI;

namespace DirectDimensional.Core {
    public static class Window {
        internal static IntPtr windowHandle;

        public static Vector2 ClientSize {
            get {
                var size = Internal_ClientSize;
                return new Vector2(size.Width, size.Height);
            }
        }

        internal static SIZE Internal_ClientSize {
            get {
                WinAPI.GetClientRect(windowHandle, out var rect);

                return new() { Width = rect.Right - rect.Left, Height = rect.Bottom - rect.Top, };
            }
        }
        public static float ClientAspectRatio {
            get {
                var size = Internal_ClientSize;
                return size.Width / (float)size.Height;
            }
        }

        internal static bool ProcessMessage(out int exit) {
            Mouse.UpdateLastStates();
            Keyboard.UpdateLastStates();

            while (WinAPI.PeekMessageW(out var msg, IntPtr.Zero, 0, 0, 1)) {
                if (msg.Message == WindowMessages.Quit) {
                    exit = (int)msg.wParam;
                    return true;
                }

                WinAPI.TranslateMessage(ref msg);
                WinAPI.DispatchMessageW(ref msg);
            }

            exit = 0;
            return false;
        }
    }
}
