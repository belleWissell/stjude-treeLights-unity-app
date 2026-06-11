using System;
using System.Collections;
using System.Collections.Generic;
using AAMVC.Unity;
//using UnityEditor.Experimental.GraphView; // this causes crashe on Build "are you missing an assembly reference?" 
using UnityEngine;

public class Z_LightCtrl : MonoBehaviour
{
    private int globalFPS = 30; // must match Application.targetFrameRate = 30;
    
    private AAMVC.Unity.ApplicationControl appControl;
    private AppConfig appConfig;
    private AppConfig.Config config;
    private LogTextCtrl logTextCtrl;
    
    public float percentageOfRegionToActivate = 0.25f;
    
    private bool initComplete = false;
    //private static int maxNumberOfLights = 50;

    //private GameObject[] lights = new GameObject[maxNumberOfLights];
    //private Z_InidivdualLightCtrl[] z3SingleLightCtrl = new Z_InidivdualLightCtrl[maxNumberOfLights];
    //private int actualNumberOfLights = 0;

    private static int actualNumberOfTrees = 3;
    public GameObject[] treeObj = new GameObject[actualNumberOfTrees];
    private TreeOrg[] treeOrg = new TreeOrg[actualNumberOfTrees];
    public GameObject globeObj;
    private TreeGlobeCtrl treeGlobeCtrl;
    
    private float highValue = 98f;
    private float lowValue = 0f;

    private float minXPosn = 999f;
    private float maxXPosn = -999f;
    
    private float minYPosn = 999f;
    private float maxYPosn = -999f;

    // **************** Reactive  mode
    private static int maxNumberOfRegions = 50;
    private int actualNumberOfRegions = 8;
    public bool[] regionIsActivated = new bool[maxNumberOfRegions];
    public float[] regionPosition = new float[maxNumberOfRegions];
    private int reactiveLightActivationTimingOn = 25;
    private int reactiveLightActivationTimingOff = 50;
    private int typicalAnimationTime = 40; // this used to 80
    
    // **************** Ambient mode
    private int waitAndResumeAmbientTimer = 0;
    private bool waitAndResumeAmbient = false;
    private bool ambientAnimationIsActive = false;
    private int ambientAnimationTimer = 0;

    // **************** ping pong mode
    private static int pingPongAnimationSequenceLength = 16;
    private int[] pingPongAnimationSequence = new int[pingPongAnimationSequenceLength];
    private int pingpongCurrentIndex = 0;
    
    

    // **************** testing testing...
    private bool toggleLightBoxTest = false;
    
    private void Awake()
    {
        appControl = AAMVC.Unity.ApplicationControl.Instance;
        appConfig = AppConfig.Instance;
        if (globalFPS == 60)
            typicalAnimationTime = 80;

        logTextCtrl = LogTextCtrl.Instance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < actualNumberOfTrees; ++i)
        {
            treeOrg[i] =treeObj[i].GetComponent<TreeOrg>();
        }
        /*
        int i;
        for (i = 0; i < maxNumberOfRegions; ++i)
        {
            regionIsActivated[i] =false;
            
        }

        i = 0;
        pingPongAnimationSequence[i] = 61; //insist on
        i += 1;
        pingPongAnimationSequence[i] = 145; //customer 
        i += 1;
        pingPongAnimationSequence[i] = 97; // bias for action
        i += 1;
        pingPongAnimationSequence[i] = 161; // dive deep
        i += 1;
        pingPongAnimationSequence[i] = 105; // invest and simplify
        i += 1;
        pingPongAnimationSequence[i] = 169; //strive to be earths
        i += 1;
        pingPongAnimationSequence[i] = 73; // are right a lot
        i += 1;
        pingPongAnimationSequence[i] = 193; //owner
        i += 1;
        pingPongAnimationSequence[i] = 81; //earn trust
        i += 1;
        
        pingPongAnimationSequence[i] = 201; // deliverer results
        i += 1;
        pingPongAnimationSequence[i] = 93; // frugality
        i += 1;
        pingPongAnimationSequence[i] = 173; // hire and dev
        i += 1;
        pingPongAnimationSequence[i] = 121; // success and scale 
        i += 1;
        pingPongAnimationSequence[i] = 205; // have backbone and commit
        i += 1;
        pingPongAnimationSequence[i] = 149; // think big
        i += 1;
        pingPongAnimationSequence[i] = 181; // learn
        */
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initComplete)
            return;


        for (int i = 0; i < actualNumberOfTrees; i++)
        {
            treeOrg[i].checkForColorChanges();
        }

        treeGlobeCtrl.checkForColorChanges();
        
        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            byte newValue = 0;
            if (z3SingleLightCtrl[i].isDirty())
            {
                z3SingleLightCtrl[i].updateFeedback();
                float retrieveBrightnessFromLight = z3SingleLightCtrl[i].getHighestBrightness();
                newValue = (byte)Math.Floor((retrieveBrightnessFromLight/100f) * 255);
                appControl.updateIndividualArtNetValue(z3SingleLightCtrl[i].DMXStartChannel, newValue);
            }

            /*
            if (z3SingleLightCtrl[i].isAmazonLogo) // override all amazon logo lights to always be on full value:
            {
                newValue = (byte)Math.Floor((highValue/100f) * 255);
                appControl.updateIndividualArtNetValue(z3SingleLightCtrl[i].DMXChannel, newValue);
            }*/
        //}

        if (ambientAnimationIsActive)
        {
            ambientAnimationTimer += 1;
            checkAmbientMode();
        }

        if (waitAndResumeAmbient)
        {
            waitAndResumeAmbientTimer += 1;
            if (waitAndResumeAmbientTimer > 180)
            {
                waitAndResumeAmbient = false;
                ambientAnimationIsActive = true;
            }
        }
    }

    public void updateAllTreeColorsTo(int whichColorChannel, Color whichColor0, Color whichColor1)
    {
        for (int i = 0; i < actualNumberOfTrees; i++)
        {
            treeOrg[i].updateColorsTo(whichColorChannel, whichColor0, whichColor1);
        }
    }
    
    public void updateSpecificTreeColorsTo(int whichTreeIndex, Color whichColor0, Color whichColor1)
    {
        for (int i = 0; i < actualNumberOfTrees; i++)
        {
            if (i == whichTreeIndex)
            {
                treeOrg[i].updateColorsTo(0, whichColor0, whichColor0); // trunk
                treeOrg[i].updateColorsTo(1, whichColor1, whichColor1); // secondary
                treeOrg[i].updateColorsTo(2, whichColor1, whichColor1); // tertiary
            }
        }
    }

    public void startAmbientGlobeSparkle()
    {
        treeGlobeCtrl.startAmbientSparkles();
    }

    public void stopAmbientGlobeSparkle()
    {
        treeGlobeCtrl.stopAmbientSparkles();
    }
    public void startWaveGlobeSparkle()
    {
        treeGlobeCtrl.startSparkleWave();
    }
    
    #region ambientMode
    public void resumeAmbientAnimation()
    {
        //waitAndResumeAmbientTimer = 0;
        //waitAndResumeAmbient = true;
        ambientAnimationIsActive = true;
        //ambientAnimationTimer = 0;
    }

    public void launchAmbientAnimationFromTheStart()
    {
        waitAndResumeAmbient = false;

        ambientAnimationTimer = 0;
        ambientAnimationIsActive = true;
    }

    public void stopAmbientAnimation()
    {
        waitAndResumeAmbient = false;
        ambientAnimationIsActive = false;
        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            z3SingleLightCtrl[i].animateHighlightValueTo(lowValue, typicalAnimationTime);
        }*/
    }

    private void checkAmbientMode()
    {
        int i, j, k;

        
        
        int turnOnTime = 80;
        int turnOffTime = 60;
        if (globalFPS == 60)
        {
            turnOnTime = 160;
            turnOffTime = 120;
        }
        
        float normalizedTime = 0f;
        
        float newSweepOnPosn = 0f;
        float sweepMidPosn = 0f;
        float newSweepOffPosn = 0f;

        //int slowdownMultiplier = 2;
        int sweep01StartTime = 30;
        int sweep01EndTime = sweep01StartTime + config.ambientModeTiming.sweepTime;
        
        int sweep02StartTime = sweep01EndTime + 90;
        int sweep02EndTime = sweep02StartTime + config.ambientModeTiming.sweepTime;
        if (globalFPS == 60)
        {
            sweep01StartTime = 60;
            sweep01EndTime = sweep01StartTime + config.ambientModeTiming.sweepTime;
        
            sweep02StartTime = sweep01EndTime + 180;
            sweep02EndTime = sweep02StartTime + config.ambientModeTiming.sweepTime;
        }

        float leftSideOfSweep = minXPosn - 3f;
        float rightSideOfSweep = maxXPosn + 3f;
        float widthOfSweep = (maxXPosn - minXPosn) * 0.2f;


        
        
        int pingPongStartTime = sweep02EndTime + 60;
        if (globalFPS == 60)
        {
            pingPongStartTime = sweep02EndTime + 120;
        }

        int pingPongHoldOnTime = config.ambientModeTiming.pingPongHoldOnTime; // each message is up for this ammount of time;
        int pingPongAdvanceTime = config.ambientModeTiming.pingPongAdvanceTime; // how quickly to move to next message
        int pingPongEndTime = pingPongStartTime + (pingPongAdvanceTime * pingPongAnimationSequenceLength + 1);
        int pingPongDMXChannel01 = 0;
        int pingPongDMXChannel02 = 0;

        int looptime = pingPongEndTime + pingPongHoldOnTime;

        float deltaTestValueLow = lowValue + (highValue - lowValue)*0.5f;
        float deltaTestValueHigh = highValue - (highValue - lowValue)*0.5f;

        if ((ambientAnimationTimer >= sweep01StartTime) && (ambientAnimationTimer < sweep01EndTime)) // first 350 = sweep left -> right
        {
            normalizedTime = (((float)ambientAnimationTimer - (float)sweep01StartTime) / ((float)sweep01EndTime - (float)sweep01StartTime));
            
            sweepMidPosn = leftSideOfSweep + ((rightSideOfSweep - leftSideOfSweep) * normalizedTime);
            
            newSweepOnPosn = sweepMidPosn + (widthOfSweep / 2f);
            newSweepOffPosn = sweepMidPosn - (widthOfSweep / 2f);

            /*
            for (i = 0; i < actualNumberOfLights; ++i)
            {
                if ((z3SingleLightCtrl[i].screenPosition.x < newSweepOnPosn) && (z3SingleLightCtrl[i].screenPosition.x > newSweepOffPosn)) // is light betwixt on and off
                {
                    if (!z3SingleLightCtrl[i].sweepingOn) // to avoid setting/animating value multiple times
                    {
                        z3SingleLightCtrl[i].sweepingOff = false;
                        z3SingleLightCtrl[i].sweepingOn = true;
                        z3SingleLightCtrl[i].animateHighlightValueTo(highValue, turnOnTime);
                    }
                }
                else
                {
                    if (!z3SingleLightCtrl[i].sweepingOff) // to avoid setting/animating value multiple times
                    {
                        z3SingleLightCtrl[i].sweepingOff = true;
                        z3SingleLightCtrl[i].sweepingOn = false;
                        z3SingleLightCtrl[i].animateHighlightValueTo(lowValue, turnOffTime);
                    }
                }
            }*/
        }
        else if (ambientAnimationTimer >= sweep02StartTime && ambientAnimationTimer < sweep02EndTime) // next 350 = sweep right -> left
        {
            normalizedTime = (((float)ambientAnimationTimer - (float)sweep02StartTime) / ((float)sweep02EndTime - (float)sweep02StartTime));
            
            sweepMidPosn = rightSideOfSweep  - ((rightSideOfSweep - leftSideOfSweep) * normalizedTime);

            
            newSweepOnPosn = sweepMidPosn - (widthOfSweep / 2f);
            newSweepOffPosn = sweepMidPosn + (widthOfSweep / 2f);
            /*
            for (i = 0; i < actualNumberOfLights; ++i)
            {
                if ((z3SingleLightCtrl[i].screenPosition.x < newSweepOffPosn) && (z3SingleLightCtrl[i].screenPosition.x > newSweepOnPosn)) // is light betwixt on and off
                {
                    if (!z3SingleLightCtrl[i].sweepingOn) // to avoid setting/animating value multiple times
                    {
                        z3SingleLightCtrl[i].sweepingOff = false;
                        z3SingleLightCtrl[i].sweepingOn = true;
                        z3SingleLightCtrl[i].animateHighlightValueTo(highValue, turnOnTime);
                    }
                }
                else
                {
                    if (!z3SingleLightCtrl[i].sweepingOff) // to avoid setting/animating value multiple times
                    {
                        z3SingleLightCtrl[i].sweepingOff = true;
                        z3SingleLightCtrl[i].sweepingOn = false;
                        z3SingleLightCtrl[i].animateHighlightValueTo(lowValue, turnOffTime);
                    }
                    
                }
            }*/
        }
        else if (ambientAnimationTimer == pingPongStartTime)
        {
            pingpongCurrentIndex = 0;
        }
        else if (ambientAnimationTimer > pingPongStartTime && ambientAnimationTimer < pingPongEndTime) // we are in ping pong mode
        {
            if (ambientAnimationTimer == pingPongStartTime + (pingPongAdvanceTime * pingpongCurrentIndex+1))
            {
                pingpongCurrentIndex += 1; // advance to next message 
                if (pingpongCurrentIndex >= pingPongAnimationSequenceLength)
                {
                    pingpongCurrentIndex = 0;
                }
                pingPongDMXChannel01 = pingPongAnimationSequence[pingpongCurrentIndex];
                
                // find the corresponding light to this DMX channel
                /*
                for (j = 0; j < actualNumberOfLights; ++j)
                {
                    if (z3SingleLightCtrl[j].DMXStartChannel == pingPongDMXChannel01) // found matching light
                    {
                        pingPongDMXChannel02 = z3SingleLightCtrl[j].partnerLightChannel01;
                        if (pingPongDMXChannel02 != 0) // if there is a partner light
                        {
                            for (k = 0; k < actualNumberOfLights; ++k) // find matching partner light
                            {
                                if (z3SingleLightCtrl[k].DMXStartChannel == pingPongDMXChannel02) // found matching partner light
                                {
                                    z3SingleLightCtrl[k].animateHighlightValueAndHoldFor(highValue, turnOnTime, pingPongHoldOnTime);
                                }
                            }
                        }
                        z3SingleLightCtrl[j].animateHighlightValueAndHoldFor(highValue, turnOnTime, pingPongHoldOnTime);
                    }
                }*/
                
                
            }
            
        }
        else if (ambientAnimationTimer > looptime)
        {
            ambientAnimationTimer = 0;
        }
        else // we are in the pauses between times
        {
            //for (i = 0; i < actualNumberOfLights; ++i)
            //{
                //z3SingleLightCtrl[i].animateHighlightValueTo(lowValue, turnOffTime);
            //}
        }

        /*
        int nextRegionToActivate = (int)Math.Floor(ambientAnimationTimer / 100f);

        int nextRegionToDeactivate = (int)Math.Floor((ambientAnimationTimer -keepRegionActiveFor) / 100f);

        if (nextRegionToActivate > 0)
        {
            if (nextRegionToDeactivate < actualNumberOfRegions)
            {
                if (!regionIsActivated[nextRegionToActivate])
                {
                    regionIsActivated[nextRegionToActivate] = true;
                    activateLightsInvicinityOf(nextRegionToActivate);
                }
            }
        }

        if (nextRegionToDeactivate > 0)
        {
            if (nextRegionToDeactivate < actualNumberOfRegions)
            {
                if (regionIsActivated[nextRegionToDeactivate])
                {
                    regionIsActivated[nextRegionToDeactivate] = false;
                    deactivateLightsInvicinityOf(nextRegionToDeactivate);
                }
            }
            else
            {
                ambientAnimationTimer = 0; // reset and start again
            }
        }*/
    }
    #endregion ambientMode

    public void turnOffAllLights()
    {
        fadeOutAllLights();
    }
    
    
    public void turnOnAllLights()
    {
        fadeInAllLights();
    }

    public void toggleAllLightboxesOrLightsOn()
    {
        toggleLightBoxTest = !toggleLightBoxTest;

        if (toggleLightBoxTest)
        {
            showAllLightBoxTest();
        }
        else
        {
            showAllLightStripTest();
        }
    }

    private void illuminateLogo()
    {
        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            if (z3SingleLightCtrl[i].isAmazonLogo)
            {
                z3SingleLightCtrl[i].animateAmbientValueTo(highValue, typicalAnimationTime);
                z3SingleLightCtrl[i].animateHighlightValueTo(0, typicalAnimationTime);
            }
        }*/
    }

    
    public void fadeOutAllLightsIgnoringReactive()
    {
        if (!initComplete)
            return;

        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            if (z3SingleLightCtrl[i].isAmazonLogo) // except for logo!
            {
                z3SingleLightCtrl[i].animateAmbientValueTo(highValue, typicalAnimationTime);
            }
            else
            {
                z3SingleLightCtrl[i].reactingOff = true;
                z3SingleLightCtrl[i].reactingOn = false;
                z3SingleLightCtrl[i].animateAmbientValueTo(lowValue, typicalAnimationTime);
            }
        }*/
    }
    
    private void fadeOutAllLights()
    {
        if (!initComplete)
            return;

        for (int i = 0; i < actualNumberOfTrees; ++i)
        {
            treeOrg[i].fadeOutAllLights();
        }

        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            /*
            if (z3SingleLightCtrl[i].isAmazonLogo) // except for logo!
            {
                z3SingleLightCtrl[i].animateAmbientValueTo(highValue, typicalAnimationTime);
            }
            else
            {
                if (appControl.currentApplicationState == ApplicationControl.ApplicationState.reactToPresence) // skip over any active regions?
                {
                    if (!isLightInTheVacinityOfAnActiveRegion(i)) // make sure light is not in an active region before shutting it down
                    {
                        if (!z3SingleLightCtrl[i].reactingOff)
                        {
                            z3SingleLightCtrl[i].reactingOff = true;
                            z3SingleLightCtrl[i].reactingOn = false;
                            z3SingleLightCtrl[i].animateAmbientValueTo(lowValue, typicalAnimationTime);
                            //z3SingleLightCtrl[i].animateHighlightValueTo(0, typicalAnimationTime);
                        }
                    }
                }
                else
                {
                    z3SingleLightCtrl[i].reactingOff = true;
                    z3SingleLightCtrl[i].reactingOn = false;
                    z3SingleLightCtrl[i].animateAmbientValueTo(lowValue, typicalAnimationTime);
                }
            //}
        }*/
    }

    private void fadeInAllLights()
    {
        if (!initComplete)
            return;

        for (int i = 0; i < actualNumberOfTrees; ++i)
        {
            treeOrg[i].fadeInAllLights();
        }
        /*
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            z3SingleLightCtrl[i].animateAmbientValueTo(highValue, typicalAnimationTime);
            //z3SingleLightCtrl[i].animateHighlightValueTo(0, typicalAnimationTime);
        }*/
    }
    
    private void showAllLightBoxTest()
    {
        if (!initComplete)
            return;

    }
    
    private void showAllLightStripTest()
    {
        if (!initComplete)
            return;

    }

    
    #region initialization
    
    
    /*
    public void assignActualNumberOfRegions(int whichNumberOfRegions)
    {
        actualNumberOfRegions = whichNumberOfRegions;
    }

    public void assignRegionPositionFromProxFeedback(int whichRegion, float whichPosition)
    {
        regionPosition[whichRegion] = whichPosition;
    }*/

    public void initLights()
    {
        config = appConfig.GetConfig;
        sortLightObjects();
        initComplete = true;
        fadeOutAllLights();
        //illuminateLogo();

    }
    
    private void sortLightObjects()
    {
        int totalLights = 0;
        for (int i = 0; i < actualNumberOfTrees; ++i)
        {
            totalLights = treeOrg[i].sortLightObjects(i);
            logTextCtrl.logText("[LightCtrl] total lights in tree "+i+" = "+ totalLights, true);
        }

        treeGlobeCtrl = globeObj.GetComponent<TreeGlobeCtrl>();
        totalLights = treeGlobeCtrl.organizeAttachedLights();
        logTextCtrl.logText("[LightCtrl] total globe lights = "+ totalLights, true);
        /*
        List<GameObject> genericLightObj = new List<GameObject>();

        Transform parent_t = this.transform;

        foreach (Transform child_t in parent_t)
        {
            if (child_t != null)
                genericLightObj.Add(child_t.gameObject);
        }*/

        /*
        actualNumberOfLights = genericLightObj.Count;
        if (actualNumberOfLights > maxNumberOfLights)
            actualNumberOfLights = maxNumberOfLights;


        string objName;
        int i;

        for (i = 0; i < actualNumberOfLights; ++i)
        {
            lights[i] = genericLightObj[i];
            z3SingleLightCtrl[i] = lights[i].GetComponent<Z_InidivdualLightCtrl>();

            z3SingleLightCtrl[i].screenPosition = lights[i].transform.position;

            if (z3SingleLightCtrl[i].screenPosition.x < minXPosn)
                minXPosn = z3SingleLightCtrl[i].screenPosition.x;
            if (z3SingleLightCtrl[i].screenPosition.y < minYPosn)
                minYPosn = z3SingleLightCtrl[i].screenPosition.y;
            if (z3SingleLightCtrl[i].screenPosition.x > maxXPosn)
                maxXPosn = z3SingleLightCtrl[i].screenPosition.x;
            if (z3SingleLightCtrl[i].screenPosition.y > maxYPosn)
                maxYPosn = z3SingleLightCtrl[i].screenPosition.y;


            if (z3SingleLightCtrl[i].isCanopy)
            {
                objName = "Canopy" + i;
                z3SingleLightCtrl[i].init(i, objName);
                lights[i].name = objName;
            }
            else
            {
                if (z3SingleLightCtrl[i].isTrunk)
                {
                    objName = "Trunk" + i;
                    z3SingleLightCtrl[i].init(i, objName);
                    lights[i].name = objName;
                }
                else
                {
                    objName = "Globe" + i;
                    z3SingleLightCtrl[i].init(i, objName);
                    lights[i].name = objName;
                }
            }
        }*/
    }
    
    #endregion initialization
}
