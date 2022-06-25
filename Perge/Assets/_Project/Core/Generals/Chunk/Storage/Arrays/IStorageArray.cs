namespace Core.Chunks.Storage.Arrays
{
    public interface IStorageArray
    {
        public byte[] GetRaw();
        
        public byte RawGet(int index);

        public void RawSet(int index, byte _byte);
        
        public ushort Get(int index);
        public int Set(int index, ushort block);
    }
}