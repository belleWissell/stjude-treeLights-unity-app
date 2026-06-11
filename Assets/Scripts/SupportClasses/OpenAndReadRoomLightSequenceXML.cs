using System;
using System.Collections.Generic;
using System.Text;

using System.Xml;
using System.IO;
using UnityEditor;

namespace AAMVC.Unity
{
    class OpenAndReadRoomLightSequenceXML
    {
        //**********************
        // XML File Variables
        private XmlDocument xDoc;
        private string targetFileName = "StreamingAssets//8x10RoomLightSeqConfig.xml";
        private bool XMLFileIsAvailable = false;

        private static int maxNumberOfPredefinedColors = 50;
        private int actualNumberOfDefinedColors = 0;
        private static int lightSettingsPerColor = 7;
        public string[] colorID = new string[maxNumberOfPredefinedColors];
        public int[,] colorDefinitions = new int[maxNumberOfPredefinedColors, lightSettingsPerColor];

        private float currentFrameRate = 60f; // used to determine frames from que time (provided in seconds in file)
        private static int maxNumberOfControlQuePoints = 50;
        public int actualNumberOfQuePoints = -1;
        
        private static int maxNumberOfLightsDefined = 6;
        public int[] quePointFrame = new int[maxNumberOfControlQuePoints]; // when to assume colors [frames]
        public int[] quePointRampTime = new int[maxNumberOfControlQuePoints];  // how quickly to assume new color(s) [frames]
        public int[,] quePointLightBrightness = new int[maxNumberOfControlQuePoints, maxNumberOfLightsDefined]; // per light brightness at que point
        public string[,] quePointLightColor = new string[maxNumberOfControlQuePoints, maxNumberOfLightsDefined]; // per light color at que point

        
        private  AAMVC.Unity.ApplicationControl appControlObj; 
        
        public OpenAndReadRoomLightSequenceXML(string whichXMLFile, ApplicationControl whichAppControl)
        {
            
            targetFileName = whichXMLFile;
            xDoc = new XmlDocument();

            appControlObj = whichAppControl;

            // init all variables
            int i, j;
            
            for (i = 0; i < maxNumberOfPredefinedColors; ++i)
            {
                colorID[i] = "ND";
                for (j = 0; j < lightSettingsPerColor; ++j)
                {
                    colorDefinitions[i, j] = 0;
                }
                
            }
            
            for (i = 0; i < maxNumberOfControlQuePoints; ++i)
            {
                quePointFrame[i] = -1;
                quePointRampTime[i] = 60;
                
                colorID[i] = "ND";
                for (j = 0; j < maxNumberOfLightsDefined; ++j)
                {
                    quePointLightBrightness[i, j] = 0;
                    quePointLightColor[i, j] = "ND";
                }
                
            }
                
        }

        public int[] getColorValuesOfLightID(string whichID)
        {
            int[] valueToReturn = new int[lightSettingsPerColor];
            int i;
            int matchingColorIndex = -1;
            
            for (i = 0; i < lightSettingsPerColor; ++i)
            {
                valueToReturn[i] = 0;
            }

            for (i = 0; i < actualNumberOfDefinedColors; ++i)
            {
                if (colorID[i] == whichID) // found matching color
                {
                    matchingColorIndex = i;
                }
            }

            if (matchingColorIndex != -1)
            {
                for (i = 0; i < lightSettingsPerColor; ++i)
                {
                    valueToReturn[i] = colorDefinitions[matchingColorIndex, i];
                }
            }
            

            
            return valueToReturn;
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
                    appControlObj.logText("[SEQXML] ERROR: XML found but won't load: " + targetFileName + " exception: " + e.Source);
                }
            }
            else
            {
                XMLFileIsAvailable = false;
                //System.Diagnostics.Debug.WriteLine("[READXML] ERROR: XML not found: " + targetFileName);
                appControlObj.logText("[SEQXML] ERROR: XML not found: " + targetFileName);

                //MessageBox.Show("[READXML] ERROR: XML not found: " + targetFileName); // this interrupts program
            }

            return XMLFileIsAvailable;
        }

        public bool openAndReadXMLFile()
        {
            //System.Diagnostics.Debug.WriteLine("[READXML] opening XML data...");
            appControlObj.logText("[SEQXML] opening sequence XML data...");

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

            //XmlNode seqData = xDoc.SelectSingleNode("outputSequences");
            XmlNodeList colorDataList = xDoc.SelectNodes("roomLightColorsAndSequence/lightColorLibrary/lightColorSetting");
            XmlNodeList seqDataList = xDoc.SelectNodes("roomLightColorsAndSequence/roomLightSequencerRunOfShow/adjustmentPoint");

            if (colorDataList != null)
            {
                success1 = readColorData(colorDataList);
            }
            else
            {
                valueToReturn = false;
            }
            if (seqDataList != null)
            {
                success2 = readSeqData(seqDataList);
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
           

            return valueToReturn;
        }

        private bool readColorData(XmlNodeList whichNodeList)
        {
            try
            {
                int i;
                string getColorID_st;
                string getR;
                string getG;
                string getB;
                string getY;
                string getW;
                string getYG;
                string getP;



                XmlNodeList sources = whichNodeList;
                actualNumberOfDefinedColors = sources.Count;
                if (actualNumberOfDefinedColors > maxNumberOfPredefinedColors)
                    actualNumberOfDefinedColors = maxNumberOfPredefinedColors;

                for (i = 0; i < actualNumberOfDefinedColors; ++i)
                {
                    if (sources[i].Attributes.GetNamedItem("name") != null)
                        getColorID_st = sources[i].Attributes.GetNamedItem("name").InnerText;
                    else
                    {
                        getColorID_st = "NA";
                    }

                    if (sources[i].Attributes.GetNamedItem("R") != null)
                        getR = sources[i].Attributes.GetNamedItem("R").InnerText;
                    else
                        getR = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("G") != null)
                        getG = sources[i].Attributes.GetNamedItem("G").InnerText;
                    else
                        getG = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("B") != null)
                        getB = sources[i].Attributes.GetNamedItem("B").InnerText;
                    else
                        getB = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("Y") != null)
                        getY = sources[i].Attributes.GetNamedItem("Y").InnerText;
                    else
                        getY = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("W") != null)
                        getW = sources[i].Attributes.GetNamedItem("W").InnerText;
                    else
                        getW = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("YG") != null)
                        getYG = sources[i].Attributes.GetNamedItem("YG").InnerText;
                    else
                        getYG = "0";
                    
                    if (sources[i].Attributes.GetNamedItem("P") != null)
                        getP = sources[i].Attributes.GetNamedItem("P").InnerText;
                    else
                        getP = "0";

                    colorID[i] = getColorID_st;
                    colorDefinitions[i, 0] = Convert.ToInt16(getR);
                    colorDefinitions[i, 1] = Convert.ToInt16(getG);
                    colorDefinitions[i, 2] = Convert.ToInt16(getB);
                    colorDefinitions[i, 3] = Convert.ToInt16(getY);
                    colorDefinitions[i, 4] = Convert.ToInt16(getW);
                    colorDefinitions[i, 5] = Convert.ToInt16(getYG);
                    colorDefinitions[i, 6] = Convert.ToInt16(getP);

                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        private bool readSeqData(XmlNodeList whichNodeList)
        {
            try
            {
                int i, j;
                string getTime_st = "0";
                string getRampTime_st = "0";
                int numberOfLightsToAdjust = 0;
                string whichLight_st;
                int whichLight;
                string whichBright_st;
                string whichColor_st;

                double getTime_seconds = 0;
                double getRampTime_seconds = 0;
                
                XmlNodeList sources = whichNodeList;
                actualNumberOfQuePoints = sources.Count;
                if (actualNumberOfQuePoints > maxNumberOfControlQuePoints)
                    actualNumberOfQuePoints = maxNumberOfControlQuePoints;

                
                for (i = 0; i < actualNumberOfQuePoints; ++i)
                {
                    if (sources[i].Attributes.GetNamedItem("timeStamp") != null)
                        getTime_st = sources[i].Attributes.GetNamedItem("timeStamp").InnerText;
                    if (sources[i].Attributes.GetNamedItem("rampTimeSeconds") != null)
                        getRampTime_st = sources[i].Attributes.GetNamedItem("rampTimeSeconds").InnerText;

                    getTime_seconds = Convert.ToDouble(getTime_st);
                    getRampTime_seconds = Convert.ToDouble(getRampTime_st);

                    quePointFrame[i] = (int)Math.Ceiling(getTime_seconds * currentFrameRate);
                    quePointRampTime[i] = (int)Math.Ceiling(getRampTime_seconds * currentFrameRate);
                    
                    XmlNodeList lightAdjustments = sources[i].SelectNodes("adjustLight");
                    numberOfLightsToAdjust = lightAdjustments.Count;
                    for (j = 0; j < numberOfLightsToAdjust; ++j)
                    {
                        whichLight_st = lightAdjustments[j].Attributes.GetNamedItem("light").InnerText;
                        whichBright_st = lightAdjustments[j].Attributes.GetNamedItem("brightness").InnerText;
                        whichColor_st = lightAdjustments[j].Attributes.GetNamedItem("color").InnerText;

                        whichLight = Convert.ToInt16(whichLight_st);
                        whichLight -= 1; // lights in the XML are numbered 1-6, we want 0-5...
                        if (whichLight < maxNumberOfLightsDefined)
                        {
                            quePointLightBrightness[i, whichLight] = Convert.ToInt16(whichBright_st);
                            quePointLightColor[i, whichLight] = whichColor_st;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }


        /*
        private bool readSeqDataOld(XmlNodeList whichNodeList)
        {
            try
            {
                int i;
                string getID_st;
                int currentIDbeingRead = 0;
                string getActive_st;
                string getTotalTime_st;
                string getDelay_st;
                string getBlink_st;
                string getBlinkOn_st;
                string getBlinkOff_st;
                
                XmlNodeList sources = whichNodeList;
                int numberOfSources = sources.Count;

                if (numberOfSources > maxNumberOfSequences)
                    numberOfSources = maxNumberOfSequences;

                for (i = 0; i < numberOfSources; ++i)
                {
                    getID_st = sources[i].Attributes.GetNamedItem("outputID").InnerText;
                    getActive_st = sources[i].Attributes.GetNamedItem("isActive").InnerText;
                    getTotalTime_st = sources[i].Attributes.GetNamedItem("totalActiveTime").InnerText;
                    getDelay_st = sources[i].Attributes.GetNamedItem("startDelay").InnerText;
                    getBlink_st = sources[i].Attributes.GetNamedItem("makeItBlink").InnerText;
                    getBlinkOn_st = sources[i].Attributes.GetNamedItem("blinkOnTime").InnerText;
                    getBlinkOff_st = sources[i].Attributes.GetNamedItem("blinkOffTime").InnerText;

                    currentIDbeingRead = Convert.ToInt16(getID_st);
                    if (currentIDbeingRead < maxNumberOfSequences)
                    {
                        isActive[currentIDbeingRead] = Convert.ToBoolean(getActive_st);
                        totalActiveTime[currentIDbeingRead] = (float)Convert.ToDouble(getTotalTime_st);
                        startDelay[currentIDbeingRead] = (float)Convert.ToDouble(getDelay_st);
                        doBlink[currentIDbeingRead] = Convert.ToBoolean(getBlink_st);
                        blinkOnTime[currentIDbeingRead] = (float)Convert.ToDouble(getBlinkOn_st);
                        blinkOffTime[currentIDbeingRead] = (float)Convert.ToDouble(getBlinkOff_st);

                    }
                }

            }
            catch (Exception e)
            {
                return false;
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
