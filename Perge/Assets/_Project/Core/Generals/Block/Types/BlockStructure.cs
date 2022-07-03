using System.Collections.Generic;
using Core.World;
using UnityEngine;

namespace Core.Generals
{
    public abstract class BlockStructure : Block
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/Structure");
        }
        public abstract void PushStructure(IWorld world, int x, int y, int z);
    }
}