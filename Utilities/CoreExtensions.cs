using System.Runtime.CompilerServices;
using System.Numerics;

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

        /// <summary>
        /// Remove items at given range. Start is inclusive, End is exclusive. 0 based index.
        /// </summary>
        public static void RemoveRange<T>(this List<T> list, Range range) {
            var start = range.Start.IsFromEnd ? list.Count - range.Start.Value : range.Start.Value;
            var end = range.End.IsFromEnd ? list.Count - range.End.Value : range.End.Value;

            list.RemoveRange(start, end - start);
        }

        /// <summary>
        /// Remove item from given index to the end of list.
        /// </summary>
        /// <param name="list">List to remove elements</param>
        /// <param name="index">Index to begin remove from, inclusive, 0 based index.</param>
        public static void RemoveFrom<T>(this List<T> list, Index index) {
            var start = index.IsFromEnd ? list.Count - index.Value : index.Value;

            list.RemoveRange(start, list.Count - start);
        }

        /// <summary>
        /// Pop given stack until it's left with given amount of elements.
        /// </summary>
        /// <param name="stack">Stack to remove</param>
        /// <param name="left">How many elements to left with?</param>
        public static void PopUntil<T>(this Stack<T> stack, int left) {
            while (stack.Count > left) stack.Pop();
        }

        public static Span<T> Replace<T>(this Span<T> span, T value, T newValue) where T : IEquatable<T> {
            int begin = 0;
            var _oldSpan = span;

            while (true) {
                if (span.IsEmpty) return _oldSpan;

                var indexOf = span[begin..].IndexOf(value);
                if (indexOf == -1) return _oldSpan;

                span[indexOf] = newValue;
                span = span[(indexOf + 1)..];
            }
        }

        /// <summary>
        /// Swap all elements between 2 lists. O(n) operation, with n is the largest size between 2 lists.
        /// </summary>
        public static void Swap<T>(this List<T> list1, List<T> list2) {
            var min = Math.Min(list1.Count, list2.Count);
            for (int i = 0; i < min; i++) {
                (list1[i], list2[i]) = (list2[i], list1[i]);
            }

            if (list2.Count < list1.Count) {
                list2.EnsureCapacity(list1.Count);
                for (int i = min; i < list1.Count; i++) {
                    list2.Add(list1[i]);
                }

                list1.RemoveRange(min, list1.Count - min);
            } else if (list1.Count < list2.Count) {
                list1.EnsureCapacity(list2.Count);
                for (int i = min; i < list2.Count; i++) {
                    list1.Add(list2[i]);
                }

                list2.RemoveRange(min, list2.Count - min);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Assign<T>(this T? nullable, ref T receiver) where T : struct {
            if (!nullable.HasValue) return false;

            receiver = nullable.Value;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryAssign<T>(this T? nullable, out T receiver) where T : struct {
            if (!nullable.HasValue) {
                receiver = default;
                return false;
            }

            receiver = nullable.Value;
            return true;
        }

        public static bool Consume<T>(ref this T? nullable, ref T receiver) where T : struct {
            if (!nullable.HasValue) return false;

            receiver = nullable.Value;
            nullable = null;

            return true;
        }

        public static SpanSingleDelimiterEnumerator<T> EnumerateDelimiter<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter) where T : IEquatable<T> {
            return new SpanSingleDelimiterEnumerator<T>(span, delimiter);
        }
        public static SpanMultipleDelimiterEnumerator<T> EnumerateDelimiters<T>(this ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters) where T : IEquatable<T> {
            return new SpanMultipleDelimiterEnumerator<T>(span, delimiters);
        }

        /// <summary>
        /// Responsible for enumerating a span with a sequence as a delimiter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public ref struct SpanSingleDelimiterEnumerator<T> where T : IEquatable<T> {
            private ReadOnlySpan<T> _span;
            private readonly ReadOnlySpan<T> _delimiter;

            public SpanSingleDelimiterEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiter) {
                _span = span;
                _delimiter = delimiter;
                Current = default;
            }

            public SpanSingleDelimiterEnumerator<T> GetEnumerator() => this;
            public ReadOnlySpan<T> Current { get; private set; }

            public bool MoveNext() {
                if (_span.Length == 0) return false;

                var index = _span.IndexOf(_delimiter);
                if (index == -1) {
                    Current = _span;
                    _span = default;
                    return true;
                }

                Current = _span[0..index];
                _span = _span[(index + _delimiter.Length)..];
                return true;
            }
        }

        /// <summary>
        /// Responsible for enumerating a span with delimiters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public ref struct SpanMultipleDelimiterEnumerator<T> where T : IEquatable<T> {
            private ReadOnlySpan<T> _span;
            private readonly ReadOnlySpan<T> _delimiters;

            public SpanMultipleDelimiterEnumerator(ReadOnlySpan<T> span, ReadOnlySpan<T> delimiters) {
                _span = span;
                _delimiters = delimiters;
                Current = default;
            }

            public SpanMultipleDelimiterEnumerator<T> GetEnumerator() => this;
            public ReadOnlySpan<T> Current { get; private set; }

            public bool MoveNext() {
                if (_span.Length == 0) return false;

                var index = _span.IndexOfAny(_delimiters);
                if (index == -1) {
                    Current = _span;
                    _span = default;
                    return true;
                }

                Current = _span[0..index];
                _span = _span[(index + 1)..];
                return true;
            }
        }
    }
}
