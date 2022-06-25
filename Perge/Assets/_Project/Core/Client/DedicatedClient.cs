using System.Collections;
using System.Threading;
using Core.Chunks;
using Core.Mono;
using Core.PackageUtils;
using Core.World;
using UnityEngine;

namespace Core
{ 
    public class DedicatedClient : Client
    {
        private LocalPlayer _localPlayer;
        private ClientWorld _clientWorld;
        public DedicatedClient(string ip, ushort port) : base(ip, port)
        {
            Debug.Log("[Client] Start");
            _clientWorld = new ClientWorld();
        }

        public override void IConnected()
        {
            Debug.Log("[Client] Connected");
            _localPlayer = new LocalPlayer(this);
        }
        
        public override void IDisconnected()
        {
            Debug.Log("[Client] Disconnected");
        }
        
        public override void IReceived(byte[] bytes)
        {
            MonoCoroutine.Instance.Play(JobChunks(bytes));
        }

        public IEnumerator JobChunks(byte[] bytes)
        {
            bool inJob = true;
            Chunk chunk = null;

            bool isDestroyed = false;
            KeyIndex keyIndex = new KeyIndex(0,0);
            
            ThreadPool.QueueUserWorkItem((c) =>
            {
                using (BlockStream stream = new BlockStream(bytes))
                {
                    if (stream.GetPackageType() == PackageType.ChunkUpload)
                    {
                        chunk = new Chunk(_clientWorld, stream);
                    }

                    if (stream.GetPackageType() == PackageType.ChunkUnload)
                    {
                        int x = stream.ReadInt();
                        int z = stream.ReadInt();

                        keyIndex = new KeyIndex(x, z);
                        
                        isDestroyed = true;
                    }
                }
                
                inJob = false;
            });

            while (inJob)
            {
                yield return null;
            }

            if (chunk != null)
            {
                _clientWorld.AddChunk(chunk);
            }

            if (isDestroyed)
            {
                _clientWorld.RemoveChunk(keyIndex);
            }
        }
    }
}
