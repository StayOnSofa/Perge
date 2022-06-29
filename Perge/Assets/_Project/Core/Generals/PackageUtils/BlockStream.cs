using System;
using System.IO;
using UnityEngine;

namespace Core.PackageUtils
{
    public class BlockStream : IDisposable
    {
        public static int InOrder;
        
        private PackageType _packageType;
        private MemoryStream _memoryStream;

        private int _inOrder = 0;
        
        public BlockStream(PackageType pType, bool useOrder = true)
        {
            _memoryStream = new MemoryStream();
            
            _memoryStream.WriteByte((byte) pType);
            _packageType = pType;

            if (useOrder)
            {
                _inOrder = InOrder;
                InOrder += 1;
            }

            WriteInt(_inOrder);
        }

        public int GetOrder()
        {
            return _inOrder;
        }

        public BlockStream(byte[] bytes)
        {
            _memoryStream = new MemoryStream(bytes);
            
            byte packageType = (byte)_memoryStream.ReadByte();
            _packageType = (PackageType) packageType;

            _inOrder = ReadInt();
        }

        public byte[] ToArray()
        {
            return _memoryStream.ToArray();
        }

        public void WriteByte(byte _byte)
        {
            _memoryStream.WriteByte(_byte);
        }

        public byte ReadByte()
        {
            return (byte)_memoryStream.ReadByte();
        }

        public PackageType GetPackageType()
        {
            return _packageType;
        }
        
        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                _memoryStream.WriteByte(bytes[i]);
            }
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                _memoryStream.WriteByte(bytes[i]);
            }
        }

        public float ReadFloat()
        {
            byte[] bytes =
            {
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
            };

            return BitConverter.ToSingle(bytes);
        }
        
        public int ReadInt()
        {
            byte[] bytes =
            {
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
                (byte)_memoryStream.ReadByte(),
            };

            return BitConverter.ToInt32(bytes);
        }

        public void WriteVector3(Vector3 value)
        {
            WriteFloat(value.x);
            WriteFloat(value.y);
            WriteFloat(value.z);
        }

        public Vector3 ReadVector3()
        {
            return new Vector3(ReadFloat(), ReadFloat(), ReadFloat());
        }

        public void WriteBytes(byte[] bytes)
        {
            WriteInt(bytes.Length);
            for (int i = 0; i < bytes.Length; i++)
            {
                _memoryStream.WriteByte(bytes[i]);
            }
        }

        public byte[] ReadBytes()
        {
            int count = ReadInt();
            byte[] bytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                bytes[i] = (byte)_memoryStream.ReadByte();
            }

            return bytes;
        }

        public void Dispose()
        {
            _memoryStream?.Dispose();
        }
    }
}