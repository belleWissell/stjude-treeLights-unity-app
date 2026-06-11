//using Kadmium_sACN.MulticastAddressProvider;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using MarnoldMulticastAddressProvider;
using MarnoldSacn;

namespace MarnoldSacnSender
{
	public class MarnoldSacnSender : IDisposable
	{
		private ISacnMulticastAddressProvider Ipv4MulticastAddressProvider { get; }
		private ISacnMulticastAddressProvider Ipv6MulticastAddressProvider { get; }
		private Socket Ipv4Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
		private Socket Ipv6Socket { get; } = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);

		public MarnoldSacnSender() : this(new SacnMulticastAddressProviderIPV4(), new SacnMulticastAddressProviderIPV6())
		{ }

		protected MarnoldSacnSender(ISacnMulticastAddressProvider ipv4AddressProvider, ISacnMulticastAddressProvider ipv6AddressProvider)
		{
			Ipv4MulticastAddressProvider = ipv4AddressProvider;
			Ipv6MulticastAddressProvider = ipv6AddressProvider;
		}

		protected async Task SendInternal(IPAddress address, MarnoldSacnPacket packet)
		{
			var endpoint = new IPEndPoint(address, MarnoldSacnConstants.Port);
			using (var owner = MemoryPool<byte>.Shared.Rent(packet.Length))
			{
				var bytes = owner.Memory.Slice(0, packet.Length);
				packet.Write(bytes.Span);
				var socket = address.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6Socket : Ipv4Socket;
				socket.EnableBroadcast = true;
				//var socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
				var args = new SocketAsyncEventArgs
				{
					SocketFlags = SocketFlags.None,
					RemoteEndPoint = endpoint
					//SendPacketsSendSize = 0
				};
				args.SetBuffer(bytes);
				var tsc = new TaskCompletionSource<SocketAsyncEventArgs>();
				args.Completed += (_, args) =>
				{
					tsc.SetResult(args);
				};
				bool result = socket.SendToAsync(args);
				if (result)
				{
					await tsc.Task;
				}

			}
		}

		public Task SendUnicast(MarnoldDataPacket packet, IPAddress remoteHost)
		{
			return SendInternal(remoteHost, packet);
		}

		public Task SendUnicast(MarnoldUniverseDiscoveryPacket packet, IPAddress remoteHost)
		{
			return SendInternal(remoteHost, packet);
		}

		/*
		public Task SendUnicast(SynchronizationPacket packet, IPAddress remoteHost)
		{
			return SendInternal(remoteHost, packet);
		}*/

		public Task SendMulticast(MarnoldDataPacket packet, bool ipv6 = false)
		{
			//return SendInternal(Ipv6MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.Universe), packet);
			return SendInternal(Ipv4MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.Universe), packet);
		}

		public Task SendMulticast(MarnoldUniverseDiscoveryPacket packet, bool ipv6 = false)
		{
			//return SendInternal(Ipv6MulticastAddressProvider.GetMulticastAddress(MarnoldUniverseDiscoveryPacket.DiscoveryUniverse), packet);
			return SendInternal(Ipv4MulticastAddressProvider.GetMulticastAddress(MarnoldUniverseDiscoveryPacket.DiscoveryUniverse), packet);
		}

		/*
		public Task SendMulticast(SynchronizationPacket packet, bool ipv6 = false)
		{
			//return SendInternal(Ipv6MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.SynchronizationAddress), packet);
			return SendInternal(Ipv4MulticastAddressProvider.GetMulticastAddress(packet.FramingLayer.SynchronizationAddress), packet);
		}*/

		public void Dispose()
		{
			Ipv4Socket.Dispose();
			Ipv6Socket.Dispose();
		}
	}
}
