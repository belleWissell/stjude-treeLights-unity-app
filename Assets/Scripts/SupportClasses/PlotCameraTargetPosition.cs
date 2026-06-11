using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine; // used only for debug

namespace AAMVC.CameraViewControl
{
    public class PlotCameraTargetPosition
    {
        // ******************
        // assigned from outside class:
        // ******************
        public double targetAzimuth = 0;
        public double targetElevation = 0;
        public double range = 4500;
        public double lookAtX = 0;
        public double lookAtY = 0;
        public double lookAtZ = 0;

        
        // ******************
        private double maxElevationAngle = 60;
        private double minElevationAngle = -5;

        // position of camera is shared with mousevelcalculator
        public double xPos, yPos, zPos = 0;

        // used to align default to world/model
        private double aziOffset = 0;

        private System.Timers.Timer animationFrameTimer = new System.Timers.Timer();
        private bool orbitingCamera = false;
        private double orbitElevation = 15;

        // speed of the orbit
        private double orbitFrames = 820;
        
        private static double convertToRad = Math.PI / 180;
        
        public PlotCameraTargetPosition()
        {
            animationFrameTimer.Interval = 33;
            animationFrameTimer.Elapsed += new System.Timers.ElapsedEventHandler(animateCameraStep);
        }

        public void udpateCameraPos(double whichAzi, double whichElev)
        {
            targetAzimuth = 0 - (whichAzi * 360); // reverse direction of drag
            //targetElevation = minElevationAngle - (whichElev * (maxElevationAngle - minElevationAngle));
            targetElevation = minElevationAngle - (whichElev * (maxElevationAngle - minElevationAngle)); // reverse direction of drag

            if (targetElevation < minElevationAngle)
                targetElevation = minElevationAngle;
            else if (targetElevation > maxElevationAngle)
                targetElevation = maxElevationAngle;
            
            plotCameraObject();
            
            //Debug.Log("[PlotCam] [ "+ targetAzimuth +", "+ targetElevation+" ]");
        }

        public void toggleOrbitingCamera()
        {
            if (orbitingCamera)
            {
                orbitingCamera = false;
                animationFrameTimer.Stop();
            }
            else
            {
                orbitingCamera = true;
                animationFrameTimer.Start();
                targetElevation = orbitElevation;
            }
        }

        public void startStopOrbitingCamera(bool startingOrbit)
        {
            if (startingOrbit)
            {
                if (!orbitingCamera)
                {
                    orbitingCamera = true;
                    animationFrameTimer.Start();
                    targetElevation = orbitElevation;
                }
            }
            else
            {
                if (orbitingCamera)
                {
                    orbitingCamera = false;
                    animationFrameTimer.Stop();
                }
            }
        }

        private void animateCameraStep(object sender, EventArgs e)
        {
            targetAzimuth += 360 / orbitFrames;
            if (targetAzimuth > 360)
            {
                targetAzimuth -= 360;
            }
            else if (targetAzimuth < 0)
            {
                targetAzimuth += 360;
            }
            plotCameraObject();
        }
        

        public void plotCameraObject()
        {
            // double trans = range * Math.Cos(targetElevation * Math.PI / 180);
            double trans = range * Math.Cos(targetElevation * convertToRad);

            /*
            //yPos = range * Math.Sin(targetElevation * Math.PI / 180);
            yPos = range * Math.Sin(targetElevation * Math.PI / 180);
            yPos += lookAtY;

            xPos = trans * Math.Cos((targetAzimuth + aziOffset) * Math.PI / 180);
            xPos += lookAtX;
            //zPos = trans * Math.Sin((targetAzimuth + aziOffset) * Math.PI / 180);
            zPos = trans * Math.Sin((targetAzimuth + aziOffset) * Math.PI / 180);
            zPos += lookAtZ;
            */
            
            yPos = range * Math.Sin(targetElevation * convertToRad);
            xPos = trans * Math.Cos((targetAzimuth + aziOffset) * convertToRad);
            zPos = trans * Math.Sin((targetAzimuth + aziOffset) * convertToRad);


            // camera orbit position is relative to the look at point:
            xPos += lookAtX;
            yPos += lookAtY;
            zPos += lookAtZ;
        }
    }
}