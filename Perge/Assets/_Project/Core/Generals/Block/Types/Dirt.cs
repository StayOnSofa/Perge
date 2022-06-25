using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public class Dirt : Block
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/Dirt");
            yield return Resources.Load<Texture2D>("Blocks/Grass");
            yield return Resources.Load<Texture2D>("Blocks/GrassSide");
        }
        
        public override Rect GetTextureOffset(BlockSide side)
        {
            if (side == BlockSide.YMinus)
            {
                return _textureOffset[0];
            }

            if (side == BlockSide.YPlus)
            {
                return _textureOffset[1];
            }

            return _textureOffset[2];
        }
    }
}