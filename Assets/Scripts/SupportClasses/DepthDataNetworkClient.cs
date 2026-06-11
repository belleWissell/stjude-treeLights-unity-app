using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using UnityEngine;
using System.Collections;


namespace Communications
{

    /// <summary>
    /// Networkclient received UDP datagram data from Kinect Server
    /// Data is sent compressed in chunks of 65k (the maximum of UDP packet size)
    /// Compressed packets are prepended with a simple header.
    /// The header is a string "KinectData" followed by the size of the compressed data as an int32
    /// </summary>
    public class DepthDataNetworkClient
    {
        private bool debugMode = false; // switch for writing to EVENTLOG
        public static int INT_SIZE = 2; //2 bytes for short, 4 bytes for int
        //public static string EVENTLOG_SOURCENAME = "CFHOF Cart Display";

        protected int width, height; //frame width, height
        protected int packetSize; //uncompressed packet size

        string connectionIP;
        int connectionPort;
        TcpClient tcpClient;
        object clientlock = new object();
        IPEndPoint serverEndPoint;

        private System.Timers.Timer attemptReconnectTimer; // use system timer (higher performance)
        private int attemptReconnectTimerTime = 3000;

        private System.Timers.Timer
            waitBeforePullingDataAfterConnectionTimer; // use system timer (higher performance)

        private int waitBeforePullingDataAfterConnectionTimerTime = 500;
        private static bool okayToPullData = false;
        private bool waitingToPullData = true;

        protected byte[] lastCompressedPacket;
        protected int lastCompressedPacketFinalSize;

        protected byte[] lastfullpacket;
        protected object lastfullpacketLock = new object();

        //protected short[] pointPositions;
        //protected object pointPositionsLock = new object();

        protected float[] userMeshRange;
        protected object userMeshRangeLock = new object();

        protected int[] whichUserMeshIndexList;
        protected object whichUserMeshIndexListLock = new object();

        protected float[] userEdgeRange;
        protected object userEdgeRangeLock = new object();

        protected int[] whichUserEdgeIndexList;
        protected object whichUserEdgeIndexListLock = new object();

        //protected int[] whichLoPolyMeshIndexList;
        //protected object whichLoPolyMeshIndexListLock = new object();

        //public bool isDirty = false;
        // **************************************
        // GET/SET has state changed?
        // **************************************
        /*
        private bool dirty = true;
        public bool isDirty()
        {
            bool valueToReturn = dirty;
            
            if (dirty)
            {
                dirty = false;
            }
            return valueToReturn;
        }
        */

        private bool connected;
        private int missedDataFrameCount;
        private int connectCountDownTimer = 0;
        private int waitToPullDataCounter = 0;

        public bool checkForNewData() // called directly before getUserMeshRangeArray, getUserMeshIndexListArray
        {
            bool valueToReturn = false;
            if (!connected)
            {
                connectCountDownTimer += 1;
                if (connectCountDownTimer == 1)
                    UnityEngine.Debug.Log("[NetworkClient] waiting to connect ");
                if (connectCountDownTimer > 15)
                {
                    doAttemptReconnect();
                }
                else
                {
                    //UnityEngine.Debug.Log("[NetworkClient] still waiting to connect ");
                    return false; // not ready yet
                }
            }
            else
            {
                if (waitingToPullData)
                {
                    waitToPullDataCounter += 1;
                    if (waitToPullDataCounter ==1)
                        UnityEngine.Debug.Log("[NetworkClient] waiting to pull data ");
                    if (waitToPullDataCounter > 15)
                    {
                        doWaitTimeCompleted();
                    }
                    else
                    {
                        //UnityEngine.Debug.Log("[NetworkClient] still waiting to pull data ");
                        return false; // not ready yet
                    }
                }
            }
            
            
            if (hasdata)
            {
                missedDataFrameCount = 0;
                valueToReturn = UnpackLastFrame();
                //UnityEngine.Debug.Log("[NetworkClient] data was found ");
            }
            else
            {
                if (!waitingToPullData) // only count frames after we have started pulling data
                    missedDataFrameCount++;
                if (missedDataFrameCount > 1000)
                {
                    UnityEngine.Debug.Log("[NetworkClient] assuming loss of connection ");
                    connected = false;
                    missedDataFrameCount = 0;
                    connectCountDownTimer = 0;
                    
                    //attemptReconnectTimer.Start();
                }
            }

            return valueToReturn;
        }

        public bool running = false;
        public bool hasdata = false;
        public bool processing = false;
        public bool reading = false;

        //protected System.Threading.Timer unpackTimer;

        protected string fileSource;
        protected string[] dataFiles;
        protected int dataFileIndex;
        protected System.Threading.Timer fileReadTimer;

        public float[] getUserMeshRangeArray() // occurs every OnUpdateFrame
        {
            float[] valueToReturn;
            valueToReturn = new float[userMeshRange.Length];
            Array.Copy(userMeshRange, valueToReturn, userMeshRange.Length);
            //System.Diagnostics.Debug.WriteLine("new mesh array " + userMeshRange.Length);
            return valueToReturn;
        }

        public int[] getUserMeshIndexListArray() // occurs every OnUpdateFrame
        {
            int[] valueToReturn;
            valueToReturn = new int[whichUserMeshIndexList.Length];
            Array.Copy(whichUserMeshIndexList, valueToReturn, whichUserMeshIndexList.Length);
            //System.Diagnostics.Debug.WriteLine("new index array " + whichUserMeshIndexList.Length);
            return valueToReturn;
        }

        public float[] getUserEdgeRangeArray() // occurs every OnUpdateFrame
        {
            float[] valueToReturn;
            valueToReturn = new float[userEdgeRange.Length];
            Array.Copy(userEdgeRange, valueToReturn, userEdgeRange.Length);
            //System.Diagnostics.Debug.WriteLine("new mesh array " + userMeshRange.Length);
            return valueToReturn;
        }

        public int[] getUserEdgeIndexListArray() // occurs every OnUpdateFrame
        {
            int[] valueToReturn;
            valueToReturn = new int[whichUserEdgeIndexList.Length];
            Array.Copy(whichUserEdgeIndexList, valueToReturn, whichUserEdgeIndexList.Length);
            //System.Diagnostics.Debug.WriteLine("new index array " + whichUserMeshIndexList.Length);
            return valueToReturn;
        }

        /* ********************************************
         * CONSTRUCTOR
         * ****************************************** */
        public DepthDataNetworkClient(String ip, Int32 port)
        {
            connectionIP = ip;
            connectionPort = port;
            /*
            attemptReconnectTimer = new System.Timers.Timer();
            attemptReconnectTimer.Interval = attemptReconnectTimerTime;
            attemptReconnectTimer.AutoReset = false;
            attemptReconnectTimer.Elapsed += new ElapsedEventHandler(attemptReconnect_Tick);
            attemptReconnectTimer.Start(); 
            */
            connected = false;
            running = true;

            /*
             waitBeforePullingDataAfterConnectionTimer = new System.Timers.Timer();
            waitBeforePullingDataAfterConnectionTimer.Interval = waitBeforePullingDataAfterConnectionTimerTime;
            waitBeforePullingDataAfterConnectionTimer.AutoReset = false;
            waitBeforePullingDataAfterConnectionTimer.Elapsed += new ElapsedEventHandler(waitTimeCompleted);
            */
            okayToPullData = false;
            waitingToPullData = true;
        }

        public DepthDataNetworkClient(string fileSource)
        {
            running = true;

            if (Directory.Exists(fileSource))
            {
                this.fileSource = fileSource;
                dataFiles = Directory.GetFiles(fileSource, "*.dat");
                dataFileIndex = 0;
                fileReadTimer = new System.Threading.Timer(ReadFile, null, 1000, 30);
                running = true;
            }

        }

        private bool connectToStream()
        {
            bool valueToReturn = false;
            //safely clean up old client
            try
            {
                if (tcpClient != null) tcpClient.Close();
            }
            catch (Exception e)
            {
                if (debugMode)
                {
                    //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] exception cleaning up old stream " +
                    //                                   e.Message);
                    UnityEngine.Debug.Log("[NetworkClient] exception cleaning up old stream " +
                                          e.Message);
                    /*EventLog.WriteEntry(EVENTLOG_SOURCENAME,
                        "[PACKETWRITER] exception cleaning up old stream " + e.Message,
                        EventLogEntryType.Information);*/
                }
            }

            //attempt to connect to client
            try
            {

                //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] new tcpclient");
                UnityEngine.Debug.Log("[NetworkClient] new tcpclient");
                tcpClient = new TcpClient();
                serverEndPoint = new IPEndPoint(IPAddress.Parse(connectionIP), connectionPort);

                //flush socket immediately when it is closed
                tcpClient.LingerState = new LingerOption(true, 0);
                //send data as soon as possible
                tcpClient.NoDelay = true;
                tcpClient.Connect(serverEndPoint);

                SocketState s = new SocketState();
                s.networkClient = this;
                //wow this looks odd, there has to be a more elegant way
                s.currentAsyncResult = s.networkClient.tcpClient.Client.BeginReceive(s.tcp_buffer, 0,
                    SocketState.TCP_BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), s);

                valueToReturn = true;
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] exception while connecting: " + e.Message);
                UnityEngine.Debug.Log("[NetworkClient] exception while connecting: " + e.Message);
                valueToReturn = false;
            }

            return valueToReturn;
        }

        public bool checkConnection()
        {
            return okayToPullData;
        }
        
        private IEnumerator myRoutine;
        private bool attemptReconnect()
        {
            //if (!okayToPullData) return false;
            if (!running) return false;
            
            //***************
            // can't do coroutine here because this is not a monoehavior
            
            //Coroutine myroutine = new Coroutine();
            //myRoutine = connectToStreamThreaded(0.0f);
            //yield return StartCoroutine("myRoutine");
            //StartCoroutine("myRoutine");
            
            if (connectToStream())
            //if (true)
            {
                if (debugMode)
                {
                    /*
                    EventLog.WriteEntry(EVENTLOG_SOURCENAME, "[PACKETWRITER] socket connected",
                        EventLogEntryType.Information); */
                    //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] attemptReconnect: connected to port");
                    UnityEngine.Debug.Log("[NetworkClient] attemptReconnect: connected to port");
                }


                //okayToPullData = false;
                //waitBeforePullingDataAfterConnectionTimer.Start();

                //ensure the socket will get written to next time we try and send
                connected = true;
                missedDataFrameCount = 0; // this is performed in waitTimeCompleted;

                //stop trying to connect 
                //attemptReconnectTimer.Stop(); // this is already performed in  attemptReconnect_Tick
                return true;
            }
            else
            {
                //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] attemptReconnect: failed to connect");
                UnityEngine.Debug.Log("[NetworkClient] attemptReconnect: failed to connect");
                connected = false;
                //okayToPullData = false;
                if (running)
                {
                    //attemptReconnectTimer.Stop(); // this is already performed in  attemptReconnect_Tick
                    //attemptReconnectTimer.Start();
                }

                return false;
            }

            return connected;
        }

        private void waitTimeCompleted(object sender, EventArgs e)
        {
            doWaitTimeCompleted();
        }
        private void doWaitTimeCompleted()
        {
            //Console.WriteLine("pulling wait completed");
            UnityEngine.Debug.Log("[NetworkClient]pulling wait completed");
            //waitBeforePullingDataAfterConnectionTimer.Stop();
            //attemptReconnectTimer.Stop();
            connected = true;
            missedDataFrameCount = 0;
            //missedDataFrameCount = 0;
            okayToPullData = true;
            waitingToPullData = false;
        }
        private void attemptReconnect_Tick(object sender, EventArgs e)
        {
            doAttemptReconnect();

        }
        
        private void doAttemptReconnect()
        {
            try
            {
                okayToPullData = false;
                //attemptReconnectTimer.Stop();
                //System.Diagnostics.Trace.WriteLine("[PACKETWRITER] attemptReconnect_Tick... ");
                UnityEngine.Debug.Log("[NetworkClient] attemptReconnect_Tick...");
                if (!attemptReconnect())
                {
                    //attemptReconnectTimer.Start();
                    connectCountDownTimer = 0; // reset the timer
                }
                else
                {
                    waitToPullDataCounter = 0;
                    waitingToPullData = true;
                    //okayToPullData = false;
                    //waitBeforePullingDataAfterConnectionTimer
                    //    .Start(); // this will set the okaytopulldata true in a moment...

                }
            }
            catch
            {
                //attemptReconnectTimer.Stop();
                //System.Diagnostics.Trace.WriteLine(
                //    "[PACKETWRITER] error attempting reconnect. timer has likely been disposed (app closing)");
                UnityEngine.Debug.Log("[NetworkClient] error attempting reconnect. timer has likely been disposed (app closing)");
            }
        }
        /* 
         * DATA PROCESSING
         */
        private void ProcessFrame(byte[] packet, int finalsize)
        {

            lock (lastfullpacketLock)
            {
                //System.Diagnostics.Trace.WriteLine("process frame");
                int newPacketLength = packet.Length;

                lastCompressedPacketFinalSize = finalsize;
                //lastCompressedPacket = new byte[packet.Length];
                //Array.Copy(packet, lastCompressedPacket, lastCompressedPacket.Length);
                lastCompressedPacket = new byte[lastCompressedPacketFinalSize];
                Array.Copy(packet, lastCompressedPacket, lastCompressedPacketFinalSize);

                hasdata = true;
            }

        }

        private bool UnpackLastFrame()
        {
            bool valueToReturn = false;
            if (!hasdata || processing) return false;

            processing = true;
            hasdata = false;

            //System.Diagnostics.Trace.WriteLine("Unpack last frame");
            //UnityEngine.Debug.Log("[NetworkClient] unpacking last frame of data ");
            try
            {
                // no compression:
                lock (this.lastfullpacketLock)
                {
                    this.lastfullpacket = new byte[this.lastCompressedPacketFinalSize];
                    //Array.Copy(this.lastCompressedPacket, this.lastfullpacket, this.lastfullpacket.Length);
                    Array.Copy(this.lastCompressedPacket, this.lastfullpacket, this.lastCompressedPacketFinalSize);
                }

                //read out pointPosition array
                int offset = 0;
                int arraylen = 0;
                string tag = "";
                int tagsize = Encoding.ASCII.GetByteCount("np");

                // user mesh ranges
                tag = Encoding.ASCII.GetString(this.lastfullpacket, offset, tagsize);
                offset += tagsize;
                if (tag == "kd")
                {
                    arraylen = BitConverter.ToInt32(this.lastfullpacket, offset);
                    offset += 4;

                    userMeshRange = new float[arraylen / 4];
                    int index = 0;
                    for (int i = 0; i < arraylen; i += 4)
                    {
                        userMeshRange[index] = BitConverter.ToSingle(lastfullpacket, offset + i);
                        index++;
                    }

                    offset += arraylen;
                }


                //do it again for user mesh indexes (user mesh)
                tag = Encoding.ASCII.GetString(this.lastfullpacket, offset, tagsize);
                offset += tagsize;
                if (tag == "kd")
                {
                    arraylen = BitConverter.ToInt32(this.lastfullpacket, offset);
                    offset += 4;

                    whichUserMeshIndexList = new int[arraylen / 4];
                    int index = 0;
                    for (int i = 0; i < arraylen; i += 4)
                    {
                        whichUserMeshIndexList[index] = BitConverter.ToInt32(lastfullpacket, offset + i);
                        index++;
                    }

                    offset += arraylen;
                }

                // user edge ranges
                tag = Encoding.ASCII.GetString(this.lastfullpacket, offset, tagsize);
                offset += tagsize;
                if (tag == "kd")
                {
                    arraylen = BitConverter.ToInt32(this.lastfullpacket, offset);
                    offset += 4;

                    userEdgeRange = new float[arraylen / 4];
                    int index = 0;
                    for (int i = 0; i < arraylen; i += 4)
                    {
                        userEdgeRange[index] = BitConverter.ToSingle(lastfullpacket, offset + i);
                        index++;
                    }

                    offset += arraylen;
                }

                //do it again for user edge indexes (user edge)
                tag = Encoding.ASCII.GetString(this.lastfullpacket, offset, tagsize);
                offset += tagsize;
                if (tag == "kd")
                {
                    arraylen = BitConverter.ToInt32(this.lastfullpacket, offset);
                    offset += 4;

                    whichUserEdgeIndexList = new int[arraylen / 4];
                    int index = 0;
                    for (int i = 0; i < arraylen; i += 4)
                    {
                        whichUserEdgeIndexList[index] = BitConverter.ToInt32(lastfullpacket, offset + i);
                        index++;
                    }

                    offset += arraylen;
                }

                valueToReturn = true;
            }
            catch (Exception e)
            {
                //System.Diagnostics.Trace.WriteLine("[NETWORK CLIENT] invalid package: " + e.Message);
                UnityEngine.Debug.Log("[NetworkClient] invalid package: " + e.Message);

                valueToReturn = false;
            }

            processing = false;
            return valueToReturn;
        }

        /* 
         * NETWORK SOURCE
         */
        public static void ReceiveCallback(IAsyncResult ar)
        {
            //if (!okayToPullData)
            //    return;

            //System.Diagnostics.Trace.WriteLine("[NETWORK CLIENT] ReceiveCallback");
            try
            {
                SocketState s = (SocketState) (ar.AsyncState);

                if (ar != s.currentAsyncResult) return;
                s.currentAsyncResult = null;

                IPEndPoint e = s.endPoint;
                //if (e == null) // this caused it to never connect to good connection...
                //    return;

                if (!s.networkClient.running)
                {
                    s.Reset();
                    return;
                }

                int recv = 0;
                try
                {
                    recv = s.networkClient.tcpClient.Client.EndReceive(ar);
                }
                catch (SocketException sE)
                {
                    //System.Diagnostics.Trace.WriteLine("error reading start listening");
                    UnityEngine.Debug.Log("[NetworkClient] error reading start listening");
                    s.networkClient.tcpClient.Close();
                    s.networkClient.connected = false;
                    return;
                }
                catch (ObjectDisposedException oE)
                {
                    //System.Diagnostics.Trace.WriteLine("socket was closed");
                    UnityEngine.Debug.Log("[NetworkClient] socket was closed");
                    s.networkClient.tcpClient.Close();
                    s.networkClient.connected = false;
                    return;
                }

                if (recv == 0)
                {
                    s.networkClient.tcpClient.Close();
                    s.networkClient.connected = false;
                    return;
                }

                s.readcount += 1;

                byte[] buff = new byte[recv];
                Array.Copy(s.tcp_buffer, buff, recv);

                int recvOffset = 0;

                //System.Diagnostics.Trace.WriteLine("data " + buff.Length + " " + s.packsize + " " + s.totalread);

                //check for new start tag, assumes tag is at front of buffer, we need to start this way to force proper alignement of packets.
                //start tag being in the middle is taken care of later down this function
                string tag = "";
                if (recv >= s.taglength) tag = Encoding.ASCII.GetString(buff, 0, s.taglength);
                if (tag == "np")
                {
                    //s.stopWatch.Stop();
                    //System.Diagnostics.Trace.WriteLine("new packet took " + s.stopWatch.ElapsedTicks / (Stopwatch.Frequency / 1000F));
                    //s.stopWatch.Reset();

                    //s.stopWatch.Start();
                    if (s.totalread != 0)
                    {
                        //System.Diagnostics.Trace.WriteLine("--- Packet dropped " + (s.packsize - s.totalread));
                        UnityEngine.Debug.Log("[NetworkClient]--- Packet dropped " + (s.packsize - s.totalread));
                    }
                    //System.Diagnostics.Trace.WriteLine("Last Packet, " + s.packsize + " " + s.totalread + " " + s.readcount);

                    s.readcount = 1;

                    //get the compressed data size so we know when this "frame" will end
                    recvOffset = s.taglength;
                    s.packsize = BitConverter.ToInt32(buff, recvOffset);
                    recvOffset += 4;

                    //get the full, unpacked size so we know how large to make the final buffer
                    s.fullsize = BitConverter.ToInt32(buff, recvOffset);
                    recvOffset += 4;

                    //System.Diagnostics.Trace.WriteLine("New Packet, current packsize " + s.packsize + " " + s.fullsize);

                    //copy frame data if there is any
                    s.packet = new byte[s.packsize];
                    int rest = recv - recvOffset;
                    Buffer.BlockCopy(buff, recvOffset, s.packet, 0, (rest > s.packsize ? s.packsize : rest));
                    s.totalread = recv - recvOffset;


                    //full frame has been received
                    if (s.totalread >= s.packet.Length)
                    {
                        //s.stopWatch.Stop();
                        //System.Diagnostics.Trace.WriteLine("Full packet compressed " + s.totalread + " " + s.readcount + " RunTime " + //s.stopWatch.ElapsedTicks / (Stopwatch.Frequency / 1000F));

                        //s.stopWatch.Reset();
                        //s.stopWatch.Start();

                        s.networkClient.ProcessFrame(s.packet, s.fullsize);

                        //s.stopWatch.Stop();
                        //System.Diagnostics.Trace.WriteLine("Full packet unpacked " + s.totalread + " RunTime " + //s.stopWatch.ElapsedTicks / (Stopwatch.Frequency / 1000F));

                        //s.stopWatch.Reset();
                        s.totalread = 0;
                        s.packet = new byte[2];

                        //s.stopWatch.Start();
                    }


                }
                else if (s.packet.Length > 2 && s.packsize != -1 && recv > 14) // NEVER GETS HERE
                {

                    //System.Diagnostics.Trace.WriteLine("more data left in packet " + recv + " " + s.totalread);
                    //UnityEngine.Debug.Log("[NetworkClient] more data left in packet " + recv + " " + s.totalread);

                    //we are receiving compressed frame data, if the current packet has more data than a frame can hold
                    //copy the current frame's data and start a new frame
                    if (s.totalread + recv > s.packet.Length)
                    {
                        //System.Diagnostics.Trace.WriteLine(
                        //    "End reached mid packet discard " + ((s.totalread + recv) - s.packet.Length));
                        UnityEngine.Debug.Log("[NetworkClient] End reached mid packet discard " + ((s.totalread + recv) - s.packet.Length));
                        s.totalread = 0;
                        s.packet = new byte[2];
                    }
                    else
                    {
                        //copy data to frame packet
                        Buffer.BlockCopy(buff, 0, s.packet, s.totalread, recv);

                        //update buffer index pointers
                        s.totalread += recv;

                        //full frame has been received
                        if (s.totalread >= s.packet.Length)
                        {
                            //s.stopWatch.Stop();
                            //System.Diagnostics.Trace.WriteLine("Full packet compressed " + s.totalread + " " + s.readcount + " RunTime " + s.stopWatch.ElapsedTicks / (Stopwatch.Frequency / 1000F));
                            //s.stopWatch.Reset();
                            //s.stopWatch.Start();

                            s.networkClient.ProcessFrame(s.packet, s.fullsize);

                            //s.stopWatch.Stop();
                            //System.Diagnostics.Trace.WriteLine("Full packet unpacked " + s.totalread + " RunTime " + s.stopWatch.ElapsedTicks / (Stopwatch.Frequency / 1000F));

                            //s.stopWatch.Reset();
                            s.totalread = 0;
                            s.packet = new byte[2];

                            //s.stopWatch.Start();
                        }
                    }
                }
                else
                {
                    //System.Diagnostics.Trace.WriteLine("invalid data?");
                    UnityEngine.Debug.Log("[NetworkClient] invalid data?");
                }

                //start reading again
                //u.BeginReceive(new AsyncCallback(ReceiveCallback), s);

                s.currentAsyncResult = s.networkClient.tcpClient.Client.BeginReceive(s.tcp_buffer, 0,
                    SocketState.TCP_BUFFER_SIZE, 0, new AsyncCallback(ReceiveCallback), s);
            }
            catch
            {
                // failed to connect to a bad connection
            }
        }

        #region raadFromFile

        

        /*
         * FILE SOURCE
         *.
         */
        public void ReadFile(System.Object stateInfo)
        {
            if (!running || reading) return;
            reading = true;

            string path = dataFiles[dataFileIndex];

            FileStream f = new FileStream(path, FileMode.Open, FileAccess.Read);

            byte[] buff = new byte[f.Length];
            f.Read(buff, 0, buff.Length);

            int recvOffset = 0;

            //check for new start tag, assumes tag is at front of buffer, we need to start this way to force proper alignement of packets.
            //start tag being in the middle is taken care of later down this function

            int taglength = Encoding.ASCII.GetByteCount("KinectData");

            string tag = Encoding.ASCII.GetString(buff, 0, taglength);
            if (tag == "KinectData")
            {
                //get the compressed data size so we know when this "frame" will end
                recvOffset = taglength;
                int packsize = BitConverter.ToInt32(buff, recvOffset);
                recvOffset += 4;

                //get the full, unpacked size so we know how large to make the final buffer
                int fullsize = BitConverter.ToInt32(buff, recvOffset);
                recvOffset += 4;

                //copy frame data if there is any
                byte[] packet = new byte[packsize];
                int rest = buff.Length - recvOffset;
                Buffer.BlockCopy(buff, recvOffset, packet, 0, rest);
                ProcessFrame(packet, fullsize);
            }

            dataFileIndex++;
            if (dataFileIndex >= dataFiles.Length) dataFileIndex = 0;

            reading = false;
        }
        
        #endregion raadFromFile

        public void Close()
        {
            running = false;

            if (tcpClient != null) tcpClient.Close();
            connected = false;
            /*
            attemptReconnectTimer.Elapsed -= new ElapsedEventHandler(attemptReconnect_Tick);
            attemptReconnectTimer.Stop();
            attemptReconnectTimer.Enabled = false;
            attemptReconnectTimer.Dispose();

            waitBeforePullingDataAfterConnectionTimer.Elapsed -= new ElapsedEventHandler(waitTimeCompleted);
            waitBeforePullingDataAfterConnectionTimer.Stop();
            waitBeforePullingDataAfterConnectionTimer.Enabled = false;
            waitBeforePullingDataAfterConnectionTimer.Dispose();
            */
            if (fileReadTimer != null) fileReadTimer.Dispose();
        }

        ~DepthDataNetworkClient()
        {
            Close();
        }
    }

    /// <summary>
    /// UdpState stores the socket state while receiving network data
    /// </summary>
    public class SocketState
    {
        public IPEndPoint endPoint; //reference to ip endpoint
        public DepthDataNetworkClient networkClient; //reference to "parent"

        public const int TCP_BUFFER_SIZE = 8192 * 16;
        public byte[] tcp_buffer = new byte[TCP_BUFFER_SIZE];

        public int packsize = -1; //size of compressed full frame
        public int fullsize = -1; //size of compressed full frame
        public int readcount = 0;

        public int totalread = 0; //total amount of bytes read for current frame
        public byte[] packet = new byte[2]; //compressed packet

        public int taglength = Encoding.ASCII.GetByteCount("np");

        public IAsyncResult currentAsyncResult;

        public Stopwatch stopWatch = new Stopwatch(); //stopwatch is use to measure performance

        public void Reset()
        {
            if (networkClient != null) networkClient.hasdata = false;

            tcp_buffer = new byte[TCP_BUFFER_SIZE];

            packsize = -1; //size of compressed full frame
            fullsize = -1; //size of compressed full frame
            readcount = 0;

            totalread = 0; //total amount of bytes read for current frame
            packet = new byte[2]; //compressed packet
        }


    }

}