using System.Diagnostics.CodeAnalysis;

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

        public static Material? CreateFromExternal(string vsPath, string psPath) {
            var vs = VertexShader.CompileFromRawFile(vsPath, vsPath);
            var ps = PixelShader.CompileFromRawFile(psPath, psPath);

            if (vs == null || ps == null) {
                vs.CheckAndDestroy();
                ps.CheckAndDestroy();

                return null;
            }

            return new Material(vs, ps);
        }
    }
}
