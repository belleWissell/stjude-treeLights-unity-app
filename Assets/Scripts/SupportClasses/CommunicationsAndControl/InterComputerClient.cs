using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;
using AAMVC.Unity; // for connection to appcontroller
using UnityEngine;

namespace AAMVC.CommunicationsAndControl
{

    public delegate void ClientCommandEventHandler(object sender, ClientCommandEventsArgs e);

    /// <summary>
    /// Arguments that go with the ClientCommandEventHandler
    /// </summary>
    public class ClientCommandEventsArgs : EventArgs
    {
        public string source = "";
        public string method = "";
        public string args = "";
    }
    
    public class InterComputerClient
    {
        // subscribe to this event handler
        public event ClientCommandEventHandler OnCommand;//Data event

        private MulticastSocket socket;

        //private AppController AppController;
        private string myName = "FEapp";
        private bool debugMode = false;
        private string myIPAddress = "192.168.10.30";

        public InterComputerClient(string ip, int port, string name)
        {
            myName = name;
            /*
            if (AppController == null)
            {
                AppController = GetComponent<AppController>();

                if (AppController == null)
                {
                    Debug.Log("[KEYBDCTRL] unable to find appcontroller by getComponent, trying alt...");
                    AppController = FindObjectOfType<AppController>();
                }
            }
            */
            try
            {
                socket = new MulticastSocket();
                socket.OnError += new MulticastSocketEventHandler(socket_OnError);
                socket.OnConnect += new MulticastSocketEventHandler(socket_OnConnect);
                socket.OnData += new MulticastSocketEventHandler(socket_OnData);

                socket.Connect(ip, port, false);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("ERROR initializing socket: " + e.Message);
            }
        }

        public void onExit()
        {
            if (socket != null)
            {
                //socket.onExit();
                socket.Destroy();
            }
        }

        private void socket_OnData(object sender, MulticastSocketEventsArgs e)
        {
            string[] split = e.data.ToString().Split(new Char[] { ',' });

            ClientCommandEventsArgs args = new ClientCommandEventsArgs();

            try // a lot of assumptions being made about the format of incoming data
            {
                args.source = split[0];
                args.method = split[1];
                args.args = split[2];
            }
            catch (Exception exception)
            {
                Console.WriteLine("[INTRCOMPCLIENT] invalid argument received: " + e.data.ToString());
                //throw;
            }
            

            if (this.OnCommand != null)
            {
                OnCommand.BeginInvoke(this, args, null, null);
            }

        }

        private void socket_OnConnect(object sender, MulticastSocketEventsArgs e)
        {
            //if (MainApp.MyWindow.debugMode)
            if (debugMode)
            {
                //MainClass.textBoxLogVar.logText("[interComputer] Socket Connected.");
            }
            sendConnectedStatus();
            System.Diagnostics.Debug.WriteLine("[INTERCOMPCLIENT] socket connected");
        }

        private void socket_OnError(object sender, MulticastSocketEventsArgs e)
        {
            //if (MainApp.MyWindow.debugMode)
            if (debugMode)
            {
                //MainClass.textBoxLogVar.logText("[interComputer] Socket Connect Error.");
            }
            throw new Exception("Inter Computer socket error: "+ e.data);
        }

        public void sendConnectedStatus()
        {
            if (socket != null)
            //socket.Send(MainClass.kioskName + ",connected," + MainClass.kioskIPaddress);
            socket.Send(myName + ",connected," + myIPAddress);
        }

        public void sendConnectedStatusWithIP(string whichIP)
        {
            if (socket != null)
                //socket.Send(MainClass.kioskName + ",connected," + MainClass.kioskIPaddress);
                socket.Send(myName + ",connected," + whichIP);
        }

        public void sendDisconnectedStatus()
        {
            if (socket != null)
                socket.Send(myName + ",disconnected,true");
        }


        public void sendNewState(string whichNewState) // ambient, runOfShow, audioTest
        {
            if (socket != null)
                socket.Send(myName + ",setStateTo,"+whichNewState);
            
        }
        public void sendNewStoryInFocus(int whichStory)
        {
            if (socket != null)
                socket.Send(myName + ",lockedOntoStory,"+whichStory);
        }
        
         
        public void sendStoryInMotionPct(float whichPct)
        {
            if (socket != null)
                socket.Send(myName + ",updateStoryOffset,"+whichPct);
        }
        
        /*
        public void sendResetUserInteraction()
        {
            if (socket != null)
                socket.Send(myName + ",resetUserInteraction,true");
        }



        public void sendRunStatus(bool isRunning)
        {
            if (socket != null)
                socket.Send(myName + ",runStatus," + isRunning);
        }

        public void sendBackgroundReSyncRequest()
        {
            if (socket != null)
                socket.Send(myName + ",reSyncBackgroundAnimations,true");
        }

        public void sendPlayMenuSound()
        {
            string whichAudioFile = "menuSwipe";
            if (socket != null)
                socket.Send(myName + ",playAudio," + whichAudioFile);
        }

        public void sendPlayStepCloserSound()
        {
            string whichAudioFile = "stepCloser";
            if (socket != null)
                socket.Send(myName + ",playAudio," + whichAudioFile);
        }

        public void sendCopyingFilesToLocal()
        {
            if (socket != null)
                socket.Send(myName + ",isUpdatingFiles,true");

        }

        public void sendCopyingFilesToLocalComplete(bool success)
        {
            if (socket != null)
                socket.Send(myName + ",fileUpdateComplete," + success.ToString());

        }

        public void sendAudioSampleRequest(string whichAudioSampleCode)
        {
            if (socket != null)
                socket.Send(myName + ",playAudio," + whichAudioSampleCode);
        }
        public void sendAudioSampleRequest(string whichAudioSampleCode, float whichVolumeLevel)
        {
            if (socket != null)
                socket.Send(myName + ",playAudio," + whichAudioSampleCode + ":" + whichVolumeLevel);
        }
        */
    }
}