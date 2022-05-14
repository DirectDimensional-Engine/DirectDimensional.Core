using System;
using System.Diagnostics;
using System.Text;

namespace DirectDimensional.Core {
    [Flags]
    public enum LoggerFlags {
        None = 0,

        LogStacktrace = 1 << 0,
        WarnStacktrace = 1 << 1,
        ErrorStacktrace = 1 << 2,

        Default = LogStacktrace | WarnStacktrace | ErrorStacktrace,
    }

    public static class Logger {
        public static LoggerFlags Flags { get; set; } = LoggerFlags.Default;

        public delegate void LogCallback(object? message, StackTrace? stacktrace);
        public delegate void WarnCallback(object? message, StackTrace? stacktrace);
        public delegate void ErrorCallback(object? message, StackTrace? stacktrace);

        public static event LogCallback? LoggingEvent;
        public static event WarnCallback? WarningEvent;
        public static event ErrorCallback? ErrorEvent;

        public static void LogWithoutStacktrace(object? obj) {
            LoggingEvent?.Invoke(obj, null);
        }

        public static void WarnWithoutStacktrace(object? obj) {
            WarningEvent?.Invoke(obj, null);
        }

        public static void ErrorWithoutStacktrace(object? obj) {
            ErrorEvent?.Invoke(obj, null);
        }

        public static void Log(object? obj) {
            StackTrace? stacktrace = null;
            if ((Flags & LoggerFlags.LogStacktrace) == LoggerFlags.LogStacktrace) {
                stacktrace = new StackTrace();
            }

            LoggingEvent?.Invoke(obj, stacktrace);
        }

        public static void Warn(object? obj) {
            StackTrace? stacktrace = null;
            if ((Flags & LoggerFlags.WarnStacktrace) == LoggerFlags.WarnStacktrace) {
                stacktrace = new StackTrace();
            }

            WarningEvent?.Invoke(obj, stacktrace);
        }

        public static void Error(object? obj) {
            StackTrace? stacktrace = null;
            if ((Flags & LoggerFlags.ErrorStacktrace) == LoggerFlags.ErrorStacktrace) {
                stacktrace = new StackTrace();
            }

            ErrorEvent?.Invoke(obj, stacktrace);
        }

        private static int _minStacktracePrint = 8;
        public static int MinimumStacktraceAmount {
            get => _minStacktracePrint;
            set => _minStacktracePrint = Math.Min(1, value);
        }

        public static void DecodeStacktrace(StackTrace st, StringBuilder output, int frameBegin = 1, int frameEnd = -1) {
            static void DecodeFrame(StackFrame frame, StringBuilder output) {
                var method = frame.GetMethod()!;

                output.Append("   at ").Append(method.DeclaringType!.FullName).Append('.').Append(method.Name).Append('(');

                {
                    var parameters = method.GetParameters();

                    if (parameters.Length != 0) {
                        foreach (var param in parameters) {
                            output.Append(param.ParameterType.Name).Append(' ').Append(param.Name).Append(", ");
                        }

                        output.Length -= 2;
                    }
                }

                output.Append(") (Position: ");

                var file = frame.GetFileName();
                if (file == null) {
                    output.Append("Unspecified");
                } else {
                    output.Append("File: \"").Append(file).Append('\"');

                    var line = frame.GetFileLineNumber();
                    if (line != 0) {
                        output.Append(", L").Append(line);
                    }
                }

                output.AppendLine(")");
            }

            frameBegin = Math.Max(1, frameBegin);

            if (frameBegin >= st.FrameCount) return;

            output.AppendLine("Stacktrace:");

            int end = frameEnd <= frameBegin ? Math.Min(frameBegin + _minStacktracePrint, st.FrameCount) : frameEnd;
            for (int i = frameBegin; i < end; i++) {
                DecodeFrame(st.GetFrame(i)!, output);
            }
        }
    }
}
