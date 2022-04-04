using System;
using System.Diagnostics;

namespace DirectDimensional.Core {
    public static class Logger {
        public static void Log(object? obj) {
            Console.WriteLine(obj);
        }

        public static void Warn(object? obj) {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(obj);
            Console.ForegroundColor = col;
        }

        public static void Error(object? obj) {
            var col = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(obj);
            Console.ForegroundColor = col;
        }

        /// <summary>
        /// Will print error message if the condition is false
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="msg">Message to print</param>
        [Conditional("DD_ASSERT")]
        public static void ErrorAssert(bool condition, string msg, Action? postError = null) {
            if (!condition) {
                Error(msg);
                postError?.Invoke();
            }
        }

        /// <summary>
        /// Will print warn message if the condition is false
        /// </summary>
        /// <param name="condition">Condition to check</param>
        /// <param name="msg">Message to print</param>
        [Conditional("DD_ASSERT")]
        public static void WarnAssert(bool condition, string msg, Action? postWarn = null) {
            if (!condition) {
                Warn(msg);
                postWarn?.Invoke();
            }
        }
    }
}
