using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArtnetForUnity;
using TMPro;


public class ArtnetCtrl : MonoBehaviour
{
    private AAMVC.Unity.LogTextCtrl logTextCtrl;
    public TMP_Text artnetStatusText;
    
    
    //public int dmxUniverse = 0;
    private int numberOfUnityUniverses = 1;
    
    public bool isConnected = false;
    private int killConnectionTimer = 0;
    private bool isKillingConnection = false;
    public bool useManualInput = false;
    private ArtnetForUnity.ArtnetManager artnetManager;
    private static int numberOfData = 512;

    //private static int actualNumberOfUniverses = 4;
    private byte[] _data0 = new byte[numberOfData];
    private byte[] _dataToSend0 = new byte[numberOfData];
    private byte[] _data1 = new byte[numberOfData];
    private byte[] _dataToSend1 = new byte[numberOfData];
    private byte[] _data2 = new byte[numberOfData];
    private byte[] _dataToSend2 = new byte[numberOfData];
    private byte[] _blankWall = new byte[numberOfData];
    [SerializeField]
    private byte[] dmxChannels = new byte[numberOfData];

    private bool dataIsDirty = false;
    
    private bool currentConnectionStatus = false;
    private bool prevConnectionStatus = false;

    
    private void Awake()
    {
        logTextCtrl = AAMVC.Unity.LogTextCtrl.Instance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        useManualInput = false;
        for (int i = 0; i < numberOfData; i++)
        {
            _data0[i] = 0;
            _data1[i] = 0;
            _data2[i] = 0;
            _blankWall[i] = 0;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (!isConnected)
            return;

        if (isKillingConnection)
        {
            killConnectionTimer += 1;
            if (killConnectionTimer > 180)
            {
                isKillingConnection = false;
                finishKillingArtnetConnection();
            }
        }
        
        if (useManualInput)
        {
            _data0 = dmxChannels;
            sendDataToArtnet();
        }
        else
        {
            if (dataIsDirty)
            {
                dataIsDirty = false;
                //sendDataToArtnet();
            }
            sendDataToArtnet();
        }
        
        currentConnectionStatus = isConnected;
        if (currentConnectionStatus!=prevConnectionStatus)
        {
            prevConnectionStatus = currentConnectionStatus;
            updateFeedbackWindow();
        }
    }

    private void updateFeedbackWindow()
    {
        ArtnetSettings settings = artnetManager.retreiveArtNetSettings();
        
        string textToReport = "Artnet Output Settings:\n";

        if (settings != null)
        {

            numberOfUnityUniverses = settings.artnetOutputs.Count;

            textToReport += "Local IP: " + settings.IPAddress + "\n";
            textToReport += "Local Network Interface ID: " + settings.InterfaceName + "\n";
            textToReport += "ConnectionStatus: " + currentConnectionStatus.ToString() + "\n";

            int i, j;
            int numberOfIPAddresses = 0;

            textToReport += "-----------\n";

            for (i = 0; i < numberOfUnityUniverses; ++i)
            {
                textToReport += "Artnet Connection ID: " + i + "\n";
                textToReport += "Artnet DMX Universe: " + settings.artnetOutputs[i].DMXUniverse + "\n";
                numberOfIPAddresses = settings.artnetOutputs[i].NodeRevcIPAddress.Count;
                for (j = 0; j < numberOfIPAddresses; ++j)
                {
                    textToReport += "Artnet Node Recv IP: " + settings.artnetOutputs[i].NodeRevcIPAddress[j] + "\n";
                }
                textToReport += "-----------\n";
            }
        }
        else
        {
            textToReport += "(info unavailable) \n";
        }

        artnetStatusText.text = textToReport;

    }
    
    private void sendDataToArtnet()
    {
        Array.Copy(_data0, _dataToSend0, numberOfData);
        artnetManager.SetArtnetData(0, _dataToSend0);
        
        //if (numberOfUnityUniverses > 1)
        //{
            Array.Copy(_data1, _dataToSend1, numberOfData);
            artnetManager.SetArtnetData(1, _dataToSend1);
        //}

        //if (numberOfUnityUniverses > 2)
        //{
            Array.Copy(_data2, _dataToSend2, numberOfData);
            artnetManager.SetArtnetData(2, _dataToSend2);
        //}
    }

    public void toggleManualInput()
    {
        useManualInput = !useManualInput;

        logTextCtrl.logText("[ARTNET] switched manual mode to " + useManualInput, true);
    }

    
    public void toggleConnection()
    {
        if (isConnected)
        {
            logTextCtrl.logText("[ARTNET] killing artnet connection ", true);
            artnetManager.Stop();
            killConnectionTimer = 0;
            isKillingConnection = true;
        }
        else
        {
            initAndConnectArtNet();
        }
    }

    private void finishKillingArtnetConnection()
    {
        logTextCtrl.logText("[ARTNET] shes dead, Jim. ", true);

        artnetManager.Dispose();
        artnetManager = null;
        isConnected = false;
    }
    
    public void updateArtnetDataChannel(int whichUnityUniverse, int whichChannel, byte incomingData)
    {
        //int actualChannel = whichChannel + 2;
        int actualChannel = whichChannel;
        //if (incomingData == 0)
        //    incomingData = 1; // minimum value to send is 1 (but at 1, light is still leaking through?)
        //if (incomingData < 2)
        //    incomingData = 0;
        if (whichUnityUniverse == 0)
        {
            if (_data0[actualChannel] != incomingData)
            {
                _data0[actualChannel] = incomingData;
                dataIsDirty = true;
            }
        }
        else if (whichUnityUniverse == 1)
        {
            if (_data1[actualChannel] != incomingData)
            {
                _data1[actualChannel] = incomingData;
                dataIsDirty = true;
            }
        }
        else if (whichUnityUniverse == 2)
        {
            if (_data2[actualChannel] != incomingData)
            {
                _data2[actualChannel] = incomingData;
                dataIsDirty = true;
            }
        }
    }

    private void sendBlank()
    {
        if (artnetManager != null)
        {
            for (int i = 0; i < numberOfUnityUniverses; ++i)
            {
                artnetManager.SetArtnetData(i, _blankWall);
            }
        }
            
    }
    
    public void initAndConnectArtNet()
    {
        //dmxUniverse = whichUniverse;
        logTextCtrl.logText("[ARTNET] initAndConnectArtNet ", true);

        artnetManager = new ArtnetForUnity.ArtnetManager();
        artnetManager.Start();
        isConnected = true;
    }

    public void onProgramExit()
    {
        sendBlank();
        
        if (artnetManager != null)
        {
            artnetManager.Stop();
            artnetManager.Dispose();
        }

    }
}
