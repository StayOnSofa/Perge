using Core.PackageUtils;

namespace Core.Generals
{
    public abstract class ServerReceiver<T> where T : IPackage
    {
        private DedicatedServer _server;
        
        public void Create(DedicatedServer server)
        {
            _server = server;
        }

        public DedicatedServer GetServer()
        {
            return _server;
        }

        public void Receive(byte[] bytes)
        {
            using (BlockStream stream = new BlockStream(bytes))
            {
                
            }
        }

        public abstract void PackageReceive(BlockStream stream);
    }
}