using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AAMVC.Unity
{
    //public class LogTextCtrl : MonoBehaviour
    public class LogTextCtrl : Singleton<LogTextCtrl>
    {
        public GameObject textFieldContainer;
        public GameObject textFieldBackground;
        public int targetWidth = -1;
        public int targetHeight = -1;
        
        private TMP_Text textField;
        
        // *********************************************
        private ApplicationControl appControl;

        private static int maxNumberOfLinesOfText = 24;
        private string[] lineOfContent = new string[maxNumberOfLinesOfText];
        private int currentLineOfText = 0;

        private float currentScreenW;
        private float currentScreenH;
        
        // *********************************************
        //private RecordEventsToLocalFile recordToLocalFile;
        private bool isRecordingToFile = false;

        void Awake()
        {
            appControl = ApplicationControl.Instance;
            
            textField = textFieldContainer.GetComponent<TMP_Text>();

            setElementSizes();
            
            currentLineOfText = -1;
            logText("> LogText Initialized", true);

        }

        // Start is called before the first frame update
        void Start()
        {
            //currentLineOfText = -1;
            //logText("> Application launch");
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        /*
        public void prepForRecordToFile(string whichFilePath, bool doActivateRecordToFile)
        {
            isRecordingToFile = doActivateRecordToFile;
            
            recordToLocalFile = new RecordEventsToLocalFile(doActivateRecordToFile);
            
            recordToLocalFile.setModeAndVariables(doActivateRecordToFile, whichFilePath);
        }*/
        
        public void logText(string whichText, bool whichDebugMode)
        {
            currentLineOfText += 1;
            if (currentLineOfText >= maxNumberOfLinesOfText)
            {
                shiftTextUp();
                currentLineOfText = maxNumberOfLinesOfText - 1;
            }
            lineOfContent[currentLineOfText] = "> " + whichText;

            //if (isRecordingToFile)
            //    recordToLocalFile.recordGenericEvent(whichText);
            
            if (whichDebugMode)
            {
                Debug.Log("LOGTEXT | "+whichText);
            }

            updateTextField();
        }

        public void logTextOnTopOfText(string whichText, bool whichDebugMode) // used for countdowns, etc
        {
            // keep current line of text the same... 
            lineOfContent[currentLineOfText] = "> " + whichText;
            updateTextField();
        }

        public void toggleLogVisibility()
        {
            
        }

        private float zPosnOfText = 1.0f;
        
        public void setWindowSize(int whichW, int whichH) // this comes through as pixels
        {
            currentScreenW = whichW/100f; // 3d world is different scale (not pixels)
            currentScreenH = whichH/100f; 
            
            //Vector3 upperRight = new Vector3(5000f+whichW/2f - targetWidth/2f - 5f, whichH/2f- targetHeight/2f - 5f, 500f);
            Vector3 upperRight = new Vector3(currentScreenW/2f - targetWidth/2f - 0.05f, currentScreenH/2f- targetHeight/2f - 0.05f, zPosnOfText);
            Vector3 lowerRight = new Vector3(currentScreenW/2f - targetWidth/2f - 0.05f, targetHeight/2f - currentScreenH/2f + 0.05f, zPosnOfText);
            this.transform.localPosition = lowerRight;
            //this.transform.position = upperRight;
            
        }

        private void setElementSizes()
        {
            // auto size the text field
            
            //Vector3 whichScale = new Vector3(350, 1000, 10);
            //Vector3 whichScale = new Vector3(350, targetHeight, 10);
            //Vector3 whichBgScale = new Vector3(30, 1, 100);
            Vector3 whichScale = new Vector3(0.350f, targetHeight, 0.010f);
            Vector3 whichBgScale = new Vector3(0.030f, 1f, 0.100f);
            
            if (targetHeight != -1 && targetWidth != -1)
            {
                whichScale.x = targetWidth;
                whichScale.y = targetHeight;
                whichScale.z = 1;

                whichBgScale.x = targetWidth / 10f;
                whichBgScale.y = 1;
                whichBgScale.z = targetHeight / 10f;
            }
            
            //textField.GetComponent<RectTransform>().sizeDelta = new Vector2(whichScale.x-5f, whichScale.y-5f);
            textField.GetComponent<RectTransform>().sizeDelta = new Vector2(whichScale.x - 0.05f, whichScale.y - 0.05f);
            textFieldBackground.transform.localScale = whichBgScale;
        }

        private void setElementPositions()
        {
            textField.ForceMeshUpdate(); // forces to most current size of field
            
            Vector3 positionOfText = new Vector3();
            Vector3 currentSizeOfText = textField.textBounds.size;
            
            // this assumes it is hugging right side of screen
            //positionOfText.x = 5000f + (currentScreenW / 2f) - (targetWidth / 2f) - 5f;
            positionOfText.x = this.transform.position.x;
            
            // move so it hugs bottom of target area:
            //positionOfText.y = (currentScreenH / 2f) - targetHeight + (currentSizeOfText.y/2f) +  5f ;
            //positionOfText.y = this.transform.position.y - targetHeight + currentSizeOfText.y + 30f;
            // upper right: 
            positionOfText.y = this.transform.position.y - targetHeight + currentSizeOfText.y + 0.30f;
            // lower right ?: 
            //positionOfText.y = targetHeight - this.transform.position.y + currentSizeOfText.y - 0.30f;

            //positionOfText.z = 500f;
            //positionOfText.z = 5f;
            positionOfText.z = zPosnOfText;

            //textFieldContainer.transform.localPosition = positionOfText;
            textFieldContainer.transform.position = positionOfText;
        }
        
        private void updateTextField()
        {
            textField.text = "";
            
            for (int i = 0; i <= currentLineOfText; ++i)
            {
                textField.text += lineOfContent[i] + "\n";
            }

            setElementPositions();
        }

        private void shiftTextUp()
        {
            string[] oldArray = new string[maxNumberOfLinesOfText];
            System.Array.Copy(lineOfContent, oldArray, maxNumberOfLinesOfText);
            
            //var newArray = new int?[oldArray.Length];
            System.Array.Copy(oldArray, 1, lineOfContent, 0, oldArray.Length - 1);
            //Array.Copy(oldArray, 1, lineOfContent, 0, oldArray.Length - 1);
        }

        public void onProgramExit()
        {
            //if (isRecordingToFile)
            //    recordToLocalFile.onProgramExit();
        }
    }
}
