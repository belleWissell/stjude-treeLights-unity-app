using System;
using UnityEngine;
//using Haukcode.sACN;
//using Kadmium_sACN;
//using Kadmium_sACN.SacnSender;

using System.Threading.Tasks; // for async
using System.Buffers; // for memory pool
using System.Net;
using System.Net.Sockets;
using System.Buffers.Binary;
using System.Linq;
using System.Timers;
using System.Text;
using MarnoldSacn;

using AAMVC.Unity;


public class SACNCtrl : MonoBehaviour
{
    
    private ApplicationControl appControl;
    private LogTextCtrl logTextCtrl;
    
    private bool initComplete = false;
    
    //private static readonly System.Runtime.Guid acnSourceId = new Guid("{B32625A6-C280-4389-BD25-E0D13F5B50E0}");
    //private static readonly string acnSourceName = "BWCO sender";
    private string acnSourceNameVar = "BWCO sender"; // does it have to be static/read only?

    //private static IPAddress send_IP;
    private IPAddress send_IP =  IPAddress.Parse("192.168.0.32");
    //private IPAddress send_IP =  IPAddress.Parse("127.0.0.1");
    //private int send_port = -1;
    //private int send_universe = 1;
    private UInt16 universe = 1;

    //private SACNClient recvClient;
    //private SACNClient sendClient;
    //SacnPacketFactory factory;
    //SacnSender sacnSender;

    //private bool SACNClientIsActive = false;
    
    private static int numberOfBytesInPacket = 512;
    private byte[] blankBytesToSend = new byte[numberOfBytesInPacket];
    private byte[] fullBytesToSend = new byte[numberOfBytesInPacket];


    private int discoveryTimer = 140;
    private int discoveryTimerMax = 150;

    private MarnoldPacketFactory marnoldPacketFactory;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        logTextCtrl = LogTextCtrl.Instance;
        appControl = ApplicationControl.Instance;
        
        for (int i = 0; i < numberOfBytesInPacket; ++i)
        {
            blankBytesToSend[i] = 0;
            fullBytesToSend[i] = Convert.ToByte(20);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initComplete)
            return;
        //sendFullData();
        //sendTestData();
        
        discoveryTimer += 1;
        if (discoveryTimer > discoveryTimerMax)
        {
            discoveryTimer = 0;
            //discovery();
        }
        
        
        sacnSendTimer += Time.deltaTime;
        if (sacnSendTimer >= sacnSendIntervalSeconds)
        {
            sacnSendTimer = 0f;
            //_ = SendTestDataAsync();
        }
        
        if (doSendDMXeveryFrame)
        {
            sendTestDataFromKeyboard3();
        }
    }

    public void toggleSacnStreamFromKeyboard()
    {
        if (testStreamIsActive)
        {
            logTextCtrl.logText("[SACN] Stopping SACN test stream", true);
            testStreamIsActive = false;
        }
        else
        {
            logTextCtrl.logText("[SACN] Starting SACN test stream", true);
            testStreamIsActive = true;
            _ = autoSendThread();
        }
    }

    private int manualDataSendCounter = 0;

    public void toggleSendTestDataFromKeyboard3()
    {
        doSendDMXeveryFrame = !doSendDMXeveryFrame;
        logTextCtrl.logText("[SACN] doSendDMXeveryFrame = " + doSendDMXeveryFrame.ToString(), true);
    }
    
    public void sendTestDataFromKeyboard3()
    {
        byte[] values = new byte[numberOfBytesInPacket];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = 20;
        }
        SendDmxFrame(values);
    }

    public void sendTestDataFromKeyboard2()
    {
        byte[] values = new byte[numberOfBytesInPacket];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = 20;
        }
        SendDMX(1, values);
    }

    public void sendTestDataFromKeyboard1()
    {


        if (!initComplete)
        {
            logTextCtrl.logText("[SACN] SACN not initialized", true);
            return;
        }

        logTextCtrl.logText("[SACN] Sending SACN test data", true);

        byte[] values = new byte[numberOfBytesInPacket];

        for (int i = 0; i < values.Length; i++)
        {
            values[i] = 20;
        }

        var packet = marnoldPacketFactory.MarnoldCreateDataPacket(universe, values);

        //if (manualDataSendCounter == 0)
        send_IP = GetMulticastAddress(packet.FramingLayer.Universe); // this returns 239.255.0.1
        /*else if (manualDataSendCounter == 1)
            send_IP = IPAddress.Parse("192.168.0.32");
        else if (manualDataSendCounter == 2)
            send_IP = IPAddress.Parse("127.0.0.1");*/
        //send_IP = IPAddress.Parse("239.255.0.1");

        // For network configuration, the standard implementation includes:
        // Protocol: UDP
        // Default Port: 5568
        // Multicast Range: 239.255.x.x (where 'x' typically corresponds to the Universe Number)

        SendInternalMarnold(send_IP, packet);

        logTextCtrl.logText("[SACN] Sent SACN test data on IP " + send_IP.ToString(), true);
        manualDataSendCounter += 1;
        if (manualDataSendCounter > 2)
            manualDataSendCounter = 0;

    }

    public void initAndConnectSACN()
    {
        
        byte[] cid = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15}; // sACN identifiers should be 16 bytes (?).
        string sourceName = acnSourceNameVar;
        //SacnPacketFactory factory = new SacnPacketFactory(cid, sourceName);
        marnoldPacketFactory = new MarnoldPacketFactory(cid, sourceName);
        //factory = new SacnPacketFactory(cid, acnSourceName);
        //sacnSender = new SacnSender();
        
        
        /*
        try
        {
            Guid acnSourceId = new Guid("{B32625A6-C280-4389-BD25-E0D13F5B50E0}");
            string acnSourceName = "BWCO sender";

            send_IP = Haukcode.Network.Utils.GetFirstBindAddress().IPAddress; // this is new to haukcode version
        
            sendClient = new SACNClient(acnSourceId, acnSourceName, send_IP);  // is port optional?

            SACNClientIsActive = true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            
        }*/
        
        //weAreRunning = true;
        //_ = autoSendThread();
        
        initComplete = true;
        logTextCtrl.logText("[SACN] SACN init complete", true);

        //IPAddress send_IP4 = IPAddress.Parse("192.168.0.32");
        //IPAddress send_IP4 = IPAddress.Parse("127.0.0.1");
        //initSacnSender("192.168.0.32", 1);
        initSacnSender("239.255.0.1", 1);

    }

    private void discovery() //It's also good manners to send Universe Discovery packets every 10 seconds.
    {
        //var packets = factory.CreateUniverseDiscoveryPackets(new UInt16[] { universe });
        
        
        var packets = marnoldPacketFactory.MarnoldCreateUniverseDiscoveryPackets(new UInt16[] { universe });
        
        foreach (var packet in packets)
        {
            //sacnSender.SendMulticast(packet);
            send_IP = GetMulticastAddress(MarnoldUniverseDiscoveryPacket.DiscoveryUniverse); // 239.255.250.214
            SendInternalMarnold(send_IP, packet);
        }
    }
    
    private void sendBlankData()
    {
        /*
        try
        {

            sendClient.SendMulticast((ushort)send_universe, blankBytesToSend);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
*/
    }

    /*
    public void sendFullData()
    {
        byte[] values = new byte[numberOfBytesInPacket];
        Array.Copy(fullBytesToSend, 0, values, 0, numberOfBytesInPacket);
        var packet = factory.CreateDataPacket(universe, values);
        //sacnSender.SendMulticast(packet);


        sacnSender.SendUnicast(packet, send_IP);
    }

    async void sendTestData()
    {
        //byte[] values = new byte[512] { 0, 1, 2, 3, 4, 5 };
        byte[] values = new byte[] { 0, 1, 2, 3, 4, 5 };
        var packet = factory.CreateDataPacket(universe, values);
        //await sacnSender.Send(packet);
        await sacnSender.SendMulticast(packet);
    }*/
    
    private bool isSendingSacn = false;
    private float sacnSendTimer = 0f;
    private const float sacnSendIntervalSeconds = 15f / 30f;

    private async Task SendTestDataAsync()
    {
        if (isSendingSacn || marnoldPacketFactory == null)
            return;

        isSendingSacn = true;

        try
        {
            byte[] values = new byte[numberOfBytesInPacket];

            for (int i = 0; i < values.Length; i++)
            {
                values[i] = 20;
            }

            //var packet = factory.CreateDataPacket(universe, values);
            var packet = marnoldPacketFactory.MarnoldCreateDataPacket(universe, values);
            //await sacnSender.SendMulticast(packet);

            //send_IP = GetMulticastAddress(packet.FramingLayer.Universe); // this returns 239.255.0.1
            //send_IP = IPAddress.Parse("192.168.0.32");
            //send_IP = IPAddress.Parse("127.0.0.1");
            await SendInternalMarnold(send_IP, packet);
        }
        catch (ObjectDisposedException)
        {
            // The application is shutting down; safe to ignore.
        }
        catch (Exception e)
        {
            Debug.LogError($"[SACN] Test data send failed: {e.Message}");
        }
        finally
        {
            isSendingSacn = false;
        }
    }
    
    public void onProgramExit()
    {
        //weAreRunning = false;
        initComplete = false;
        //SACNClientIsActive = false;
        
       //if (sacnSender != null)
       //     sacnSender.Dispose();
        
        /*
        if (sendClient != null)
        {
            sendBlankData();
            //recvClient.Dispose();
            sendClient.Dispose();
        }*/
    }
    
    //private Socket Ipv4Socket { get; } = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    //private Socket Ipv6Socket { get; } = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);

    protected async Task SendInternalMarnold(IPAddress address, MarnoldSacnPacket packet)
    {
        var endpoint = new IPEndPoint(address, MarnoldSacnConstants.Port);
        using (var owner = MemoryPool<byte>.Shared.Rent(packet.Length))
        {
            var bytes = owner.Memory.Slice(0, packet.Length);
            packet.Write(bytes.Span);
            //var socket = address.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6Socket : Ipv4Socket;
            var socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //var socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Rdm, ProtocolType.Udp); // RDM hangs here...
            socket2.EnableBroadcast = true; // recommended by Google Ai
            
            //socket2.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //socket2.SetSocketOption(Policy
            
            
            var args2 = new SocketAsyncEventArgs
            {
                SocketFlags = SocketFlags.None,
                RemoteEndPoint = endpoint,
                //SendPacketsSendSize = 0
                //SendPacketsSendSize = packet.Length // this appears to be -1 (?)
            };
            args2.SetBuffer(bytes);
            var tsc = new TaskCompletionSource<SocketAsyncEventArgs>();
            args2.Completed += (_, args2) =>
            {
                tsc.SetResult(args2);
            };
            bool result = socket2.SendToAsync(args2);
            
            //socket2.Send(bytes, args2.Count, args2.RemoteEndPoint, new AsyncCallback(SendCallback), socket2);
            
            //socket2.BeginSend(args2.Buffer.ToList(), args2.Count, args2.RemoteEndPoint, new AsyncCallback(SendCallback), socket2);
            //udpClient.BeginSend(data, data.Length, remoteEndPoint, new AsyncCallback(SendCallback), udpClient);
            
            if (result)
            {
                await tsc.Task;
            }

        }
    }
    
    private void SendCallback(IAsyncResult ar)
    {
        UdpClient client = (UdpClient)ar.AsyncState;
        int bytesSent = client.EndSend(ar);
        // Trace confirmation logs on background threads responsibly
    }

    private void SendInternalMarnold2(IPAddress address, MarnoldSacnPacket packet)
    {
        var endpoint = new IPEndPoint(address, MarnoldSacnConstants.Port);
        var owner = MemoryPool<byte>.Shared.Rent(packet.Length);

        var bytes = owner.Memory.Slice(0, packet.Length);
        //packet.Write(bytes.Span);
        //var socket = address.AddressFamily == AddressFamily.InterNetworkV6 ? Ipv6Socket : Ipv4Socket;
        var socket3 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        var args3 = new SocketAsyncEventArgs
        {
            SocketFlags = SocketFlags.None,
            RemoteEndPoint = endpoint
        };
        args3.SetBuffer(bytes);
        //var tsc = new TaskCompletionSource<SocketAsyncEventArgs>();
        //args3.Completed += (_, args2) => { tsc.SetResult(args2); };
        //bool result = socket3.SendToAsync(args3);

        bool result2 = socket3.SendAsync(args3); // only used when destination point is locked in (TCP)
        
        if (result2)
        {
            //await tsc.Task;
        }


    }


    public IPAddress GetMulticastAddress(UInt16 universe)
    {
        var universeBytes = GetUniverseBytes(universe);
        IPAddress address = new IPAddress(new byte[] { 239, 255, universeBytes[0], universeBytes[1] });
        return address;
    }
    
    private static Span<byte> GetUniverseBytes(UInt16 universe)
    {
        byte[] universeBytes = new byte[2];
        BinaryPrimitives.WriteUInt16BigEndian(universeBytes, universe);
        return universeBytes;
    }

    
    private bool testStreamIsActive = false;
    
    protected async Task autoSendThread()
    {
        
        byte[] cid_test = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        UInt16 universe_test = 1;
        string sourceName_test = "My lovely source v2";
        //SacnPacketFactory factory_test = new SacnPacketFactory(cid_test, sourceName_test);
        //using var sacnSender_test = new SacnSender();
        MarnoldPacketFactory factory_test = new MarnoldPacketFactory(cid_test, sourceName_test);
        using var sacnSender_test = new MarnoldSacnSender.MarnoldSacnSender();

        using Timer timer = new Timer(10000);
        timer.Elapsed += async (sender, e) =>
        {
            var packets = factory_test.MarnoldCreateUniverseDiscoveryPackets(new UInt16[] { universe });
            foreach (var packet in packets)
            {
                await sacnSender_test.SendMulticast(packet);
            }
        };
        timer.Start();

        byte[] values_test = new byte[512];
        while (testStreamIsActive)
        {
            for (byte i = 0; i < 255; i+=10)
            {
                Array.Fill(values_test, i);
                var packet_test = factory_test.MarnoldCreateDataPacket(universe_test, values_test);
                await sacnSender_test.SendMulticast(packet_test);
                System.Diagnostics.Debug.WriteLine("[SACN] sending "+i);
                await Task.Delay(1000 / 40);
            }
        }
        timer.Stop();
        timer.Dispose();
        sacnSender_test.Dispose();
        
    }
    
    /*
    protected async Task SendInternal(IPAddress address, SacnPacket packet)
    {
        IPEndPoint endpoint = new IPEndPoint(address, 5568);
        using (IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(packet.Length))
        {
            Memory<byte> bytes = owner.Memory.Slice(0, packet.Length);
            packet.Write(bytes.Span);
            Socket socket = address.AddressFamily == AddressFamily.InterNetworkV6 ? this.Ipv6Socket : this.Ipv4Socket;
            SocketAsyncEventArgs args1 = new SocketAsyncEventArgs()
            {
                SocketFlags = SocketFlags.None,
                RemoteEndPoint = (EndPoint) endpoint
            };
            args1.SetBuffer(bytes);
            TaskCompletionSource<SocketAsyncEventArgs> tsc = new TaskCompletionSource<SocketAsyncEventArgs>();
            args1.Completed += (EventHandler<SocketAsyncEventArgs>) ((_, args) => tsc.SetResult(args));
            if (socket.SendToAsync(args1))
            {
                SocketAsyncEventArgs task = await tsc.Task;
            }
            bytes = new Memory<byte>();
            socket = (Socket) null;
            args1 = (SocketAsyncEventArgs) null;
        }
        endpoint = (IPEndPoint) null;
    }*/
    
        private static readonly byte[] rootLayer = new byte[]
    {
        0x00, 0x10, // Preamble Size
        0x00, 0x00, // Post-amble Size
        0x41, 0x53, 0x43, 0x2d, 0x45, 0x31, 0x2e, 0x33, 0x31, 0x00, 0x00, 0x00, // "ASC-E1.31\0\0\0"
        0x00, 0x00, 0x00, 0x02, // Root Vector (Data Packet)
        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 // CID (Placeholder)
    };

    private static readonly byte[] frameLayer = new byte[]
    {
        0x00, 0x00, 0x00, 0x00, // Reserved
        0x00, 0x00, // Frame Vector (Data)
        0x00, 0x00, // Source Name (Will be filled later)
        0x00, // Priority (0-200)
        0x00, 0x00, // Sync Address
        0x00, // Sequence
        0x00, // Options
        0x00, 0x00  // Universe
    };

    private static readonly byte[] dmpLayer = new byte[]
    {
        0x02, // DMP Vector (Complete)
        0xa1, // Address Type (Flags)
        0x00, 0x00, // First Property Address
        0x00, 0x01, // Address Increment
        0x00, 0x01, // Property Value Count (1 start code + 512 channels)
        0x00  // DMX Start Code (0x00 is standard DMX)
    };

    /// <summary>
    /// result of serach "C# send sacn via UdpClient class"
    /// </summary>
    bool doSendDMXeveryFrame = false;
    public static void SendDMX(ushort universe, byte[] dmxData)
    {
        int length = rootLayer.Length + frameLayer.Length + dmpLayer.Length + dmxData.Length;
        byte[] packet = new byte[length];

        // 1. Root Layer
        Buffer.BlockCopy(rootLayer, 0, packet, 0, rootLayer.Length);
        
        // Root Length
        short rootLength = (short)(0x7000 | (length - 12)); 
        packet[22] = (byte)((rootLength >> 8) & 0xff);
        packet[23] = (byte)(rootLength & 0xff);

        // 2. Frame Layer
        int frameOffset = rootLayer.Length;
        Buffer.BlockCopy(frameLayer, 0, packet, frameOffset, frameLayer.Length);
        
        // Frame Length
        short frameLength = (short)(0x7000 | (length - rootLayer.Length));
        packet[frameOffset + 2] = (byte)((frameLength >> 8) & 0xff);
        packet[frameOffset + 3] = (byte)(frameLength & 0xff);

        // Universe
        packet[frameOffset + 18] = (byte)((universe >> 8) & 0xff);
        packet[frameOffset + 19] = (byte)(universe & 0xff);

        // 3. DMP Layer
        int dmpOffset = frameOffset + frameLayer.Length;
        Buffer.BlockCopy(dmpLayer, 0, packet, dmpOffset, dmpLayer.Length);
        
        // DMP Length
        short dmpLength = (short)(0x7000 | (length - rootLayer.Length - frameLayer.Length));
        packet[dmpOffset + 1] = (byte)((dmpLength >> 8) & 0xff);
        packet[dmpOffset + 2] = (byte)(dmpLength & 0xff);

        // 4. DMX Data (up to 512)
        int dmxOffset = dmpOffset + dmpLayer.Length;
        //Buffer.BlockCopy(dmxData, 0, packet, dmxOffset, Math.min(dmxData.Length, 512));
        Buffer.BlockCopy(dmxData, 0, packet, dmxOffset, Math.Min(dmxData.Length, 512));
        
        // 5. Send over UDP
        using (var client = new UdpClient())
        {
            client.EnableBroadcast = true;
            //client.Client.SetSocketOption()
            // Multicast IP for Universe 1 (or Unicast IP of your lighting node)
            IPAddress targetAddress = IPAddress.Parse("239.255.0.1"); 
            IPEndPoint endPoint = new IPEndPoint(targetAddress, 5568);

            client.Send(packet, packet.Length, endPoint);
        }
    }
    
    private const int SacnPort = 5568;
    //private readonly UdpClient _udpClient; // read only throws error
    //private readonly IPEndPoint _endPoint;
    private UdpClient _udpClient;
    private IPEndPoint _endPoint;
    private readonly byte[] _packetBuffer = new byte[638];
    private byte _sequenceId = 0;

    public void initSacnSender(string targetIp, byte universeId)
    {
        // Target IP can be a specific device (unicast) or the multicast address for the universe, 
        // e.g., 239.255.0.1 for Universe 1 (239.255.[Universe_High].[Universe_Low])
        _udpClient = new UdpClient();
        _endPoint = new IPEndPoint(IPAddress.Parse(targetIp), SacnPort);

        // Build the fixed sACN header and root layers
        InitializePacket(universeId);
    }

    private void InitializePacket(byte universeId)
    {
        // 1. Root Layer Preamble (16 bytes)
        Array.Copy(new byte[] { 0x00, 0x10, 0x00, 0x00 }, 0, _packetBuffer, 0, 4); // Preamble Size
        Encoding.ASCII.GetBytes("ASC-E1.17").CopyTo(_packetBuffer, 4);           // ACN Root Layer Protocol ID

        // Root Layer Protocol (22 bytes)
        // Flags (0x7) + Length (0x0266 = 638 bytes)
        _packetBuffer[16] = 0x72;
        _packetBuffer[17] = 0x66;
        _packetBuffer[18] = 0x00; // Vector: Root Layer Data (0x00000004)
        _packetBuffer[19] = 0x00;
        _packetBuffer[20] = 0x00;
        _packetBuffer[21] = 0x04;
        // 128-bit Component Identifier (UUID) can go here. Leaving as 0s is standard for basic sources.

        // 2. Framing Layer (77 bytes)
        // Flags + Length
        _packetBuffer[38] = 0x72;
        _packetBuffer[39] = 0x58;
        _packetBuffer[40] = 0x00; // Vector: Framing Layer Data (0x00000008)
        _packetBuffer[41] = 0x00;
        _packetBuffer[42] = 0x00;
        _packetBuffer[43] = 0x08;
        // Source Name (64 bytes)
        Encoding.ASCII.GetBytes("C# sACN Sender").CopyTo(_packetBuffer, 44);
        // Priority (1 byte, default 100)
        _packetBuffer[108] = 100;
        // Synchronization Address (2 bytes, 0 for unsync)
        _packetBuffer[109] = 0x00;
        _packetBuffer[110] = 0x00;
        // Sequence Number (1 byte, auto-increments in SendDmx)
        // Options (1 byte, normal operation = 0x00)
        _packetBuffer[112] = 0x00;
        // Universe (2 bytes)
        byte[] uniBytes = BitConverter.GetBytes((short)universeId);
        if (BitConverter.IsLittleEndian) Array.Reverse(uniBytes);
        _packetBuffer[113] = uniBytes[0];
        _packetBuffer[114] = uniBytes[1];

        // 3. DMP Layer (513 bytes)
        _packetBuffer[115] = 0x72; // Flags & Length
        _packetBuffer[116] = 0x00; // Vector: DMP Set Property (0x02)
        _packetBuffer[117] = 0x02;
        _packetBuffer[118] = 0xa1; // Address Type (DMP Address & Data format)
        _packetBuffer[119] = 0x00; // First Property Address (0)
        _packetBuffer[120] = 0x00;
        _packetBuffer[121] = 0x00; // Address Increment (1)
        _packetBuffer[122] = 0x01;
        _packetBuffer[123] = 0x00; // Property value count (513 bytes total: Start Code + 512 channels)
        _packetBuffer[124] = 0x02;
        _packetBuffer[125] = 0x01; // DMX Start Code (0x00 for standard DMX)
    }

    public void SendDmxFrame(byte[] dmxData)
    {
        // Increment sequence ID
        _packetBuffer[111] = _sequenceId++;

        // Ensure we don't copy more than the 512 channels allowed in a universe
        int lengthToWrite = Math.Min(dmxData.Length, 512);
        
        // Copy DMX values starting at byte 126
        Array.Copy(dmxData, 0, _packetBuffer, 126, lengthToWrite);

        // Send over UDP
        _udpClient.Send(_packetBuffer, _packetBuffer.Length, _endPoint);
        
        //_udpClient.SendToAsync
    }
}
