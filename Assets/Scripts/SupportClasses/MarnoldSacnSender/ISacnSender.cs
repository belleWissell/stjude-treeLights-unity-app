using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MarnoldSacn;

namespace MarnoldSacnSender
{
	interface ISacnSender : IDisposable
	{
		Task Send(MarnoldDataPacket packet);
		Task Send(MarnoldUniverseDiscoveryPacket packet);
		//Task Send(SynchronizationPacket packet);
	}
}
