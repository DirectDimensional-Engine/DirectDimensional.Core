using System.Runtime.InteropServices;
using DirectDimensional.Bindings.WinAPI;

namespace DirectDimensional.Core;

internal class ApplicationRunner {
    public static int Main() {
        IntPtr hInstance = System.Diagnostics.Process.GetCurrentProcess().Handle;

        WNDCLASSEXW classEx = default;
        classEx.cbSize = (uint)Marshal.SizeOf<WNDCLASSEXW>();
        classEx.lpszClassName = "Main Window";
        classEx.lpfnWndProc = WndProc;
        classEx.style = 0x20;   // CS_OWNDC
        classEx.hInstance = hInstance;
        classEx.hCursor = WinAPI.LoadCursorW(IntPtr.Zero, StandardCursorID.IDC_ARROW);

        WinAPI.RegisterClassExW(ref classEx);

        const uint overlappedWindow = 13565952U;    // Standard window style that will always be used (not because I'm lazy)
        Window.windowHandle = WinAPI.CreateWindowExW(0, "Main Window", "DirectDimensional", overlappedWindow, 0, 0, 800, 600, IntPtr.Zero, IntPtr.Zero, hInstance, IntPtr.Zero);
        WinAPI.ShowWindow(Window.windowHandle, 3);

        EngineLifecycle.Initialize();

        int exitCode;
        while (true) {
            if (Window.ProcessMessage(out exitCode)) break;

            EngineLifecycle.Cycle();
        }

        EngineLifecycle.CleanUp();

        return exitCode;
    }

    public static nint WndProc(IntPtr hwnd, WindowMessages msg, nuint wParam, nint lParam) {
        var wndSize = Window.Internal_ClientSize;

        switch (msg) {
            case WindowMessages.Destroy:    // WM_DESTROY
                WinAPI.PostQuitMessage(0);
                return 0;

            case WindowMessages.KeyDown:
            case WindowMessages.SysKeyDown:
                Keyboard.RegisterKeyDown((byte)wParam);
                break;

            case WindowMessages.KeyUp:
            case WindowMessages.SysKeyUp:
                Keyboard.RegisterKeyRelease((byte)wParam);
                break;

            case WindowMessages.MouseMove: {
                POINTS p = WinAPI.MakePOINTS(lParam);

                if (p.X >= 0 && p.X < wndSize.Width && p.Y >= 0 && p.Y < wndSize.Height) {
                    Mouse.RegisterMouseMove(p.X, p.Y);

                    if (!Mouse.InsideWindow) {
                        WinAPI.SetCapture(Window.windowHandle);
                        Mouse.RegisterMouseEnterWnd();
                    }
                } else {
                    // Check for MK_LBUTTON and MK_RBUTTON is down
                    if ((wParam & (0x0001 | 0x0002)) != 0) {
                        Mouse.RegisterMouseMove(p.X, p.Y);
                    } else {
                        WinAPI.ReleaseCapture();
                        Mouse.RegisterMouseLeaveWnd();
                    }
                }

                Mouse.RegisterMouseMove(p.X, p.Y);
                break;
            }

            case WindowMessages.MouseWheel:
                Mouse.RegisterMouseWheel(Math.Sign((short)WinAPI.HIWORD((int)wParam)));
                break;

            case WindowMessages.LMouseDown:
                Mouse.RegisterLMouseDown();
                WinAPI.SetForegroundWindow(Window.windowHandle);
                break;

            case WindowMessages.LMouseUp: {
                Mouse.RegisterLMouseUp();

                POINTS p = WinAPI.MakePOINTS(lParam);
                if (p.X < 0 || p.X >= wndSize.Width || p.Y < 0 || p.Y >= wndSize.Height) {
                    WinAPI.ReleaseCapture();
                    Mouse.RegisterMouseLeaveWnd();
                }
                break;
            }

            case WindowMessages.RMouseDown:
                Mouse.RegisterRMouseDown();
                WinAPI.SetForegroundWindow(Window.windowHandle);
                break;

            case WindowMessages.RMouseUp: {
                Mouse.RegisterRMouseUp();

                POINTS p = WinAPI.MakePOINTS(lParam);
                if (p.X < 0 || p.X >= wndSize.Width || p.Y < 0 || p.Y >= wndSize.Height) {
                    WinAPI.ReleaseCapture();
                    Mouse.RegisterMouseLeaveWnd();
                }
                break;
            }

            case WindowMessages.MMouseDown:
                Mouse.RegisterMMouseDown();
                WinAPI.SetForegroundWindow(Window.windowHandle);
                break;

            case WindowMessages.MMouseUp: {
                Mouse.RegisterMMouseUp();

                POINTS p = WinAPI.MakePOINTS(lParam);
                if (p.X < 0 || p.X >= wndSize.Width || p.Y < 0 || p.Y >= wndSize.Height) {
                    WinAPI.ReleaseCapture();
                    Mouse.RegisterMouseLeaveWnd();
                }
                break;
            }
        }

        return WinAPI.DefWindowProcW(hwnd, msg, wParam, lParam);
    }
}