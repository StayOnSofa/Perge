namespace Core.PackageUtils
{
    public abstract class PackageServerHandler
    {
        private PackageType _packageType;
        private DedicatedServer _server;
        
        public PackageServerHandler(PackageType type, DedicatedServer server)
        {
            _packageType = type;
            _server = server;
        }

        public DedicatedServer GetServer()
        {
            return _server;
        }

        public void Receive(Player player, BlockStream stream)
        {
            if (stream.GetPackageType() != _packageType)
            {
                Handle(player, stream);
            }
        }

        public abstract void Handle(Player player, BlockStream stream);
    }
}