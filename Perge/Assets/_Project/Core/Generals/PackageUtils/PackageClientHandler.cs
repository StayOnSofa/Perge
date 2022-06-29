namespace Core.PackageUtils
{
    public abstract class PackageClientHandler
    {
        public void Receive(BlockStream stream)
        {
            Handle(stream);
        }
        
        public abstract void Handle(BlockStream stream);
    }
}