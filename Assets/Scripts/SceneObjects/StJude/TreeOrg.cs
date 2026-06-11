using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAMVC.Unity;

public class TreeOrg : MonoBehaviour
{
    private LogTextCtrl logTextCtrl;

    private int numberOfDMXChannelsPerLight = 3;
    
    public int treeID;
    private static int maxNUmberOfTrunks = 2;
    public int actualNumberOfTrunks = 0;
    public GameObject[] treeTrunks = new GameObject[maxNUmberOfTrunks];
    private TreePartOrg[] trunkCtrl = new TreePartOrg[maxNUmberOfTrunks];
    private static int maxNUmberOfCanopies = 8;
    public int actualNumberOfCanopies = 0;
    public GameObject[] treeCanopies = new GameObject[maxNUmberOfCanopies];
    private TreePartOrg[] canopyCtrl = new TreePartOrg[maxNUmberOfCanopies];
    
    public GameObject treeGlobes;
    private TreeGlobeCtrl treeGlobeCtrl;
    
    private void Awake()
    {
        logTextCtrl = LogTextCtrl.Instance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateColorsTo(int whichColorChannel, Color whichColor0, Color whichColor1)
    {
        int i;
        
        if (whichColorChannel == 0) // primary = trunks
        {
            for (i = 0; i < actualNumberOfTrunks; ++i)
            {
                trunkCtrl[i].adjustColorsTo(whichColor0, whichColor1);
            }
        }
        else if (whichColorChannel ==1) // secondary - some/most canopies
        {
            for (i = 0; i < actualNumberOfCanopies; ++i)
            {
                if (!canopyCtrl[i].isTertiaryCanopy)
                {
                    canopyCtrl[i].adjustColorsTo(whichColor0, whichColor1);
                }
                
            }
        }
        else if (whichColorChannel == 2) // tertiary - just a few canopies
        {
            for (i = 0; i < actualNumberOfCanopies; ++i)
            {
                if (canopyCtrl[i].isTertiaryCanopy)
                {
                    canopyCtrl[i].adjustColorsTo(whichColor0, whichColor1);
                }
            }
        }
    }
    
    public void fadeOutAllLights()
    {
        Color black = Color.black;
        int i;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i].adjustColorsTo(black, black);
        }
        for (i = 0; i < actualNumberOfCanopies; ++i)
        { 
            canopyCtrl[i].adjustColorsTo(black, black);
        }
    }
    
    public void fadeInAllLights()
    {
        
    }
    
    public int sortLightObjects(int whichTree)
    {
        treeID = whichTree;
        
        int valueToReturn = 0;
        int i;

        int numberOfTrunkLights = 0;
        int numberOfLightsTemp = 0;
        int numberOfCanopyLights = 0;

        int runningDMXChannelCounter = 0;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i] = treeTrunks[i].GetComponent<TreePartOrg>();
            numberOfLightsTemp = trunkCtrl[i].organizeAttachedLights(treeID, i, runningDMXChannelCounter);
            runningDMXChannelCounter += numberOfLightsTemp * numberOfDMXChannelsPerLight;
            logTextCtrl.logText("[TREEORG] Tree #"+treeID+" trunk "+i+" light count "+ numberOfLightsTemp, true);
            numberOfTrunkLights += numberOfLightsTemp;
        }

        for (i = 0; i < actualNumberOfCanopies; ++i)
        {
            canopyCtrl[i] = treeCanopies[i].GetComponent<TreePartOrg>();
            numberOfLightsTemp = canopyCtrl[i].organizeAttachedLights(treeID, i, runningDMXChannelCounter);
            runningDMXChannelCounter += numberOfLightsTemp * numberOfDMXChannelsPerLight;
            logTextCtrl.logText("[TREEORG] Tree #"+treeID+" canopy "+i+" light count "+ numberOfLightsTemp, true);
            numberOfCanopyLights += numberOfLightsTemp;
        }
        
        logTextCtrl.logText("[TREEORG] Tree #"+treeID+" trunk count "+numberOfTrunkLights+" canopy count "+ numberOfCanopyLights, true);
        
        valueToReturn = numberOfTrunkLights + numberOfCanopyLights;
        return valueToReturn;
    }

    public int sortGlobeLights(int whichTree, int whichRunningDMXChannel) 
    {
        int valueToReturn = 0;
        treeGlobeCtrl = treeGlobes.GetComponent<TreeGlobeCtrl>();

        int numberOfLightsTemp = treeGlobeCtrl.organizeAttachedLights();
        logTextCtrl.logText("[TREEORG] Globe light count "+ numberOfLightsTemp, true);

        return valueToReturn;
    }

    public void checkForColorChanges()
    {
        int i;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i].checkForColorChanges();
        }

        for (i = 0; i < actualNumberOfCanopies; ++i)
        {
            canopyCtrl[i].checkForColorChanges();
        }
    }
    
    
}
