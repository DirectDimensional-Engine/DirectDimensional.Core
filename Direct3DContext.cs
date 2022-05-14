using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings.DXGI;
using DirectDimensional.Bindings.WinAPI;
using DirectDimensional.Bindings;
using System.Runtime.InteropServices;
using System.Numerics;
using System.Runtime.CompilerServices;

using D3D11Device = DirectDimensional.Bindings.Direct3D11.Device;
using D3D11Texture2D = DirectDimensional.Bindings.Direct3D11.Texture2D;

namespace DirectDimensional.Core {
    internal static unsafe class Direct3DContext {
        private static D3D11Device _device = null!;
        private static DeviceContext _devctx = null!;
        private static SwapChain _sc = null!;

        private static ComArray<RenderTargetView> _renderOutputs = null!;

        public static D3D11Device Device => _device;
        public static DeviceContext DevCtx => _devctx;

        private static ID3D11Debug _debug = null!;
        private static ID3D11InfoQueue _queue = null!;

        public static SwapChain SwapChain => _sc;
        public static RenderTargetView RenderingOutput => _renderOutputs[0]!;
        public static ComArray<RenderTargetView> RenderingOutputAsArray => _renderOutputs;

        private static ComArray<RenderTargetView> _unbindRTVs = null!;

        public static void Initialize(IntPtr outputWindow) {
            D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_FLAG.None;
            flags |= D3D11_CREATE_DEVICE_FLAG.Debug;

            WinAPI.GetClientRect(outputWindow, out var rect);

            DXGI_SWAP_CHAIN_DESC swDesc = default;
            swDesc.BufferCount = 2u;
            swDesc.Windowed = true;
            swDesc.SampleDesc.Count = 1u;
            swDesc.SampleDesc.Quality = 0u;
            swDesc.BufferDesc.Width = (uint)rect.Right;
            swDesc.BufferDesc.Height = (uint)rect.Bottom;
            swDesc.BufferDesc.Format = DXGI_FORMAT.R8G8B8A8_UNORM;
            swDesc.BufferDesc.RefreshRate.Numerator = 1u;
            swDesc.BufferDesc.RefreshRate.Denominator = 0u;
            swDesc.BufferUsage = DXGI_USAGE.RenderTargetOutput;
            swDesc.OutputWindow = outputWindow;
            swDesc.SwapEffect = DXGI_SWAP_EFFECT.FlipSequential;

            D3D11.CreateDeviceAndSwapChain(D3D_DRIVER_TYPE.Hardware, IntPtr.Zero, flags, null, swDesc, out _sc!, out _device!, out _, out _devctx!).ThrowExceptionIfError();

            _debug = _device.QueryInterface<ID3D11Debug>()!;
            _queue = _device.QueryInterface<ID3D11InfoQueue>()!;

            _queue.PushEmptyStorageFilter();

            _sc.GetBuffer<D3D11Texture2D>(0, out var outputTexture).ThrowExceptionIfError();
            _device.CreateRenderTargetView(outputTexture!, null, out var _rtv).ThrowExceptionIfError();
            _renderOutputs = new(_rtv);

            outputTexture!.Release();

            ResetFirstViewport(swDesc.BufferDesc.Width, swDesc.BufferDesc.Height);

            _unbindRTVs = new(1);
        }

        public static void Shutdown() {
            _unbindRTVs.Dispose();

            _renderOutputs.TrueDispose();
            _sc.Release();

            _devctx.ClearState();

            _debug.Release();
            _queue.Release();

            _device.Release();
            _devctx.Release();
        }

        public static void ReportLiveObjects() {
            _debug.ReportLiveDeviceObjects(D3D11_RLDO_FLAGS.Detail);
        }

        public static void FlushD3D11DebugMessages(Action<IntPtr> action) {
            ulong msgCount = _queue!.GetNumStoredMessages();
            for (ulong i = 0; i < msgCount; i++) {
                nuint msgSize = 0;
                _queue.GetMessage(i, null, ref msgSize).ThrowExceptionIfError();

                IntPtr pMessage = Marshal.AllocHGlobal((int)msgSize);
                Unsafe.InitBlock(pMessage.ToPointer(), 0x00, (uint)msgSize);

                try {
                    _queue.GetMessage(i, (D3D11_MESSAGE*)pMessage.ToPointer(), ref msgSize).ThrowExceptionIfError();
                    action(pMessage);
                } finally {
                    Marshal.FreeHGlobal(pMessage);
                }
            }
        }

        public static void ClearD3D11DebugMessages() {
            _queue.ClearStoredMessages();
        }

        public static void ResetFirstViewport(float width, float height) {
            DevCtx.RSSetViewport(new D3D11_VIEWPORT {
                Width = width,
                Height = height,
                TopLeftX = 0,
                TopLeftY = 0,
                MinDepth = 0,
                MaxDepth = 1,
            });
        }

        public static void ResizeSwapChain() {
            if (_sc.Alive()) {
                _devctx.OMSetRenderTargets(_unbindRTVs, null);

                _renderOutputs[0].CheckAndRelease();

                _sc.ResizeBuffer(0, 0, 0, DXGI_FORMAT.Unknown, DXGI_SWAP_CHAIN_FLAG.None).ThrowExceptionIfError();

                _sc.GetBuffer<D3D11Texture2D>(0, out var pTexture).ThrowExceptionIfError();
                _device.CreateRenderTargetView(pTexture!, null, out var _rtv).ThrowExceptionIfError();
                _renderOutputs[0] = _rtv;

                var desc = pTexture!.Description;
                ResetFirstViewport(desc.Width, desc.Height);

                pTexture!.Release();
            }
        }
    }
}
