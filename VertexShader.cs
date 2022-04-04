using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using System.Text;
using DirectDimensional.Bindings.D3DCompiler;
using DirectDimensional.Bindings.Direct3D11;
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

            try {
                return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path);
            } catch (Exception e) {
                Logger.Error("Exception thrown while trying to compile Vertex Shader from file '" + path + "'" + Environment.NewLine + e.ToString());
                return null;
            }
        }

        public static VertexShader? CompileFromRawFile(string path, string? sourceName, out Blob? bytecode) {
            if (!File.Exists(path)) {
                bytecode = null;
                return null;
            }

            try {
                return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path, out bytecode);
            } catch (Exception e) {
                Logger.Error("Exception thrown while trying to compile Vertex Shader from file '" + path + "'" + Environment.NewLine + e.ToString());

                bytecode = null;
                return null;
            }
        }

        public static VertexShader? CompileFromString(string input, string? sourceName) {
            D3DCOMPILE flags = D3DCOMPILE.None;
#if DEBUG
            flags |= D3DCOMPILE.Debug;
#endif
            var device = Direct3DContext.Device;

            HRESULT hr = D3D.Compile(input, sourceName, null, StandardShaderInclude.Instance, "main", "vs_4_0", flags, out var outputBlob, out var errorBlob);
            if (hr.Failed) {
                Console.WriteLine(Marshal.PtrToStringAnsi(errorBlob!.GetBufferPointer()));

                outputBlob.CheckAndRelease();
                errorBlob.Release();

                return null;
            }

            hr = device.CreateVertexShader(outputBlob!, out var shader);
            if (hr.Failed) {
                hr.PrintExceptionIfError();

                return null;
            }

            if (D3D.Reflect<ShaderReflection>(outputBlob!, out var reflection).Failed) {
                return null;
            }

            return new VertexShader() { _vs = shader!, _reflection = reflection! };
        }

        internal static VertexShader? CompileFromString(string input, string? sourceName, out Blob? bytecode) {
            D3DCOMPILE flags = D3DCOMPILE.None;
#if DEBUG
            flags |= D3DCOMPILE.Debug;
#endif
            var device = Direct3DContext.Device;

            HRESULT hr = D3D.Compile(input, sourceName, null, StandardShaderInclude.Instance, "main", "vs_4_0", flags, out bytecode, out var errorBlob);
            if (hr.Failed) {
                Console.WriteLine(Marshal.PtrToStringAnsi(errorBlob!.GetBufferPointer()));

                bytecode.CheckAndRelease();
                bytecode = null;

                errorBlob.Release();

                return null;
            }

            hr = device.CreateVertexShader(bytecode!, out var shader);
            if (hr.Failed) {
                hr.PrintExceptionIfError();

                return null;
            }

            if (D3D.Reflect<ShaderReflection>(bytecode!, out var reflection).Failed) {
                return null;
            }

            return new VertexShader() { _vs = shader!, _reflection = reflection! };
        }
    }
}
