using UnityEngine;

namespace Core.Chunks
{
    public struct KeyIndex
    {
        public int X;
        public int Z;
        
        public KeyIndex(int x, int z)
        {
            X = x;
            Z = z;
        }
        
        public static KeyIndex ToChunkCoordinates(int globalX, int globalZ)
        {
            int chunkX = (int)Mathf.Floor(globalX / Chunk.Scale);
            int chunkZ = (int)Mathf.Floor(globalZ / Chunk.Scale);

            return new KeyIndex(chunkX, chunkZ);
        }

        public override bool Equals(object other)
        {
            if (!(other is KeyIndex)) return false;
            return Equals((KeyIndex)other);
        }
        
        public bool Equals(KeyIndex other)
        {
            return X == other.X && Z == other.Z;
        }
        
        public override int GetHashCode()
        {
            return X.GetHashCode() ^ (Z.GetHashCode() << 2);
        }
    }
}