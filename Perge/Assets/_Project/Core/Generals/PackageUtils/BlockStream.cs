using System;
using System.IO;
using UnityEngine;

namespace Core.PackageUtils
{
    public class BlockStream : MemoryStream
    {
        private PackageType _packageType;
        
        public BlockStream(PackageType pType) : base()
        {
            WriteByte((byte) pType);
            _packageType = pType;
        }
        public BlockStream(byte[] bytes) : base(bytes)
        {
            byte packageType = (byte)ReadByte();
            _packageType = (PackageType) packageType;
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
                WriteByte(bytes[i]);
            }
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            for (int i = 0; i < bytes.Length; i++)
            {
                WriteByte(bytes[i]);
            }
        }

        public float ReadFloat()
        {
            byte[] bytes =
            {
                (byte)ReadByte(),
                (byte)ReadByte(),
                (byte)ReadByte(),
                (byte)ReadByte(),
            };

            return BitConverter.ToSingle(bytes);
        }
        
        public int ReadInt()
        {
            byte[] bytes =
            {
                (byte)ReadByte(),
                (byte)ReadByte(),
                (byte)ReadByte(),
                (byte)ReadByte(),
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
                WriteByte(bytes[i]);
            }
        }

        public byte[] ReadBytes()
        {
            int count = ReadInt();
            byte[] bytes = new byte[count];

            for (int i = 0; i < count; i++)
            {
                bytes[i] = (byte)ReadByte();
            }

            return bytes;
        }
    }
}