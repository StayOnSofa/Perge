using Core.Chunks;
using Core.Generals;
using UnityEngine;

namespace Core.Graphics
{
    public class MeshChunk : ExpandableMesh
    {
        private static MeshBlockSide _xPlus;
        private static MeshBlockSide _xMinus;
        private static MeshBlockSide _yPlus;
        private static MeshBlockSide _yMinus;
        private static MeshBlockSide _zPlus;
        private static MeshBlockSide _zMinus;
        
        private static bool _registered = false;
        
        private static void RegisterBlockSides()
        {
            if (_registered == false)
            {
                _xPlus = BlockSidePlusX.GetMesh();
                _xMinus = BlockSideMinusX.GetMesh();

                _yPlus = BlockSidePlusY.GetMesh();
                _yMinus = BlockSideMinusY.GetMesh();

                _zPlus = BlockSidePlusZ.GetMesh();
                _zMinus = BlockSideMinusZ.GetMesh();

                _registered = true;
            }
        }

        private Chunk _chunk;

        public MeshChunk(Chunk chunk)
        {
            RegisterBlockSides();
            _chunk = chunk;
        }

        public void Build(int layer)
        {
            Clear();
            
            for (int x = 0; x < Chunk.Scale; x++)
            {
                int sLayer = layer * Chunk.Scale;
                int eLayer = sLayer + Chunk.Scale;

                for (int y = sLayer; y < eLayer; y++)
                {
                    int _y = y - sLayer;

                    for (int z = 0; z < Chunk.Scale; z++)
                    {
                        ushort blockId = _chunk.GetBlock(x, y, z);

                        if (blockId != 0)
                        {
                            Block block = BlockRegister.GetBlock(blockId);

                            if (!BlockRegister.IsSpecialBlock(blockId))
                            {
                                int _plusX = x + 1;
                                if (IsAirOrSpecial(_plusX, y, z))
                                {
                                    AddSide(BlockSide.XPlus, block, new Vector3(x, _y, z));
                                }

                                int _minusX = x - 1;
                                if (IsAirOrSpecial(_minusX, y, z))
                                {
                                    AddSide(BlockSide.XMinus, block, new Vector3(x, _y, z));
                                }

                                int _plusZ = z + 1;
                                if (IsAirOrSpecial(x, y, _plusZ))
                                {
                                    AddSide(BlockSide.ZPlus, block, new Vector3(x, _y, z));
                                }

                                int _minusZ = z - 1;
                                if (IsAirOrSpecial(x, y, _minusZ))
                                {
                                    AddSide(BlockSide.ZMinus, block, new Vector3(x, _y, z));
                                }

                                int _plusY = y + 1;
                                if (IsAirOrSpecial(x, _plusY, z))
                                {
                                    AddSide(BlockSide.YPlus, block, new Vector3(x, _y, z));
                                }

                                int _minusY = y - 1;
                                if (IsAirOrSpecial(x, _minusY, z))
                                {
                                    AddSide(BlockSide.YMinus, block, new Vector3(x, _y, z));
                                }
                            }
                            else
                            {
                                AddMesh((BlockMesh)block, new Vector3(x, _y, z));
                            }
                        }
                    }
                }
            }
        }
        
        private bool IsAirOrSpecial(int x, int y, int z)
        {
            ushort blockId = _chunk.GetBlock(x, y, z);
            return BlockRegister.IsAirOrSpecialBlock(blockId);
        }

        public void AddMesh(BlockMesh block, Vector3 position)
        {
            Rect textureCoordinates = block.GetTextureOffset(BlockSide.YPlus);
            AddMesh(block.GetVertices(), block.GetNormals(), block.GetUvs(), block.GetTriangles(), textureCoordinates, position);
        }

        public void AddSide(BlockSide side, Block block, Vector3 position)
        {
            Rect textureCoordinates = block.GetTextureOffset(side);

            switch (side)
            {
                case BlockSide.XMinus:
                    AddSide(textureCoordinates, position, _xMinus.Vertices, _xMinus.Uvs, _xMinus.Normal, _xMinus.Triangles);
                    break;
                case BlockSide.XPlus:
                    AddSide(textureCoordinates, position, _xPlus.Vertices, _xPlus.Uvs, _xPlus.Normal, _xPlus.Triangles);
                    break;
                case BlockSide.ZPlus:
                    AddSide(textureCoordinates, position, _yMinus.Vertices, _yMinus.Uvs, _yMinus.Normal, _yMinus.Triangles);
                    break;
                case BlockSide.ZMinus:
                    AddSide(textureCoordinates, position, _yPlus.Vertices, _yPlus.Uvs, _yPlus.Normal, _yPlus.Triangles);
                    break;
                case BlockSide.YMinus:
                    AddSide(textureCoordinates, position, _zMinus.Vertices, _zMinus.Uvs, _zMinus.Normal, _zMinus.Triangles);
                    break;
                case BlockSide.YPlus:
                    AddSide(textureCoordinates, position, _zPlus.Vertices, _zPlus.Uvs, _zPlus.Normal, _zPlus.Triangles);
                    break;
            }
        }
    }
}