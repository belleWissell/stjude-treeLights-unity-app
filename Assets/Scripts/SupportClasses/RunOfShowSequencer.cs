using System;

namespace AAMVC.Unity
{
    public class RunOfShowSequencer
    {
        private OpenAndReadRoomLightSequenceXML getValuesFromXML;
        private string targetFileName = "StreamingAssets//AppConfig.xml";

        private bool dirty;
        private int RunOfShowFrameCounter = 0;
        public bool runOfShowCompleted = true;
        
        private static int maxNumberOfControlQuePoints = 50;
        private static int numberOfLightsDefined = 6;
        private static int numberOfLightSettings = 8;

        private bool[] expiredQuePoints = new bool[maxNumberOfControlQuePoints];

        public int[,] currentLightValues = new int[numberOfLightsDefined, numberOfLightSettings];
        public int currentRampSpeed = 1;
        
        //private int lastActiveQue = -1;
        public int currentlyActiveQuePoint = -1;

        public string[] currentLightColorIds = new string[numberOfLightsDefined];
        
        public bool isDirty()
        {
            bool valueToReturn = dirty;
            if (dirty)
            {
                dirty = false;
            }

            return valueToReturn;
        }
        public void initClass(string whichXMLFile, ApplicationControl whichAppControl)
        {

            getValuesFromXML = new OpenAndReadRoomLightSequenceXML(whichXMLFile, whichAppControl);
            getValuesFromXML.openAndReadXMLFile();
            
        }

        public void resetRunOfShow()
        {
            RunOfShowFrameCounter = 0;
            runOfShowCompleted = false;
            for (int i = 0; i < maxNumberOfControlQuePoints; ++i)
            {
                expiredQuePoints[i] = false;
            }
        }

        public void forceNextCueFromKeyboard()
        {
            bool foundNextCuePoint = false;
            int nextCuePoint = -1;
            
            for (int i = 0; i < getValuesFromXML.actualNumberOfQuePoints; ++i)
            {
                if (!expiredQuePoints[i]) // we haven't encountered this que yet
                {
                    if (!foundNextCuePoint)
                    {
                        foundNextCuePoint = true;
                        nextCuePoint = i;
                    }
                }
            }

            int nextCuePointFrame = -1;
            
            if (nextCuePoint != -1)
            {
                nextCuePointFrame = getValuesFromXML.quePointFrame[nextCuePoint];
            }

            if (nextCuePointFrame != -1)
            {
                // fastforward counter to just before cue point
                RunOfShowFrameCounter = nextCuePointFrame - 5;
            }
        }

        public void updateRunOfShowSeq()
        {
            RunOfShowFrameCounter += 1;

            for (int i = 0; i < getValuesFromXML.actualNumberOfQuePoints; ++i)
            {
                
                if (RunOfShowFrameCounter == getValuesFromXML.quePointFrame[i]) // we have surpassed a que point
                {
                    if (!expiredQuePoints[i]) // we haven't encountered this que yet
                    {
                        expiredQuePoints[i] = true;
                        currentlyActiveQuePoint = i;
                        updateCurrentTargetValuesToCurrentlyActiveQuePoint();
                        dirty = true; // flag that something has changed

                        if (i == (getValuesFromXML.actualNumberOfQuePoints -1) ) // the last que point has been reached
                        {
                            runOfShowCompleted = true;
                        }
                    }
                }
            }
        }

        public string getCurrentTimeInSec()
        {
            string valueToReturn = "00.00";
            
            float valueToReturn_f = 0.01f * (float)Math.Round(RunOfShowFrameCounter / 0.6f);
            valueToReturn = String.Format("{0:0.##}", valueToReturn_f); // two decimal places (as needed)
            
            return valueToReturn;
        }

        private void updateCurrentTargetValuesToCurrentlyActiveQuePoint()
        {
            currentRampSpeed = getValuesFromXML.quePointRampTime[currentlyActiveQuePoint];

            string currentColorIDOfLight;
            int i, j;
            int currentLightColorValue;
            int[] newLightColor = new int[7]; // values retreived from library of colors in XML file
            
            for (i = 0; i < numberOfLightsDefined; ++i)
            {
                currentLightValues[i, 0] = getValuesFromXML.quePointLightBrightness[currentlyActiveQuePoint, i]; // first value is brightness
                currentColorIDOfLight = getValuesFromXML.quePointLightColor[currentlyActiveQuePoint, i];
                currentLightColorIds[i] = currentColorIDOfLight; // this is for visual feedback on screen
                
                if (currentColorIDOfLight != "ND") // there is no update for this light, cue point
                {
                    
                    newLightColor = getValuesFromXML.getColorValuesOfLightID(currentColorIDOfLight);

                    for (j = 1; j < numberOfLightSettings; ++j) // values 1-7
                    {
                        currentLightValues[i, j] = newLightColor[j - 1]; // colors retreived from xml file are indexed 0-6, we're placing them in indeces 1-7 (with index 0 dedicated to brightness)
                    }
                }
            }
        }

        

    }
}