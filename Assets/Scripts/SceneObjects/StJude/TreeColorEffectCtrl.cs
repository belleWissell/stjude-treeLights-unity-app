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

    [Header("Trunk Colors:")] public Color[] primaryColor = new Color[2]; //Color.white;

    [Header("Secondary Colors:")] public Color[] secondaryColor = new Color[2]; 
    
    [Header("Tertiary Colors:")] public Color[] tertiaryColor = new Color[2];

    private Color[] primaryColor_local = new Color[2];
    private Color[] secondaryColor_local = new Color[2];
    private Color[] tertiaryColor_local = new Color[2];
    
    private void Awake()
    {
        primaryColor[0] = Color.white;
        primaryColor[1] = Color.white;
        secondaryColor[0] = Color.black;
        secondaryColor[1] = Color.black;
        tertiaryColor[0] = Color.gray;
        tertiaryColor[1] = Color.gray;
        
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
        }
    }

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

    }

    public void init()
    {
        initComplete = true;
    }
}
