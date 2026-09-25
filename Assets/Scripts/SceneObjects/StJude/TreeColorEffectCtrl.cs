using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAMVC.Unity;
//using UnityEditor.Experimental.GraphView;

public class TreeColorEffectCtrl : MonoBehaviour
{
    private ApplicationControl appControl;
    private AppConfig appConfig;
    private AppConfig.Config config;
    private LogTextCtrl logTextCtrl;
    private bool configLoaded = false;

    private bool initComplete = false;

    [Header("Tree A:")]
    [Header("Trunk A Colors:")] public Color[] trunkColorA = new Color[2]; //Color.white;
    [Header("Canopy A Colors:")] public Color[] canopyColorA = new Color[2]; 
    [Header("Globe A Colors:")] public Color globeColorA = new Color(0,0,0);

    [Header("Tree B:")]
    [Header("Trunk B Colors:")] public Color[] trunkColorB = new Color[2]; //Color.white;
    [Header("Canopy B Colors:")] public Color[] canopyColorB = new Color[2]; 
    [Header("Globe B Colors:")] public Color globeColorB = new Color(0,0,0);

    [Header("Tree C:")]
    [Header("Trunk C Colors:")] public Color[] trunkColorC = new Color[2]; //Color.white;
    [Header("Canopy C Colors:")] public Color[] canopyColorC = new Color[2]; 
    [Header("Globe C Colors:")] public Color globeColorC = new Color(0,0,0);

    private static int numberOfTrees = 3;
    private Color[,] trunkColor_local = new Color[numberOfTrees, 2];
    private Color[,] canopyColor_local = new Color[numberOfTrees, 2];
    private Color[] globeColor_local = new Color[numberOfTrees];

    private int currentPresetIndex = -1;
    private int previousPresetIndex = -1;
    private int presetIndexChangeCounter = 0;
    
    private void Awake()
    {
        for (int i = 0; i < numberOfTrees; ++i)
        {
            trunkColor_local[i, 0] = Color.white;
            trunkColor_local[i, 1] = Color.white;
            
            canopyColor_local[i, 0] = Color.gray;
            canopyColor_local[i, 1] = Color.gray;

            globeColor_local[i] = Color.blueViolet;
        }
        
        appControl = AAMVC.Unity.ApplicationControl.Instance;
        appConfig = AppConfig.Instance;
        appConfig.OnConfigLoaded.AddListener( OnConfigLoaded);

        logTextCtrl = LogTextCtrl.Instance;

    }

    void OnConfigLoaded()
    {
        if (configLoaded)
            return; // we already did this

        config = appConfig.GetConfig;
        configLoaded = true;
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

        bool doupdateFeedback = false;
        
        // did someone change (trunk) in the interface?
        if (trunkColor_local[0, 0] != trunkColorA[0] ||  trunkColor_local[0, 1] != trunkColorA[1])
        {
            trunkColor_local[0, 0] =  trunkColorA[0];
            trunkColor_local[0, 1] =  trunkColorA[1];
            appControl.updateSingleTreeTrunkColorsTo(0, trunkColor_local[0, 0], trunkColor_local[0, 1]);
            doupdateFeedback = true;
        }
        else if (trunkColor_local[1, 0] != trunkColorB[0] ||  trunkColor_local[1, 1] != trunkColorB[1])
        {
            trunkColor_local[1, 0] =  trunkColorB[0];
            trunkColor_local[1, 1] =  trunkColorB[1];
            appControl.updateSingleTreeTrunkColorsTo(1, trunkColor_local[1, 0], trunkColor_local[1, 1]);
            doupdateFeedback = true;
        }
        else if (trunkColor_local[2, 0] != trunkColorC[0] ||  trunkColor_local[2, 1] != trunkColorC[1])
        {
            trunkColor_local[2, 0] =  trunkColorC[0];
            trunkColor_local[2, 1] =  trunkColorC[1];
            appControl.updateSingleTreeTrunkColorsTo(2, trunkColor_local[2, 0], trunkColor_local[2, 1]);
            doupdateFeedback = true;
        }
        
        // did someone change (canopy) in the interface?
        if (canopyColor_local[0, 0] != canopyColorA[0] ||  canopyColor_local[0, 1] != canopyColorA[1])
        {
            canopyColor_local[0, 0] =  canopyColorA[0];
            canopyColor_local[0, 1] =  canopyColorA[1];
            appControl.updateSingleTreeCanopyColorsTo(0, canopyColor_local[0, 0], canopyColor_local[0, 1]);
            doupdateFeedback = true;
        }
        else if (canopyColor_local[1, 0] != canopyColorB[0] ||  canopyColor_local[1, 1] != canopyColorB[1])
        {
            canopyColor_local[1, 0] =  canopyColorB[0];
            canopyColor_local[1, 1] =  canopyColorB[1];
            appControl.updateSingleTreeCanopyColorsTo(1, canopyColor_local[1, 0], canopyColor_local[1, 1]);
            doupdateFeedback = true;
        }
        else if (canopyColor_local[2, 0] != canopyColorC[0] ||  canopyColor_local[2, 1] != canopyColorC[1])
        {
            canopyColor_local[2, 0] =  canopyColorC[0];
            canopyColor_local[2, 1] =  canopyColorC[1];
            appControl.updateSingleTreeCanopyColorsTo(2, canopyColor_local[2, 0], canopyColor_local[2, 1]);
            doupdateFeedback = true;
        }

        // did someone change (globe) in the interface?
        if (globeColor_local[0] != globeColorA)
        {
            globeColor_local[0] = globeColorA;
            appControl.updateSingleTreeGlobeColorTo(0, globeColor_local[0]);
            doupdateFeedback = true;
        }
        else if (globeColor_local[1] != globeColorB)
        {
            globeColor_local[1] = globeColorB;
            appControl.updateSingleTreeGlobeColorTo(1, globeColor_local[1]);
            doupdateFeedback = true;
        }
        else if (globeColor_local[2] != globeColorC)
        {
            globeColor_local[2] = globeColorC;
            appControl.updateSingleTreeGlobeColorTo(2, globeColor_local[2]);
            doupdateFeedback = true;
        }

        
        if (doupdateFeedback)
        {
            if (currentPresetIndex == previousPresetIndex) 
                appControl.updateModeFeedbackToCustom(); // colors changed, but not from a preset
            
        }

        if (currentPresetIndex != previousPresetIndex)
        {
            presetIndexChangeCounter += 1;
            if (presetIndexChangeCounter >= 3)
            {
                previousPresetIndex = currentPresetIndex;
            }
        }


        /*
        if (appControl.currentTreeColorMode == ApplicationControl.TreeColorMode.solidTreesNewDay || appControl.currentTreeColorMode == ApplicationControl.TreeColorMode.solidTreesPeakDay)
        {
            if ((primaryColor[0] != primaryColor_local[0]) || (primaryColor[1] != primaryColor_local[1]))
            {
                primaryColor_local[0] = primaryColor[0];
                primaryColor_local[1] = primaryColor[1];
                appControl.updateIndividualTreeColorsTo(0, primaryColor[0], primaryColor[1]); // send to tree near wall
            }
            if ((secondaryColor[0] != secondaryColor_local[0]) || (secondaryColor[1] != secondaryColor_local[1]))
            {
                secondaryColor_local[0] = secondaryColor[0];
                secondaryColor_local[1] = secondaryColor[1];
                appControl.updateIndividualTreeColorsTo(1, secondaryColor[0], secondaryColor[1]);
            }
            if ((tertiaryColor[0] != tertiaryColor_local[0]) || (tertiaryColor[1] != tertiaryColor_local[1]))
            {
                tertiaryColor_local[0] = tertiaryColor[0];
                tertiaryColor_local[1] = tertiaryColor[1];
                appControl.updateIndividualTreeColorsTo(2, tertiaryColor[0], tertiaryColor[1]); // send to tree near reception desk
            }
        }
        else
        {
            // did someone change something in the interface?

            if ((primaryColor[0] != primaryColor_local[0]) || (primaryColor[1] != primaryColor_local[1]))
            {
                primaryColor_local[0] = primaryColor[0];
                primaryColor_local[1] = primaryColor[1];
                appControl.updateAllTreeColorsTo(0, primaryColor[0], primaryColor[1]);
            }

            if ((secondaryColor[0] != secondaryColor_local[0]) || (secondaryColor[1] != secondaryColor_local[1]))
            {
                secondaryColor_local[0] = secondaryColor[0];
                secondaryColor_local[1] = secondaryColor[1];
                appControl.updateAllTreeColorsTo(1, secondaryColor[0], secondaryColor[1]);
            }

            if ((tertiaryColor[0] != tertiaryColor_local[0]) || (tertiaryColor[1] != tertiaryColor_local[1]))
            {
                tertiaryColor_local[0] = tertiaryColor[0];
                tertiaryColor_local[1] = tertiaryColor[1];
                appControl.updateAllTreeColorsTo(2, tertiaryColor[0], tertiaryColor[1]);
            }
        }*/
    }
    
    public void changeColorSchemeTo(int whichColorPresetIndex)
    {
        int i, j;
        
        string[,] whichNewTrunkHex = new string[3,2];
        string[,] whichNewCanopyHex = new string[3,2];
        string[] whichNewGlobeHex = new string[3];
        logTextCtrl.logText("[TreeColor] adjusting to color mode index " + whichColorPresetIndex , true);

        if (whichColorPresetIndex >= config.treeColorPresets.treeColorPreset.Length)
        {
            logTextCtrl.logText("[TreeColor] color mode ID is not defined" , true);
            //yield break;
        }
        else
        {
            currentPresetIndex = whichColorPresetIndex;
            presetIndexChangeCounter = 0;
            
            logTextCtrl.logText("[TreeColor] adjusting to color mode " + config.treeColorPresets.treeColorPreset[whichColorPresetIndex].name, true);

            int whichTreeID = 0;

            whichNewTrunkHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorA.trunkGradA;
            whichNewTrunkHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorA.trunkGradB;
            whichNewCanopyHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorA.canopyGradA;
            whichNewCanopyHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorA.canopyGradB;
            whichNewGlobeHex[whichTreeID] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorA.globe;

            whichTreeID = 1;
            whichNewTrunkHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorB.trunkGradA;
            whichNewTrunkHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorB.trunkGradB;
            whichNewCanopyHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorB.canopyGradA;
            whichNewCanopyHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorB.canopyGradB;
            whichNewGlobeHex[whichTreeID] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorB.globe;

            whichTreeID = 2;
            whichNewTrunkHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorC.trunkGradA;
            whichNewTrunkHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorC.trunkGradB;
            whichNewCanopyHex[whichTreeID, 0] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorC.canopyGradA;
            whichNewCanopyHex[whichTreeID, 1] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorC.canopyGradB;
            whichNewGlobeHex[whichTreeID] = config.treeColorPresets.treeColorPreset[whichColorPresetIndex].treeColorC.globe;

            // now convert from hex to colors:
            Color newColor;

            for (j = 0; j < 2; ++j)
            {
                // trunk colors ----------------------------------------------------------------------------
                if (ColorUtility.TryParseHtmlString(whichNewTrunkHex[0, j], out newColor))
                {
                    trunkColorA[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] trunk A color " + j + " [" + whichNewTrunkHex[0, j] + "] is not a valid color", true);
                }

                if (ColorUtility.TryParseHtmlString(whichNewTrunkHex[1, j], out newColor))
                {
                    trunkColorB[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] trunk B color " + j + " [" + whichNewTrunkHex[1, j] + "] is not a valid color", true);
                }

                if (ColorUtility.TryParseHtmlString(whichNewTrunkHex[2, j], out newColor))
                {
                    trunkColorC[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] trunk C color " + j + " [" + whichNewTrunkHex[2, j] + "] is not a valid color", true);
                }

                // canopy colors ----------------------------------------------------------------------------
                if (ColorUtility.TryParseHtmlString(whichNewCanopyHex[0, j], out newColor))
                {
                    canopyColorA[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] canopy A color " + j + " [" + whichNewCanopyHex[0, j] + "] is not a valid color", true);
                }

                if (ColorUtility.TryParseHtmlString(whichNewCanopyHex[1, j], out newColor))
                {
                    canopyColorB[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] canopy B color " + j + " [" + whichNewCanopyHex[1, j] + "] is not a valid color", true);
                }

                if (ColorUtility.TryParseHtmlString(whichNewCanopyHex[2, j], out newColor))
                {
                    canopyColorC[j] = newColor;
                }
                else
                {
                    logTextCtrl.logText("[TreeColor] canopy C color " + j + " [" + whichNewCanopyHex[2, j] + "] is not a valid color", true);
                }

            }

            // globe colors ----------------------------------------------------------------------------
            if (ColorUtility.TryParseHtmlString(whichNewGlobeHex[0], out newColor))
            {
                globeColorA = newColor;
            }
            else
            {
                logTextCtrl.logText("[TreeColor] globe A color [" + whichNewGlobeHex[0] + "] is not a valid color", true);
            }

            if (ColorUtility.TryParseHtmlString(whichNewGlobeHex[1], out newColor))
            {
                globeColorB = newColor;
            }
            else
            {
                logTextCtrl.logText("[TreeColor] globe B color [" + whichNewGlobeHex[1] + "] is not a valid color", true);
            }

            if (ColorUtility.TryParseHtmlString(whichNewGlobeHex[2], out newColor))
            {
                globeColorC = newColor;
            }
            else
            {
                logTextCtrl.logText("[TreeColor] globe C color [" + whichNewGlobeHex[2] + "] is not a valid color", true);
            }
            
            
            
        }
    }
    

    /*
    public void adjustColorsTo(ApplicationControl.TreeColorMode whichNewColorMode)
    {
        string[] whichNewPrimaryHex = new string[2];
        string[] whichNewSecondaryHex = new string[2];
        string[] whichNewTertiaryHex = new string[2];

        switch (whichNewColorMode)
        {
            case ApplicationControl.TreeColorMode.newDay:
                whichNewPrimaryHex[0] = config.lightColorSettings0.trunk1Hex;
                whichNewPrimaryHex[1] = config.lightColorSettings0.trunk2Hex;
                whichNewSecondaryHex[0] = config.lightColorSettings0.secondary1Hex;
                whichNewSecondaryHex[1] = config.lightColorSettings0.secondary2Hex;
                whichNewTertiaryHex[0] = config.lightColorSettings0.tertiary1Hex;
                whichNewTertiaryHex[1] = config.lightColorSettings0.tertiary2Hex;
                break;
            case ApplicationControl.TreeColorMode.midDay:
                whichNewPrimaryHex[0] = config.lightColorSettings1.trunk1Hex;
                whichNewPrimaryHex[1] = config.lightColorSettings1.trunk2Hex;
                whichNewSecondaryHex[0] = config.lightColorSettings1.secondary1Hex;
                whichNewSecondaryHex[1] = config.lightColorSettings1.secondary2Hex;
                whichNewTertiaryHex[0] = config.lightColorSettings1.tertiary1Hex;
                whichNewTertiaryHex[1] = config.lightColorSettings1.tertiary2Hex;
                break;
            case ApplicationControl.TreeColorMode.lateDay:
                whichNewPrimaryHex[0] = config.lightColorSettings2.trunk1Hex;
                whichNewPrimaryHex[1] = config.lightColorSettings2.trunk2Hex;
                whichNewSecondaryHex[0] = config.lightColorSettings2.secondary1Hex;
                whichNewSecondaryHex[1] = config.lightColorSettings2.secondary2Hex;
                whichNewTertiaryHex[0] = config.lightColorSettings2.tertiary1Hex;
                whichNewTertiaryHex[1] = config.lightColorSettings2.tertiary2Hex;
                break;
            case ApplicationControl.TreeColorMode.solidTreesNewDay:
                whichNewPrimaryHex[0] = config.solidTreeColorSettings0.tree1aHex;
                whichNewPrimaryHex[1] = config.solidTreeColorSettings0.tree1bHex;
                whichNewSecondaryHex[0] = config.solidTreeColorSettings0.tree2aHex;
                whichNewSecondaryHex[1] = config.solidTreeColorSettings0.tree2bHex;
                whichNewTertiaryHex[0] = config.solidTreeColorSettings0.tree3aHex;
                whichNewTertiaryHex[1] = config.solidTreeColorSettings0.tree3bHex;
                break;
            case ApplicationControl.TreeColorMode.solidTreesPeakDay:
                whichNewPrimaryHex[0] = config.solidTreeColorSettings1.tree1aHex;
                whichNewPrimaryHex[1] = config.solidTreeColorSettings1.tree1bHex;
                whichNewSecondaryHex[0] = config.solidTreeColorSettings1.tree2aHex;
                whichNewSecondaryHex[1] = config.solidTreeColorSettings1.tree2bHex;
                whichNewTertiaryHex[0] = config.solidTreeColorSettings1.tree3aHex;
                whichNewTertiaryHex[1] = config.solidTreeColorSettings1.tree3bHex;
                break;
            case ApplicationControl.TreeColorMode.solidTreesLateDay:
                whichNewPrimaryHex[0] = config.solidTreeColorSettings2.tree1aHex;
                whichNewPrimaryHex[1] = config.solidTreeColorSettings2.tree1bHex;
                whichNewSecondaryHex[0] = config.solidTreeColorSettings2.tree2aHex;
                whichNewSecondaryHex[1] = config.solidTreeColorSettings2.tree2bHex;
                whichNewTertiaryHex[0] = config.solidTreeColorSettings2.tree3aHex;
                whichNewTertiaryHex[1] = config.solidTreeColorSettings2.tree3bHex;
                break;
         }

        Color newColor;

        /*
        logTextCtrl.logText("[TreeColor] adjusting to color mode " + whichNewColorMode.ToString(), true);
        for (int i = 0; i < 2; i++)
        {
            if (ColorUtility.TryParseHtmlString(whichNewPrimaryHex[i], out newColor))
                primaryColor[i] = newColor;
            if (ColorUtility.TryParseHtmlString(whichNewSecondaryHex[i], out newColor))
                secondaryColor[i] = newColor;
            if (ColorUtility.TryParseHtmlString(whichNewTertiaryHex[i], out newColor))
                tertiaryColor[i] = newColor;
        }
        */

    //}

    public void init() // called from initializeClassesAfterPause
    {
        initComplete = true;
    } 
}
