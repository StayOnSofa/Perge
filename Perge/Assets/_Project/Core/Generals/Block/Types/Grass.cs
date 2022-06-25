using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public class Grass : Block
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/Grass");
        }
    }
}