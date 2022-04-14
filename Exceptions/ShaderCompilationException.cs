using System;
using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using DirectDimensional.Bindings.Direct3D11;

namespace DirectDimensional.Core.Exceptions {
    public sealed class ShaderCompilationException : Exception {
        public ShaderCompilationException(Blob? errorBlob) : base(errorBlob.Alive() ? Marshal.PtrToStringAnsi(errorBlob.GetBufferPointer()) : "Unprovided information") { }
    }
}
