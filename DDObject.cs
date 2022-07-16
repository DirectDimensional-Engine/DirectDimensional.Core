using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace DirectDimensional.Core {
    public abstract class DDObject {
        public string? Name { get; set; }

        public abstract bool Alive();
        public abstract void Destroy();
    }

    public static class DDObjectExtension {
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveInlining)]
        public static bool CheckAndDestroy(this DDObject? obj) {
            if (obj.IsAlive()) {
                obj.Destroy();
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveInlining)]
        public static bool IsAlive([NotNullWhen(true)] this DDObject? obj) {
            return obj != null && obj.Alive();
        }
    }
}
