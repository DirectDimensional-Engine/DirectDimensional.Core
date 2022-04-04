using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings.D3DCompiler;
using DirectDimensional.Bindings.DXGI;
using DirectDimensional.Bindings;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Numerics;
using DirectDimensional.Core.Miscs;
using DirectDimensional.Core.Utilities;

using D3D11Buffer = DirectDimensional.Bindings.Direct3D11.Buffer;

using D3D11Texture2D = DirectDimensional.Bindings.Direct3D11.Texture2D;
using D3D11VertexShader = DirectDimensional.Bindings.Direct3D11.VertexShader;
using D3D11PixelShader = DirectDimensional.Bindings.Direct3D11.PixelShader;

namespace DirectDimensional.Core {
    internal static unsafe class EngineLifecycle {
        private static ComArray<RenderTargetView> _rtvs = null!;

        private static InputLayout _il = null!;

        private static ComArray<D3D11Buffer> _constantBuffers = null!;

        private static Mesh _mesh = null!;
        private static Material _standard3DShader = null!;

        public static void Initialize() {
            var device = Direct3DContext.Device;

            Direct3DContext.SwapChain.GetBuffer<D3D11Texture2D>(0, out var outputTexture).ThrowExceptionIfError();
            device.CreateRenderTargetView(outputTexture!, null, out var _rtv).ThrowExceptionIfError();
            _rtvs = new(_rtv);

            StandardShaderInclude.UpdateInstance(@"D:\C# Projects\DirectDimensional.Core\Resources", @"D:\C# Projects\DirectDimensional.Core\Resources");

            _mesh = new();

            _mesh.SetVertices(new List<Vertex>() {
                new Vertex(new(0, 0.5f, 0), Color32.White, new(0, 0)),
                new Vertex(new(0.5f, -0.5f, 0), Color32.White, new(0, 0)),
                new Vertex(new(-0.5f, -0.5f, 0), Color32.White, new(0, 0)),
            });

            _mesh.SetIndices(new List<ushort>() {
                0, 1, 2
            });

            var _vs = VertexShader.CompileFromRawFile(@"D:\C# Projects\DirectDimensional.Core\Resources\Standard3DVS.hlsl", null, out var pBytecode);

            D3D11_INPUT_ELEMENT_DESC[] inputElementDescs = new D3D11_INPUT_ELEMENT_DESC[] {
                new("Position", 0, DXGI_FORMAT.R32G32B32_FLOAT, 0, 0, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),
                new("Color", 0, DXGI_FORMAT.R8G8B8A8_UNORM, 0, 12, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),
                new("TexCoord", 0, DXGI_FORMAT.R32G32_FLOAT, 0, 16, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),

                new("Normal", 0, DXGI_FORMAT.R32G32B32_FLOAT, 0, 24, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),
                new("Tangent", 0, DXGI_FORMAT.R32G32B32_FLOAT, 0, 36, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),
                new("Bitangent", 0, DXGI_FORMAT.R32G32B32_FLOAT, 0, 48, D3D11_INPUT_CLASSIFICATION.PerVertexData, 0),
            };

            device.CreateInputLayout(inputElementDescs, pBytecode!, out _il!).ThrowExceptionIfError();

            for (int i = 0; i < inputElementDescs.Length; i++) {
                inputElementDescs[i].Dispose();
            }

            pBytecode.CheckAndRelease();

            var _ps = PixelShader.CompileFromRawFile(@"D:\C# Projects\DirectDimensional.Core\Resources\Standard3DPS.hlsl", null);

            _standard3DShader = new(_vs!, _ps!);

            pBytecode.CheckAndRelease();

            var clientSize = Window.Internal_ClientSize;
            Direct3DContext.DevCtx.RSSetViewports(new D3D11_VIEWPORT {
                Width = clientSize.Width,
                Height = clientSize.Height,
                TopLeftX = 0,
                TopLeftY = 0,
                MinDepth = 0,
                MaxDepth = 1,
            });

            _constantBuffers = new(RenderingConstants.SystemBufferCount);

            D3D11_BUFFER_DESC desc = default;
            desc.Usage = D3D11_USAGE.Dynamic;
            desc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.Write;
            desc.BindFlags = D3D11_BIND_FLAG.ConstantBuffer;

            // Initialize PerFrame buffer
            desc.ByteWidth = 16;

            D3D11Buffer outputBuffer;
            device.CreateBuffer(desc, null, out outputBuffer!).ThrowExceptionIfError();
            _constantBuffers[RenderingConstants.PerFrameBufferIndex] = outputBuffer;

            IntPtr pData = Marshal.AllocHGlobal(192);
            Matrix4x4 view = DDMath.LookToLH(-Vector3.UnitZ, Vector3.UnitZ, Vector3.UnitY);

            Unsafe.CopyBlock(pData.ToPointer(), &view, 64);
            Matrix4x4 projection = DDMath.PerspectiveFovLH(1.5f, Window.ClientAspectRatio, 0.3f, 1000f);
            Unsafe.CopyBlock((pData + 64).ToPointer(), &projection, 64);

            Matrix4x4 vp = view * projection;
            Unsafe.CopyBlock((pData + 128).ToPointer(), &vp, 64);

            D3D11_SUBRESOURCE_DATA srd = default;
            srd.pData = pData;

            desc.ByteWidth = 192;
            device.CreateBuffer(desc, &srd, out outputBuffer!).ThrowExceptionIfError();
            _constantBuffers[RenderingConstants.PerCameraBufferIndex] = outputBuffer;

            Marshal.FreeHGlobal(pData);

            pData = Marshal.AllocHGlobal(64);
            Matrix4x4 model = Matrix4x4.Identity;

            Unsafe.CopyBlock(pData.ToPointer(), &model, 64);
            desc.ByteWidth = 64;

            srd.pData = pData;

            device.CreateBuffer(desc, &srd, out outputBuffer!).ThrowExceptionIfError();
            _constantBuffers[RenderingConstants.PerObjectBuiltInIndex] = outputBuffer;

            Marshal.FreeHGlobal(pData);

            ComList<D3D11Buffer> comList = new(4);

            desc.ByteWidth = 64;
            device.CreateBuffer(desc, null, out outputBuffer!).ThrowExceptionIfError();
            comList.Add(outputBuffer);
            device.CreateBuffer(desc, null, out outputBuffer!).ThrowExceptionIfError();
            comList.Add(outputBuffer);
            device.CreateBuffer(desc, null, out outputBuffer!).ThrowExceptionIfError();
            comList.Add(outputBuffer);
            device.CreateBuffer(desc, null, out outputBuffer!).ThrowExceptionIfError();
            comList.Add(outputBuffer);

            int it = 0;
            foreach (D3D11Buffer? buffer in comList) {
                Console.WriteLine(it + ": " + buffer.GetNativePtr());
                it++;
            }

            it = 0;
            foreach (D3D11Buffer? buffer in comList) {
                Console.WriteLine(it + ": " + Marshal.ReadIntPtr(comList.NativePointer, it * IntPtr.Size));
                it++;
            }

            comList.TrueDispose();
        }

        public static void Cycle() {
            var d3dctx = Direct3DContext.DevCtx;

            d3dctx.ClearRenderTargetView(_rtvs![0]!, new FLOAT4(0, 0, 0, 0));
            d3dctx.OMSetRenderTargets(_rtvs, null);

            d3dctx.IASetInputLayout(_il);

            d3dctx.IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY.TriangleList);

            d3dctx.VSSetShader(_standard3DShader.VertexShader!.Shader);
            d3dctx.PSSetShader(_standard3DShader.PixelShader!.Shader);

            d3dctx.IASetVertexBuffers(0u, _mesh.VertexBuffers!, new[] { (uint)Vertex.MemorySize }, new[] { 0u });
            d3dctx.IASetIndexBuffer(_mesh.IndexBuffer, DXGI_FORMAT.R16_UINT, 0);

            d3dctx.VSSetConstantBuffers(0u, _constantBuffers!);
            d3dctx.PSSetConstantBuffers(0u, _constantBuffers!);

            d3dctx.DrawIndexed(3, 0, 0);

            Direct3DContext.SwapChain.Present(0u, DXGI_PRESENT.None);
        }

        public static void CleanUp() {
            _il.CheckAndRelease();
            _standard3DShader.Destroy();
            _mesh.Destroy();
            _rtvs.TrueDispose();

            _constantBuffers.TrueDispose();

            Direct3DContext.DisposeContext();
        }
    }
}
