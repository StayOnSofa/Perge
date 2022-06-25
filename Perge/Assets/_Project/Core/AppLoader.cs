using Core.Mono;
using UnityEngine;

namespace Core
{
    public class AppLoader : MonoSingleton<AppLoader>
    {
        private DedicatedClient _client;
        private DedicatedServer _server;
        
        private void Start()
        {
            ENet.Library.Initialize();
            Debug.Log("[PrepareSinglePlayer]");
            
            _server = new DedicatedServer(25567);
            _client = new DedicatedClient("localhost", 25567);
        }
        private void Update()
        {
            float dt = Time.deltaTime;
            
            _server.Tick(dt);
            _client.Tick(dt);
        }

        private void OnDestroy()
        {
            Debug.Log("[PrepareUnloading]");
            
            _server.Dispose();
            _client.Dispose();
            
            ENet.Library.Deinitialize();
        }
    }
}