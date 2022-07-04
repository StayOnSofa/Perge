using Core.Generals;

namespace Core.PackageUtils
{
    [PackageRegister]
    public struct PackageChunk : IPackage
    {
        public byte[] GetBytes()
        {
            return null;
        }
    }
}