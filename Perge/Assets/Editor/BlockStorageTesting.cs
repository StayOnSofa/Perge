using Core.Chunks;
using Core.Chunks.Storage;
using Core.PackageUtils;
using NUnit.Framework;
using UnityEngine;

public class BlockStorageTesting
{
    [Test]
    public void TestingPalleteStorage()
    {
        PalleteStorage palleteStorage = new PalleteStorage();

        int random = Random.Range(0, 255);
        
        for (ushort i = 0; i < random; i++)
        {
            palleteStorage.GetLocalValue(i);
        }

        byte[] bytes;
        
        using (var memoryStream = new BlockStream(PackageType.ChunkUpload))
        {
            palleteStorage.Write(memoryStream);
            bytes = memoryStream.ToArray();
        }

        PalleteStorage restored;
        
        using (var memoryStream = new BlockStream(bytes))
        {
            restored = new PalleteStorage(memoryStream);
        }
        
        Assert.AreEqual(restored.GetLocals(), palleteStorage.GetLocals());
        Assert.AreEqual(restored.GetBlocks(), palleteStorage.GetBlocks());
    }


    [Test]
    public void TestingBlockStream()
    {
        bool result = true;   
        
        int[] randomValues = new int[512];

        for (int i = 0; i < randomValues.Length; i++)
        {
            randomValues[i] = Random.Range(0, 512);
        }
    
        byte[] bytes;
        
        using (var blockStream = new BlockStream(PackageType.ChunkUpload))
        {
            foreach (int i in randomValues)
            {
                blockStream.WriteInt(i);
            }

            bytes = blockStream.ToArray();
        }
        
        using (var blockStream = new BlockStream(bytes))
        {
            foreach (int i in randomValues)
            {
                if (i != blockStream.ReadInt())
                {
                    result = false;
                }
            }
        }
        
        Assert.IsTrue(result);
    }


    private void FillStorageArray(BlockStorage blockStorage, int uniqueIds = 256)
    {
        int scale = Chunk.Scale;
        
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    ushort value = (ushort)Random.Range(0, uniqueIds);
                    blockStorage.SetBlock(value, x, y, z);
                }   
            }
        }
    }

    private bool _blockStorageEquals(BlockStorage first, BlockStorage second)
    {
        int scale = Chunk.Scale;
        
        for (int x = 0; x < scale; x++)
        {
            for (int y = 0; y < scale; y++)
            {
                for (int z = 0; z < scale; z++)
                {
                    if (first.GetBlock(x, y, z) != second.GetBlock(x, y, z))
                    {
                        return false;
                    }
                }   
            }
        }

        return true;
    }

    [Test]
    public void BlockStorageEquals()
    {
        bool result = true;
        
        BlockStorage blockStorage = new BlockStorage();
        FillStorageArray(blockStorage, 14);

        Debug.Log("Bits: " + blockStorage.BitsPerBlock());
        
        byte[] bytes;
        
        using (BlockStream memoryStream = new BlockStream(PackageType.ChunkUpload))
        {
            blockStorage.Write(memoryStream);
            bytes = memoryStream.ToArray();
        }

        using (BlockStream memoryStream = new BlockStream(bytes))
        {
            BlockStorage restored = new BlockStorage(memoryStream);
            result = _blockStorageEquals(restored, blockStorage);
        }

        Assert.IsTrue(result);
    }
}
