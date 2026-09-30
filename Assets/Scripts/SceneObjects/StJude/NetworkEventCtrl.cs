using System;
using UnityEngine;
using AAMVC.Unity; // for connection to appcontroller
using AAMVC.CommunicationsAndControl; // for networking code

public class NetworkEventCtrl : MonoBehaviour
{
    private InterComputerClient interComputerClient;
    private ApplicationControl appControl;
    //private UdpConnection connection;


    //private string commsIPAddress = "127.0.0.1"; // where is the sensor control broadcasting on THIS DOESNT WORK
    private string commsIPAddress = "224.1.1.1"; // where is the sensor control broadcasting on

    //private string commsIPAddress = "192.168.1.22"; // where is the sensor control broadcasting on
    //private string commsIPAddress = "192.168.1.49"; // where is the sensor control broadcasting on THIS DIRECTED DOESNT WORK
    //private int CommsPortUdp = 5309; // only used tfor UDP mode

    private int commsPortIntercomputer = 8675; // used with intercomputerclient

    private string transmitSignatureToUse = "myApp";

    //private string wallControlIP = "192.68.50.52";
    //private int wallControlPort = 8675;
    private bool ignoreEventsUntilInitialized = true;
    private string applicationIPAddress;

    //private ApplicationControl AppController;

    //private bool doUseInterCompClient = true;

    //public bool sensorApplicationStatus = false; // has sensor been tripped or not?
    //private int sensorActivatedCounter = 0;
    private bool doDisableApplicationStatusWhenReady = false;

    public string lastCommandFromUDP = "NAN";

    // **************************************
    // GET/SET has state changed?
    // **************************************
    private bool dirty = false;

    public bool isDirty()
    {
        bool valueToReturn = dirty;
        if (dirty)
        {
            dirty = false;
        }

        return valueToReturn;
    }

    private void Start()
    {
        appControl = ApplicationControl.Instance;
    }

    public void initializeNetworkConnection(string whichIP, int whichPort, string whichSignature)
    {
        commsIPAddress = whichIP;
        commsPortIntercomputer = whichPort;
        transmitSignatureToUse = whichSignature;

        initCommunicationsWithComputers();
        interComputerClient.sendConnectedStatus();
    }

    public void onProgramExit()
    {
        //connection.Stop();
        //if (doUseInterCompClient)
        //{
        interComputerClient.sendDisconnectedStatus();
        interComputerClient.onExit();
        //}
        //else
        //{
        //    connection.Stop();
        //}
    }


    private void initCommunicationsWithComputers()
    {
        interComputerClient = new InterComputerClient(commsIPAddress, commsPortIntercomputer, transmitSignatureToUse);
        interComputerClient.OnCommand += new ClientCommandEventHandler(interComputerClient_OnCommand);


        ignoreEventsUntilInitialized = false;
    }

    void Update()
    {

        //checkActivationStatus();

        /*
        if (sensorApplicationStatus)
        {
            sensorActivatedCounter += 1;
            if (doDisableApplicationStatusWhenReady)
            {
                if (sensorActivatedCounter > 60)
                {
                    doDisableApplicationStatusWhenReady = false;
                    dirty = true;
                    sensorApplicationStatus = false;
                }
            }
        }*/
    }

    public void requestCurrentModeFromOrchestrator()
    {
        interComputerClient.sendRequestForCurrentState();
    }
    
    public void transmitNewState(string whichState)
    {
        interComputerClient.sendNewState(whichState);
    }
    
    public void transmitStartRunOfShow()
    {
        interComputerClient.sendNewState("runOfShow"); // ambient, runOfShow, audioTest
    }

    public void transmitHaltAudio()
    {
        interComputerClient.sendNewState("haltAllAudio"); // ambient, runOfShow, audioTest
    }

    public void transmitStartAudioTest()
    {
        interComputerClient.sendNewState("audioTest"); // ambient, runOfShow, audioTest
    }

    public void transmitStartAmbient()
    {
        interComputerClient.sendNewState("ambient"); // ambient, runOfShow, audioTest
    }

    public void sendNewStoryInFocus(int whichStoryID)
    {
        interComputerClient.sendNewStoryInFocus(whichStoryID);
    }

    public void sendStoryMotionPct(float whichPct)
    {
        interComputerClient.sendStoryInMotionPct(whichPct);
    }

    void interComputerClient_OnCommand(object sender, ClientCommandEventsArgs e)
    {
        if (ignoreEventsUntilInitialized)
            return;

        //System.Diagnostics.Debug.WriteLine("[SENSORLISTENER] Client sender =[" + e.source + "] method =[" + e.method + "] args =[" + e.args + "]");
        int whichRegion = -1;

        lastCommandFromUDP = "[NETWORKLISTENER] Client sender =[" + e.source + "] method =[" + e.method + "] args =[" + e.args + "]";


        /*
        if (e.args != null)
        {
            try
            {
                whichRegion = Convert.ToInt16(e.args);
            }
            catch (Exception exc)
            {
                // which region failed to read
                //throw;
            }

        }
        */



        switch (e.method)
        {
            case "updateStoryOffset":
                //udpateDragOffset(e.args);
                break;
            case "lockedOntoStory":
                //udpateStoryTo(e.args);
                break;
            case "sensing": // adding special case for presence sensor: controlSystem,sensing,on 
                updateFromMotionSensor(e.args);
                break;
            case "connected":
                if (e.source == "orchestrator")
                    interComputerClient.sendConnectedStatus();
                //appControl.updateConnectedAppStatus(e.source, true);
                break;
            case "setStateTo":
                if (e.source == "orchestrator")
                    appControl.adjustCurrentStateWithStringFromNetwork(e.args);
                //appControl.updateConnectedAppStatus(e.source, true);
                break;
            case "nextPresets":
                if (e.source == "orchestrator")
                    appControl.proceedToNextPresetFromNetwork(e.args);
                break;
        }

    }

    /*
    public void manuallyToggleSensor(bool whichStatus)
    {
        if (whichStatus)
        {
            updateFromMotionSensor("on");
        }
        else
        {
            updateFromMotionSensor("off");
        }
    }
    */

    

    private void updateFromMotionSensor(string whichNewStatus)
    {
        /*
        switch (whichNewStatus)
        {
            case "on":
            case "ON":
            case "On":
                dirty = true;
                doDisableApplicationStatusWhenReady = false;
                sensorActivatedCounter = 0; // reset counter
                sensorApplicationStatus = true;
                break;
            case "off":
            case "OFF":
            case "Off":
                //dirty = true;
                disableApplicationStatus();
                //sensorApplicationStatus = false;
                break;
        }*/
    }

    private void disableApplicationStatus()
    {
        /*
        if (sensorActivatedCounter > 60) // has to be on for min time
        {
            dirty = true;
            sensorApplicationStatus = false;
        }
        else // sensor has not been activated very long - wait
        {
            doDisableApplicationStatusWhenReady = true;
        }*/
    }
}
