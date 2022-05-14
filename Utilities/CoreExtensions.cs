using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectDimensional.Core.Utilities {
    public static class CoreExtensions {
        public static int Count<T>(this Span<T> span, T value) where T : IEquatable<T> {
            int c = 0;
            for (int i = 0; i < span.Length; i++) {
                if (span[i].Equals(value)) c++;
            }

            return c;
        }
        public static int Count<T>(this ReadOnlySpan<T> span, T value) where T : IEquatable<T> {
            int c = 0;
            for (int i = 0; i < span.Length; i++) {
                if (span[i].Equals(value)) c++;
            }

            return c;
        }

        public static int CountAny<T>(this Span<T> span, Span<T> values) where T : IEquatable<T> {
            int c = 0;

            for (int i = 0; i < span.Length; i++) {
                for (int j = 0; j < values.Length; i++) {
                    if (span[i].Equals(values[j])) {
                        c++;
                        continue;
                    }
                }
            }

            return c;
        }
        public static int CountAny<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> values) where T : IEquatable<T> {
            int c = 0;

            for (int i = 0; i < span.Length; i++) {
                for (int j = 0; j < values.Length; i++) {
                    if (span[i].Equals(values[j])) {
                        c++;
                        continue;
                    }
                }
            }

            return c;
        }
    }
}
