namespace Core.Generals
{
    public class CoreRegister
    {
        [BlockRegister]
        public void Register()
        {
            BlockRegister.Register<Air>();   
            BlockRegister.Register<Dirt>();   
            BlockRegister.Register<Grass>();   
        }
    }
}