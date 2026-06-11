using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using AAMVC.Unity;
using AnimationEngine;
using TMPro;

public class feedbackMessagesCtrl : MonoBehaviour
{

    public TextMeshPro labelRac;
    public TextMeshPro labelSku;
    public TextMeshPro labelSqu;
    public TextMeshPro labelCat;
    public TextMeshPro labelOtt;
    public TextMeshPro labelDee;
    
    public GameObject labelRacc;
    public GameObject labelSkun;
    public GameObject labelSqur;
    public GameObject labelCatb;
    public GameObject labelOtte;
    public GameObject labelDeer;
    
    private ApplicationControl appControlObj;

    private static int maxNumberOfLabels = 8;
    private int actualNumberOfLabels = 6;
    private GeneralPurposeAnimLinear[] labelAnim = new GeneralPurposeAnimLinear[maxNumberOfLabels];
    private float[] currentLabelAlpha = new float[maxNumberOfLabels];

    private bool initCompleted = false;
    // Start is called before the first frame update
    void Start()
    {
        appControlObj = ApplicationControl.Instance;
        
        /*while (appControlObj == null) // wait for it to exist first avoid Null Reference
        {
            appControlObj = FindObjectOfType<ApplicationControl>();
            yield return new WaitForEndOfFrame();
        }*/

        for (int i = 0; i < actualNumberOfLabels; ++i)
        {
            labelAnim[i] = new GeneralPurposeAnimLinear();
        }

        initializeAnimationStartState();

        initCompleted = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (initCompleted)
        {
            for (int i = 0; i < actualNumberOfLabels; ++i)
            {
                labelAnim[i].Update();

                if (labelAnim[i].alpha != currentLabelAlpha[i])
                {
                    currentLabelAlpha[i] = labelAnim[i].alpha;
                    Color newLableColor = new Color(1, 1, 1, currentLabelAlpha[i]);
                    switch (i)
                    {
                        case 0:
                            labelRac.color = newLableColor;
                            labelRacc.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                        case 1:
                            labelSku.color = newLableColor;
                            labelSkun.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                        case 2:
                            labelSqu.color = newLableColor;
                            labelSqur.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                        case 3:
                            labelCat.color = newLableColor;
                            labelCatb.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                        case 4:
                            labelOtt.color = newLableColor;
                            labelOtte.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                        case 5:
                            labelDee.color = newLableColor;
                            labelDeer.GetComponent<Renderer>().material.color = newLableColor;
                            break;
                    }
                }
            }
        }
    }

    public void animateInLabel(int whichLabel)
    {
        labelAnim[whichLabel].SetNewAlpha(1, 65, 125);
    }
    public void animateOutLabel(int whichLabel)
    {
        labelAnim[whichLabel].SetNewAlpha(0, 20);
    }

    private void initializeAnimationStartState()
    {
        float from = 10f;
        float to = 20.0f;

        /*
        for (int i = 0; i < actualNumberOfLabels; ++i)
        {
            labelAnim[i].alpha = from;
            labelAnim[i].targetAlpha = to;

            currentLabelAlpha[i] = to;
            labelAnim[i].SetNewAlpha(0, 5);
        }*/

        for (int i = 0; i < actualNumberOfLabels; ++i)
        {
            currentLabelAlpha[i] = to;
            labelAnim[i].initAnim();
        }
    }
}
