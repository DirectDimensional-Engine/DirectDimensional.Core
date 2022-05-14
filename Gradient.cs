using System;
using System.Collections.ObjectModel;
using DirectDimensional.Core.Utilities;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace DirectDimensional.Core {
    public enum GradientColorMode {
        Interpolation, Fixed,
    }

    public struct GradientKey {
        public Color32 Color { get; set; }
        public GradientColorMode Mode { get; set; }
        public ushort Position { get; set; }

        public float NormalizedPosition {
            get => (float)Position / ushort.MaxValue;
            set => Position = (ushort)MathF.Round(DDMath.Saturate(value) * ushort.MaxValue);
        }

        public GradientKey(Color32 color) : this(color, GradientColorMode.Interpolation, 0) { }
        public GradientKey(Color32 color, ushort pos) : this(color, GradientColorMode.Interpolation, pos) { }
        public GradientKey(Color32 color, GradientColorMode mode) : this(color, mode, 0) { }

        public GradientKey(Color32 color, GradientColorMode mode, ushort pos) {
            Color = color;
            Mode = mode;
            Position = pos;
        }
    }

    public sealed class Gradient {
        public static readonly Color32 FallbackColor = Color32.White;

        private List<GradientKey> _keys;

        /// <summary>
        /// For reading purpose only, not for modification. Or else unexpected thing can happen if Position value is modified
        /// </summary>
        public ReadOnlyCollection<GradientKey> Keys => _keys.AsReadOnly();

        public int KeyCount => _keys.Count;

        private bool _requireReorder = false;

        public Gradient() {
            _keys = new();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureReorder() {
            if (_requireReorder) ReorderKeys();
        }

        public Color32 Sample(int t) => Sample((ushort)t);
        public Color32 Sample(ushort t) {
            EnsureReorder();

            t = (ushort)Math.Clamp(t, 0u, ushort.MaxValue);

            switch (KeyCount) {
                case 0: return FallbackColor;
                case 1: return _keys[0].Color;
                default:
                    switch (t) {
                        case var _ when t < _keys[0].Position:
                            return _keys[0].Color;

                        case var _ when t >= _keys[^1].Position:
                            return _keys[^1].Color;

                        default:
                            int min = 0;
                            int max = _keys.Count - 1;

                            while (min <= max) {
                                var mid = min + (max - min) / 2;
                                var key = _keys[mid];

                                if (key.Position == t) {
                                    return _keys[mid].Color;
                                }

                                if (key.Position < t) {
                                    min = mid + 1;
                                } else {
                                    max = mid - 1;
                                }
                            }

                            var leftKey = _keys[max];

                            switch (leftKey.Mode) {
                                default:
                                    var rightKey = _keys[min];
                                    return Color32.Lerp(leftKey.Color, rightKey.Color, DDMath.InverseLerp(t, leftKey.Position, rightKey.Position));

                                case GradientColorMode.Fixed:
                                    return leftKey.Color;
                            }
                    }
            }
        }
        public Color32 Sample(float normalize) => Sample((ushort)MathF.Round(DDMath.Saturate(normalize) * ushort.MaxValue));

        public bool TryFindClosest(int position, out GradientKey output) {
            if (KeyCount == 0) {
                output = default;
                return false;
            }

            EnsureReorder();

            if (position <= _keys[0].Position) {
                output = _keys[0];
                return true;
            } else if (position >= _keys[^1].Position) {
                output = _keys[^1];
                return true;
            }

            int min = 0, max = _keys.Count - 1;
            while (min < max) {
                var mid = (max - min) / 2;
                var midKey = _keys[mid];

                if (midKey.Position == position) {
                    output = midKey;
                    return true;
                }

                var nextKey = _keys[mid + 1];

                if (position < midKey.Position) {
                    if (mid > 0 && position > midKey.Position) {
                        var prevKey = _keys[mid - 1];
                        output = position - prevKey.Position >= midKey.Position - position ? midKey : prevKey;
                        return true;
                    }

                    max = mid;
                } else {
                    if (mid < _keys.Count - 1 && position < nextKey.Position) {
                        output = position - midKey.Position >= nextKey.Position - position ? nextKey : midKey;
                        return true;
                    }

                    min = mid + 1;
                }
            }

            output = default;
            return false;
        }

        private static readonly Func<GradientKey, float> _reorder = (x) => x.Position;
        private void ReorderKeys() {
            _keys = _keys.DistinctBy(_reorder).OrderBy(_reorder).ToList();
            _requireReorder = false;
        }

        public unsafe void CopyKeys(List<GradientKey> destination) {
            destination.EnsureCapacity(_keys.Count);

            for (int i = 0; i < _keys.Count; i++) {
                destination.Add(_keys[i]);
            }

            //fixed (GradientKey* pDest = CollectionsMarshal.AsSpan(destination)) {
            //    fixed (GradientKey* pSrc = CollectionsMarshal.AsSpan(_keys)) {
            //        Unsafe.CopyBlock(pDest, pSrc, (uint)(sizeof(GradientKey) * _keys.Count));
            //    }
            //}
        }

        public unsafe void AssignKeys(IEnumerable<GradientKey>? donor) {
            _keys.Clear();

            if (donor != null) {
                foreach (var key in donor) {
                    _keys.Add(key);
                }

                //fixed (GradientKey* pDest = CollectionsMarshal.AsSpan(_keys)) {
                //    fixed (GradientKey* pSrc = CollectionsMarshal.AsSpan(donor)) {
                //        Unsafe.CopyBlock(pDest, pSrc, (uint)(sizeof(GradientKey) * donor.Count));
                //    }
                //}

                _requireReorder = true;
            }
        }
    }
}
