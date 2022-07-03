using System.Collections.Generic;
using UnityEngine;

namespace Core.Generals
{
    public class GrassPlant : BlockMesh
    {
        public override IEnumerable<Texture2D> GetTextures()
        {
            yield return Resources.Load<Texture2D>("Blocks/Grass");
        }

        public override Mesh GetLoadingMesh()
        {
            return Resources.Load<Mesh>("Mesh/Grass");
        }
    }
}