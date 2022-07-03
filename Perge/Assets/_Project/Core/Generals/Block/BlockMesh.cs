using UnityEngine;

namespace Core.Generals
{
    public abstract class BlockMesh : Block
    {
        private Mesh _mesh;
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _uvs;
        private int[] _triangles;
        public override void Init(ushort blockId)
        {
            base.Init(blockId);
            _mesh = GetLoadingMesh();

            _vertices = _mesh.vertices;
            _normals = _mesh.normals;
            _uvs = _mesh.uv;
            _triangles = _mesh.triangles;
        }
        
        public abstract Mesh GetLoadingMesh();
        
        
        public Vector3[] GetVertices()
        {
            return _vertices;
        }
        
        public Vector3[] GetNormals()
        {
            return _normals;
        }
        
        public Vector2[] GetUvs()
        {
            return _uvs;
        }

        public int[] GetTriangles()
        {
            return _triangles;
        }
    }
}