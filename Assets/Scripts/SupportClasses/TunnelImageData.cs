using UnityEngine;
using System.IO;


namespace AAMVC.Unity
{
    public class TunnelImageData
    {

        private int myID;
        private string date;
        private string title;
        private string imageFileName;
        private string imageCaption;
        private string imageCredit;

        private Texture2D imageTexture = null;

        public TunnelImageData(int whichID)
        {
            myID = whichID;
        }

        public string getImageFileName()
        {
            return imageFileName;
        }

        /*
        public void assignTexture(Texture2D whichTexture)
        {
            
            imageTexture = new Texture2D();
            imageTexture = whichTexture;
        }
        */
        
        public void assignData(string whichDate, string whichTitle, string whichFileName, string whichCaption, string whichCredit)
        {
            date = whichDate;
            title = whichTitle;
            imageFileName = whichFileName;
            imageCaption = whichCaption;
            imageCredit = whichCredit;
            
        }

        public void loadImage()
        {
            loadImageFileToTexture(imageFileName);
        }
        
        private void loadImageFileToTexture(string filePath) {
   
            //appControlObj.logText("[imgctrl] loading file " + filePath);
            System.Diagnostics.Debug.WriteLine("[wybk] ID "+myID+" loading file " + filePath);
            //Texture2D tex = null;
            byte[] fileData;
 
            if (File.Exists(filePath))     {
                fileData = File.ReadAllBytes(filePath);
                imageTexture = new Texture2D(2, 2);
                imageTexture.LoadImage(fileData); //..this will auto-resize the texture dimensions.
            }
            else
            {
                //appControlObj.logText("[imgctrl] unable to locate file " + filePath);
                System.Diagnostics.Debug.WriteLine("[wybk] unable to locate file " + filePath);

            }
            //return tex;
        }
        
    }
}