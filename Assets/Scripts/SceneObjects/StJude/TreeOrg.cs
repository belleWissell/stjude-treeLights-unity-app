using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AAMVC.Unity;

public class TreeOrg : MonoBehaviour
{
    private LogTextCtrl logTextCtrl;
    private int numberOfDMXChannelsPerLight = 3;
    public int treeID;
    
    public GameObject trunkGradPointA;
    public GameObject trunkGradPointB;
    public GameObject canopyGradPointA;
    public GameObject canopyGradPointB;
    private Vector3 trunkGradPointAPosition;
    private Vector3 trunkGradPointBPosition;
    private Vector3 canopyGradPointAPosition;
    private Vector3 canopyGradPointBPosition;
    
    private static int maxNUmberOfTrunks = 2;
    public int actualNumberOfTrunks = 0;
    public GameObject[] treeTrunks = new GameObject[maxNUmberOfTrunks];
    private TreePartOrg[] trunkCtrl = new TreePartOrg[maxNUmberOfTrunks];
    private static int maxNUmberOfCanopies = 8;
    public int actualNumberOfCanopies = 0;
    public GameObject[] treeCanopies = new GameObject[maxNUmberOfCanopies];
    private TreePartOrg[] canopyCtrl = new TreePartOrg[maxNUmberOfCanopies];
    private static int maxNUmberOfGlobes = 8;
    
    //public GameObject treeGlobes;
    //private TreeGlobeCtrl treeGlobeCtrl;
    
    private void Awake()
    {
        logTextCtrl = LogTextCtrl.Instance;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        trunkGradPointAPosition = trunkGradPointA.transform.position;
        trunkGradPointBPosition = trunkGradPointB.transform.position;
        canopyGradPointAPosition = canopyGradPointA.transform.position;
        canopyGradPointBPosition = canopyGradPointB.transform.position;
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
                trunkCtrl[i].adjustColorsToAndDistributeColorBetweenPoints(whichColor0, whichColor1, trunkGradPointAPosition, trunkGradPointBPosition);
            }
        }
        else if (whichColorChannel ==1) // secondary = canopies
        {
            for (i = 0; i < actualNumberOfCanopies; ++i)
            {
                canopyCtrl[i].adjustColorsToAndDistributeColorBetweenPoints(whichColor0, whichColor1, canopyGradPointAPosition, canopyGradPointBPosition);
            }
        }
        /*
        else if (whichColorChannel == 2) // globes 
        {
            
                treeGlobeCtrl.adjustColorsTo(whichColor0, whichColor1);
            
        }*/
    }
    
    public void fadeOutAllLights()
    {
        Color black = Color.black;
        int i;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i].adjustColorsToAndEvenlyGradient(black, black);
        }
        for (i = 0; i < actualNumberOfCanopies; ++i)
        { 
            canopyCtrl[i].adjustColorsToAndEvenlyGradient(black, black);
        }
    }
    
    public void fadeInAllLights()
    {
        Color white = Color.white;
        int i;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i].adjustColorsToAndEvenlyGradient(white, white);
        }
        for (i = 0; i < actualNumberOfCanopies; ++i)
        { 
            canopyCtrl[i].adjustColorsToAndEvenlyGradient(white, white);
        }
    }
    
    public int sortLightObjects(int whichTree)
    {
        treeID = whichTree;
        
        int valueToReturn = 0;
        int i;

        int numberOfTrunkLights = 0;
        int numberOfLightsTemp = 0;
        int numberOfCanopyLights = 0;

        //int runningDMXChannelCounter = 0;
        
        for (i = 0; i < actualNumberOfTrunks; ++i)
        {
            trunkCtrl[i] = treeTrunks[i].GetComponent<TreePartOrg>();
            numberOfLightsTemp = trunkCtrl[i].organizeAttachedLights(treeID, i);
            //runningDMXChannelCounter += numberOfLightsTemp * numberOfDMXChannelsPerLight;
            logTextCtrl.logText("[TREEORG] Tree #"+treeID+" trunk "+i+" light count "+ numberOfLightsTemp, true);
            numberOfTrunkLights += numberOfLightsTemp;
        }

        for (i = 0; i < actualNumberOfCanopies; ++i)
        {
            canopyCtrl[i] = treeCanopies[i].GetComponent<TreePartOrg>();
            numberOfLightsTemp = canopyCtrl[i].organizeAttachedLights(treeID, i);
            //runningDMXChannelCounter += numberOfLightsTemp * numberOfDMXChannelsPerLight;
            logTextCtrl.logText("[TREEORG] Tree #"+treeID+" canopy "+i+" light count "+ numberOfLightsTemp, true);
            numberOfCanopyLights += numberOfLightsTemp;
        }
        
        logTextCtrl.logText("[TREEORG] Tree #"+treeID+" trunk count "+numberOfTrunkLights+" canopy count "+ numberOfCanopyLights, true);
        
        valueToReturn = numberOfTrunkLights + numberOfCanopyLights;
        return valueToReturn;
    }

    /*
    public int sortGlobeLights(int whichTree, int whichRunningDMXChannel) 
    {
        int valueToReturn = 0;
        treeGlobeCtrl = treeGlobes.GetComponent<TreeGlobeCtrl>();

        int numberOfLightsTemp = treeGlobeCtrl.organizeAttachedLights();
        logTextCtrl.logText("[TREEORG] Globe light count "+ numberOfLightsTemp, true);

        return valueToReturn;
    }
    */

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
