using System.Diagnostics.CodeAnalysis;
using DirectDimensional.Bindings;
using DirectDimensional.Bindings.D3DCompiler;

namespace DirectDimensional.Core {
    public sealed class Material : DDObjects {
        public VertexShader? VertexShader { get; set; }
        public PixelShader? PixelShader { get; set; }

        public Material(VertexShader vs, PixelShader ps) {
            VertexShader = vs;
            PixelShader = ps;
        }

        [MemberNotNullWhen(true, "VertexShader", "PixelShader")]
        public override bool Alive() {
            return VertexShader.IsAlive() && PixelShader.IsAlive();
        }

        public override void Destroy() {
            VertexShader.CheckAndDestroy();
            PixelShader.CheckAndDestroy();

            VertexShader = null;
            PixelShader = null;
        }

        /// <summary>
        /// Create material directly from external shader files
        /// </summary>
        /// <exception cref="Exceptions.ShaderCompilationException"></exception>
        public static Material? CompileFromExternal(string vsPath, string psPath) {
            VertexShader? vs = null;
            PixelShader? ps = null;

            try {
                vs = VertexShader.CompileFromRawFile(vsPath, vsPath);
                ps = PixelShader.CompileFromRawFile(psPath, psPath);

                return new Material(vs!, ps!);
            } catch {
                vs.CheckAndDestroy();
                ps.CheckAndDestroy();

                throw;
            }
        }

        public static Material? Compile(string vsInput, string? vsSourceName, string psInput, string? psSourceName, D3DCOMPILE flags = D3DCOMPILE.None) {
            VertexShader? vs = null;
            PixelShader? ps = null;

            try {
                vs = VertexShader.CompileFromString(vsInput, vsSourceName, flags);
                ps = PixelShader.CompileFromString(psInput, psSourceName, flags);

                return new Material(vs!, ps!);
            } catch {
                vs.CheckAndDestroy();
                ps.CheckAndDestroy();

                throw;
            }
        }

        internal static Material? Compile(string vsInput, string? vsSourceName, string psInput, string? psSourceName, out Bindings.Direct3D11.Blob? vsBytecode, D3DCOMPILE flags = D3DCOMPILE.None) {
            VertexShader? vs = null;
            PixelShader? ps = null;

            vsBytecode = null;

            try {
                vs = VertexShader.CompileFromString(vsInput, vsSourceName, out vsBytecode, flags);
                ps = PixelShader.CompileFromString(psInput, psSourceName, flags);

                return new Material(vs!, ps!);
            } catch {
                vs.CheckAndDestroy();
                ps.CheckAndDestroy();

                vsBytecode.CheckAndRelease();
                vsBytecode = null;

                throw;
            }
        }
    }
}
