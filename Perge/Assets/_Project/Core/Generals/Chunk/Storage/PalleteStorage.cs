using System;
using System.Collections.Generic;
using Core.PackageUtils;

namespace Core.Chunks.Storage
{
    public class PalleteStorage
    {
        private const byte ByteMask = 0xFF;
        public ushort AIR = 0;

        private Dictionary<byte, ushort> _locals;
        private Dictionary<ushort, byte> _blocks;
        
        private ushort GetShort(byte a, byte b)
        {
            return (ushort)((b << 8) | a);
        }

        public Dictionary<byte, ushort> GetLocals()
        {
            return _locals;
        }

        public Dictionary<ushort, byte> GetBlocks()
        {
            return _blocks;
        }
        public PalleteStorage()
        {
            _locals = new Dictionary<byte, ushort>(256);
            _blocks = new Dictionary<ushort, byte>(256);

            GetLocalValue(AIR);
        }

        public PalleteStorage(BlockStream stream)
        {
            _locals = new Dictionary<byte, ushort>(256);
            _blocks = new Dictionary<ushort, byte>(256);
            
            byte count = (byte)stream.ReadByte();

            for (byte i = 0; i < count; i++)
            {
                ushort _short = GetShort((byte)stream.ReadByte(), (byte)stream.ReadByte());
                
                _blocks.Add(_short, i);
                _locals.Add(i, _short);
            }
        }
                public void Write(BlockStream stream)
        {
            stream.WriteByte((byte)Count);
            
            foreach (ushort value in _locals.Values)
            {
                byte byte1 = (byte)((value >> 8) & ByteMask);
                byte byte2 = (byte)(value & ByteMask);
                
                stream.WriteByte(byte2);
                stream.WriteByte(byte1);
            }
        }

        public int BitsPerBlock()
        {
            if (Count < 16)
                return 4;

            if (Count < 256)
                return 8;

            return 16;
        }

        public int Count => _locals.Count;
        
        public byte GetLocalValue(ushort blockID)
        {
            if (_blocks.TryGetValue(blockID, out byte value))
            {
                return value;
            }

            byte id = (byte)Count;
            
            _blocks.Add(blockID, id);
            _locals.Add(id, blockID);

            return id;
        }

        public ushort GetBlockValue(byte local)
        {
            if (_locals.TryGetValue(local, out ushort block))
            {
                return block;
            }

            throw new IndexOutOfRangeException();
        }
        
    }
}