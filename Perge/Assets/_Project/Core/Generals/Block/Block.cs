using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public abstract class Block
    {
        private ushort _blockID;
        
        protected Rect[] _textureOffset;

        public Rect[] GetTextureRects()
        {
            return _textureOffset;
        }

        public void Init(ushort blockId)
        {
            _blockID = blockId;
        }

        public virtual void Init(Rect[] rect)
        {
            _textureOffset = rect;
        }

        public ushort GetBlockID()
        {
            return _blockID;
        }

        public virtual Rect GetTextureOffset(BlockSide side)
        {
            return _textureOffset[0];
        }

        public abstract IEnumerable<Texture2D> GetTextures();
    }
}