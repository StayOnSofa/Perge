using Core.Generals;
using Core.Mono;
using UnityEngine;

namespace Core
{
    public class AppLoader : MonoSingleton<AppLoader>
    {
        private bool _enableServer = true;
        private bool _enableClient = true;
        
        private DedicatedClient _client;
        private DedicatedServer _server;
        
        private void Start()
        {
            UnityEngine.Application.runInBackground = true;

            if (_enableServer)
                Screen.SetResolution(800, 240, FullScreenMode.Windowed);

            if (_enableClient)
                Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            
            ENet.Library.Initialize();
            Debug.Log("[PrepareSinglePlayer]");

            if (_enableServer)
                _server = new DedicatedServer(27015);
            
            if (_enableClient)
                _client = new DedicatedClient("95.58.138.2", 27015);
            
        }
        private void Update()
        {
            float dt = Time.deltaTime;
            
            if (_enableServer)
                _server.Tick(dt);
            
            if (_enableClient)
                _client.Tick(dt);
        }

        private void OnDestroy()
        {
            Debug.Log("[PrepareUnloading]");
            
            if (_enableServer)
                _server.Dispose();
            
            if (_enableClient)
                _client.Dispose();
            
            ENet.Library.Deinitialize();
        }
    }
}