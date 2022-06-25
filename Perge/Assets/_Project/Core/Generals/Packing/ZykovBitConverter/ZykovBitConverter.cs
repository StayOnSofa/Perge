using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Packing
{
    public class ZikovBitConverter
    {
        private const byte HalfByteMask = 0xF;
        private const byte ByteMask = 0xFF;

        private byte[] _byteBuffer = new byte[2];

        public byte[] GetBytes(ushort c)
        {
            _byteBuffer[1] = (byte)((c >> 8) & ByteMask);
            _byteBuffer[0] = (byte)(c & ByteMask);
            
            return _byteBuffer;
        }

        public ushort GetShort(byte a, byte b)
        {
            return (ushort)((b << 8) | a);
        }
        
        public byte GetByte(byte a, byte b)
        {
            return (byte)((a << 4) | b);
        }

        public byte[] GetShort(byte _byte)
        {
            _byteBuffer[0] = (byte)((_byte >> 4) & HalfByteMask);
            _byteBuffer[1] = (byte)(_byte & HalfByteMask);

            return _byteBuffer;
        }

    }
}
