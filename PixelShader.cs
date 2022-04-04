using System.Runtime.InteropServices;
using DirectDimensional.Bindings;
using DirectDimensional.Core.Miscs;
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

            try {
                return CompileFromString(File.ReadAllText(path, Encoding.UTF8), sourceName ?? path);
            } catch (Exception e) {
                Logger.Error("Exception thrown while trying to compile Pixel Shader from file '" + path + "'" + Environment.NewLine + e.ToString());
                return null;
            }
        }

        public static PixelShader? CompileFromString(string input, string? sourceName) {
            D3DCOMPILE flags = D3DCOMPILE.None;
#if DEBUG
            flags |= D3DCOMPILE.Debug;
#endif

            HRESULT hr = D3D.Compile(input, sourceName, null, StandardShaderInclude.Instance, "main", "ps_4_0", flags, out var outputBlob, out var errorBlob);
            if (hr.Failed) {
                Console.WriteLine(Marshal.PtrToStringAnsi(errorBlob!.GetBufferPointer()));

                outputBlob.CheckAndRelease();
                errorBlob.Release();

                hr.ThrowExceptionIfError();

                return null;
            }

            hr = Direct3DContext.Device.CreatePixelShader(outputBlob!, out var shader);
            if (hr.Failed) {
                hr.ThrowExceptionIfError();

                return null;
            }

            if (D3D.Reflect<ShaderReflection>(outputBlob!, out var reflection).Failed) {
                outputBlob!.Release();

                return null;
            }

            outputBlob!.Release();
            return new PixelShader() { _ps = shader!, _reflection = reflection! };
        }
    }
}
