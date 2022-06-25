using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Graphics
{
    public class MeshBlockSide
    {
        public Vector3[] Vertices { get; private set; } = new Vector3[4];
        public Vector2[] Uvs { get; private set; } = new Vector2[4];
        public Vector3[] Triangles { get; private set; } = new Vector3[6];

        public Vector3 Normal;
        public MeshBlockSide(Vector3[] vertices, Vector2[] uvs, Vector3[] triangles, Vector3 normal)
        {
            Vertices = vertices;
            Uvs = uvs;
            Triangles = triangles;
            Normal = normal;
        }
    }
}
