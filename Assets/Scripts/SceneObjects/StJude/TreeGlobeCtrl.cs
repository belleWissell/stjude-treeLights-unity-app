using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGlobeCtrl : MonoBehaviour
{
    private AAMVC.Unity.ApplicationControl appControl;

    private bool initComplete = false;
    private static int numberOfTrees = 3;
    public GameObject[] treeGlobeObjects = new GameObject[numberOfTrees];
    
    public static int maxNumberOfGlobeLights = 250;
    private int actualNumberOfLights = 0;
    private GameObject[] lights = new GameObject[maxNumberOfGlobeLights];
    private Z_InidivdualLightCtrl[] lightCtrl = new Z_InidivdualLightCtrl[maxNumberOfGlobeLights];

    private int numberOfDMXChannelsPerLight = 3;
    
    // ambient sparkles
    [Header("Ambient Sparkle Settings:")] 
    private bool ambientSparkleAnimationIsActive = false;
    private int ambientSparkleCounter = 0;
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

    private void turnOffAllLights()
    {
        Vector3 lightOff = new Vector3(0, 0, 0);
        
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            lightCtrl[i].animatColorValueTo(lightOff, 10);
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
                pickRandomLight = UnityEngine.Random.Range(0, actualNumberOfLights);
                lightCtrl[pickRandomLight].startAmbientGlobeSparkle(sparkleRampUp, sparkleHold);
            }
        }
    }

    private void resetAllLightSparkleWaves()
    {
        for (int i = 0; i < actualNumberOfLights; ++i)
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
        
        for (int i = 0; i < actualNumberOfLights; ++i)
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
        for (int i = 0; i < actualNumberOfLights; ++i)
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

            /*
            if (z3SingleLightCtrl[i].isAmazonLogo) // override all amazon logo lights to always be on full value:
            {
                newValue = (byte)Math.Floor((highValue/100f) * 255);
                appControl.updateIndividualArtNetValue(z3SingleLightCtrl[i].DMXChannel, newValue);
            }*/
        }
    }
    
    #region initialize

    public int organizeAttachedLights()
    {
        int valueToReturn = 0;
        int numberOfGlobeLightsInTree = 0;
        int i, j;
        // collect all children of this GO (should all be lights)
        int runningDMXChannelCounter = 0;

        int runningLightControllerCounter = 0;
        float rangeFromWallTarget;
        
        for (i = 0; i < numberOfTrees; ++i) // go through each of the trees
        {
            List<GameObject> genericLightPart = new List<GameObject>();

            Transform parent_t = treeGlobeObjects[i].transform;

            foreach (Transform child_t in parent_t)
            {
                if (child_t != null)
                    genericLightPart.Add(child_t.gameObject);
            }

            
            numberOfGlobeLightsInTree = genericLightPart.Count;
            actualNumberOfLights += numberOfGlobeLightsInTree;
            
            for (j = 0; j < numberOfGlobeLightsInTree; ++j)
            {
                lights[runningLightControllerCounter] = genericLightPart[j];
                lightCtrl[runningLightControllerCounter] = lights[runningLightControllerCounter].GetComponent<Z_InidivdualLightCtrl>();
                if (lightCtrl[runningLightControllerCounter].isGlobe)
                {
                    lights[runningLightControllerCounter].name = "T" + i + "_gb_lt_" + j;
                    lightCtrl[runningLightControllerCounter].DMXUniverse = 4;
                    lightCtrl[runningLightControllerCounter].screenPosition = lights[runningLightControllerCounter].transform.position;
                    rangeFromWallTarget = Vector3.Distance(lights[runningLightControllerCounter].transform.position, globeWaveCenterPoint.transform.position);
                    lightCtrl[runningLightControllerCounter].globeRangeFromWall = rangeFromWallTarget;
                    lightCtrl[runningLightControllerCounter].DMXStartChannel = runningDMXChannelCounter;
                
                    runningLightControllerCounter += 1;
                    runningDMXChannelCounter += numberOfDMXChannelsPerLight;
                }

            }
        }
        
        if (actualNumberOfLights > maxNumberOfGlobeLights)
            actualNumberOfLights = maxNumberOfGlobeLights;
        
        initComplete = true;
        turnOffAllLights();
        globeWaveCenterPoint.SetActive(false);
        return actualNumberOfLights;
        
    }

    #endregion initialize
}
