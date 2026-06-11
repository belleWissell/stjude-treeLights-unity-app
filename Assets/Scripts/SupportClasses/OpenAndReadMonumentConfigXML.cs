using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AAMVC.Unity
{
    class OpenAndReadMonumentConfigXML
    {
        //**********************
        // XML File Variables
        private XmlDocument xDoc;
        private string targetFileName = "StreamingAssets//monumentSceneAppConfig.xml";
        private bool XMLFileIsAvailable = false;


        public bool debugMode = true;
        public bool doRecordDataToFile = false;

        public string thisApplicationID = "myApp";
        private static int maxNumberOfInstallations = 3;
        public int actualNumberOfInstallations = 0;
        public string[] applicationIds = new string[maxNumberOfInstallations];
        
        public string UDPnetworkTransmitIP = "192.168.1.100";
        public int UDPnetworkTransmitPort = 5309;
        public string UDPnetworkTransmitSignature = "myApp";
        public bool UDPnetworkdoConnect = false;

        /*
        public string lightTransmitIP01 = "192.168.1.100";
        public int lightTransmitPort01 = 5309;
        public string lightTransmitIP02 = "192.168.1.100";
        public int lightTransmitPort02 = 5309; */
        public string lightTransmitIP = "192.168.1.100";
        public int lightTransmitPort = 5309;
        public bool lightDoConnect = false;
        
        public string depthTransmitIP01 = "192.168.1.100";
        public int depthTransmitPort01 = 5309;
        public string depthTransmitIP02 = "192.168.1.100";
        public int depthTransmitPort02 = 5309;
        public bool doActivateDepthListener01 = false;
        public bool doActivateDepthListener02 = false;
        public int numberOfPanels = 5;
        

        public Vector4 ambientColor = new Vector4();
        public Vector4 highlightColor = new Vector4();
        public float maxWaveBright = 99f;
        public float minWaveBright = 1f;
        public float maxBright = 99f;
        public float minBright = 1f;
        public float dmxMax = 1f;
        public float dmxMin = 1f;
        
        
        // AUDIO
        /*
        public string audioContentFolder = "..//RESOURCEs//content/";
        public int masterAudioVolume = 50;
        private static int maxNumberOfAudioVoices = 100;
        public int actualNumberOfAudioVoices = 0;
        public string[] audioID = new string[maxNumberOfAudioVoices];
        public string[] audioFileName = new string[maxNumberOfAudioVoices];
        public bool[] doLoopAudio = new bool[maxNumberOfAudioVoices];
        public float[] audioLoopPoint = new float[maxNumberOfAudioVoices];
        */
        
        private  AAMVC.Unity.ApplicationControl appControlObj; 
        
        public OpenAndReadMonumentConfigXML(string whichXMLFile, ApplicationControl whichAppControl)
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
            XmlNode colorData = xDoc.SelectSingleNode("appSettings/colorSettings");

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

            if (colorData != null)
            {
                success4 = readColorDataData(colorData);
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
                
                /*getIPSt = whichNode.SelectSingleNode("lightDataIPAddress02").InnerText;
                getPortSt = whichNode.SelectSingleNode("lightDataPort02").InnerText;
                
                lightTransmitIP02 = getIPSt;
                lightTransmitPort02 = Convert.ToInt16(getPortSt);*/

                lightDoConnect = Convert.ToBoolean(getdonConnectSt);

                string getNumberOfPanelsSt = whichNode.SelectSingleNode("numberOfPanelsToTrack").InnerText;
                numberOfPanels = Convert.ToInt16(getNumberOfPanelsSt);
                
                getIPSt = whichNode.SelectSingleNode("depthDataIPAddress01").InnerText;
                getPortSt = whichNode.SelectSingleNode("depthDataPort01").InnerText;
                getdonConnectSt = whichNode.SelectSingleNode("depthDoConnect01").InnerText;

                depthTransmitIP01 = getIPSt;
                depthTransmitPort01 = Convert.ToInt16(getPortSt);
                doActivateDepthListener01 = Convert.ToBoolean(getdonConnectSt);
                
                getIPSt = whichNode.SelectSingleNode("depthDataIPAddress02").InnerText;
                getPortSt = whichNode.SelectSingleNode("depthDataPort02").InnerText;
                getdonConnectSt = whichNode.SelectSingleNode("depthDoConnect02").InnerText;

                depthTransmitIP02 = getIPSt;
                depthTransmitPort02 = Convert.ToInt16(getPortSt);
                doActivateDepthListener02 = Convert.ToBoolean(getdonConnectSt);
            }
            catch
            {
                return false;
            }
            
            return true;
        }
        
        private bool readColorDataData(XmlNode whichNode)
        {
            try
            {
                appControlObj.logText("[READXML] color Data");
                string getR = whichNode.SelectSingleNode("ambientColorR").InnerText;
                string getG = whichNode.SelectSingleNode("ambientColorG").InnerText;
                string getB = whichNode.SelectSingleNode("ambientColorB").InnerText;
                string getW = whichNode.SelectSingleNode("ambientColorW").InnerText;
                
                ambientColor.x = (float)Convert.ToDouble(getR);
                ambientColor.y = (float)Convert.ToDouble(getG);
                ambientColor.z = (float)Convert.ToDouble(getB);
                ambientColor.w = (float)Convert.ToDouble(getW);

                getR = whichNode.SelectSingleNode("highlightColorR").InnerText;
                getG = whichNode.SelectSingleNode("highlightColorG").InnerText;
                getB = whichNode.SelectSingleNode("highlightColorB").InnerText;
                getW = whichNode.SelectSingleNode("highlightColorW").InnerText;

                highlightColor.x = (float)Convert.ToDouble(getR);
                highlightColor.y = (float)Convert.ToDouble(getG);
                highlightColor.z = (float)Convert.ToDouble(getB);
                highlightColor.w = (float)Convert.ToDouble(getW);

                
                string getMaxWaveBright_s = whichNode.SelectSingleNode("maxWaveBrightness").InnerText;
                string getMinWaveBright_s = whichNode.SelectSingleNode("minWaveBrightness").InnerText;

                maxWaveBright = (float)Convert.ToDouble(getMaxWaveBright_s);
                minWaveBright = (float)Convert.ToDouble(getMinWaveBright_s);
                
                    
                string getMaxBright_s = whichNode.SelectSingleNode("maxBrightness").InnerText;
                string getMinBright_s = whichNode.SelectSingleNode("minBrightness").InnerText;

                maxBright = (float)Convert.ToDouble(getMaxBright_s);
                minBright = (float)Convert.ToDouble(getMinBright_s);

                string getMaxDMX = whichNode.SelectSingleNode("dmxValueON").InnerText;
                string getMinDMX = whichNode.SelectSingleNode("dmxValueOFF").InnerText;

                dmxMax = (float)Convert.ToDouble(getMaxDMX);
                dmxMin = (float)Convert.ToDouble(getMinDMX);
                
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
        } */
        
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
