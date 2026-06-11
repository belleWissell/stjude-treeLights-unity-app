using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace AAMVC.CameraViewControl
{
    public class SceneViewCameraControl
    {

        private bool captureMode = false;

        public int stageWidth = 1024;
        public int stageHeight = 600;

        double mousePosX, mousePosY;
        double targetPosX, targetPosY, targetPosZ;
        double deltaPosX, deltaPosY, deltaPosZ;
        double currentPosX, currentPosY, currentPosZ;

        double targetLookPosX, targetLookPosY, targetLookPosZ;
        double deltaLookPosX, deltaLookPosY, deltaLookPosZ;
        double currentLookPosX, currentLookPosY, currentLookPosZ;

        //public double[] camPresetPosn = new double[3]; // camera and target preset
        //public double[] camTargetPresetPosn = new double[3]; // camera and target preset

        private bool cameraOrbiting = false;
        private System.Timers.Timer orbitTickTimer = new System.Timers.Timer();
        private System.Timers.Timer startOrbitTimer = new System.Timers.Timer();

        private bool startOrbitTimerEnabled = true;

        private PlotCameraTargetPosition plotCameraTargetVar;


        // store several "orbits" for the camera:
        private static int numberOfOrbitsToStore = 5;
        private double[] cameraTargetX = new double[numberOfOrbitsToStore];
        private double[] cameraTargetY = new double[numberOfOrbitsToStore];
        private double[] cameraTargetZ = new double[numberOfOrbitsToStore];
        private double[] cameraTargetRange = new double[numberOfOrbitsToStore];
        
        private double currentCameraTargetX = 0;
        private double currentCameraTargetY = 0;
        private double currentCameraTargetZ = 0;
        private double currentCameraRange = nominalCameraRange;

        private static double nominalCameraRange = 1000;
        private double maxCameraRange = nominalCameraRange * 5;
        private double minCameraRange = nominalCameraRange * 0.5;

        //private vipTouch.openGL.OpenGLCameraPath openGlCameraPathVar;
        private int currentCameraPosFrame = 0;
        private int numberOfCameraFrames = 0;

        private double easeValue = 10;
        //private double easeValue = 30; // special case for camera capture

        // set up for dragging motion to adjust view
        private double mouseStartPosX, mouseStartPosY;
        //private double mouseDeltaX, mouseDeltaY;
        private double mouseEndPosX, mouseEndPosY;

        private bool isDragging = false;
        
        public SceneViewCameraControl(int whichW, int whichH)
        {
            stageWidth = whichW;
            stageHeight = whichH;
        }

        public void init()
        {
            //***************************************
            // timer used to wait before orbiting camera automatically
            startOrbitTimer.Elapsed += new System.Timers.ElapsedEventHandler(startOrbitTimerExpired);
            startOrbitTimer.Interval = 5000;
            startOrbitTimer.AutoReset = false;
            if (startOrbitTimerEnabled)
                startOrbitTimer.Start();
            //***************************************


            plotCameraTargetVar = new PlotCameraTargetPosition();

            assignCameraOrbits();

            setCameraOrbit(0);

            isDragging = false;
            plotCameraTargetVar.udpateCameraPos(0.25, 0.25);
        }

        public void setCameraOrbit(int whichOrbitID)
        {
            if (whichOrbitID < cameraTargetX.Length) // insure that it is a valid range
            {
                currentCameraTargetX = cameraTargetX[whichOrbitID];
                currentCameraTargetY = cameraTargetY[whichOrbitID];
                currentCameraTargetZ = cameraTargetZ[whichOrbitID];
                currentCameraRange = cameraTargetRange[whichOrbitID];

                plotCameraTargetVar.lookAtX = currentCameraTargetX;
                plotCameraTargetVar.lookAtY = currentCameraTargetY;
                plotCameraTargetVar.lookAtZ = currentCameraTargetZ;
                plotCameraTargetVar.range = currentCameraRange;

                startOrbitTimerEnabled = true;
            }
        }

        /*
        public void updateAzimuthOffset(double newAziOffset)
        {
            plotCameraTargetVar.aziOffset = newAziOffset;
        }

        public void fadeUpModel()
        {
            if (!captureMode)
            {
                currentLookPosZ = 100000f;
            }
        }
        */
        
        /*
        public void toggleCapturePosition(bool useAlternatePosition)
        {
            if (useAlternatePosition)
            {
                camPresetPosn[0] = 5000;
                camPresetPosn[1] = -5980;
                camPresetPosn[2] = 1735;

                camTargetPresetPosn[0] = 1361;
                camTargetPresetPosn[1] = 450;
                camTargetPresetPosn[2] = 15;
            }
            else // use the default camera capture position
            {
                camPresetPosn[0] = 3428;
                camPresetPosn[1] = 5853;
                camPresetPosn[2] = 1569;

                camTargetPresetPosn[0] = 1793;
                camTargetPresetPosn[1] = 1240;
                camTargetPresetPosn[2] = -125;
            }
        }
        */
        
        public void setWindowSize(int whichW, int whichH)
        {

            mouseEndPosX = (mouseEndPosX/(double)stageWidth) * (double)whichW; // shift last mouse pos to new screen coords
	        mouseEndPosY = (mouseEndPosY/(double)stageHeight) * (double)whichH; // shift last mouse pos to new screen coords
            stageWidth = whichW;
            stageHeight = whichH;

        }

        //*********************************************************************************
        // MOUSE FUNCTIONS
        //*********************************************************************************
        public void mouseUp()
        {
            isDragging = false;

            mouseEndPosX = mousePosX;
            mouseEndPosY = mousePosY;
            resetCameraOrbitTimer();
        }

        public void mouseDn(float newMousePosX, float newMousePosY)
        {
            mouseStartPosX = (double)newMousePosX;
            mouseStartPosY = (double)newMousePosY;

            isDragging = true;


            if (cameraOrbiting)
            {
                haltCameraOrbit();
            }
            resetCameraOrbitTimer();
        }

        public void mouseMove(float newMousePosX, float newMousePosY)
        {
            if (isDragging)
            {
                double mouseDeltaPosX = (double)newMousePosX - mouseStartPosX;
                double mouseDeltaPosY = (double)newMousePosY - mouseStartPosY;

                mousePosX = mouseEndPosX + mouseDeltaPosX;
                mousePosY = mouseEndPosY + mouseDeltaPosY;

                //if (cameraOrbiting)
                //{
                haltCameraOrbit();
                //}
                resetCameraOrbitTimer();
            }
            updateCameraTarget();
        }

        public void mouseWheel(float mouseWheelDir)
        {
            // TODO make this value scale with the size of the scene 
            double scaleOfZoom = nominalCameraRange * 0.05f;
            currentCameraRange += (double)mouseWheelDir * scaleOfZoom;
            double newCameraRange = currentCameraRange + ((double)mouseWheelDir * scaleOfZoom);
            if (newCameraRange > maxCameraRange)
            {
                currentCameraRange = maxCameraRange;
            }
            else if (newCameraRange < minCameraRange)
                currentCameraRange = minCameraRange;
            plotCameraTargetVar.range = currentCameraRange;

            haltCameraOrbit();
            resetCameraOrbitTimer();
            updateCameraTarget();
        }
        

        //*********************************************************************************
        // CAMERA ORBIT FUNCTIONS
        //*********************************************************************************
        public void toggleCameraOrbit()
        {
            plotCameraTargetVar.toggleOrbitingCamera();
        }

        public void startStopCameraOrbit(bool startOrbit)
        {
            plotCameraTargetVar.startStopOrbitingCamera(startOrbit);
            if (!startOrbit)
            {
                startOrbitTimerEnabled = false;
            }
            else
            {
                startOrbitTimerEnabled = true;
            }
        }

        /*
        public void easeToCapturePosition()
        {
            // stop orbting
            plotCameraTargetVar.startStopOrbitingCamera(false);
            //startOrbitTimerEnabled = false;

            // set new target positions:
            plotCameraTargetVar.xPos = camPresetPosn[0];
            plotCameraTargetVar.yPos = camPresetPosn[1];
            plotCameraTargetVar.zPos = camPresetPosn[2];

            plotCameraTargetVar.lookAtX = camTargetPresetPosn[0];
            plotCameraTargetVar.lookAtY = camTargetPresetPosn[1];
            plotCameraTargetVar.lookAtZ = camTargetPresetPosn[2];

            // speed up the transition by decreasing the easing:
            easeValue = 3;
            //easeValue = 6;
        }
        */
        
        private void haltCameraOrbit()
        {
            //System.Diagnostics.Debug.WriteLine("[CAMERACONTROL] haltCameraOrbit ");
            plotCameraTargetVar.startStopOrbitingCamera(false);
        }

        private void startCameraOrbit()
        {
            //System.Diagnostics.Debug.WriteLine("[CAMERACONTROL] startCameraOrbit ");
            plotCameraTargetVar.startStopOrbitingCamera(true);
        }

        private void resetCameraOrbitTimer()
        {
            startOrbitTimer.Stop();
            startOrbitTimer.Start();
        }

        private void startOrbitTimerExpired(object sender, EventArgs e) // timer expired, begin orbiting again
        {
            if (startOrbitTimerEnabled)
            {
                startCameraOrbit();
            }
        }

        //*********************************************************************************


        private void updateCameraTarget()
        {
            double targetCameraAzimuthNormalized = (mousePosX / (double)stageWidth);
            double targetCameraElevationNormalized = (mousePosY / (double)stageHeight);

            plotCameraTargetVar.udpateCameraPos(targetCameraAzimuthNormalized, targetCameraElevationNormalized);

        }

        public int plotCameraPos()
        {
            int frameToReturn = 0;

            targetPosX = plotCameraTargetVar.xPos;
            targetPosY = plotCameraTargetVar.yPos;
            targetPosZ = plotCameraTargetVar.zPos;

            deltaPosX = targetPosX - currentPosX;
            deltaPosY = targetPosY - currentPosY;
            deltaPosZ = targetPosZ - currentPosZ;

            currentPosX += deltaPosX / easeValue;
            currentPosY += deltaPosY / easeValue;
            currentPosZ += deltaPosZ / easeValue;

            targetLookPosX = plotCameraTargetVar.lookAtX;
            targetLookPosY = plotCameraTargetVar.lookAtY;
            targetLookPosZ = plotCameraTargetVar.lookAtZ;

            deltaLookPosX = targetLookPosX - currentLookPosX;
            deltaLookPosY = targetLookPosY - currentLookPosY;
            deltaLookPosZ = targetLookPosZ - currentLookPosZ;

            currentLookPosX += deltaLookPosX / easeValue;
            currentLookPosY += deltaLookPosY / easeValue;
            currentLookPosZ += deltaLookPosZ / easeValue;
            return frameToReturn;
        }


        public UnityEngine.Vector3 getCameraPosition()
        {
            //return new UnityEngine.Vector3((float)currentPosX, (float)currentPosY, (float)currentPosZ);
            return new UnityEngine.Vector3((float)currentPosX,  (float)currentPosY, (float)currentPosZ);
        }

        
        public UnityEngine.Vector3 getCameraTarget()
        {
            //return new UnityEngine.Vector3((float)currentLookPosX, (float)currentLookPosY, (float)currentLookPosZ);
            return new UnityEngine.Vector3((float)currentLookPosX,(float)currentLookPosY,  (float)currentLookPosZ);
        }


        public float getCamPosX()
        {
            return (float)(currentPosX);
        }
        public float getCamPosY()
        {
            return (float)(currentPosY);
        }
        public float getCamPosZ()
        {
            return (float)(currentPosZ);
        }

        public float getCamTargetX()
        {
            return (float)(currentLookPosX);
        }
        public float getCamTargetY()
        {
            return (float)(currentLookPosY);
        }
        public float getCamTargetZ()
        {
            return (float)(currentLookPosZ);
        }

        public string getCameraAzimuth()
        {
            double value;
            string valueToReturn = "0";
            value = 1 - ((plotCameraTargetVar.targetAzimuth) / 360);
            valueToReturn = Convert.ToString(Math.Round(value, 3)); // only send 3 digits of precision
            return valueToReturn;
        }

        public string getCameraRange()
        {
            double value;
            string valueToReturn = "0";
            value = plotCameraTargetVar.range;
            valueToReturn = Convert.ToString(Math.Round(value, 3)); // only send 3 digits of precision
            return valueToReturn;
            
        }

        public string getCameraElevation()
        {
            double value;
            string valueToReturn = "0";
            value = 1 - ((plotCameraTargetVar.targetElevation) / 360);
            valueToReturn = Convert.ToString(Math.Round(value, 3)); // only send 3 digits of precision
            return valueToReturn;
        }

        //*********************************************************************************

        public Vector3 getCameraOrbitZero()
        {
            Vector3 valueToReturn = new Vector3();
            valueToReturn.x = (float)cameraTargetX[0];
            valueToReturn.y = (float)cameraTargetY[0];
            valueToReturn.z = 0f - (float)cameraTargetRange[0];

            return valueToReturn;
        }
        
        private void assignCameraOrbits()
        {
            cameraTargetX[0] = 0;
            cameraTargetY[0] = 0;
            cameraTargetZ[0] = 0;
            cameraTargetRange[0] = 1000; // make sure to adjust min/max range variables as well...

            // add other preset camera positions as needed:
        }
    }
}