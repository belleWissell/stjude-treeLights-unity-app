using UnityEngine;
using AnimationEngine;
using System;
using System.Diagnostics;

namespace SupportClasses
{
    public class ImageAnimator
    {
        private bool doAnimateImage = false;
        private bool goDark = true;
        
        public float currentAlpha;
        public Vector3 currentPosn;
        public Vector3 currentScale;
        public float currentValue;
        private GeneralPurposeAnimLinearNoBuffer anim;
        private float screenW = 1920.0f;
        private float screenH = 1080.0f;

        private float targetMinScale = 100f; // longest side fits into window
        private float targetMaxScale = 120f; // shortest side fist into window
        private string imagePosn = "C"; // C = center, LL = start in lower left, UR = start in UR
        public bool drawOnTop = false;
        private int keepImageOnScreenFrames = 250;
        private int fadeRateFrames = 60;

        private bool isTallAspect = false;

        private float hugBottomMinY = 0f;
        private float hugBottomMaxY = 0f;
        private float hugTopMinY = 0f;
        private float hugTopMaxY = 0f;

        private float hugLeftMinX = 0f;
        private float hugLeftMaxX = 0f;
        private float hugRightMinX = 0f;
        private float hugRightMaxX = 0f;

        private float localImageRate = 5.0f; // how many seconds to leave image on frame

        private float targetDetailScale = 1.0f;
        private Vector3 targetDetailPosn;

        public bool isAnimating = false;
        
        public ImageAnimator(int whichScreenW, int whichScreenH, float whichImageRate, bool whichAnimateSetting)
        {
            doAnimateImage = whichAnimateSetting;
            screenW = (float)whichScreenW;
            screenH = (float)whichScreenH;
            localImageRate = whichImageRate;

            anim = new GeneralPurposeAnimLinearNoBuffer();
            keepImageOnScreenFrames = Convert.ToInt32(Math.Floor(  whichImageRate * 60f)); // this assumes a locked frame rate of 60
            fadeRateFrames = Convert.ToInt32(Math.Floor(0.75f * 60f)); // this assumes a locked frame rate of 60
        }

        public void calculateImageDetailViewMode(int whichMaxW, int whichMaxH, float whichImageW, float whichImageH)
        {
            float minWidthScale = whichMaxW / whichImageW;
            float minHeightScale = whichMaxH / whichImageH;
            
            // take the smaller of the two
            if (minWidthScale > minHeightScale)
            {
                targetDetailScale = minHeightScale;
            }
            else
            {
                targetDetailScale = minWidthScale;
            }


            float imageHtWhenScaled = whichImageH * targetDetailScale;

            float yPosn = 1920 - 866 - imageHtWhenScaled / 2;
            
            targetDetailPosn = new Vector3(0, yPosn, 0); 
        }
        
        
        public void calculateImageScale(float whichImageW, float whichImageH)
        {
            float minWidthScale = screenW / whichImageW;
            float minHeightScale = screenH / whichImageH;
        
            // take the lesser of the two:
            if (minWidthScale < minHeightScale)
            {
                targetMinScale = minHeightScale;
            }
            else
            {
                targetMinScale = minWidthScale;
            }

            targetMaxScale = targetMinScale * 1.2f;
            
            
            float imageHeightAtMinScale = whichImageH * targetMinScale;
            hugTopMinY = (screenH - imageHeightAtMinScale + (imageHeightAtMinScale*0.03f)) / 2f;
            hugBottomMinY = 0f - hugTopMinY;

            float imageHeightAtMaxScale = whichImageH * targetMaxScale;
            hugTopMaxY = (screenH - imageHeightAtMaxScale + (imageHeightAtMaxScale*0.06f)) / 2f;
            hugBottomMaxY = 0f - hugTopMaxY;

            float imageWidthAtMinScale = whichImageW * targetMinScale;
            hugRightMinX = (screenW - imageWidthAtMinScale + (imageWidthAtMinScale * 0.02f)) / 2f;
            hugLeftMinX = 0f - hugRightMinX;

            float imageWidthAtMaxScale = whichImageW * targetMaxScale;
            hugRightMaxX = (screenW - imageWidthAtMaxScale + (imageWidthAtMaxScale * 0.05f)) / 2f;
            hugLeftMaxX = 0f - hugRightMaxX;

            
            anim.SetNewScale(targetMinScale, 5);
            if (goDark)
                anim.SetNewValue(0f, 5);
            else
                anim.SetNewValue(10f, 5);

        }

        public void stopAllAnimations()
        {
            anim.stopAllAnimations();
            //isAnimating = false;
        }
        public void startAnimWithOption(string whichPosnOption, string whichZoomOption)
        {
            //isAnimating = true;
            bool doZoomIn = false;
            bool doZoomOut = false;
            float animateToScale = targetMinScale;
            
            switch (whichZoomOption)
            {
                case "ZI": 
                    doZoomIn = true;
                    anim.scale = targetMinScale;
                    animateToScale = targetMaxScale;
                    break;
                case "ZO":
                    doZoomOut = true;
                    anim.scale = targetMaxScale;
                    animateToScale = targetMinScale;
                    break;
                case "NA":
                default:
                    anim.scale = targetMinScale;
                    //animateToScale = targetMinScale;
                    animateToScale = (targetMinScale + targetMinScale + targetMaxScale)/3f; // need to always animate something (replace no zoom with subtle zoom in)
                    break;
            }
            
            float animateToXPosn = 0;
            float animateToYPosn = 0;

            switch (whichPosnOption)
            {
                case "UL":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = hugTopMinY;
                        animateToXPosn = hugRightMaxX;
                        animateToYPosn = hugTopMaxY;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugLeftMaxX;
                        anim.posn[1] = hugTopMaxY;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = hugTopMinY;
                    }
                    else
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = hugTopMinY;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = hugTopMinY;
                    }
                    break;
                case "LL":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = hugBottomMinY;
                        animateToXPosn = hugRightMaxX;
                        animateToYPosn = hugBottomMaxY;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugLeftMaxX;
                        anim.posn[1] = hugBottomMaxY;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = hugBottomMinY;
                    }
                    else
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = hugBottomMinY;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = hugBottomMinY;
                    }
                    break;
                case "CL":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugRightMaxX;
                        animateToYPosn = 0;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugLeftMaxX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = 0;
                    }
                    else
                    {
                        anim.posn[0] = hugLeftMinX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugRightMinX;
                        animateToYPosn = 0;
                    }
                    break;
                case "UR":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = hugTopMinY;
                        animateToXPosn = hugLeftMaxX;
                        animateToYPosn = hugTopMaxY;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugRightMaxX;
                        anim.posn[1] = hugTopMaxY;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = hugTopMinY;
                    }
                    else
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = hugTopMinY;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = hugTopMinY;
                    }
                    break;
                case "LR":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = hugBottomMinY;
                        animateToXPosn = hugLeftMaxX;
                        animateToYPosn = hugBottomMaxY;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugRightMaxX;
                        anim.posn[1] = hugBottomMaxY;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = hugBottomMinY;
                    }
                    else
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = hugBottomMinY;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = hugBottomMinY;
                    }
                    break;
                case "CR":
                    if (doZoomIn)
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugLeftMaxX;
                        animateToYPosn = 0;
                    }
                    else if (doZoomOut)
                    {
                        anim.posn[0] = hugRightMaxX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = 0;
                    }
                    else
                    {
                        anim.posn[0] = hugRightMinX;
                        anim.posn[1] = 0;
                        animateToXPosn = hugLeftMinX;
                        animateToYPosn = 0;
                    }
                    break;
                case "NA":
                default:
                    anim.posn[0] = 0;
                    anim.posn[1] = 0;
                    animateToXPosn = 0;
                    animateToYPosn = 0;
                    break;
            }

            int longAnimation = keepImageOnScreenFrames + fadeRateFrames;

            drawOnTop = true;
            //anim.SetNewAlpha(1f, fadeRateFrames);
            anim.SetNewPosn(0, animateToXPosn, longAnimation); // move in front
            anim.SetNewPosn(1, animateToYPosn, longAnimation); // move in front
            //anim.SetNewPosn(2, 50, 2); // move in front
            /*
            if (goDark)
            {
                anim.SetNewValue(1f, 35, 20);
            }
            else
                anim.SetNewValue(1f, 35, 20);*/
            
            anim.SetNewScale(animateToScale, longAnimation);
        }
        
        
        private void startAnim(bool doHugRight)
        {
            //isAnimating = true;
            
            float animateToScale = targetMaxScale;
            float animateToXPosn = 0;
            float animateToYPosn = 0;
            int longAnimation = keepImageOnScreenFrames + fadeRateFrames;

            // set start points:
            anim.scale = targetMinScale;
            if (doHugRight)
            {
                anim.posn[0] = hugRightMinX;
                anim.posn[1] = hugTopMinY;
                animateToXPosn = hugRightMaxX;
                animateToYPosn = hugTopMaxY;
            }
            else
            {
                anim.posn[0] = hugLeftMinX;
                anim.posn[1] = hugTopMinY;
                animateToXPosn = hugLeftMaxX;
                animateToYPosn = hugTopMaxY;
            }

            

            drawOnTop = true;
            anim.SetNewAlpha(1f, fadeRateFrames);
            anim.SetNewPosn(0, animateToXPosn, longAnimation); // move in front
            anim.SetNewPosn(1, animateToYPosn, longAnimation); // move in front
            anim.SetNewPosn(2, 50, 2); // move in front
            if (goDark)
            {
                anim.SetNewValue(1f, 35, 20);
            }
            else
                anim.SetNewValue(1f, 35, 20);
            anim.SetNewScale(animateToScale, longAnimation);
        }

        public void animateToDetailView()
        {
            int animLength = 45;
            anim.SetNewPosn(0, targetDetailPosn.x, animLength);
            anim.SetNewPosn(1, targetDetailPosn.y, animLength);
            anim.SetNewScale(targetDetailScale, animLength - 15, 15);
        }

        public void animateToDetailViewQuickly()
        {
            if (anim.scale != targetDetailScale) // only if we're not already there...
            {
                int animLength = 5;
                anim.SetNewPosn(0, targetDetailPosn.x, animLength);
                anim.SetNewPosn(1, targetDetailPosn.y, animLength);
                anim.SetNewScale(targetDetailScale, animLength);
            }
        }
        
        public void update()
        {
            anim.Update();

            isAnimating = anim.animationIsActive;
            
            currentPosn = anim.GetCurrentPosition();
            currentAlpha = anim.alpha;
            currentScale.x = anim.scale;
            currentScale.y = anim.scale;
            currentScale.z = anim.scale;
            currentValue = anim.value;
        }
    }
}