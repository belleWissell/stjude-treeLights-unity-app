//using Kadmium_sACN.Layers;
//using Kadmium_sACN.Layers.Framing;
using MarnoldSacn.Layers;
using MarnoldSacn.Layers.Framing;
using System;

namespace MarnoldSacn
{
    public abstract class MarnoldSacnPacket
    {
        public RootLayer RootLayer { get; set; }
        public abstract int Length { get; }
        public abstract void Write(Span<byte> bytes);

        public static MarnoldSacnPacket Parse(ReadOnlySpan<byte> bytes)
        {
            RootLayer rootLayer = RootLayer.Parse(bytes);
            if (rootLayer == null)
            {
                return null;
            }
            bytes = bytes.Slice(RootLayer.Length);

            FramingLayer framingLayer = FramingLayer.Parse(bytes, rootLayer.Vector);
			
            switch(framingLayer)
            {
                case DataPacketFramingLayer dataLayer:
                    bytes = bytes.Slice(DataPacketFramingLayer.Length);
                    return MarnoldDataPacket.Parse(bytes, rootLayer, dataLayer);
                //case SynchronizationPacketFramingLayer syncLayer:
                //    bytes = bytes.Slice(SynchronizationPacketFramingLayer.Length);
                //    return SynchronizationPacket.Parse(bytes, rootLayer, syncLayer);
                case UniverseDiscoveryPacketFramingLayer discoveryLayer:
                    bytes = bytes.Slice(UniverseDiscoveryPacketFramingLayer.Length);
                    return MarnoldUniverseDiscoveryPacket.Parse(bytes, rootLayer, discoveryLayer);
                default:
                    return null;
            }
        }
    }
}