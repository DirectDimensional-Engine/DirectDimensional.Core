using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using DirectDimensional.Core.Miscs;
using DirectDimensional.Core.Exceptions;
using DirectDimensional.Bindings.D3DCompiler;
using System.Text;
using System.Diagnostics.CodeAnalysis;

using D3D11PixelShader = DirectDimensional.Bindings.Direct3D11.PixelShader;

namespace DirectDimensional.Core {
    public sealed class PixelShader : DDObjects {
        private D3D11PixelShader? _ps = null!;
        private ShaderReflection? _reflection = null!;

        internal D3D11PixelShader? Shader => _ps;
        internal ShaderReflection? Reflection => _reflection;

        public override void Destroy() {
            _ps.CheckAndRelease();
            _reflection.CheckAndRelease();

            _ps = null;
            _reflection = null;
        }

        [MemberNotNullWhen(true, "_ps", "_reflection", "Shader", "Reflection")]
        public override bool Alive() {
            return _ps.Alive() && _reflection.Alive();
        }

        public static PixelShader? CompileFromRawFile(string path, string? sourceName) {
            if (!File.Exists(path)) return null;

            return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path);
        }

        /// <summary>
        /// Compile Pixel Shader 4.0 model directly from input string at runtime.
        /// </summary>
        /// <param name="input">Shader code uncompiled</param>
        /// <param name="sourceName">Source name to output. Useful for error handling</param>
        /// <param name="flags">Compilation flags</param>
        /// <returns>Shader instance if the compilation process has no error.</returns>
        /// <exception cref="ShaderCompilationException">Throw if compilation error are met.</exception>
        public static PixelShader? CompileFromString(string input, string? sourceName, D3DCOMPILE flags = D3DCOMPILE.None) {
            HRESULT hr = D3D.Compile(input, sourceName, null, StandardShaderInclude.Instance, "main", "ps_4_0", flags, out var outputBlob, out var errorBlob);
            if (hr.Failed) {
                outputBlob.CheckAndRelease();

                try {
                    throw new ShaderCompilationException(errorBlob);
                } finally {
                    errorBlob?.CheckAndRelease();
                }
            }

            hr = Direct3DContext.Device.CreatePixelShader(outputBlob!, out var shader);
            if (hr.Failed) {
                outputBlob!.Release();
                hr.ThrowExceptionIfError();

                return null;
            }

            hr = D3D.Reflect<ShaderReflection>(outputBlob!, out var reflection);
            if (hr.Failed) {
                outputBlob!.Release();
                hr.ThrowExceptionIfError();

                return null;
            }

            outputBlob!.Release();
            return new PixelShader() { _ps = shader!, _reflection = reflection! };
        }
    }
}
