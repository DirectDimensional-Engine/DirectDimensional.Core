using System;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using DirectDimensional.Bindings.Direct3D11;
using DirectDimensional.Bindings;
using DirectDimensional.Core.Utilities;

using D3D11Buffer = DirectDimensional.Bindings.Direct3D11.Buffer;

namespace DirectDimensional.Core {
    public sealed unsafe class Mesh : DDObjects {
        private ComArray<D3D11Buffer>? _vbs;
        private D3D11Buffer? _ib;

        internal ComArray<D3D11Buffer>? VertexBuffers => _vbs;
        internal D3D11Buffer? IndexBuffer => _ib;

        uint vertexCount, indexCount;

        public uint VertexCount => vertexCount;
        public uint IndexCount => indexCount;

        public Mesh() {
            _vbs = new(1);
        }

        ~Mesh() {
            _vbs?.TrueDispose();
            _ib.CheckAndRelease();
        }

        public void SetVertices(List<Vertex>? vertices, BufferResizingMethod resizeMethod = BufferResizingMethod.Fit) {
            if (_vbs == null) return;

            if (vertices == null || vertices.Count == 0) {
                _vbs[0].CheckAndRelease();

                vertexCount = 0;
                return;
            }

            var device = Direct3DContext.Device;
            var devctx = Direct3DContext.DevCtx;

            Span<Vertex> span = CollectionsMarshal.AsSpan(vertices);

            if (!_vbs[0].Alive()) {
                vertexCount = (uint)vertices.Count;
                device.CreateVertexBuffer(span, true, out var _vb).ThrowExceptionIfError();
                _vbs[0] = _vb;

                return;
            }

            var desc = _vbs[0]!.Description;
            int oldLength = (int)(desc.ByteWidth / Vertex.MemorySize);

            if (vertices.Count <= oldLength) {
                vertexCount = (uint)vertices.Count;

                D3D11_MAPPED_SUBRESOURCE msr;
                devctx.Map(_vbs[0]!, 0u, D3D11_MAP.WriteDiscard, &msr);

                fixed (Vertex* pVertex = span) {
                    Unsafe.CopyBlock(msr.pData.ToPointer(), pVertex, desc.ByteWidth);
                }

                devctx.Unmap(_vbs[0]!, 0u);
            } else {
                _vbs[0].CheckAndRelease();

                switch (resizeMethod) {
                    default: {
                        vertexCount = (uint)vertices.Count;
                        device.CreateVertexBuffer(span, true, out var _vb).ThrowExceptionIfError();
                        _vbs[0] = _vb;
                        break;
                    }

                    case BufferResizingMethod.Double: {
                        while (oldLength < vertices.Count) oldLength *= 2;
                        desc.ByteWidth = (uint)(oldLength * Vertex.MemorySize);

                        fixed (Vertex* pVertex = span) {
                            D3D11_SUBRESOURCE_DATA srd = default;
                            srd.pData = new IntPtr(pVertex);

                            vertexCount = (uint)vertices.Count;
                            device.CreateBuffer(desc, &srd, out var _vb).ThrowExceptionIfError();
                            _vbs[0] = _vb;
                        }
                        break;
                    }
                }
            }
        }

        public void SetIndices(List<ushort>? indices, BufferResizingMethod resizeMethod = BufferResizingMethod.Fit) {
            if (indices == null || indices.Count == 0) {
                _ib.CheckAndRelease();
                indexCount = 0;

                return;
            }

            var device = Direct3DContext.Device;
            var devctx = Direct3DContext.DevCtx;

            var span = CollectionsMarshal.AsSpan(indices);

            if (!_ib.Alive()) {
                indexCount = (uint)indices.Count;
                device.CreateIndexBuffer(span, true, out _ib).ThrowExceptionIfError();
                return;
            }

            var desc = _ib.Description;
            int oldLength = (int)(desc.ByteWidth / sizeof(ushort));

            if (indices.Count <= oldLength) {
                indexCount = (uint)indices.Count;

                D3D11_MAPPED_SUBRESOURCE msr;
                devctx.Map(_ib, 0u, D3D11_MAP.WriteDiscard, &msr);

                fixed (ushort* pIndex = span) {
                    Unsafe.CopyBlock(msr.pData.ToPointer(), pIndex, desc.ByteWidth);
                }

                devctx.Unmap(_ib, 0u);
            } else {
                _ib.CheckAndRelease();

                switch (resizeMethod) {
                    default:
                        indexCount = (uint)indices.Count;
                        device.CreateIndexBuffer(span, true, out _ib).ThrowExceptionIfError();
                        break;

                    case BufferResizingMethod.Double: {
                        while (oldLength < indices.Count) oldLength *= 2;
                        desc.ByteWidth = (uint)(oldLength * sizeof(ushort));

                        fixed (ushort* pVertex = span) {
                            D3D11_SUBRESOURCE_DATA srd = default;
                            srd.pData = new IntPtr(pVertex);

                            indexCount = (uint)indices.Count;
                            device.CreateBuffer(desc, &srd, out _ib).ThrowExceptionIfError();
                        }
                        break;
                    }
                }
            }
        }

        public override bool Alive() {
            return _vbs != null && _vbs[0].Alive() && _ib.Alive();
        }

        public override void Destroy() {
            _vbs?.TrueDispose();
            _ib.CheckAndRelease();

            _vbs = null;
            _ib = null;

            vertexCount = indexCount = 0;
        }
    }
}
