using System;

namespace DirectDimensional.Core.Utilities {
    public static class KeyboardAxisRegister {
        public unsafe readonly struct KeyboardAxis {
            private KeyboardCode Negative { get; init; }
            private KeyboardCode Positive { get; init; }

            public KeyboardAxis(KeyboardCode negative, KeyboardCode positive) {
                Negative = negative;
                Positive = positive;
            }

            public int Pressed() {
                bool negative = Keyboard.Pressed(Negative);
                bool positive = Keyboard.Pressed(Positive);

                return *(int*)&positive - *(int*)&negative;
            }

            public int Hold() {
                bool negative = Keyboard.Holding(Negative);
                bool positive = Keyboard.Holding(Positive);

                return *(int*)&positive - *(int*)&negative;
            }

            public int Released() {
                bool negative = Keyboard.Released(Negative);
                bool positive = Keyboard.Released(Positive);

                return *(int*)&positive - *(int*)&negative;
            }
        }

        private static readonly Dictionary<int, KeyboardAxis> _axes;

        static KeyboardAxisRegister() {
            _axes = new(8);
        }

        public static int Pressed(int hash) {
            if (_axes.TryGetValue(hash, out var axis)) {
                return axis.Pressed();
            }

            Logger.Error($"Failed to Evaluate keyboard axis as the axis with hash value of {hash} is not exists.");
            return 0;
        }

        public static int Holding(int hash) {
            if (_axes.TryGetValue(hash, out var axis)) {
                return axis.Hold();
            }

            Logger.Error($"Failed to Evaluate keyboard axis as the axis with hash value of {hash} is not exists.");
            return 0;
        }

        public static int Released(int hash) {
            if (_axes.TryGetValue(hash, out var axis)) {
                return axis.Released();
            }

            Logger.Error($"Failed to Evaluate keyboard axis as the axis with hash value of {hash} is not exists.");
            return 0;
        }

        public static bool Register(int hash, KeyboardCode negative, KeyboardCode positive) {
            return _axes.TryAdd(hash, new KeyboardAxis(negative, positive));
        }

        public static bool Unregister(int hash) {
            return _axes.Remove(hash);
        }

        public static bool TryRead(int hash, out KeyboardAxis output) {
            return _axes.TryGetValue(hash, out output);
        }
    }
}
