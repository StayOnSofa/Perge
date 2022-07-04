using Core.Generals;
using Core.Graphics;
using UnityEngine;

namespace Core
{
    public class Application : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        static void OnRuntime()
        {
            BlockRegister.Register();
            Atlas.Prepare();
            PackageRegisterAttribute.Register();
            
            AppLoader.Prepare();
        }
    }
}
