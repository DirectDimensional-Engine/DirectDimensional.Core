using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings.DXGI;
using DirectDimensional.Core.Utilities;

using D3D11Texture2D = DirectDimensional.Bindings.Direct3D11.Texture2D;

namespace DirectDimensional.Core {
    public sealed unsafe class Texture2D : DDObjects {
        public static readonly int TextureMaximumSize = 4096;

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

        public Texture2D(ReadOnlySpan<byte> channels, int width, TextureFlags flags) {
            if (width <= 0 || width > TextureMaximumSize) {
                width = Math.Clamp(width, 1, TextureMaximumSize);
                Logger.Warn("Invalid width to create texture. Fallback to " + width);
            }

            Height = channels.Length / 4 / width;
            if (Height <= 0) throw new ArgumentOutOfRangeException(nameof(channels), "Not sufficient enough to make a valid texture as the height of it will be 0");

            Width = width;
            Flags = flags;

            int byteCount = Width * Height * 4;
            _pixels = Marshal.AllocHGlobal(byteCount);

            fixed (byte* pChannels = channels) {
                Unsafe.CopyBlock(_pixels.ToPointer(), pChannels, (uint)byteCount);
            }

            InitializeDXObjects(this, flags, out _dxtexture, out _dxsrv, out _dxsampler);
        }
        public Texture2D(ReadOnlySpan<int> pixels, int width, TextureFlags flags) : this(MemoryMarshal.Cast<int, Color32>(pixels), width, flags) {}
        public Texture2D(ReadOnlySpan<Color32> pixels, int width, TextureFlags flags) {
            if (width <= 0 || width > TextureMaximumSize) {
                width = Math.Clamp(width, 1, TextureMaximumSize);
                Logger.Warn("Invalid width to create texture. Fallback to " + width);
            }

            Height = pixels.Length / width;
            if (Height <= 0) throw new ArgumentOutOfRangeException(nameof(pixels), "Not sufficient enough to make a valid texture as the height of it will be 0");

            Width = width;
            Flags = flags;

            int byteCount = Width * Height * 4;
            _pixels = Marshal.AllocHGlobal(byteCount);

            fixed (Color32* pChannels = pixels) {
                Unsafe.CopyBlock(_pixels.ToPointer(), pChannels, (uint)byteCount);
            }

            InitializeDXObjects(this, flags, out _dxtexture, out _dxsrv, out _dxsampler);
        }

        public Texture2D(int width, int height, TextureFlags flags) {
            if (width <= 0 || width > TextureMaximumSize) {
                width = Math.Clamp(width, 1, TextureMaximumSize);
                Logger.Warn("Invalid width to create texture. Fallback to " + width);
            }
            if (height <= 0 || height > TextureMaximumSize) {
                height = Math.Clamp(height, 1, TextureMaximumSize);
                Logger.Warn("Invalid width to create texture. Fallback to " + height);
            }

            Width = width;
            Height = height;
            Flags = flags;

            _pixels = Marshal.AllocHGlobal(width * height * 4);
            Unsafe.InitBlock(_pixels.ToPointer(), 0xFF, (uint)(width * height * 4));

            InitializeDXObjects(this, flags, out _dxtexture, out _dxsrv, out _dxsampler);
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

        [MemberNotNullWhen(true, "_dxtexture", "_dxsrv", "_dxsampler", "DXTexture", "DXSRV", "DXSampler")]
        public bool IsRenderable => (Flags & TextureFlags.Render) == TextureFlags.Render;
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
            ctx.Map(_dxtexture, 0u, D3D11_MAP.WriteDiscard, &msr).ThrowExceptionIfError();

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

        private static void InitializeDXObjects(Texture2D texture, TextureFlags flags, out D3D11Texture2D? outputTexture, out ShaderResourceView? srv, out SamplerState? sampler) {
            if ((flags & TextureFlags.Render) == TextureFlags.Render) {
                D3D11_TEXTURE2D_DESC desc = default;

                if ((flags & TextureFlags.Write) == TextureFlags.Write) {
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
                desc.Width = (uint)texture.Width;
                desc.Height = (uint)texture.Height;

                D3D11_SUBRESOURCE_DATA srd = default;

                srd.pData = texture._pixels;
                srd.SysMemPitch = desc.Width * 4;

                var device = Direct3DContext.Device;

                device.CreateTexture2D(desc, &srd, out outputTexture).ThrowExceptionIfError();

                D3D11_SHADER_RESOURCE_VIEW_DESC vdesc = default;
                vdesc.Format = desc.Format;
                vdesc.ViewDimension = D3D11_SRV_DIMENSION.Texture2D;
                vdesc.Texture2D.MipLevels = 1u;
                vdesc.Texture2D.MostDetailedMip = 0u;

                device.CreateShaderResourceView(outputTexture!, &vdesc, out srv).ThrowExceptionIfError();

                D3D11_SAMPLER_DESC sd = default;
                sd.Filter = D3D11_FILTER.MinMagMipPoint;
                sd.AddressU = sd.AddressV = sd.AddressW = D3D11_TEXTURE_ADDRESS_MODE.Wrap;

                device.CreateSamplerState(sd, out sampler).ThrowExceptionIfError();

                return;
            }

            outputTexture = null;
            srv = null;
            sampler = null;
        }
    }
}
