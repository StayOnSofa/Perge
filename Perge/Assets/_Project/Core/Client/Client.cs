using ENet;
using System;

namespace Core
{
    public abstract class Client : IDisposable
    {
        private Host _client;
        private Peer _serverPeer;

        public Client(string ip, ushort port)
        {
            _client = new Host();

            Address address = new Address();

            address.SetHost(ip);
            address.Port = port;

            _client.Create();

            _serverPeer = _client.Connect(address);
        }

        public virtual void Tick(float dt)
        {
			Event netEvent;
			bool polled = false;

			while (!polled)
			{
				if (_client.CheckEvents(out netEvent) <= 0)
				{
					if (_client.Service(0, out netEvent) <= 0)
						break;

					polled = true;
				}

				switch (netEvent.Type)
				{
					case EventType.None:
						break;

					case EventType.Connect:
						IConnected();
						break;

					case EventType.Disconnect:
						IDisconnected();
						break;

					case EventType.Timeout:
						IDisconnected();
						break;

					case EventType.Receive:

						byte[] buffer = new byte[netEvent.Packet.Length];
						netEvent.Packet.CopyTo(buffer);

						IReceived(buffer);

						netEvent.Packet.Dispose();
						break;
				}
			}

			_client.Flush();
		}

		public abstract void IConnected();
		public abstract void IDisconnected();
		public abstract void IReceived(byte[] bytes);

		public void SendBytes(PacketFlags flag, byte[] bytes)
        {
			Packet packet = default(Packet);
			packet.Create(bytes, flag);

			_serverPeer.Send(0, ref packet);
        }

        public void Dispose()
        {
			_client?.Dispose();
		}
    }
}
