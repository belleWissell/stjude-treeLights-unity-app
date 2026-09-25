using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGlobeCtrl : MonoBehaviour
{
    private AAMVC.Unity.ApplicationControl appControl;

    private bool initComplete = false;
    
    private static int maxNumberOfGlobeStringsPerTree = 6;
    [Header("Globe Lights:")]

    public int actualNumberOfGlobeStrings0 = 0;
    public GameObject[] tree0GlobeStrings= new GameObject[maxNumberOfGlobeStringsPerTree];
    public int actualNumberOfGlobeStrings1= 0;
    public GameObject[] tree1GlobeStrings= new GameObject[maxNumberOfGlobeStringsPerTree];
    public int actualNumberOfGlobeStrings2 = 0;
    public GameObject[] tree2GlobeStrings= new GameObject[maxNumberOfGlobeStringsPerTree];
    
    [Header("Relays:")]

    public GameObject relayGlobeString;
    
    
    private static int maxNumberOfGlobeLights = 250;
    private int actualNumberOfGlobeLights = 0;
    private GameObject[] lights = new GameObject[maxNumberOfGlobeLights];
    private Z_InidivdualLightCtrl[] lightCtrl = new Z_InidivdualLightCtrl[maxNumberOfGlobeLights];

    private int numberOfDMXChannelsPerLight = 3;
    
    // ambient sparkles
    [Header("Ambient Sparkle Settings:")] 
    private bool ambientSparkleAnimationIsActive = false;
    private int ambientSparkleCounter = 0;
    
    [Header("Animation variables:")]

    public int ambientSparkleFrequency = 8;
    public int numberOfAmbientSparklesPerPass = 3;
    public int sparkleRampUp = 8;
    public int sparkleHold = 20;

    // wave sparkles
    public GameObject globeWaveCenterPoint;
    private float sparkleRadius;
    private bool sparkleWaveIsActive = false;
    public float sparkleWaveSpeed = 0.1f;
    private void Awake()
    {
        appControl = AAMVC.Unity.ApplicationControl.Instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!initComplete)
            return;
        
        
        if (ambientSparkleAnimationIsActive)
        {
            updateAmbientSparkles();
        }

        if (sparkleWaveIsActive)
        {
            updateSparkleWave();
        }
    }

    public void fadeOutAllLights()
    {
        Vector3 lightOff = new Vector3(0, 0, 0);
        Vector3 lightOn = new Vector3(1, 1, 1);
        //ambientSparkleAnimationIsActive = false;
        
        for (int i = 0; i < actualNumberOfGlobeLights; ++i)
        {
            if (i==actualNumberOfGlobeLights-1)
                lightCtrl[i].animatColorValueTo(lightOn, 10);
            else
                lightCtrl[i].animatColorValueTo(lightOff, 10);
        }
    }

    public void fadeInAllLights()
    {
        Vector3 lightOn = new Vector3(1, 1, 1);

        for (int i = 0; i < actualNumberOfGlobeLights; ++i)
        {
            lightCtrl[i].animatColorValueTo(lightOn, 10);
        }

    }

    public void updateColorsTo(int whichTreeIndex, Color whichColor0)
    {
        
        Vector3 newColor = new Vector3(whichColor0.r, whichColor0.g, whichColor0.b); // converts from color to vector3
        
        for (int i = 0; i < actualNumberOfGlobeLights; ++i)
        {
            if (lightCtrl[i].treeId == whichTreeIndex)
                lightCtrl[i].adjustGlobeDefaultColorTo(newColor);
        }
    }
    
    public void startAmbientSparkles()
    {
        ambientSparkleCounter = 0;
        ambientSparkleAnimationIsActive = true;
    }

    public void stopAmbientSparkles()
    {
        ambientSparkleAnimationIsActive = false;
    }

    public void startSparkleWave()
    {
        sparkleRadius = 0;
        resetAllLightSparkleWaves();
        sparkleWaveIsActive = true;
    }

    
    private void updateAmbientSparkles()
    {
        ambientSparkleCounter += 1;
        int pickRandomLight = -1;
        int i;
        
        if (ambientSparkleCounter >= ambientSparkleFrequency)
        {
            ambientSparkleCounter = 0;
            // select n random lights and instruct them to sparkle
            for (i = 0; i < numberOfAmbientSparklesPerPass; ++i)
            {
                pickRandomLight = UnityEngine.Random.Range(0, actualNumberOfGlobeLights-1);
                lightCtrl[pickRandomLight].startAmbientGlobeSparkle(sparkleRampUp, sparkleHold);
            }
        }
    }

    private void resetAllLightSparkleWaves()
    {
        for (int i = 0; i < actualNumberOfGlobeLights-1; ++i)
        {

            lightCtrl[i].sparkleWaveAlreadyHit = false;
           
        }
    }
    private void updateSparkleWave()
    {
        sparkleRadius += sparkleWaveSpeed;
        int randomRampUp;
        int randomDelay;
        int randomHold;
        
        for (int i = 0; i < actualNumberOfGlobeLights-1; ++i)
        {
            if (lightCtrl[i].globeRangeFromWall < sparkleRadius)
            {
                if (!lightCtrl[i].sparkleWaveAlreadyHit)
                {
                    randomRampUp = UnityEngine.Random.Range(8, 12);
                    randomHold = UnityEngine.Random.Range(10, 20);
                    randomDelay = UnityEngine.Random.Range(0, 10);
                    lightCtrl[i].startAmbientGlobeSparkle(randomRampUp, randomHold+randomDelay, randomDelay);
                    lightCtrl[i].sparkleWaveAlreadyHit = true;
                }
            }
        }

        if (sparkleRadius > 20)
            sparkleWaveIsActive = false;
    }
    
    
    public void checkForColorChanges()
    {
        for (int i = 0; i < actualNumberOfGlobeLights; ++i)
        {
            byte newValue = 0;
            if (lightCtrl[i].isDirty())
            {
                lightCtrl[i].updateFeedback();
                //float retrieveBrightnessFromLight = lightCtrl[i].highlightBrightness;
                
                // retreive R, G, B
                newValue = (byte)Math.Floor(lightCtrl[i].currentColor.x * 255); // artnet values are between 0-255
                appControl.updateIndividualArtNetValue(lightCtrl[i].DMXUniverse,lightCtrl[i].DMXStartChannel, newValue);
                newValue = (byte)Math.Floor(lightCtrl[i].currentColor.y * 255);
                appControl.updateIndividualArtNetValue(lightCtrl[i].DMXUniverse,lightCtrl[i].DMXStartChannel + 1, newValue);
                newValue = (byte)Math.Floor(lightCtrl[i].currentColor.z * 255);
                appControl.updateIndividualArtNetValue(lightCtrl[i].DMXUniverse,lightCtrl[i].DMXStartChannel + 2, newValue);

                // retreive W
                //newValue = (byte)Math.Floor((lightCtrl[i].highlightBrightness / 100f) * 255);
                //appControl.updateIndividualArtNetValue(lightCtrl[i].DMXUniverse, lightCtrl[i].DMXStartChannel + 4, newValue);
            }
        }
    }
    
    #region initialize

    public int organizeAttachedLights()
    {
        int valueToReturn = 0;
        //int numberOfGlobeLightsInTree = 0;
        int numberOfGlobeLightsInLightString = 0;
        int i, j;
        // collect all children of this GO (should all be lights)
        int runningDMXChannelCounter = 0;
        int whichDMXUniverse;

        int runningLightControllerCounter = 0;
        float rangeFromWallTarget;


        for (i = 0; i < actualNumberOfGlobeStrings0; ++i)
        {
            List<GameObject> genericLightPart0 = new List<GameObject>();
            Transform parent_LightString = tree0GlobeStrings[i].transform;
            TreePartOrg treePartOrg0 = tree0GlobeStrings[i].GetComponent<TreePartOrg>();

            foreach (Transform child_t in parent_LightString)
            {
                if (child_t != null)
                    genericLightPart0.Add(child_t.gameObject);
            }

            numberOfGlobeLightsInLightString = genericLightPart0.Count;
            actualNumberOfGlobeLights += numberOfGlobeLightsInLightString;
            runningDMXChannelCounter = treePartOrg0.treePartDMXStartChannel - 1;
            whichDMXUniverse = treePartOrg0.treePartUniverse - 1;
            for (j = 0; j < numberOfGlobeLightsInLightString; ++j)
            {
                lights[runningLightControllerCounter] = genericLightPart0[j];
                lightCtrl[runningLightControllerCounter] = lights[runningLightControllerCounter].GetComponent<Z_InidivdualLightCtrl>();
                if (lightCtrl[runningLightControllerCounter].isGlobe)
                {
                    lightCtrl[runningLightControllerCounter].treeId = 0;
                    lights[runningLightControllerCounter].name = "T0_gb_lt_" + j;
                    lightCtrl[runningLightControllerCounter].DMXUniverse = whichDMXUniverse;
                    lightCtrl[runningLightControllerCounter].screenPosition = lights[runningLightControllerCounter].transform.position;
                    rangeFromWallTarget = Vector3.Distance(lights[runningLightControllerCounter].transform.position, globeWaveCenterPoint.transform.position);
                    lightCtrl[runningLightControllerCounter].globeRangeFromWall = rangeFromWallTarget;
                    lightCtrl[runningLightControllerCounter].DMXStartChannel = runningDMXChannelCounter;

                    runningLightControllerCounter += 1;
                    runningDMXChannelCounter += numberOfDMXChannelsPerLight;
                }
            }
        }


        for (i = 0; i < actualNumberOfGlobeStrings1; ++i)
        {
            List<GameObject> genericLightPart1 = new List<GameObject>();
            Transform parent_LightString = tree1GlobeStrings[i].transform;
            TreePartOrg treePartOrg1 = tree1GlobeStrings[i].GetComponent<TreePartOrg>();

            foreach (Transform child_t in parent_LightString)
            {
                if (child_t != null)
                    genericLightPart1.Add(child_t.gameObject);
            }

            numberOfGlobeLightsInLightString = genericLightPart1.Count;
            actualNumberOfGlobeLights += numberOfGlobeLightsInLightString;
            runningDMXChannelCounter = treePartOrg1.treePartDMXStartChannel - 1;
            whichDMXUniverse = treePartOrg1.treePartUniverse - 1;

            for (j = 0; j < numberOfGlobeLightsInLightString; ++j)
            {
                lights[runningLightControllerCounter] = genericLightPart1[j];
                lightCtrl[runningLightControllerCounter] = lights[runningLightControllerCounter].GetComponent<Z_InidivdualLightCtrl>();
                if (lightCtrl[runningLightControllerCounter].isGlobe)
                {
                    lightCtrl[runningLightControllerCounter].treeId = 1;
                    lights[runningLightControllerCounter].name = "T1_gb_lt_" + j;
                    lightCtrl[runningLightControllerCounter].DMXUniverse = whichDMXUniverse;
                    lightCtrl[runningLightControllerCounter].screenPosition = lights[runningLightControllerCounter].transform.position;
                    rangeFromWallTarget = Vector3.Distance(lights[runningLightControllerCounter].transform.position, globeWaveCenterPoint.transform.position);
                    lightCtrl[runningLightControllerCounter].globeRangeFromWall = rangeFromWallTarget;
                    lightCtrl[runningLightControllerCounter].DMXStartChannel = runningDMXChannelCounter;

                    runningLightControllerCounter += 1;
                    runningDMXChannelCounter += numberOfDMXChannelsPerLight;
                }
            }
        }


        for (i = 0; i < actualNumberOfGlobeStrings2; ++i)
        {
            List<GameObject> genericLightPart2 = new List<GameObject>();
            Transform parent_LightString = tree2GlobeStrings[i].transform;
            TreePartOrg treePartOrg2 = tree2GlobeStrings[i].GetComponent<TreePartOrg>();

            foreach (Transform child_t in parent_LightString)
            {
                if (child_t != null)
                    genericLightPart2.Add(child_t.gameObject);
            }

            numberOfGlobeLightsInLightString = genericLightPart2.Count;
            actualNumberOfGlobeLights += numberOfGlobeLightsInLightString;
            runningDMXChannelCounter = treePartOrg2.treePartDMXStartChannel - 1;
            whichDMXUniverse = treePartOrg2.treePartUniverse - 1;

            for (j = 0; j < numberOfGlobeLightsInLightString; ++j)
            {
                lights[runningLightControllerCounter] = genericLightPart2[j];
                lightCtrl[runningLightControllerCounter] = lights[runningLightControllerCounter].GetComponent<Z_InidivdualLightCtrl>();
                if (lightCtrl[runningLightControllerCounter].isGlobe)
                {
                    lightCtrl[runningLightControllerCounter].treeId = 2;
                    lights[runningLightControllerCounter].name = "T2_gb_lt_" + j;
                    lightCtrl[runningLightControllerCounter].DMXUniverse = whichDMXUniverse;
                    lightCtrl[runningLightControllerCounter].screenPosition = lights[runningLightControllerCounter].transform.position;
                    rangeFromWallTarget = Vector3.Distance(lights[runningLightControllerCounter].transform.position, globeWaveCenterPoint.transform.position);
                    lightCtrl[runningLightControllerCounter].globeRangeFromWall = rangeFromWallTarget;
                    lightCtrl[runningLightControllerCounter].DMXStartChannel = runningDMXChannelCounter;

                    runningLightControllerCounter += 1;
                    runningDMXChannelCounter += numberOfDMXChannelsPerLight;
                }
            }
        }

        List<GameObject> genericRelayLightPart = new List<GameObject>();
        Transform parent_LightString3 = relayGlobeString.transform;
        TreePartOrg relayPartOrg = relayGlobeString.GetComponent<TreePartOrg>();

        foreach (Transform child_t in parent_LightString3)
        {
            if (child_t != null)
                genericRelayLightPart.Add(child_t.gameObject);
        }

        numberOfGlobeLightsInLightString = genericRelayLightPart.Count;
        actualNumberOfGlobeLights += numberOfGlobeLightsInLightString;
        runningDMXChannelCounter = relayPartOrg.treePartDMXStartChannel - 1;
        whichDMXUniverse = relayPartOrg.treePartUniverse - 1;

        for (j = 0; j < numberOfGlobeLightsInLightString; ++j)
        {
            lights[runningLightControllerCounter] = genericRelayLightPart[j];
            lightCtrl[runningLightControllerCounter] = lights[runningLightControllerCounter].GetComponent<Z_InidivdualLightCtrl>();
            if (lightCtrl[runningLightControllerCounter].isGlobe)
            {
                lightCtrl[runningLightControllerCounter].treeId = 3;
                lights[runningLightControllerCounter].name = "T3_gb_lt_" + j;
                lightCtrl[runningLightControllerCounter].DMXUniverse = whichDMXUniverse;
                lightCtrl[runningLightControllerCounter].screenPosition = lights[runningLightControllerCounter].transform.position;
                rangeFromWallTarget = Vector3.Distance(lights[runningLightControllerCounter].transform.position, globeWaveCenterPoint.transform.position);
                lightCtrl[runningLightControllerCounter].globeRangeFromWall = rangeFromWallTarget;
                lightCtrl[runningLightControllerCounter].DMXStartChannel = runningDMXChannelCounter;

                runningLightControllerCounter += 1;
                runningDMXChannelCounter += numberOfDMXChannelsPerLight;
            }
        }


        appControl.logText("[TREEGLOBECTRL] Globes organized successfully. qty: " + actualNumberOfGlobeLights);
        initComplete = true;
        fadeOutAllLights();
        globeWaveCenterPoint.SetActive(false);
        return actualNumberOfGlobeLights;

    }

    #endregion initialize
}
