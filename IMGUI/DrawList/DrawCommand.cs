﻿namespace ImGui
{
    struct DrawCommand
    {
        int elemCount;
        Rect clipRect;
        Texture textureData;

        public static DrawCommand Default = new DrawCommand
        {
            elemCount = 0,
            clipRect = new Rect(-8192.0f, -8192.0f, 8192.0f, 8192.0f),
            textureData = null
        };

        /// <summary>
        /// Number of indices (multiple of 3) to be rendered as triangles. Vertices are stored in the callee DrawList's vtx_buffer[] array, indices in idx_buffer[].
        /// </summary>
        /// <remarks>Added when calling <see cref="ImGui.DrawBuffer.PrimReserve"/></remarks>
        public int ElemCount
        {
            get { return elemCount; }
            set { elemCount = value; }
        }

        /// <summary>
        /// Clipping rectangle
        /// </summary>
        public Rect ClipRect
        {
            get { return clipRect; }
            set { clipRect = value; }
        }

        /// <summary>
        /// texture data
        /// </summary>
        public Texture TextureData
        {
            get { return textureData; }
            set { textureData = value; }
        }
    }
}
