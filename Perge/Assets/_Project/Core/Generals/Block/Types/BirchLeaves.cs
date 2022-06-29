using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public class BirchLeaves : Block
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/Leaves");
        }
    }
}