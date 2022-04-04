using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings.DXGI;

using D3D11Texture2D = DirectDimensional.Bindings.Direct3D11.Texture2D;

namespace DirectDimensional.Core {
    public sealed unsafe class Texture2D : DDObjects {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private IntPtr _pixels;
        private D3D11Texture2D? _dxtexture;
        internal D3D11Texture2D? DXTexture => _dxtexture;

        private ShaderResourceView? _dxsrv;
        internal ShaderResourceView? DXSRV => _dxsrv;

        private SamplerState? _dxsampler;
        internal SamplerState? DXSampler => _dxsampler;

        public TextureFlags Flags { get; private set; }

        public Texture2D(int width, int height, TextureFlags flags) {
            Logger.WarnAssert(width > 0 && width <= 2048, "Invalid width to create texture. Fallback to 4", () => width = 4);
            Logger.WarnAssert(height > 0 && height <= 2048, "Invalid height to create texture. Fallback to 4", () => height = 4);

            Width = width;
            Height = height;

            Flags = flags;

            _pixels = Marshal.AllocHGlobal(width * height * 4);
            Unsafe.InitBlock(_pixels.ToPointer(), 0xFF, (uint)(width * height * 4));

            if (IsRenderable) {
                D3D11_TEXTURE2D_DESC desc = default;

                if (IsWritable) {
                    desc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.Write;
                    desc.Usage = D3D11_USAGE.Dynamic;
                } else {
                    desc.CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.None;
                    desc.Usage = D3D11_USAGE.Default;
                }

                desc.MipLevels = desc.ArraySize = 1u;
                desc.Format = DXGI_FORMAT.R8G8B8A8_UNORM;
                desc.BindFlags = D3D11_BIND_FLAG.ShaderResource;
                desc.SampleDesc.Count = 1u;
                desc.Width = (uint)width;
                desc.Height = (uint)height;

                D3D11_SUBRESOURCE_DATA srd = default;

                srd.pData = _pixels;
                srd.SysMemPitch = (uint)width;

                var device = Direct3DContext.Device;

                device.CreateTexture2D(desc, &srd, out _dxtexture).ThrowExceptionIfError();

                D3D11_SHADER_RESOURCE_VIEW_DESC vdesc = default;
                vdesc.Format = desc.Format;
                vdesc.ViewDimension = D3D11_SRV_DIMENSION.Texture2D;
                vdesc.Texture2D.MipLevels = 1u;
                vdesc.Texture2D.MostDetailedMip = 0u;

                device.CreateShaderResourceView(_dxtexture!, &vdesc, out _dxsrv).ThrowExceptionIfError();

                D3D11_SAMPLER_DESC sampler = default;
                sampler.Filter = D3D11_FILTER.MinMagMipPoint;
                sampler.AddressU = sampler.AddressV = sampler.AddressW = D3D11_TEXTURE_ADDRESS_MODE.Wrap;

                device.CreateSamplerState(sampler, out _dxsampler).ThrowExceptionIfError();
            }
        }

        public override void Destroy() {
            if (IsRenderable) {
                _dxsrv.Release();
                _dxsampler.Release();
                _dxtexture.Release();

                _dxsrv = null;
                _dxsampler = null;
                _dxtexture = null;
            }

            Marshal.FreeHGlobal(_pixels);
            _pixels = IntPtr.Zero;
        }

        public override bool Alive() {
            return _pixels != IntPtr.Zero;
        }

        [MemberNotNullWhen(true, "_dxtexture", "_dxsrv", "_dxsampler")]
        public bool IsRenderable => (Flags & TextureFlags.Render) == TextureFlags.Render;
        [MemberNotNullWhen(true, "_dxtexture", "_dxsrv", "_dxsampler")]
        public bool IsWritable => (Flags & TextureFlags.Write) == TextureFlags.Write;

        public Color32 this[int x, int y] {
            get {
                if (x < 0 || x >= Width) {
                    throw new ArgumentOutOfRangeException(nameof(x));
                }
                if (y < 0 || y >= Height) {
                    throw new ArgumentOutOfRangeException(nameof(y));
                }

                return new Color32(Marshal.ReadInt32(_pixels, (x + y * Width) * 4));
            }

            set {
                if (IsRenderable && !IsWritable) {
                    Logger.Error("Trying to Write renderable Texture without Write flag");
                    return;
                }

                if (x < 0 || x >= Width) {
                    throw new ArgumentOutOfRangeException(nameof(x));
                }
                if (y < 0 || y >= Height) {
                    throw new ArgumentOutOfRangeException(nameof(y));
                }

                Marshal.WriteInt32(_pixels, (x + y * Width) * 4, value.Integer);
            }
        }

        public unsafe void Apply() {
            if (!IsRenderable) return;

            var ctx = Direct3DContext.DevCtx;

            D3D11_MAPPED_SUBRESOURCE msr;
            ctx.Map(_dxtexture, 0u, D3D11_MAP.WriteDiscard, &msr);

            IntPtr pixels = _pixels;
            IntPtr pData = msr.pData;

            uint bpr = (uint)(Width * 4);

            for (int y = 0; y < Height; y++) {
                Unsafe.CopyBlock(pData.ToPointer(), pixels.ToPointer(), bpr);
                pixels += (int)bpr;
                pData += (int)msr.RowPitch;
            }

            ctx.Unmap(_dxtexture, 0u);
        }
    }
}
