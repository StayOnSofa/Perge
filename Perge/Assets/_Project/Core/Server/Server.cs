using ENet;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public abstract class Server : IDisposable
	{
		public Action<Peer> OnConnected;
		public Action<Peer> OnDisconnected;

		public Action<Peer, byte> OnRecived;

		private Host _server;
		private Address _address;

		public Server(ushort port, int maxClients = 16)
		{
			_server = new Host();
			_address = new Address();

			_address.Port = port;
			_server.Create(_address, maxClients);
		}

		public virtual void Tick(float dt)
		{
			ENet.Event netEvent;

				bool polled = false;

				while (!polled)
				{
					if (_server.CheckEvents(out netEvent) <= 0)
					{
						if (_server.Service((int)dt, out netEvent) <= 0)
							break;

						polled = true;
					}

					Peer peer = netEvent.Peer;

					switch (netEvent.Type)
					{
						case ENet.EventType.None:
							break;

						case ENet.EventType.Connect:

							IncomingConnection(peer);
							OnConnected?.Invoke(peer);

							break;

						case ENet.EventType.Disconnect:

							IncomingDisconnection(peer);
							OnDisconnected?.Invoke(peer);

							break;

						case ENet.EventType.Timeout:

							IncomingDisconnection(peer);
							OnDisconnected?.Invoke(peer);

							break;

						case ENet.EventType.Receive:

							byte[] buffer = new byte[netEvent.Packet.Length];
							netEvent.Packet.CopyTo(buffer);

							IncomingReceived(peer, buffer);

							netEvent.Packet.Dispose();
							break;
					}
				}

			_server.Flush();
			
		}

		public abstract void IncomingConnection(Peer peerId);
		public abstract void IncomingDisconnection(Peer peerId);
		public abstract void IncomingReceived(Peer peerId, byte[] bytes);

		public void SendBytes(PacketFlags flag, Peer peer, byte[] bytes)
		{
			Packet packet = default(Packet);
			packet.Create(bytes, flag);
			
			peer.Send(0, ref packet);
		}

		public void Dispose()
		{
			_server?.Dispose();
		}
	}
}
