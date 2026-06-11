using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.IO;
using UnityEditor;

namespace AAMVC.Unity
{
    class OpenAndReadConfigXML
    {
        //**********************
        // XML File Variables
        private XmlDocument xDoc;
        private string targetFileName = "StreamingAssets//AppConfig.xml";
        private bool XMLFileIsAvailable = false;


        public bool debugMode = true;
        public bool doDisplay3DRoom = true;
        public bool doAutoLoopShow = false;
        public int autoLoopInterval = 20;
        public bool doRecordDataToFile = false;

        public string thisApplicationID = "myApp";
        private static int maxNumberOfInstallations = 3;
        public int actualNumberOfInstallations = 0;
        public string[] applicationIds = new string[maxNumberOfInstallations];
        
        public string UDPnetworkTransmitIP = "192.168.1.100";
        public int UDPnetworkTransmitPort = 5309;
        public string UDPnetworkTransmitSignature = "myApp";
        public bool UDPnetworkdoConnect = false;

        public string lightTransmitIP = "192.168.1.100";
        public int lightTransmitPort = 5309;
        public bool lightDoConnect = false;
        
        public string depthTransmitIP = "192.168.1.100";
        public int depthTransmitPort = 5309;
        public bool depthDoConnect = false;
        
        // AUDIO
        public string audioContentFolder = "..//RESOURCEs//content/";
        public int masterAudioVolume = 50;
        private static int maxNumberOfAudioVoices = 100;
        public int actualNumberOfAudioVoices = 0;
        public string[] audioID = new string[maxNumberOfAudioVoices];
        public string[] audioFileName = new string[maxNumberOfAudioVoices];
        public bool[] doLoopAudio = new bool[maxNumberOfAudioVoices];
        public float[] audioLoopPoint = new float[maxNumberOfAudioVoices];

        
        private  AAMVC.Unity.ApplicationControl appControlObj; 
        
        public OpenAndReadConfigXML(string whichXMLFile, ApplicationControl whichAppControl)
        {
            targetFileName = whichXMLFile;
            xDoc = new XmlDocument();

            appControlObj = whichAppControl;
        }

        #region readXML

        private bool isXMLFileAvailable()
        {
            XMLFileIsAvailable = false;

            if (File.Exists(MakeAbsolutePath(targetFileName))) // does this work for web based retreival?
            {
                try
                {
                    xDoc.Load(targetFileName); // testing with server service
                    XMLFileIsAvailable = true;
                }
                catch (Exception e)
                {
                    XMLFileIsAvailable = false;
                    //System.Diagnostics.Debug.WriteLine("[READXML] ERROR: XML found but won't load: " + targetFileName + " exception: " + e.Source);
                    appControlObj.logText("[READXML] ERROR: XML found but won't load: " + targetFileName + " exception: " + e.Source);
                }
            }
            else
            {
                XMLFileIsAvailable = false;
                //System.Diagnostics.Debug.WriteLine("[READXML] ERROR: XML not found: " + targetFileName);
                appControlObj.logText("[READXML] ERROR: XML not found: " + targetFileName);

                //MessageBox.Show("[READXML] ERROR: XML not found: " + targetFileName); // this interrupts program
            }

            return XMLFileIsAvailable;
        }

        public bool openAndReadXMLFile()
        {
            //System.Diagnostics.Debug.WriteLine("[READXML] opening XML data...");
            appControlObj.logText("[READXML] opening XML data...");

            bool valueToReturn = false;

            if (isXMLFileAvailable())
            {
                if (readXMLFile())
                    valueToReturn = true;
            }

            return valueToReturn;
        }


        private bool readXMLFile()
        {
            bool valueToReturn = true;
            bool success1 = false;
            bool success2 = false;
            bool success3 = false; 
            bool success4 = false; 

            XmlNode globalData = xDoc.SelectSingleNode("appSettings/globalSettings");
            XmlNode applicationData = xDoc.SelectSingleNode("appSettings/applicationSettings");
            XmlNode networkData = xDoc.SelectSingleNode("appSettings/networkSettings");
            XmlNode audioData = xDoc.SelectSingleNode("appSettings/audioSettings");

            if (globalData != null)
            {
                success1 = readGlobalData(globalData);
            }
            else
            {
                valueToReturn = false;
            }
            
            if (applicationData != null)
            {
                success2 = readApplicationData(applicationData);
            }
            else
            {
                valueToReturn = false;
            }

            if (networkData != null)
            {
                success3 = readNetworkData(networkData);
            }
            else
            {
                valueToReturn = false;
            }

            if (audioData != null)
            {
                success4 = readAudioData(audioData);
            }
            else
            {
                valueToReturn = false;
            }
            
            // did any of them fail to find data?
            if (!success1)
                valueToReturn = false;
            if (!success2)
                valueToReturn = false;
            if (!success3)
                valueToReturn = false;
            if (!success4)
                valueToReturn = false;

            return valueToReturn;
        }

        private bool readGlobalData(XmlNode whichNode)
        {
            try
            {
                if (whichNode.SelectSingleNode("debugMode") != null)
                {
                    string getConfigBool = whichNode.SelectSingleNode("debugMode").InnerText;
                    debugMode = Convert.ToBoolean(getConfigBool);
                }

                if (whichNode.SelectSingleNode("display3dRoomRealtimeRender") != null)
                {
                    string getdisplayBool = whichNode.SelectSingleNode("display3dRoomRealtimeRender").InnerText;
                    doDisplay3DRoom = Convert.ToBoolean(getdisplayBool);
                }
                if (whichNode.SelectSingleNode("autoLoopShow") != null)
                {
                    string getDoLoop= whichNode.SelectSingleNode("autoLoopShow").InnerText;
                    doAutoLoopShow = Convert.ToBoolean(getDoLoop);
                }
                if (whichNode.SelectSingleNode("secondsBetweenLoops") != null)
                {
                    string getLoopTime = whichNode.SelectSingleNode("secondsBetweenLoops").InnerText;
                    autoLoopInterval = Convert.ToInt16(getLoopTime);
                }
                
                if (whichNode.SelectSingleNode("recordActivityToLocalFile") != null)
                {
                    string getSensorMode = whichNode.SelectSingleNode("recordActivityToLocalFile").InnerText;
                    doRecordDataToFile = Convert.ToBoolean(getSensorMode);
                }

                appControlObj.logText("[READXML] debugmode set to =  " + debugMode);
                appControlObj.logText("[READXML] will record activity =  " + doRecordDataToFile);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private bool readApplicationData(XmlNode whichNode)
        {
            int i;
            try
            {
                string getAppIdSt = whichNode.SelectSingleNode("thisAppId").InnerText;
                thisApplicationID = getAppIdSt;
                //appControlObj.logText("[READXML] thisAppID set to =  " + thisApplicationID);

                XmlNode applicationIdData = xDoc.SelectSingleNode("appSettings/applicationSettings/availableApplicationIds");
                if (applicationIdData != null)
                {
                    XmlNodeList appIds = applicationIdData.SelectNodes("applicationId");
                    
                    
                    actualNumberOfInstallations = appIds.Count;
                    if (actualNumberOfInstallations > maxNumberOfInstallations)
                        actualNumberOfInstallations = maxNumberOfInstallations;
                    
                    string getIds;
                    
                    for (i = 0; i < actualNumberOfInstallations; ++i)
                    {
                        getIds = appIds[i].InnerText;
                        applicationIds[i] = getIds;
                        //appControlObj.logText("[READXML] appID "+i+" =  " + applicationIds[i]);
                    }
                    
                    
                }
                else
                {
                    
                }
                
            }
            catch
            {
                return false;
            }
            
            return true;
        }
        
        private bool readNetworkData(XmlNode whichNode)
        {
            try
            {
                string getIPSt = whichNode.SelectSingleNode("UDPsignalTransmitIPAddress").InnerText;
                string getPortSt = whichNode.SelectSingleNode("UDPsignalTransmitPort").InnerText;
                string getSignatureSt = whichNode.SelectSingleNode("UDPsignalTransmitSignatureName").InnerText;
                string getdonConnectSt = whichNode.SelectSingleNode("UDPdoConnect").InnerText;
                
                UDPnetworkTransmitIP = getIPSt;
                UDPnetworkTransmitPort = Convert.ToInt16(getPortSt);
                UDPnetworkTransmitSignature = getSignatureSt;
                UDPnetworkdoConnect = Convert.ToBoolean(getdonConnectSt);
                
                getIPSt = whichNode.SelectSingleNode("lightDataIPAddress").InnerText;
                getPortSt = whichNode.SelectSingleNode("lightDataPort").InnerText;
                getdonConnectSt = whichNode.SelectSingleNode("lightDoConnect").InnerText;

                lightTransmitIP = getIPSt;
                lightTransmitPort = Convert.ToInt16(getPortSt);
                lightDoConnect = Convert.ToBoolean(getdonConnectSt);
                
                getIPSt = whichNode.SelectSingleNode("depthDataIPAddress").InnerText;
                getPortSt = whichNode.SelectSingleNode("depthDataPort").InnerText;
                getdonConnectSt = whichNode.SelectSingleNode("depthDoConnect").InnerText;

                depthTransmitIP = getIPSt;
                depthTransmitPort = Convert.ToInt16(getPortSt);
                depthDoConnect = Convert.ToBoolean(getdonConnectSt);
            }
            catch
            {
                return false;
            }
            
            return true;
        }

        
        private bool readAudioData(XmlNode whichNode)
        {
            try
            {
                appControlObj.logText("[READXML] audio Data");
                string getDocPathSt = whichNode.SelectSingleNode("audioFileFolder").InnerText;
                audioContentFolder = getDocPathSt;
                string getVolSt = whichNode.SelectSingleNode("MasterAudioLevel").InnerText;
                masterAudioVolume = Convert.ToInt32(getVolSt);

                XmlNodeList videos = whichNode.SelectNodes("audio");
                actualNumberOfAudioVoices = videos.Count;
                if (actualNumberOfAudioVoices > maxNumberOfAudioVoices)
                    actualNumberOfAudioVoices = maxNumberOfAudioVoices;

                int i;
                string getID;
                string getFile;
                string getDoLoop;
                string getLoopPoint;
                
                for (i = 0; i < actualNumberOfAudioVoices; ++i)
                {
                    getID = videos[i].SelectSingleNode("audioID").InnerText;
                    getFile = videos[i].SelectSingleNode("audioMediaFile").InnerText;
                    getDoLoop = videos[i].SelectSingleNode("doLoopAudio").InnerText;
                    getLoopPoint = videos[i].SelectSingleNode("audioLoopPoint").InnerText;

                    audioID[i] = getID;
                    audioFileName[i] = getFile;
                    doLoopAudio[i] = Convert.ToBoolean(getDoLoop);
                    audioLoopPoint[i] = (float)Convert.ToDouble(getLoopPoint);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
        /*
        private bool readAudioData(XmlNode whichNode)
        {
            XmlNodeList appIds = whichNode.SelectNodes("audioSettings");
            int actualNumberOfInstallations = appIds.Count;
            int i;
            string whichAppId = "NA";
            string ambientAudioVolumeLevelSt = null;

            for (i = 0; i < actualNumberOfInstallations; ++i)
            {
                whichAppId = appIds[i].Attributes["id"].InnerText;

                if (whichAppId == thisApplicationID)
                {
                    ambientAudioFileName = appIds[i].SelectSingleNode("ambientAudioFile").InnerText;
                    ambientAudioVolumeLevelSt = appIds[i].SelectSingleNode("ambientAudioVolumePct").InnerText;
                }
            }

            try
            {
                if (ambientAudioVolumeLevelSt != null)
                    ambientAudioVolumeLevel = Convert.ToInt16(ambientAudioVolumeLevelSt);
            }
            catch (Exception e)
            {
                return false;
            }
            
            return true;
        }*/
  
        
        /*
        private bool readDesignData(XmlNode whichNode)
        {
            XmlNodeList appIds = whichNode.SelectNodes("designSettings");
            int actualNumberOfInstallations = appIds.Count;
            int i;
            string whichAppId = "NA";
            for (i = 0; i < actualNumberOfInstallations; ++i)
            {
                whichAppId = appIds[i].Attributes["id"].InnerText;

                if (whichAppId == thisApplicationID)
                {
                    titleColorHex = appIds[i].SelectSingleNode("titleColor").InnerText;
                    yearColorHex = appIds[i].SelectSingleNode("yearColor").InnerText;
                    captionColorHex = appIds[i].SelectSingleNode("captionColor").InnerText;

                    if (appIds[i].SelectSingleNode("startOnStory").InnerText != null)
                    {
                        string startOnStory_St = appIds[i].SelectSingleNode("startOnStory").InnerText;
                        try
                        {
                            startOnStory = Convert.ToInt16(startOnStory_St);
                        }
                        catch (Exception e)
                        {
                            startOnStory = 0;
                        }
                    }
                }
            }
            
            return true;
        }*/
        #endregion readXML

        private string MakeAbsolutePath(string file)
        {
            string result = file;
            //create folder if it does not exist
            string folder = Path.GetDirectoryName(result);
            if (folder == String.Empty)
            {
                //if so prepend current directory
                result = Directory.GetCurrentDirectory() + "\\" + file;
            }
            return result;
        }
        
       
    }
}
