using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Chunks.Storage.Arrays
{
    public class NibbleArray : ICloneable, IEnumerable, IStorageArray
    {
        private List<int> _list;
        public byte[] RawData { get; private set; }
       
        public byte this[int index] { get => _get(index);
            set => _set(index, value);
        }

        private PalleteStorage _palleteStorage;
        
        public NibbleArray(int size, PalleteStorage palleteStorage)
        {
            RawData = new byte[size / 2];
            _palleteStorage = palleteStorage;
        }
    
        public NibbleArray(int size, byte value, PalleteStorage palleteStorage) {
            RawData = new byte[size / 2];
            if (value != 0) {
                Fill(value);
            }

            _palleteStorage = palleteStorage;
        }
    
        public NibbleArray(byte[] rawData) {
            RawData = rawData;
        }
    
        public int Count => 2 * RawData.Length;
        public int ByteSize => RawData.Length;

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
            RawData[index] = _byte;
        }

        public ushort Get(int index)
        {
            return _palleteStorage.GetBlockValue(_get(index));
        }

        public int Set(int index, ushort block)
        {
            _set(index, _palleteStorage.GetLocalValue(block));
            return _palleteStorage.Count;
        }

        private byte _get(int index) {
            byte val = RawData[index / 2];
            if (index % 2 == 0) {
                return (byte) (val & 0x0f);
            } else {
                return (byte) ((val & 0xf0) >> 4);
            }
        }

        private void _set(int index, byte value) {
            value &= 0xf;
            int half = index / 2;
            byte previous = RawData[half];
            if (index % 2 == 0) {
                RawData[half] = (byte) (previous & 0xf0 | value);
            } else {
                RawData[half] = (byte) (previous & 0x0f | value << 4);
            }
        }
    
        public void Fill(byte value)
        {
            value &= 0xf;
            Array.Fill(RawData, (byte) (value << 4 | value));
        }
    
        public void SetRawData(byte[] source) {
            Array.Copy(source, RawData, source.Length);
        }
    
        public object Clone()
        {
            return new NibbleArray((byte[])RawData.Clone());
        }

        public IEnumerator GetEnumerator()
        {
            return RawData.GetEnumerator();
        }
    }
}