using System;
using System.Collections.Generic;
using System.Linq;
//using Kadmium_sACN;


namespace MarnoldSacn
{
    public class MarnoldPacketFactory
    {

        public byte[] CID { get; set; }
        public string SourceName { get; set; }
        private byte SequenceNumber { get; set; }

        public MarnoldPacketFactory(byte[] cid, string sourceName)
        {
            CID = cid;
            SourceName = sourceName;
        }

        public MarnoldDataPacket MarnoldCreateDataPacket(UInt16 universe, IEnumerable<byte> properties, byte priority = MarnoldSacnConstants.Priority_Default, byte startCode = 0)
        {
            MarnoldDataPacket dataPacket = new MarnoldDataPacket();
            dataPacket.RootLayer.CID = CID;
            dataPacket.FramingLayer.SourceName = SourceName;
            dataPacket.FramingLayer.SequenceNumber = SequenceNumber;
            dataPacket.FramingLayer.Universe = universe;
            dataPacket.FramingLayer.Priority = priority;
            dataPacket.DMPLayer.StartCode = startCode;
            dataPacket.DMPLayer.PropertyValues = properties;

            SequenceNumber = (SequenceNumber == byte.MaxValue) ? byte.MinValue : (byte)(SequenceNumber + 1);

            return dataPacket;
        }

        public IEnumerable<MarnoldUniverseDiscoveryPacket> MarnoldCreateUniverseDiscoveryPackets(IEnumerable<UInt16> universes)
        {
            var chunks = universes.Chunk(MarnoldUniverseDiscoveryPacket.PageSize);

            byte page = 0;
            byte lastPage = (byte)chunks.Count();

            foreach (var chunk in chunks)
            {
                yield return MarnoldCreateUniverseDiscoveryPage(chunk, page, lastPage);
                page++;
            }
        }

        private MarnoldUniverseDiscoveryPacket MarnoldCreateUniverseDiscoveryPage(IEnumerable<UInt16> universes, byte page, byte lastPage)
        {
            MarnoldUniverseDiscoveryPacket packet = new MarnoldUniverseDiscoveryPacket();
            packet.RootLayer.CID = CID;
            packet.FramingLayer.SourceName = SourceName;
            packet.UniverseDiscoveryLayer.Page = page;
            packet.UniverseDiscoveryLayer.LastPage = lastPage;
            packet.UniverseDiscoveryLayer.Universes = universes.ToArray();
            return packet;
        }



    }
}