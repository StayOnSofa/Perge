using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public class BirchLog : Block
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/BirchLogTop");
            yield return Resources.Load<Texture2D>("Blocks/BirchLog");
        }
        
        public override Rect GetTextureOffset(BlockSide side)
        {
            if (side == BlockSide.YMinus)
            {
                return _textureOffset[0];
            }

            if (side == BlockSide.YPlus)
            {
                return _textureOffset[0];
            }

            return _textureOffset[1];
        }
    }
}