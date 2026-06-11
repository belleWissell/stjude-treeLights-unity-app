using System.Collections;
using System.Collections.Generic;
//using UnityEngine;
using System.Text; // encoding
using System;
using System.Net; // bitencoder

using AAMVC.CommunicationsAndControl;

public class TransmitDataToDMX
{
    private DataPacketWriter dataWriterVar;
    private bool activateDataSream = false; // global switch from config to activate or not
    public bool connectionStatus = false;
    public int connectionCount = 0;

    private int sendPort;
    private string sendIP;
    
    private static int numberOfDMXValues = 1024;
    private int[] dmxLightValues = new int[numberOfDMXValues];
    private int currentNumberOfValuesToSend = 0;

    public TransmitDataToDMX(string whichIP, int whichPort, bool whichDoConnect)
    {
        sendPort = whichPort;
        sendIP = whichIP;

        activateDataSream = whichDoConnect;
        initDMXValues();
        initDepthTransmit(sendIP, sendPort);
    }
    
    private void initDMXValues()
    {
        for (int i = 0; i < numberOfDMXValues; ++i)
        {
            dmxLightValues[i] = 0;
        }
    }
    
    public void initDepthTransmit(string whichIP, int whichPort)
    {
        if (activateDataSream)
        {
            // setup to use packet writer only:
            if (dataWriterVar == null) // only start if we haven't already
                dataWriterVar = new DataPacketWriter(whichIP, whichPort);
        }
    }

    public void haltingProgram()
    {
        if (activateDataSream)
        {
            if (dataWriterVar != null)
                dataWriterVar.halt();
        }
    }


    // ************************************************************

    #region sendData

    public int getConnectionStatus()
    {
        if (dataWriterVar != null)
        {
            connectionStatus = dataWriterVar.IsConnected();
            connectionCount = dataWriterVar.GetConnectedClientCount();
            return connectionCount;
        }
        else
            return 0;
    }

    public void sendNewDMXValues(int[] whichNewValues, int howManyValues, int offset)
    {
        currentNumberOfValuesToSend = howManyValues;
        Array.Copy(whichNewValues, dmxLightValues, currentNumberOfValuesToSend);
        transmitData();
    }
    
    private void transmitData()
    {

        //connectionStatus = depthDataWriterVar.isConnected;
        //  check against min transmit rate (don't overwhelm connection) 
        // note: rate of transmission is controlled from cameraControl updateDepthData_Tick (Main class: private static int depthDataTransmitRate)
        /*if (DateTime.UtcNow.Ticks - lastTransmitTick < (TimeSpan.TicksPerMillisecond * minTransmitInterval))
        {
            return;
        }*/

        // create local arrays which are the exact length needed:
        /*
        int localValidUserMeshPointCounter = validUserMeshPointCounter;
        float[] localUserMeshRange = new float[localValidUserMeshPointCounter];
        int[] localUserMeshPosition = new int[localValidUserMeshPointCounter * 2];

        Array.Copy(userMeshDepthForTransmit, localUserMeshRange,
            localValidUserMeshPointCounter); // raw depth data which is not 0 or max range
        Array.Copy(userMeshIndexes, localUserMeshPosition,
            localValidUserMeshPointCounter * 2); // address for each  of the depth points sent

        int localValidUserEdgePointCounter = validUserEdgePointCounter;
        float[] localUserEdgeRange = new float[localValidUserEdgePointCounter];
        int[] localUserEdgePosition = new int[localValidUserEdgePointCounter * 2];

        Array.Copy(userEdgeDepthForTransmit, localUserEdgeRange,
            localValidUserEdgePointCounter); // raw depth data which is not 0 or max range
        Array.Copy(userEdgeIndexes, localUserEdgePosition,
            localValidUserEdgePointCounter * 2); // address for each  of the depth points sent
        */
        
        int[] localLightValueData = new int[currentNumberOfValuesToSend];
        
        Array.Copy(dmxLightValues, localLightValueData, currentNumberOfValuesToSend);
                    
        //MainApp.MyWindow.update_fp.Start("array_header");
        /*if (doEnableLocalFileInterrop)
        {
            if (playAndRecordDataFromFileVar.isRecording)
            {
                playAndRecordDataFromFileVar.recordFrame(userEdgeDepthForTransmit, userEdgeIndexes,
                    localValidUserEdgePointCounter, userMeshDepthForTransmit, userMeshIndexes,
                    localValidUserMeshPointCounter);
            }
        }*/

        //array header interspersed with data:
        int tagInbetweenData = Encoding.ASCII.GetByteCount("dx");
        byte[] headerInbetweenData = new byte[tagInbetweenData + 4];

        //MainApp.MyWindow.update_fp.Stop();
        //MainApp.MyWindow.update_fp.Start("array_data");

        //array data
        int buffer_size = 0;
        int offset = 0;

        buffer_size += headerInbetweenData.Length * 4;

        byte[] dmxLightValues_data = Utilities.IntArrayToByteArray(localLightValueData, currentNumberOfValuesToSend);
        buffer_size += dmxLightValues_data.Length;

        //byte[] userMeshRange_data = Utilities.FloatArrayToByteArray(localUserMeshRange, localValidUserMeshPointCounter);
        //buffer_size += userMeshRange_data.Length;

        //byte[] userMeshPosition_data =
        //    Utilities.IntArrayToByteArray(localUserMeshPosition, localValidUserMeshPointCounter * 2);
        //buffer_size += userMeshPosition_data.Length;

        //byte[] userEdgeRange_data = Utilities.FloatArrayToByteArray(localUserEdgeRange, localValidUserEdgePointCounter);
        //buffer_size += userEdgeRange_data.Length;

        //byte[] userEdgePosition_data =
        //    Utilities.IntArrayToByteArray(localUserEdgePosition, localValidUserEdgePointCounter * 2);
        //buffer_size += userEdgePosition_data.Length;

        //package that will be sent and compressed
        byte[] byte_data = new byte[buffer_size];

        //pack points
        //offset = packArray(headerInbetweenData, userMeshRange_data, byte_data, offset);
        //offset = packArray(headerInbetweenData, userMeshPosition_data, byte_data, offset);
        //offset = packArray(headerInbetweenData, userEdgeRange_data, byte_data, offset);
        //offset = packArray(headerInbetweenData, userEdgePosition_data, byte_data, offset);

        offset = packArray(headerInbetweenData, dmxLightValues_data, byte_data, offset);
        
        
        byte[] localDataBuffer = byte_data;
        // data compression used to be here.. (determined to be source of instability)

        int tagForWholePackage = Encoding.ASCII.GetByteCount("np");
        byte[] headerForWholePackage = new byte[tagForWholePackage + (sizeof(int) * 2)];

        // this is what is delivered:
        byte[] packageToSend = new byte[localDataBuffer.Length + headerForWholePackage.Length];

        System.Buffer.BlockCopy(Encoding.ASCII.GetBytes("np"), 0, headerForWholePackage, 0, tagForWholePackage);
        System.Buffer.BlockCopy(BitConverter.GetBytes(localDataBuffer.Length), 0, headerForWholePackage,
            tagForWholePackage, sizeof(int));
        System.Buffer.BlockCopy(BitConverter.GetBytes(byte_data.Length), 0, headerForWholePackage,
            tagForWholePackage + 4, sizeof(int));

        System.Buffer.BlockCopy(headerForWholePackage, 0, packageToSend, 0, headerForWholePackage.Length);
        System.Buffer.BlockCopy(localDataBuffer, 0, packageToSend, headerForWholePackage.Length,
            localDataBuffer.Length);


        if (dataWriterVar != null)
        {
            dataWriterVar.SendMeshData(packageToSend);
        }
        //return localValidUserMeshPointCounter;

    }

    private int packArray(byte[] header, byte[] source, byte[] buffer, int offset)
    {

        int tagsize = Encoding.ASCII.GetByteCount("dx");

        // [array] SRC, [int] SRC offset, [array] dst, [int] dstoffset, [int] count

        System.Buffer.BlockCopy(Encoding.ASCII.GetBytes("dx"), 0, header, 0, tagsize);

        System.Buffer.BlockCopy(BitConverter.GetBytes(source.Length), 0, header, tagsize, sizeof(int));
        //Buffer.BlockCopy(BitConverter.GetBytes(source.Length), 0, header, tagsize, 0);

        //copy header into package
        System.Buffer.BlockCopy(header, 0, buffer, offset, header.Length);
        offset += header.Length;
        //copy data into package
        System.Buffer.BlockCopy(source, 0, buffer, offset, source.Length);
        offset += source.Length;

        return offset;

    }

    #endregion sendData

    // ************************************************************

    public class Utilities
    {

        //round about way to convert int array to bytes
        public static byte[] IntArrayToByteArray(int[] arr, int length = -1)
        {
            byte[] raw = new byte[arr.Length * 4];
            if (length == -1) length = arr.Length;

            int offset = 0;
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(arr[i]), 0, raw, offset, 4);
                offset += 4;
            }
            return raw;
        }


        //round about way to convert float array to bytes (same as int to byte array)
        public static byte[] FloatArrayToByteArray(float[] arr, int length = -1)
        {
            byte[] raw = new byte[arr.Length * 4];
            if (length == -1) length = arr.Length;

            int offset = 0;
            for (int i = 0; i < length; i++)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(arr[i]), 0, raw, offset, 4);
                offset += 4;
            }
            return raw;
        }
    }
}

