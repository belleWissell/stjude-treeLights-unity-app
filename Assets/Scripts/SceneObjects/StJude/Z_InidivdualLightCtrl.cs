using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using AnimationEngine;

public class Z_InidivdualLightCtrl : MonoBehaviour
{
    public int treeId = 0;
    public bool isTrunk = false;
    public bool isCanopy = false;
    public int canopyId = 0;
    public bool isGlobe = false;
    public int DMXStartChannel;
    public int DMXUniverse;
    private int myID;
    public TextMeshPro valueFeedback;

    public int partnerLightChannel01 = 0; 
    public int partnerLightChannel02 = 0; // some messages have two nearby lights
    
    //private int value_i = 0;
    //public float ambientBrightness = 0;  // between 0 and 100
    public float highlightBrightness = 0;  // between 0 and 100
    public float targetHighlightBrightness = 0; 
    
    public Vector3 targetColor = Vector3.zero; // between 0 and 255
    public Vector3 currentColor = Vector3.zero; // between 0 and 255
    
    //public GameObject quadAmbient;
    //public GameObject quadHighlight;
    public GameObject lighSampleObj;
    
    
    private GeneralPurposeAnimEasy anim;

    // for sending out a wave of sparkles:
    public Vector2 screenPosition;
    public float globeRangeFromWall = 0f;
    public bool sparkleWaveAlreadyHit = false;
    
    private bool waitAndThenTurnOffHighlight = false;

    private int waitAndTurnOffHighlightTimer = 0;
    private int waitAndTurnOffHighlightTimerLimit = 0;

    public bool sweepingOn = false;
    public bool sweepingOff = false;
    
    public bool reactingOn = false;
    public bool reactingOff = false;

    //public TextMeshPro[] text = new TextMeshPro[maxNumberOfBars];
    [HideInInspector]
    public bool dirty = false;
    public bool isDirty()
    {
        bool valueToReturn = dirty;
        if (dirty)
        {
            dirty = false;
        }
        return valueToReturn;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        anim = new GeneralPurposeAnimEasy();
    }

    // Update is called once per frame
    void Update()
    {
        anim.Update();
        
        if (anim.GetCurrentPosition() != currentColor)  // value 0
        {
            currentColor = anim.GetCurrentPosition();
            
            dirty = true;
        }

        if (anim.alpha != targetHighlightBrightness)
        {
            targetHighlightBrightness = anim.alpha;
            
            //dirty = true;
        }

        if (targetHighlightBrightness != highlightBrightness)
        {
            float delta = targetHighlightBrightness - highlightBrightness;
            if (Math.Abs(delta) < 0.5)
            {
                highlightBrightness = targetHighlightBrightness;
            }
            else
            {
                highlightBrightness += (targetHighlightBrightness - highlightBrightness)/8f;
            }
            dirty = true;
        }

        /*
        if (dirty)
        {
            updateFeedback();
        }*/

        if (waitAndThenTurnOffHighlight)
        {
            waitAndTurnOffHighlightTimer += 1;
            if (waitAndTurnOffHighlightTimer >= waitAndTurnOffHighlightTimerLimit)
            {
                waitAndThenTurnOffHighlight = false;
                removeGlobeAmbientSparkle();
                //animateHighlightValueTo(0, 50);
            }
        }
    }

    /*
    public float getHighestBrightness()
    {
        float valueToReturn = ambientBrightness;
        if (highlightBrightness > ambientBrightness)
            valueToReturn = highlightBrightness;
        
        return valueToReturn;
    } 
    
    public void init(int whichID, string whichTitle)
    {
        myID = whichID;
        
        //title.text = whichTitle;
        
    }*/

    public void animatColorValueTo(Vector3 whichNewValue, int whichRampTime)
    {
        if (whichNewValue.x != currentColor.x)
            anim.SetNewPosn(0, whichNewValue.x, whichRampTime);
        if (whichNewValue.y != currentColor.y)
            anim.SetNewPosn(1, whichNewValue.y, whichRampTime);
        if (whichNewValue.z != currentColor.z)
            anim.SetNewPosn(2, whichNewValue.z, whichRampTime);
    }
    
    public void animatColorValueTo(Vector3 whichNewValue, int whichRampTime, int whichDelayTime)
    {
        if (whichNewValue.x != currentColor.x)
            anim.SetNewPosn(0, whichNewValue.x, whichRampTime, whichDelayTime);
        if (whichNewValue.y != currentColor.y)
            anim.SetNewPosn(1, whichNewValue.y, whichRampTime, whichDelayTime);
        if (whichNewValue.z != currentColor.z)
            anim.SetNewPosn(2, whichNewValue.z, whichRampTime, whichDelayTime);
    }
    
    public void animateHighlightValueTo(float whichNewValue, int whichRampTime)
    {
        anim.SetNewAlpha( whichNewValue,whichRampTime);
    }

    public void startAmbientGlobeSparkle(int whichRampTime, int whichHoldTime)
    {
        waitAndThenTurnOffHighlight = true;
        waitAndTurnOffHighlightTimerLimit = whichHoldTime;
        waitAndTurnOffHighlightTimer = 0;
        animatColorValueTo(new Vector3(1, 1, 1), whichRampTime);
    }
    
    public void startAmbientGlobeSparkle(int whichRampTime, int whichHoldTime, int whichDelayTime)
    {
        waitAndThenTurnOffHighlight = true;
        waitAndTurnOffHighlightTimerLimit = whichHoldTime;
        waitAndTurnOffHighlightTimer = 0;
        animatColorValueTo(new Vector3(1, 1, 1), whichRampTime, whichDelayTime);
    }


    private void removeGlobeAmbientSparkle()
    {
        Vector3 turnOff = new Vector3(0, 0, 0);
        animatColorValueTo(turnOff, 45);
    }

    public void animateHighlightValueAndHoldFor(float whichNewValue, int whichRampTime, int whichHoldTime)
    {
        waitAndThenTurnOffHighlight = true;
        waitAndTurnOffHighlightTimerLimit = whichHoldTime;
        waitAndTurnOffHighlightTimer = 0;
        anim.SetNewAlpha(whichNewValue,whichRampTime);
    }

    public void updateFeedback()
    {
        /*
        float combinedBrightness = 0f;
        if (ambientBrightness > highlightBrightness)
            combinedBrightness = ambientBrightness * 0.75f + highlightBrightness * 0.25f;
        else 
            combinedBrightness = ambientBrightness * 0.25f + highlightBrightness * 0.75f;
        */
        
        //int valueR_i = (int)Math.Floor(currentColor.x); // currentColor is between
        //int valueG_i = (int)Math.Floor(currentColor.y);
        //int valueB_i = (int)Math.Floor(currentColor.z);
        int valueH_i = (int)Math.Floor((highlightBrightness/100f) * 255f); // used for feedback only
        //valueFeedback.text = "A:"+valueA_i;  // feedback is between 0-255
        valueFeedback.text = "H:"+valueH_i;  // feedback is between 0-255
        
        //ChangeColorAndAlpha(quad.GetComponent<Renderer>().material, new Vector3(1f, 0f, 0f), ambientBrightness);

        //quadAmbient.GetComponent<Renderer>().material.color = new Color(currentColor.x/255f, currentColor.y/255f, currentColor.z/255f);
        if (lighSampleObj != null)
            lighSampleObj.GetComponent<Renderer>().material.color = new Color(currentColor.x, currentColor.y, currentColor.z); // these values are between 0-1
        else
        {
            // catch error
            valueFeedback.text = "sos!";
        }
        //quadHighlight.GetComponent<Renderer>().material.color = new Color(highlightBrightness/100f, highlightBrightness/100f, highlightBrightness/100f);
    }
    
    void ChangeColorAndAlpha(Material whichMat, Vector3 newRGB, float newAlpha)
    {
        //Color oldColor = whichMat.color;
        Color newColor = new Color(newRGB.x, newRGB.y, newRGB.z, newAlpha);
        whichMat.SetColor("_Color", newColor);
    }
    
}
