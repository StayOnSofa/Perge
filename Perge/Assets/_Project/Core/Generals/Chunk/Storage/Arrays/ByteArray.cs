using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Chunks.Storage.Arrays
{
    public class ByteArray : ICloneable, IEnumerable, IStorageArray
    {
        public byte[] RawData { private set; get; }

        private PalleteStorage _palleteStorage;
        
        public byte this[int index] { get => _get(index);
            set => _set(index, value);
        }

        private byte _get(int index )
        {
            return RawData[index];
        }

        private void _set(int index, byte value)
        {
            RawData[index] = value;
        }

        public ByteArray(int scale, PalleteStorage palleteStorage)
        {
            RawData = new byte[scale];
            _palleteStorage = palleteStorage;
        }

        public ByteArray(byte[] rawArray)
        {
            RawData = rawArray;
        }

        public object Clone()
        {
            return new ByteArray((byte[])RawData.Clone());
        }

        public IEnumerator GetEnumerator()
        {
            return RawData.GetEnumerator();
        }

        public byte[] GetRaw()
        {
            return RawData;
        }

        public byte RawGet(int index)
        {
            return RawData[index];
        }

        public void RawSet(int index, byte _byte)
        {
            _set(index, _byte);
        }

        public ushort Get(int index)
        {
            return _palleteStorage.GetBlockValue(RawData[index]);
        }

        public int Set(int index, ushort block)
        {
            RawData[index] = _palleteStorage.GetLocalValue(block);
            
            return _palleteStorage.Count;
        }
    }
}