//using Kadmium_sACN.Layers;
//using Kadmium_sACN.Layers.Framing;
using System;
using MarnoldSacn.Layers.Framing;
using MarnoldSacn.Layers;

namespace MarnoldSacn
{
    public class MarnoldDataPacket : MarnoldSacnPacket
    {
        public DataPacketFramingLayer FramingLayer { get; set; }
        public DMPLayer DMPLayer { get; set; }
        public override int Length => RootLayer.Length + DataPacketFramingLayer.Length + DMPLayer.Length;
        public const int MAX_LENGTH = 1143;

        public MarnoldDataPacket()
        {
            RootLayer = new RootLayer
            {
                Vector = RootLayerVector.VECTOR_ROOT_E131_DATA
            };
            FramingLayer = new DataPacketFramingLayer();
            DMPLayer = new DMPLayer();
        }

        public static MarnoldDataPacket Parse(ReadOnlySpan<byte> bytes, RootLayer rootLayer, DataPacketFramingLayer framingLayer)
        {
            MarnoldDataPacket dataPacket = new MarnoldDataPacket
            {
                RootLayer = rootLayer,
                FramingLayer = framingLayer,
                DMPLayer = DMPLayer.Parse(bytes)
            };

            return dataPacket;
        }

        public override void Write(Span<byte> bytes)
        {
            RootLayer.Write(bytes, (UInt16)(DataPacketFramingLayer.Length + DMPLayer.Length));
            FramingLayer.Write(bytes.Slice(RootLayer.Length), (UInt16)(DMPLayer.Length));
            DMPLayer.Write(bytes.Slice(RootLayer.Length + DataPacketFramingLayer.Length));
        }
    }
    
}