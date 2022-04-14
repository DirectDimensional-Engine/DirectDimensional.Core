using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using System.Text;
using DirectDimensional.Bindings.D3DCompiler;
using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Core.Exceptions;
using DirectDimensional.Core.Miscs;
using System.Diagnostics.CodeAnalysis;

using D3D11VertexShader = DirectDimensional.Bindings.Direct3D11.VertexShader;

namespace DirectDimensional.Core {
    public sealed class VertexShader : DDObjects {
        private D3D11VertexShader? _vs = null!;
        private ShaderReflection? _reflection = null!;

        internal D3D11VertexShader? Shader => _vs;
        internal ShaderReflection? Reflection => _reflection;

        public override void Destroy() {
            _vs.CheckAndRelease();
            _reflection.CheckAndRelease();

            _vs = null;
            _reflection = null;
        }

        [MemberNotNullWhen(true, "_vs", "_reflection", "Shader", "Reflection")]
        public override bool Alive() {
            return _vs.Alive() && _reflection.Alive();
        }

        public static VertexShader? CompileFromRawFile(string path, string? sourceName) {
            if (!File.Exists(path)) return null;

            return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path);
        }

        public static VertexShader? CompileFromRawFile(string path, string? sourceName, out Blob? bytecode) {
            if (!File.Exists(path)) {
                bytecode = null;
                return null;
            }

            return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path, out bytecode);
        }

        /// <summary>
        /// Compile Shader directly from input string at runtime.
        /// </summary>
        /// <param name="input">Shader code uncompiled</param>
        /// <param name="sourceName">Source name to output. Useful for error handling</param>
        /// <returns>Shader instance if the compilation process has no error.</returns>
        /// <exception cref="ShaderCompilationException">Throw if compilation error are met.</exception>
        public static VertexShader? CompileFromString(string input, string? sourceName, D3DCOMPILE flags = D3DCOMPILE.None) {
            Blob? bytecode = null;

            try {
                return CompileFromString(input, sourceName, out bytecode, flags);
            } finally {
                bytecode.CheckAndRelease();
            }
        }

        /// <summary>
        /// Compile Vertex Shader 4.0 model directly from input string at runtime.
        /// </summary>
        /// <param name="input">Shader code uncompiled</param>
        /// <param name="sourceName">Source name to output. Useful for error handling</param>
        /// <param name="bytecode">Reference to output bytecode. Must call Release() at some point.</param>
        /// <param name="flags">Compilation flags</param>
        /// <returns>Shader instance if the compilation process has no error.</returns>
        /// <exception cref="ShaderCompilationException">Throw if compilation error are met.</exception>
        internal static VertexShader? CompileFromString(string input, string? sourceName, out Blob? bytecode, D3DCOMPILE flags = D3DCOMPILE.None) {
            var device = Direct3DContext.Device;

            HRESULT hr = D3D.Compile(input, sourceName, null, StandardShaderInclude.Instance, "main", "vs_4_0", flags, out bytecode, out var errorBlob);
            if (hr.Failed) {
                bytecode.CheckAndRelease();
                bytecode = null;

                try {
                    throw new ShaderCompilationException(errorBlob);
                } finally {
                    errorBlob?.CheckAndRelease();
                }
            }

            hr = device.CreateVertexShader(bytecode!, out var shader);
            if (hr.Failed) {
                bytecode!.Release();
                hr.ThrowExceptionIfError();

                return null;
            }

            hr = D3D.Reflect<ShaderReflection>(bytecode!, out var reflection);
            if (hr.Failed) {
                bytecode!.Release();
                hr.ThrowExceptionIfError();

                return null;
            }

            return new VertexShader() { _vs = shader!, _reflection = reflection! };
        }
    }
}
