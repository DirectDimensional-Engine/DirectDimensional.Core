using System;
using System.Runtime.InteropServices;
using System.Numerics;

namespace DirectDimensional.Core {
    public struct Vertex {
        public static readonly int MemorySize = Marshal.SizeOf<Vertex>();

        public Vector3 Position { get; set; }
        public Color32 Color { get; set; }
        public Vector2 TexCoord { get; set; }

        public Vector3 Normal { get; set; }
        public Vector3 Tangent { get; set; }
        public Vector3 Bitangent { get; set; }

        public Vertex(Vector3 position, Color32 color, Vector2 uv) {
            Position = position;
            Color = color;
            TexCoord = uv;
            Normal = -Vector3.UnitZ;
            Tangent = Vector3.UnitX;
            Bitangent = Vector3.UnitY;
        }

        public Vertex(Vector3 position, Color32 color, Vector2 uv, Vector3 normal, Vector3 tangent, Vector3 bitangent) {
            Position = position;
            Color = color;
            TexCoord = uv;
            Normal = normal;
            Tangent = tangent;
            Bitangent = bitangent;
        }
    }
}
