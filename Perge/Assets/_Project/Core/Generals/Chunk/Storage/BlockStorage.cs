using System.IO;
using Core.Chunks.Storage.Arrays;
using Core.PackageUtils;
using Packing;

namespace Core.Chunks.Storage
{
    public class BlockStorage
    {
        private const int Scale = Chunk.Scale;
        private const int StorageScale = Scale * Scale * Scale;

        private IStorageArray _storageArray;
        private PalleteStorage _palleteStorage;

        private int _bitsPerBlock = 4;
        
        public BlockStorage()
        {
            _palleteStorage = new PalleteStorage();
            _storageArray = new NibbleArray(StorageScale, _palleteStorage);
    
            _bitsPerBlock = 4;
        }

        public BlockStorage(BlockStream stream)
        {
            var bytes = stream.ReadBytes();
            bytes = Deflate.Decompress(bytes);

            using (BlockStream blockStream = new BlockStream(bytes))
            {
                int bitsPerBlock = blockStream.ReadInt();

                IStorageArray storage = null;

                if (bitsPerBlock <= 8)
                {
                    _palleteStorage = new PalleteStorage(blockStream);
                }

                if (bitsPerBlock == 4)
                    storage = new NibbleArray(StorageScale, _palleteStorage);

                if (bitsPerBlock == 8)
                    storage = new ByteArray(StorageScale, _palleteStorage);

                if (bitsPerBlock == 16)
                    storage = new UshortArray(StorageScale);

                int count = blockStream.ReadInt();

                for (int i = 0; i < count; i++)
                {
                    storage.RawSet(i, (byte) blockStream.ReadByte());
                }

                _storageArray = storage;
            }
        }

        public IStorageArray GetStorage()
        {
            return _storageArray;
        }

        public void Write(BlockStream stream)
        {
            using (var blockStorage = new BlockStream(PackageType.Empty))
            {
                blockStorage.WriteInt(_bitsPerBlock);
                if (_bitsPerBlock <= 8)
                {
                    _palleteStorage.Write(blockStorage);
                }

                blockStorage.WriteInt(_storageArray.GetRaw().Length);

                for (int i = 0; i < _storageArray.GetRaw().Length; i++)
                {
                    blockStorage.WriteByte(_storageArray.GetRaw()[i]);
                }

                var bytes = blockStorage.ToArray();
                bytes = Deflate.Compress(bytes);
                
                stream.WriteBytes(bytes);
            }
        }
        
        private static int Index(int x, int y, int z)
        {
            return (x * Scale * Scale) + (y * Scale) + z;
        }

        public ushort GetBlock(int x, int y, int z)
        {
            return _storageArray.Get(Index(x, y, z));
        }

        public int BitsPerBlock()
        {
            return _bitsPerBlock;
        }

        public int PalleteCount()
        {
            return _palleteStorage.Count;
        }

        public void SetBlock(ushort blockID, int x, int y, int z)
        {
            int count = _storageArray.Set(Index(x,y,z), blockID);
            
            if (_bitsPerBlock <= 8)
            {
                if (_bitsPerBlock == 4)
                {
                    if (count == 15)
                    {
                        ByteArray byteArray = new ByteArray(StorageScale, _palleteStorage);

                        for (int i = 0; i < StorageScale; i++)
                        {
                            byteArray[i] = _storageArray.RawGet(i);
                        }

                        _storageArray = byteArray;
                        _bitsPerBlock = 8;
                    }
                    
                    return;
                }
                
                if (count == 255)
                {
                    UshortArray ushortArray = new UshortArray(StorageScale);

                    for (int i = 0; i < StorageScale; i++)
                    {
                        ushortArray[i] = _storageArray.Get(i);
                    }

                    _storageArray = ushortArray;
                    _bitsPerBlock = 16;
                }
            }
        }
    }
}