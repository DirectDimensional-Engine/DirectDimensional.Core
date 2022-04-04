namespace DirectDimensional.Core {
    [Flags]
    public enum TextureFlags {
        None                = 0,
        
        Render              = 1 << 0,

        /// <summary>
        /// Allow Write if Renderable
        /// </summary>
        Write               = 1 << 1,
    }
}
