using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace DirectDimensional.Core {
    public struct CurveKey {
        public float Position { get; set; }
        public float Value { get; set; }
        public float InTangent { get; set; }
        public float OutTangent { get; set; }

        public CurveKey(float pos, float value, float inTang, float outTang) {
            Position = pos; Value = value; InTangent = inTang; OutTangent = outTang;
        }
    }

    /// <summary>
    /// Replicate Unity's AnimationCurve.
    /// </summary>
    public sealed class CurveGraph {
        [JsonProperty(PropertyName = "Keys")]
        private List<CurveKey> _keys;

        /// <summary>
        /// For reading purpose only, not for modification. Or else unexpected thing can happen if Position value is modified
        /// </summary>
        [JsonIgnore]
        public ReadOnlyCollection<CurveKey> Keys {
            get {
                ReorderKeys();
                return _keys.AsReadOnly();
            }
        }

        private bool _requireReorder;

        [JsonIgnore]
        public int KeyCount {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _keys.Count;
        }

        public CurveGraph() : this(8) { }
        public CurveGraph(int capacity) {
            _keys = new(capacity);
        }
        public CurveGraph(List<CurveKey> donor) {
            _keys = new(donor);
            _requireReorder = true;
        }

        public float Sample(float t) {
            ReorderKeys();

            switch (_keys.Count) {
                case 0: return 0;
                case 1: return _keys[0].Value;
                default:
                    switch (t) {
                        case var _ when t < _keys[0].Position: return _keys[0].Position;
                        case var _ when t > _keys[^1].Position: return _keys[^1].Position;
                        default:
                            int min = 0;
                            int max = _keys.Count - 1;

                            while (min <= max) {
                                var mid = min + (max - min) / 2;
                                var key = _keys[mid];

                                if (key.Position == t) {
                                    return _keys[mid].Value;
                                }

                                if (key.Position < t) {
                                    min = mid + 1;
                                } else {
                                    max = mid - 1;
                                }
                            }

                            return Evaluate(t, _keys[max], _keys[min]);
                    }
            }
        }

        private static readonly Func<CurveKey, float> _reorder = (x) => x.Position;
        private void ReorderKeys() {
            if (_requireReorder) {
                _keys = _keys.DistinctBy(_reorder).OrderBy(_reorder).ToList();
                _requireReorder = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyKeys(List<CurveKey> destination) {
            CopyKeys(destination, 0);
        }
        public void CopyKeys(List<CurveKey> destination, int offset) {
            int end = Math.Min(destination.Capacity, _keys.Count - offset);

            for (int i = offset; i < end; i++) {
                destination.Add(_keys[i]);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyKeys(Span<CurveKey> destination) {
            CopyKeys(destination, 0);
        }
        public void CopyKeys(Span<CurveKey> destination, int offset) {
            int end = Math.Min(destination.Length, _keys.Count - offset);

            for (int i = offset; i < end; i++) {
                destination[i] = _keys[i];
            }
        }

        public void AssignKeys(IEnumerable<CurveKey>? donor) {
            _keys.Clear();

            if (donor != null) {
                _keys.AddRange(donor);
                _requireReorder = true;
            }
        }
        public void AssignKeys(ReadOnlySpan<CurveKey> donor) {
            _keys.Clear();

            if (!donor.IsEmpty) {
                _keys.EnsureCapacity(donor.Length);
                for (int i = 0; i < donor.Length; i++) {
                    _keys.Add(donor[i]);
                }

                _requireReorder = true;
            }
        }

        [OnDeserialized]
        private void AfterDeserialized(StreamingContext ctx) {
            _requireReorder = true;
        }

        // https://en.wikipedia.org/wiki/Cubic_Hermite_spline
        // https://answers.unity.com/questions/464782/t-is-the-math-behind-animationcurveevaluate.html
        private static float Evaluate(float t, CurveKey left, CurveKey right) {
            float dt = right.Position - left.Position;

            float t2 = t * t;
            float t3 = t2 * t;

            return (2 * t3 - 3 * t2 + 1) * left.Value + 
                   (t3 - 2 * t2 + t) * left.OutTangent * dt + 
                   (t3 - t2) * right.InTangent * dt + 
                   (-2 * t3 + 3 * t2) * right.Value;
        }
    }
}
