using System;
using System.Collections;

namespace Core.Chunks.Storage.Arrays
{
    public class UshortArray : ICloneable, IEnumerable, IStorageArray
    {
        private const byte ByteMask = 0xFF;
        public byte[] RawData { private set; get; }

        public UshortArray(byte[] rawData)
        {
            RawData = rawData;
        }
        
        public UshortArray(int scale)
        {
            RawData = new byte[scale * 2];
        }
        
        public int Count => RawData.Length/2;
        public int ByteSize => RawData.Length;
        
        public ushort this[int index] { get => Get(index);
            set => Set(index, value);
        }
        
        private ushort GetShort(byte a, byte b)
        {
            return (ushort)((b << 8) | a);
        }

        public byte[] GetRaw()
        {
            return RawData;
        }

        public byte RawGet(int index)
        {
            return 0;
        }

        public void RawSet(int index, byte _byte)
        {
            RawData[index] = _byte;
        }

        public ushort Get(int index)
        {
            index *= 2;
            return GetShort(RawData[index], RawData[index + 1]);
        }

        public int Set(int index, ushort value)
        {
            index *= 2;
            
            byte byte1 = (byte)((value >> 8) & ByteMask);
            byte byte2 = (byte)(value & ByteMask);

            RawData[index] = byte2;
            RawData[index + 1] = byte1;

            return 0;
        }

        public object Clone()
        {
            return new UshortArray((byte[])RawData.Clone());
        }

        public IEnumerator GetEnumerator()
        {
            return RawData.GetEnumerator();
        }
    }
}