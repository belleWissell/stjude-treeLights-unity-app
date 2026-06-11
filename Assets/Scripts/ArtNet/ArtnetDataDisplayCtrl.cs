using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArtnetForUnity;
using UnityEditor;
using TMPro;

public class ArtnetDataDisplayCtrl : MonoBehaviour
{
    
    ArtnetSettings settings;

    public TextMeshPro feedbackText;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initDataDisplay()
    {
        settings = ArtnetForUnity.ArtUtils.LoadSettings();
        List<ArtnetForUnity.ArtnetOutputs> outputs = settings.artnetOutputs;

        int whichPort = ArtnetForUnity.ArtUtils.ArtnetPort;

        string whichLocalIP = settings.IPAddress;
        
        int numberOfOutputs = outputs.Count;
        int whichUniverse = -1;
        int whichDMXUnverse = -1;
        string whichRevc = "0.0.0.0";
        
        if (numberOfOutputs > 0)
        {
            whichUniverse = outputs[0].Universe;
            whichDMXUnverse = outputs[0].DMXUniverse;

            List<string> nodeRevcIPAddress = outputs[0].NodeRevcIPAddress;
            int numberOfIps = nodeRevcIPAddress.Count;
            if (numberOfIps > 0)
            {
                whichRevc = nodeRevcIPAddress[0];
            }
        }

        string dataReport = "Current Settings: \n";
        
        dataReport += "Local IP: " + whichLocalIP + "\n";
        dataReport += "Universe: " + whichUniverse + "\n";
        dataReport += "DMX Universe: " + whichDMXUnverse + "\n";
        dataReport += "Receiver IP: " + whichRevc + "\n";
        dataReport += "Receiver Port: " + whichPort + "\n";
        
        
        feedbackText.text = dataReport;
    }
}
