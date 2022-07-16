using System.Collections.ObjectModel;
using DirectDimensional.Core.Utilities;
using System.Runtime.CompilerServices;
using System.Collections;
using DirectDimensional.Core.Miscs.JConverters;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DirectDimensional.Core {
    public enum GradientColorMode {
        Interpolation, Fixed,
    }

    [JsonConverter(typeof(GradientKeyConverter))]
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

        public override string ToString() {
            return nameof(GradientKey) + "(" + Color + ", " + Mode + ", " + Position + ")";
        }
    }

    [JsonConverter(typeof(GradientConverter))]
    public sealed class Gradient : IEnumerable<GradientKey> {
        public static readonly Color32 FallbackColor = Color32.White;

        private List<GradientKey> _keys;

        /// <summary>
        /// For reading purpose only, not for modification. Or else unexpected thing can happen if Position value is modified
        /// </summary>
        public ReadOnlyCollection<GradientKey> Keys {
            get {
                ReorderKeys();
                return _keys.AsReadOnly();
            }
        }

        public bool Wrapping { get; set; } = false;

        public int KeyCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _keys.Count;
        }

        private bool _requireReorder = false;

        public Gradient() {
            _keys = new();
        }

        public Gradient(int capacity) {
            _keys = new(capacity);
        }

        public Gradient(Gradient other) {
            _keys = new(other._keys.Count);

            other.ReorderKeys();
            for (int i = 0; i < other._keys.Count; i++) {
                _keys.Add(other._keys[i]);
            }
        }

        public Gradient(IEnumerable<GradientKey> keys) {
            _keys = new(keys);
            _requireReorder = true;
        }

        public Color32 Sample(int t) => Sample((ushort)t);
        public Color32 Sample(ushort t) {
            ReorderKeys();

            t = (ushort)Math.Clamp(t, 0u, ushort.MaxValue);

            switch (KeyCount) {
                case 0: return FallbackColor;
                case 1: return _keys[0].Color;
                default:
                    switch (t) {
                        case var _ when t <= _keys[0].Position:
                            if (Wrapping) {
                                var last = _keys[^1];

                                return last.Mode switch {
                                    GradientColorMode.Fixed => last.Color,
                                    _ => Color32.Lerp(last.Color, _keys[0].Color, DDMath.InverseLerp(t, last.Position - ushort.MaxValue, _keys[0].Position)),
                                };
                            }
                            return _keys[0].Color;

                        case var _ when t >= _keys[^1].Position:
                            if (Wrapping) {
                                switch (_keys[^1].Mode) {
                                    case GradientColorMode.Fixed:
                                        return _keys[^1].Color;

                                    case GradientColorMode.Interpolation:
                                        var first = _keys[0];
                                        return Color32.Lerp(_keys[^1].Color, first.Color, DDMath.InverseLerp(t, _keys[^1].Position, first.Position + ushort.MaxValue));
                                }
                            }
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

        /// <summary>
        /// Create a new Gradient instance the same as this gradient, but all Key has inversed direction
        /// </summary>
        public Gradient CreateInverse() {
            ReorderKeys();
            Gradient ret = new(_keys.Count);

            for (int i = 0; i < _keys.Count; i++) {
                var r = _keys[i];

                ret._keys.Add(new(r.Color, r.Mode, (ushort)(ushort.MaxValue - r.Position)));
            }

            ret.ForceReorderKeys();
            return ret;
        }

        public Gradient CreateGrayscale() {
            ReorderKeys();
            Gradient ret = new(_keys.Count);

            for (int i = 0; i < _keys.Count; i++) {
                var r = _keys[i];

                ret._keys.Add(new(r.Color.GrayscaleColor, r.Mode, r.Position));
            }

            return ret;
        }

        private static readonly Func<GradientKey, float> _reorder = (x) => x.Position;
        private void ReorderKeys() {
            if (_requireReorder) {
                ForceReorderKeys();
            }
        }
        private void ForceReorderKeys() {
            _keys = _keys.DistinctBy(_reorder).OrderBy(_reorder).ToList();
            _requireReorder = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<GradientKey> CopyKeys() {
            List<GradientKey> output = new(_keys.Count);
            CopyKeys(output, 0);
            return output;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyKeys(List<GradientKey> destination) {
            CopyKeys(destination, 0);
        }
        public void CopyKeys(List<GradientKey> destination, int offset) {
            int end = Math.Min(destination.Capacity, _keys.Count - offset);

            for (int i = offset; i < end; i++) {
                destination.Add(_keys[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyKeys(Span<GradientKey> destination) {
            CopyKeys(destination, 0);
        }
        public void CopyKeys(Span<GradientKey> destination, int offset) {
            int end = Math.Min(destination.Length, _keys.Count - offset + 1);

            for (int i = offset; i < end; i++) {
                destination[i] = _keys[i];
            }
        }

        public void AssignKeys(IEnumerable<GradientKey>? donor) {
            _keys.Clear();

            if (donor != null) {
                _keys.AddRange(donor);
                _requireReorder = true;
            }
        }
        public void AssignKeys(ReadOnlySpan<GradientKey> donor) {
            _keys.Clear();

            if (!donor.IsEmpty) {
                _keys.EnsureCapacity(donor.Length);
                for (int i = 0; i < donor.Length; i++) {
                    _keys.Add(donor[i]);
                }

                _requireReorder = true;
            }
        }

        public void Clear() {
            _keys.Clear();
            _requireReorder = false;
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext ctx) {
            _requireReorder = true;
        }

        public IEnumerator<GradientKey> GetEnumerator() {
            ReorderKeys();
            return _keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            ReorderKeys();
            return ((IEnumerable)_keys).GetEnumerator();
        }
    }
}
