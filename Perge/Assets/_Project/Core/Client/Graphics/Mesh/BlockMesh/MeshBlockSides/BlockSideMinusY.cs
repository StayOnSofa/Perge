using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Graphics
{
    public class BlockSideMinusY
    {
        public static MeshBlockSide GetMesh()
        {
            Vector3[] verices = {
                new Vector3(1.000000f, 1.000000f, 1.000000f),
                    new Vector3(1.000000f, -1.000000f, 1.000000f),
                    new Vector3(-1.000000f, 1.000000f, 1.000000f),
                    new Vector3(-1.000000f, -1.000000f, 1.000000f),
                };

            Vector2[] uvs = new Vector2[4]
         {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
         };

            Vector3[] triangles = {
                    new Vector3(1 - 1, 1 - 1, 1 - 1), new Vector3(4 - 1,2 - 1,1 - 1),  new Vector3(2 - 1,3 - 1,1 - 1),
                    new Vector3(1 - 1,1 - 1,1 - 1), new Vector3(3 - 1,4 - 1,1 - 1), new Vector3(4 - 1,2 - 1,1 - 1),
                };


            Vector3 normal = new Vector3(0.0000f, 0.0000f, 1.0000f);

            return new MeshBlockSide(verices, uvs, triangles, normal);
        }
    }
}