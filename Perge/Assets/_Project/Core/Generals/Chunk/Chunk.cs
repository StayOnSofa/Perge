using System;
using System.Collections.Generic;
using Core.Biomes;
using Core.Chunks.Storage;
using Core.PackageUtils;
using Core.World;

namespace Core.Chunks
{
    public class Chunk : IObserverChunk
    {
        public Action OnUpdate;
        
        public enum Heighbour : int
        {
            XPlus = 0,
            XMinus = 1,
            ZPlus = 2,
            ZMinus = 3,
        }

        public const int Height = 256;
        public const int Scale = 32;

        public int NeighboursCount {private set; get;}
        
        public KeyIndex KeyIndex { private set; get; }
        
        private BlockStorage[] _slices = new BlockStorage[(Height/Scale)];

        private List<ISubscriberChunk> _subscribers 
            = new List<ISubscriberChunk>();

        private Chunk[] _heighbours 
            = new Chunk[4];

        private bool _isDestroyed = false;
        
        private IWorld _world;
        
        public Chunk(IWorld world, KeyIndex index, BiomeHandler handler)
        {
            for (int i = 0; i < _slices.Length; i++)
            {
                _slices[i] = new BlockStorage();
            }
            
            _world = world;
            NeighboursCount = 0;
            
            KeyIndex = index;
            Build(handler);

            if (world != null)
            {
                UpdateHeighbour();
            }
        }
        
        public Chunk(KeyIndex index, BiomeHandler handler)
        {
            for (int i = 0; i < _slices.Length; i++)
            {
                _slices[i] = new BlockStorage();
            }
            
            NeighboursCount = 0;
            
            KeyIndex = index;
            Build(handler);
        }
        
        public Chunk(IWorld world, BlockStream stream)
        {
            byte[] bytes =
            {
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
            };

            int x = BitConverter.ToInt32(bytes);
            
            byte[] bytes1 =
            {
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
                (byte)stream.ReadByte(),
            };

            int z = BitConverter.ToInt32(bytes1);
            
            for (int i = 0; i < _slices.Length; i++)
            {
                _slices[i] = new BlockStorage(stream);
            }
            
            _world = world;
            NeighboursCount = 0;
            
            KeyIndex = new KeyIndex(x, z);
            UpdateHeighbour();
        }
        
        public void UpdateHeighbour()
        {
            var index = KeyIndex;
            
            var chunk = _world.GetChunk(index.X + 1, index.Z);
            if (chunk != null)
            {
                AddHeighbour(Heighbour.XPlus, chunk);
                if (chunk.AddHeighbour(Heighbour.XMinus, this))
                    chunk.Update();
            }
            
            chunk = _world.GetChunk(index.X - 1, index.Z);
            if (chunk != null)
            {
                AddHeighbour(Heighbour.XMinus, chunk);
                if (chunk.AddHeighbour(Heighbour.XPlus, this))
                    chunk.Update();
            }
            
            chunk = _world.GetChunk(index.X, index.Z + 1);
            if (chunk != null)
            {
                AddHeighbour(Heighbour.ZPlus, chunk);
                if (chunk.AddHeighbour(Heighbour.ZMinus, this))
                    chunk.Update();
            }
            
            chunk = _world.GetChunk(index.X, index.Z - 1);
            if (chunk != null)
            {
                AddHeighbour(Heighbour.ZMinus, chunk);
                if (chunk.AddHeighbour(Heighbour.ZPlus, this))
                    chunk.Update();
            }
        }

        public bool AddHeighbour(Heighbour side, Chunk chunk)
        {
            int index = (int) side;

            if (_heighbours[index] == null)
            {
                _heighbours[index] = chunk;
                NeighboursCount += 1;

                return true;
            }

            return false;
        }

        public void Update()
        {
            OnUpdate?.Invoke();
        }

        private void Build(BiomeHandler handler)
        {
            for (int x = 0; x < Scale; x++)
            {
                for (int z = 0; z < Scale; z++)
                {
                    int height = handler.GetMixedHeight(x + KeyIndex.X * Scale, z + KeyIndex.Z * Scale);

                    for (int y = 0; y <= height; y++)
                    {
                        PlaceBlock(1, x, y, z);
                    }
                }
            }
        }

        private bool InBorder(int value)
        {
            return (value >= 0 && value < Scale);
        }

        private bool InBorder(int x, int y, int z)
        {
            return InBorder(x) && InBorder(z) && y >= 0 && y < Height;
        }
        
        public ushort GetBlock(int x, int y, int z)
        {
            if (InBorder(x, y, z))
            {
                int slice = y / Scale;
                int _y = y % Scale;
                
                return _slices[slice].GetBlock(x, _y, z);
            }
            
            {
                if (x == Scale)
                {
                    var chunk = _heighbours[(int) Heighbour.XPlus];
                    if (chunk != null)
                    {
                        return chunk.GetBlock(0, y, z);
                    }
                }

                if (x == -1)
                {
                    var chunk = _heighbours[(int) Heighbour.XMinus];
                    if (chunk != null)
                    {
                        return chunk.GetBlock(Scale - 1, y, z);
                    }
                }
                
                if (z == Scale)
                {
                    var chunk = _heighbours[(int) Heighbour.ZPlus];
                    if (chunk != null)
                    {
                        return chunk.GetBlock(x, y, 0);
                    }
                }

                if (z == -1)
                {
                    var chunk = _heighbours[(int) Heighbour.ZMinus];
                    if (chunk != null)
                    {
                        return chunk.GetBlock(x, y, Scale - 1);
                    }
                }
            }

            return 0;
        }

        public void BreakBlock(int x, int y, int z)
        {
            if (InBorder(x, y, z))
            {
                int slice = y / Scale;
                int _y = y % Scale;
                
                _slices[slice].SetBlock(0, x, _y, z);
                UpdateLayers(y);
            }
        }

        public void PlaceBlock(ushort blockId, int x, int y, int z)
        {
            if (InBorder(x, y, z))
            {
                int slice = y / Scale;
                int _y = y % Scale;
                
                if (_slices[slice].GetBlock(x, _y, z) == 0)
                {
                    _slices[slice].SetBlock(blockId, x, _y, z);
                    UpdateLayers(y);
                }
            }
        }

        public void UpdateLayers(int y)
        {
            foreach (var sub in _subscribers)
            {
                sub.UpdateLayer(y);
            }
        }

        public void Subscribe(ISubscriberChunk subscriber)
        {
            if (!_subscribers.Contains(subscriber))
            {
                _subscribers.Add(subscriber);
            }
        }

        public void Unsubscribe(ISubscriberChunk subscriber)
        {
            if (_subscribers.Contains(subscriber))
            {
                _subscribers.Remove(subscriber);
            }
        }
        
        public void Write(BlockStream stream)
        {
            byte[] bytes = BitConverter.GetBytes(KeyIndex.X);

            for (int i = 0; i < bytes.Length; i++)
            {
                stream.WriteByte(bytes[i]);
            }
            
            bytes = BitConverter.GetBytes(KeyIndex.Z);

            for (int i = 0; i < bytes.Length; i++)
            {
                stream.WriteByte(bytes[i]);
            }
            
            for (int i = 0; i < _slices.Length; i++)
            {
                _slices[i].Write(stream);
            }
        }

        public void Destroy()
        {
            _isDestroyed = true;
        }

        public bool IsDestroyed()
        {
            return _isDestroyed;
        }
    }
}