namespace DirectDimensional.Core.Miscs {
    internal static class RenderingConstants {
        public const int SystemBufferCount = 5;

        public const string PerFrameBufferName = "_PerFrameBuffer";
        public const string PerCameraBufferName = "_PerCameraBuffer";
        public const string PerObjectBuiltInName = "_PerObjectBIBuffer";
        public const string PerObjectName = "_PerObjectBuffer";
        public const string PerInstanceName = "_PerInstanceBuffer";

        public const string MainTextureName = "_MainTex";

        public const int PerFrameBufferIndex = 0;
        public const int PerCameraBufferIndex = 1;
        public const int PerObjectBuiltInIndex = 2;
        public const int PerObjectIndex = 3;
        public const int PerInstanceIndex = 4;
    }
}
