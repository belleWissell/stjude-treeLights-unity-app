using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using AAMVC.Unity;
public class TreePartOrg : MonoBehaviour
{
    private AAMVC.Unity.ApplicationControl appControl;
    private AppConfig appConfig;
    private AppConfig.Config config;

    public bool isTertiaryCanopy = false;
    
    private int globalFPS = 30; // must match Application.targetFrameRate = 30;
    private int typicalAnimationTime = 40; // this used to 80
    private int numberOfDMXChannelsPerLight = 3;
    
    public static int maxNumberOfLightsPerPart = 16;
    private int actualNumberOfLights = 0;
    private GameObject[] lights = new GameObject[maxNumberOfLightsPerPart];
    private Z_InidivdualLightCtrl[] lightCtrl = new Z_InidivdualLightCtrl[maxNumberOfLightsPerPart];
        
    private void Awake()
    {
        appControl = AAMVC.Unity.ApplicationControl.Instance;
        appConfig = AppConfig.Instance;
        if (globalFPS == 60)
            typicalAnimationTime = 80;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void adjustColorsTo(Color whichColor0, Color whichColor1) // will distribute range of colors between these two values
    {
        
        Vector3 startColor = new Vector3(whichColor0.r, whichColor0.g, whichColor0.b);
        Vector3 endColor = new Vector3(whichColor1.r, whichColor1.g, whichColor1.b);
        float deltaR = endColor.x - startColor.x;
        float deltaG = endColor.y - startColor.y;
        float deltaB = endColor.z - startColor.z;

        Vector3 newColor;
        
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            newColor.x = startColor.x + deltaR * i/(float)actualNumberOfLights;
            newColor.y = startColor.y + deltaG * i/(float)actualNumberOfLights;
            newColor.z = startColor.z + deltaB * i/(float)actualNumberOfLights;
            
            lightCtrl[i].animateHighlightValueTo(0, 15);
            lightCtrl[i].animatColorValueTo(newColor, 75);
        }
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

    public int organizeAttachedLights(int whichTreeID, int whichPartID, int whichStartDMXChannel)
    {
        int valueToReturn = 0;
        // collect all children of this GO (should all be lights)
        List<GameObject> genericLightPart = new List<GameObject>();
        Transform parent_t = this.transform;
        
        foreach (Transform child_t in parent_t)
        {
            if (child_t != null)
                genericLightPart.Add(child_t.gameObject);
        }

        int i;
        actualNumberOfLights = genericLightPart.Count;
        int runningDMXChannelCounter = whichStartDMXChannel;
        for (i = 0; i < actualNumberOfLights; ++i)
        {
            lights[i] = genericLightPart[i];
            lightCtrl[i] = lights[i].GetComponent<Z_InidivdualLightCtrl>();
            if (lightCtrl[i].isTrunk)
            {
                lights[i].name = "T" + whichTreeID + "_tk_" + whichPartID + "_lt_" + i;
                lightCtrl[i].DMXUniverse = whichTreeID;
            }
            else if (lightCtrl[i].isCanopy)
            {
                lights[i].name = "T" + whichTreeID + "_ca_" + whichPartID + "_lt_" + i;
                lightCtrl[i].DMXUniverse = whichTreeID;
            }
            /*
            else if (lightCtrl[i].isGlobe)
            {
                lights[i].name = "T" + whichTreeID + "_gb_" + whichPartID + "_lt_" + i;
                lightCtrl[i].DMXUniverse = 4;
            }*/
            
            lightCtrl[i].DMXStartChannel = runningDMXChannelCounter;
            
            runningDMXChannelCounter += numberOfDMXChannelsPerLight;
        }

        //turnOffAllLights();
        return actualNumberOfLights;
        
    }

    public void turnOffAllLights()
    {
        Vector3 offColor = new Vector3(0, 0, 0);
        for (int i = 0; i < actualNumberOfLights; ++i)
        {
            lightCtrl[i].animateHighlightValueTo(0, 15);
            lightCtrl[i].animatColorValueTo(offColor, 25);
        }
    }
}
