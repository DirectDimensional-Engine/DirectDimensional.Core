using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings.DXGI;

using D3D11Device = DirectDimensional.Bindings.Direct3D11.Device;

namespace DirectDimensional.Core {
    internal static class Direct3DContext {
        public static D3D11Device _device;
        public static DeviceContext _devctx;
        public static SwapChain _sw;

        public static D3D11Device Device => _device;
        public static DeviceContext DevCtx => _devctx;
        public static SwapChain SwapChain => _sw;

        static Direct3DContext() {
            D3D11_CREATE_DEVICE_FLAG flags = D3D11_CREATE_DEVICE_FLAG.None;
#if DEBUG
            flags |= D3D11_CREATE_DEVICE_FLAG.Debug;
#endif
            var clientSize = Window.Internal_ClientSize;

            DXGI_SWAP_CHAIN_DESC swDesc = default;
            swDesc.BufferCount = 2u;
            swDesc.Windowed = true;
            swDesc.SampleDesc.Count = 1u;
            swDesc.SampleDesc.Quality = 0u;
            swDesc.BufferDesc.Width = (uint)clientSize.Width;
            swDesc.BufferDesc.Height = (uint)clientSize.Height;
            swDesc.BufferDesc.Format = DXGI_FORMAT.R8G8B8A8_UNORM;
            swDesc.BufferDesc.RefreshRate.Numerator = 1u;
            swDesc.BufferDesc.RefreshRate.Denominator = 0u;
            swDesc.BufferUsage = DXGI_USAGE.RenderTargetOutput;
            swDesc.OutputWindow = Window.windowHandle;
            swDesc.SwapEffect = DXGI_SWAP_EFFECT.FlipSequential;

            D3D11.CreateDeviceAndSwapChain(D3D_DRIVER_TYPE.Hardware, IntPtr.Zero, flags, null, swDesc, out _sw!, out _device!, out _, out _devctx!).ThrowExceptionIfError();
        }

        internal static void DisposeContext() {
            _sw.Release();
            _device.Release();
            _devctx.Release();
        }
    }
}
