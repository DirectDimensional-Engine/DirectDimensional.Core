using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDimensional.Core {
    public enum InputCode : byte {
        Cancel = 0x03,
        Backspace = 0x08,
        Tab,
        Clear = 0x0C,
        Return = 0x0D,
        Shift = 0x10,
        Control,
        Menu,
        Pause,
        CapsLock,
        Escape = 0x1B,
        Space = 0x20,
        PageUp,
        PageDown,
        End,
        Home,
        Left = 0x25, Up, Right, Down,
        Select,
        Print, Execute, PrtScr, Insert,
        Delete, Help,

        Numeric_0, Numeric_1, Numeric_2, Numeric_3, Numeric_4, Numeric_5, Numeric_6, Numeric_7, Numeric_8, Numeric_9,

        A = 0x41, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,

        LeftWnd, RightWnd, Apps,
        Sleep = 0x5F,

        Numpad_0, Numpad_1, Numpad_2, Numpad_3, Numpad_4, Numpad_5, Numpad_6, Numpad_7, Numpad_8, Numpad_9,
        Multiply, Add, Seperator, Subtract, Decimal, Divide,

        F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12, F13, F14, F15, F16, F17, F18, F19, F20, F21, F22, F23, F24,

        NumLock = 0x90, ScrollLock,

        OEM_1 = 0xBA, OEM_Plus, OEM_Comma, OEM_Minus, OEM_Period, OEM_2, OEM_3,
        OEM_4 = 0xDB, OEM_5, OEM_6, OEM_7, OEM_8,

        OEM_102 = 0xE2, OEM_Clear = 0xFE,
    }
}
