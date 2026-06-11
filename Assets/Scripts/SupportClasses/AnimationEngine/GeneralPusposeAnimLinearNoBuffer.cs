using UnityEngine;

namespace AnimationEngine
{
    public class GeneralPurposeAnimLinearNoBuffer
    {

        public bool animationIsActive = false;
        
        // ********************************************************************************
        // 3d Position
        public Vector3 currentPosition;
        public float[] basePosition = new float[3];
        public float[] posn = new float[3];
        public float[] targetPosn = new float[3];

        //private Vector3 targetPosn;
        private int[] posnAnimationFrame = new int[3]; // x, y, and z
        private bool[] posnAnimationActive = new bool[3]; // x, y, and z
        private int[] posnAnimationLength = new int[3]; // x, y, and z
        private AEEaseInOutLinearOneStage[] targetPosnAnim = new AEEaseInOutLinearOneStage[3]; // x, y, and z

        // ********************************************************************************
        // 3d Rotation 
        public Vector3 currentRotation;
        public float[] baseRotation = new float[3];
        public float[] rotn = new float[3];
        public float[] targetRotn = new float[3];

        //private Vector3 targetPosn;
        private int[] rotnAnimationFrame = new int[3]; // x, y, and z
        private bool[] rotnAnimationActive = new bool[3]; // x, y, and z
        private int[] rotnAnimationLength = new int[3]; // x, y, and z
        private AEEaseInOutLinearOneStage[] targetRotnAnim = new AEEaseInOutLinearOneStage[3]; // x, y, and z


        // ********************************************************************************
        // Scale
        public float scale;
        public float targetScale;
        private bool scaleAnimationActive = false;
        private int scaleAnimationFrame = 0;
        private int scaleAnimationLength = 20;
        private AEEaseInOutLinearOneStage targetScaleAnim;

        // ********************************************************************************
        // Transparency
        public float alpha;
        public float targetAlpha;
        private bool alphaAnimationActive = false;
        private int alphaAnimationFrame = 0;
        private int alphaAnimationLength = 20;
        private AEEaseInOutLinearOneStage targetAlphaAnim;

        // ********************************************************************************
        // other animated value
        public float value;
        public float targetValue;
        private bool valueAnimationActive = false;
        private int valueAnimationFrame = 0;
        private int valueAnimationLength = 20;
        private AEEaseInOutLinearOneStage targetValueAnim;
        
        public GeneralPurposeAnimLinearNoBuffer()
        {
            currentPosition.x = 0.0f;
            currentPosition.y = 0.0f;
            currentPosition.z = 0.0f;
            
            alpha = 0.0f;
            targetAlpha = 0.1f;
            scale = 0.0f;
            targetScale = 100.0f;
            value = 0.0f;
            targetValue = 0.0f;

            int i;
            for (i = 0; i < 3; ++i)
            {
                basePosition[i] = 0.0f;
                posn[i] = 0.0f;
                targetPosn[i] = 0.0f;
                targetPosnAnim[i] = new AEEaseInOutLinearOneStage();

                baseRotation[i] = 0.0f;
                rotn[i] = 0.0f;
                targetRotn[i] = 0.0f;
                targetRotnAnim[i] = new AEEaseInOutLinearOneStage();

            }
            
            targetScaleAnim = new AEEaseInOutLinearOneStage();
            targetAlphaAnim = new AEEaseInOutLinearOneStage();
            targetValueAnim = new AEEaseInOutLinearOneStage();
        }    
        
        public void SetPosnOffsets(float whichX, float whichY, float whichZ)
        {
            basePosition[0] = whichX;
            basePosition[1] = whichY;
            basePosition[2] = whichZ;

            int animLeng = StartPositionAnimation(2); // animate to Z position
            StartPositionAnimation(0, animLeng);
            StartPositionAnimation(1, animLeng);
            StartAlphaAnimation(animLeng);
            StartScaleAnimation(animLeng);
        }
        
        public void SetRotnOffsets(float whichX, float whichY, float whichZ)
        {
            baseRotation[0] = whichX;
            baseRotation[1] = whichY;
            baseRotation[2] = whichZ;
 
            int animLeng = StartRotationAnimation(2); // animate to Z position
            StartRotationAnimation(0, animLeng);
            StartRotationAnimation(1, animLeng);
        }
        // ****************************************************************************************************************************************************************
        public void stopAllAnimations()
        {
            StopPositionAnimation();
            StopRotationAnimation();
            StopScaleAnimation();
            StopAlphaAnimation();
            StopValueAnimation();
            
        }
        
        #region set new values and start aniamtions

        public int SetNewPosn(int whichAxis, float whichPosn)
        {
            //if (whichPosn != targetPosn[whichAxis])
            //{
                targetPosn[whichAxis] = whichPosn;
                return StartPositionAnimation(whichAxis);
            //}
            //else
            //    return 15;
        }
        public int SetNewPosn(int whichAxis, float whichPosn, int whichAnimLength)
        {
            //if (whichPosn != targetPosn[whichAxis])
            //{
                targetPosn[whichAxis] = whichPosn;
                return StartPositionAnimation(whichAxis, whichAnimLength);
            //}
            //else
            //    return 15;
        }
        public int SetNewPosn(int whichAxis, float whichPosn, int whichAnimLength, int whichDelay)
        {
            //if (whichPosn != targetPosn[whichAxis])
            //{
                targetPosn[whichAxis] = whichPosn;
                return StartPositionAnimation(whichAxis, whichAnimLength, whichDelay);
            //}
            //else
            //    return 15;
        }



        // ****************************************************************************************************************************************************************

        public int SetNewRotn(int whichAxis, float whichRotn)
        {
            targetRotn[whichAxis] = whichRotn;
            return StartRotationAnimation(whichAxis);
        }
        public int SetNewRotn(int whichAxis, float whichRotn, int whichAnimLength)
        {
            targetRotn[whichAxis] = whichRotn;
            return StartRotationAnimation(whichAxis, whichAnimLength);
        }
        public int SetNewRotn(int whichAxis, float whichRotn, int whichAnimLength, int whichDelay)
        {
            targetRotn[whichAxis] = whichRotn;
            return StartRotationAnimation(whichAxis, whichAnimLength, whichDelay);
        }



        // ****************************************************************************************************************************************************************

        public void SetNewAlpha(float whichA)
        {
            //if (whichA != targetAlpha)
            //{
                targetAlpha = whichA;
                StartAlphaAnimation(-1);
            //}
            //else
            //{
            //    stopAlphaAnimation();
            //}
        }
        public void SetNewAlpha(float whichA, int whichL)
        {
            //if (whichA != targetAlpha)
            //{
                targetAlpha = whichA;
                StartAlphaAnimation(whichL);
            //}
            //else
            //{
            //    stopAlphaAnimation();
            //}
        }
        public void SetNewAlpha(float whichA, int whichL, int whichDelay)
        {
            //if (whichA != targetAlpha)
            //{
                targetAlpha = whichA;
                StartAlphaAnimation(whichL, whichDelay);
            //}
            //else
            //{
            //    stopAlphaAnimation();
            //}
        }
        // ****************************************************************************************************************************************************************

        public void forceCurrentValueTo(float whichV)
        {
            targetValue = whichV;
            value = whichV;
            //StartValueAnimation(-1);
            //targetValueAnim.SetAnimationValues(value, targetValue, 0, 1);
        }
        
        public void SetNewValue(float whichV)
        {
            targetValue = whichV;
            StartValueAnimation(-1);
        }
        public void SetNewValue(float whichV, int whichL)
        {
            targetValue = whichV;
            StartValueAnimation(whichL);
        }
        public void SetNewValue(float whichV, float whichLf)
        {
            int whichL = (int)Mathf.Floor(whichLf);
            targetValue = whichV;
            StartValueAnimation(whichL);
        }
        public void SetNewValue(float whichV, int whichL, int whichDelay)
        {
            targetValue = whichV;
            StartValueAnimation(whichL, whichDelay);
        }

        // ****************************************************************************************************************************************************************

        public void SetNewScale(float whichS)
        {
            //if (whichS != targetScale)
            //{
                targetScale = whichS;
                StartScaleAnimation(-1);
            //}
        }
        public void SetNewScale(float whichS, int whichL)
        {
            //if (whichS != targetScale)
            //{
                targetScale = whichS;
                StartScaleAnimation(whichL);
            //}
        }
        public void SetNewScale(float whichS, int whichL, int whichDelay)
        {
            //if (whichS != targetScale)
            //{
                targetScale = whichS;
                StartScaleAnimation(whichL, whichDelay);
            //}
        }

        #endregion set new values and start aniamtions

        // ****************************************************************************************************************************************************************

        public void Update()
        {
            float delta;
            animationIsActive = false; // assume nothing is animating
            float animationLengthBuffer = 1.01f;
            //unsafe
            //{
                for (int i = 0; i < 3; ++i)
                {
                    if (posnAnimationActive[i])
                    {
                        animationIsActive = true;
                        delta = targetPosn[i] - posn[i];
                        //System.Diagnostics.Debug.WriteLine("new position: " + posn.Y + " target:" + targetPosn.Y);
                        if (posnAnimationFrame[i] < posnAnimationLength[i] * animationLengthBuffer) // still animating target
                        {
                            posnAnimationFrame[i] += 1;
                            targetPosn[i] = targetPosnAnim[i].GetAnimatedValue(posnAnimationFrame[i]);
                            delta = targetPosn[i] - posn[i];
                            posn[i] += delta / 2.0f;
                        }
                        else
                        {
                            if (Mathf.Abs(delta) > 1.0) // still not there
                            {
                                posn[i] += delta / 2.0f;
                            }
                            else
                            {
                                //posn[i] = targetPosnAnim[i].GetAnimatedValue(posnAnimationLength[i] * 2); // set to end value
                                posn[i] = targetPosnAnim[i].ReturnFinalValue();
                                posnAnimationActive[i] = false;
                            }
                        }
                    }
                    

                    if (rotnAnimationActive[i])
                    {
                        animationIsActive = true;
                        delta = targetRotn[i] - rotn[i];
                        if (rotnAnimationFrame[i] < rotnAnimationLength[i] * animationLengthBuffer) // still animating target
                        {
                            rotnAnimationFrame[i] += 1;
                            targetRotn[i] = targetRotnAnim[i].GetAnimatedValue(rotnAnimationFrame[i]);
                            delta = targetRotn[i] - rotn[i];
                            rotn[i] += delta / 2.0f;
                        }
                        else
                        {
                            if (Mathf.Abs(delta) > 1.0) // still not there
                            {
                                rotn[i] += delta / 2.0f;
                            }
                            else
                            {
                                //rotn[i] = targetRotnAnim[i].GetAnimatedValue(rotnAnimationLength[i] * 2); // set to end value
                                rotn[i] = targetRotnAnim[i].ReturnFinalValue();
                                rotnAnimationActive[i] = false;
                            }
                        }
                    }
                }

                if (scaleAnimationActive)
                {
                    animationIsActive = true;
                    delta = targetScale - scale;
                    if (scaleAnimationFrame < scaleAnimationLength * animationLengthBuffer) // still animating target
                    {
                        scaleAnimationFrame += 1;
                        targetScale = targetScaleAnim.GetAnimatedValue(scaleAnimationFrame);
                        delta = targetScale - scale;
                        scale += delta / 4.0f;
                    }
                    else
                    {
                        if (Mathf.Abs(delta) > 0.01) // still not there
                        {
                            scale += delta / 4.0f;
                        }
                        else
                        {
                            //scale = targetScaleAnim.GetAnimatedValue(scaleAnimationLength * 2); // set to end value
                            scale = targetScaleAnim.ReturnFinalValue();
                            scaleAnimationActive = false;
                        }
                    }
                }

                if (alphaAnimationActive)
                {
                    animationIsActive = true;
                    delta = targetAlpha - alpha;
                    if (alphaAnimationFrame < alphaAnimationLength * animationLengthBuffer) // still animating target
                    {
                        alphaAnimationFrame += 1;
                        targetAlpha = targetAlphaAnim.GetAnimatedValue(alphaAnimationFrame);
                        delta = targetAlpha - alpha;
                        alpha += delta / 4.0f;
                    }
                    else
                    {
                        if (Mathf.Abs(delta) > 0.01) // still not there
                        {
                            alpha += delta / 4.0f;
                        }
                        else
                        {
                            //alpha = targetAlphaAnim.GetAnimatedValue(alphaAnimationLength * 2); // set to end value
                            alpha = targetAlphaAnim.ReturnFinalValue();
                            alphaAnimationActive = false;
                        }
                    }
                }
                if (valueAnimationActive)
                {
                    animationIsActive = true;
                    delta = targetValue - value;
                    if (valueAnimationFrame < valueAnimationLength * animationLengthBuffer) // still animating target
                    {
                        valueAnimationFrame += 1;
                        targetValue = targetValueAnim.GetAnimatedValue(valueAnimationFrame);
                        delta = targetValue - value;
                        value += delta / 4.0f;
                    }
                    else
                    {
                        //if (Math.Abs(delta) > 0.01) // still not there
                        if (Mathf.Abs(delta) > Mathf.Abs((targetValueAnim.ReturnFinalValue() / 100.0f))) // still not there (off by more than 1 percent)
                        {
                            value += delta / 4.0f;
                        }
                        else
                        {
                            //value = targetValueAnim.getAnimatedValue(valueAnimationLength * 2); // set to end value
                            value = targetValueAnim.ReturnFinalValue();
                            valueAnimationActive = false;
                        }
                    }
                }
            //}// end of unsafe
        }

        // ****************************************************************************************************************************************************************
        #region position animation

        public Vector3 GetCurrentPosition()
        {
            currentPosition.x = posn[0] + basePosition[0];
            currentPosition.y = posn[1] + basePosition[1];
            currentPosition.z = posn[2] + basePosition[2];

            return currentPosition;
        }

        public Vector3 GetCurrentPositionWoBaseOffset()
        {
            Vector3 valueToReturn;

            valueToReturn.x = posn[0];
            valueToReturn.y = posn[1];
            valueToReturn.z = posn[2];

            return valueToReturn;
        }

        public Vector3 AddBaseOffsetTo(Vector3 incomingPosition)
        {
            Vector3 valueToReturn;

            valueToReturn.x = incomingPosition.x + basePosition[0];
            valueToReturn.y = incomingPosition.y + basePosition[1];
            valueToReturn.z = incomingPosition.z + basePosition[2];

            return valueToReturn;
        }


        private int StartPositionAnimation(int whichAxis)
        {
            //int numberOfFrames = (int)Mathf.Ceiling(Mathf.Abs((double)(targetPosn[whichAxis] - posn[whichAxis])) / 12.0);
            int numberOfFrames = (int) Mathf.Ceil(Mathf.Abs((targetPosn[whichAxis] - posn[whichAxis])) / 12.0f);  
            if (numberOfFrames < 15)
                numberOfFrames = 15;

            posnAnimationLength[whichAxis] = numberOfFrames;
            posnAnimationFrame[whichAxis] = 0;
            posnAnimationActive[whichAxis] = true;

            targetPosnAnim[whichAxis].SetAnimationValues(posn[whichAxis], targetPosn[whichAxis], posnAnimationFrame[whichAxis], posnAnimationLength[whichAxis]);

            return posnAnimationLength[whichAxis];
        }

        private int StartPositionAnimation(int whichAxis, int whichLength)
        {
            //int numberOfFrames = (int)Math.Ceiling(Math.Abs((double)(targetPosn[whichAxis] - posn[whichAxis])) / 30.0);
            //if (numberOfFrames < 15)
            //    numberOfFrames = 15;

            posnAnimationLength[whichAxis] = whichLength;
            posnAnimationFrame[whichAxis] = 0;
            posnAnimationActive[whichAxis] = true;

            targetPosnAnim[whichAxis].SetAnimationValues(posn[whichAxis], targetPosn[whichAxis], posnAnimationFrame[whichAxis], posnAnimationLength[whichAxis]);

            return posnAnimationLength[whichAxis];
        }

        private int StartPositionAnimation(int whichAxis, int whichLength, int whichDelay)
        {
            //int numberOfFrames = (int)Math.Ceiling(Math.Abs((double)(targetPosn[whichAxis] - posn[whichAxis])) / 30.0);
            //if (numberOfFrames < 15)
            //    numberOfFrames = 15;

            posnAnimationLength[whichAxis] = whichLength + whichDelay;
            posnAnimationFrame[whichAxis] = 0;
            posnAnimationActive[whichAxis] = true;

            targetPosnAnim[whichAxis].SetAnimationValues(posn[whichAxis], targetPosn[whichAxis], posnAnimationFrame[whichAxis] + whichDelay, posnAnimationLength[whichAxis]);

            return posnAnimationLength[whichAxis];
        }
        private void StopPositionAnimation()
        {
            posnAnimationActive[0] = false;
            posnAnimationActive[1] = false;
            posnAnimationActive[2] = false;
        }
        #endregion position animation

        // ****************************************************************************************************************************************************************


        // ****************************************************************************************************************************************************************
        #region rotation animation

        public Vector3 GetCurrentRotation()
        {
            currentRotation.x = rotn[0] + baseRotation[0];
            currentRotation.y = rotn[1] + baseRotation[1];
            currentRotation.z = rotn[2] + baseRotation[2];

            return currentRotation;
        }        
        public Quaternion GetCurrentRotationQuat()
        {
            //Quaternion valueToReturn = new Quaternion();
            
            currentRotation.x = rotn[0] + baseRotation[0];
            currentRotation.y = rotn[1] + baseRotation[1];
            currentRotation.z = rotn[2] + baseRotation[2];

            Quaternion valueToReturn = Quaternion.Euler(currentRotation.x, currentRotation.y, currentRotation.z);
            return valueToReturn;
        }

        /// <summary>
        /// returns normalized vector 0-1
        /// </summary>
        /// <returns></returns>
        public Vector3 GetCurrentRotationForShader()
        {
            currentRotation.x = (rotn[0] + baseRotation[0]) / 360.0f;
            currentRotation.y = (rotn[1] + baseRotation[1]) / 360.0f;
            currentRotation.z = (rotn[2] + baseRotation[2]) / 360.0f;

            return currentRotation;
        }


        private int StartRotationAnimation(int whichAxis)
        {
            int numberOfFrames = (int)Mathf.Ceil(Mathf.Abs((targetRotn[whichAxis] - rotn[whichAxis])) / 12.0f);
            if (numberOfFrames < 15)
                numberOfFrames = 15;

            rotnAnimationLength[whichAxis] = numberOfFrames;
            rotnAnimationFrame[whichAxis] = 0;
            rotnAnimationActive[whichAxis] = true;

            targetRotnAnim[whichAxis].SetAnimationValues(rotn[whichAxis], targetRotn[whichAxis], rotnAnimationFrame[whichAxis], rotnAnimationLength[whichAxis]);

            return rotnAnimationLength[whichAxis];
        }

        private int StartRotationAnimation(int whichAxis, int whichLength)
        {
            //int numberOfFrames = (int)Math.Ceiling(Math.Abs((double)(targetPosn[whichAxis] - posn[whichAxis])) / 30.0);
            //if (numberOfFrames < 15)
            //    numberOfFrames = 15;

            rotnAnimationLength[whichAxis] = whichLength;
            rotnAnimationFrame[whichAxis] = 0;
            rotnAnimationActive[whichAxis] = true;

            targetRotnAnim[whichAxis].SetAnimationValues(rotn[whichAxis], targetRotn[whichAxis], rotnAnimationFrame[whichAxis], rotnAnimationLength[whichAxis]);

            return rotnAnimationLength[whichAxis];
        }

        private int StartRotationAnimation(int whichAxis, int whichLength, int whichDelay)
        {
            //int numberOfFrames = (int)Math.Ceiling(Math.Abs((double)(targetPosn[whichAxis] - posn[whichAxis])) / 30.0);
            //if (numberOfFrames < 15)
            //    numberOfFrames = 15;

            rotnAnimationLength[whichAxis] = whichLength + whichDelay;
            rotnAnimationFrame[whichAxis] = 0;
            rotnAnimationActive[whichAxis] = true;

            targetRotnAnim[whichAxis].SetAnimationValues(rotn[whichAxis], targetRotn[whichAxis], rotnAnimationFrame[whichAxis] + whichDelay, rotnAnimationLength[whichAxis]);

            return rotnAnimationLength[whichAxis];
        }

        private void StopRotationAnimation()
        {
            rotnAnimationActive[0] = false;
            rotnAnimationActive[1] = false;
            rotnAnimationActive[2] = false;
        }
        #endregion rotation animation

        // ****************************************************************************************************************************************************************
        

        // ****************************************************************************************************************************************************************
        #region scale animation

        private void StartScaleAnimation(int whichNumberOfFrames)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 20;

            scaleAnimationLength = whichNumberOfFrames;
            scaleAnimationFrame = 0;
            scaleAnimationActive = true;

            targetScaleAnim.SetAnimationValues(scale, targetScale, scaleAnimationFrame, scaleAnimationLength);
        }

        private void StartScaleAnimation(int whichNumberOfFrames, int whichDelay)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 20;

            scaleAnimationLength = whichNumberOfFrames + whichDelay;
            scaleAnimationFrame = 0;
            scaleAnimationActive = true;

            targetScaleAnim.SetAnimationValues(scale, targetScale, whichDelay, scaleAnimationLength);
        }
        private void StopScaleAnimation()
        {
            scaleAnimationActive = false;
        }
        #endregion scale animation

        // ****************************************************************************************************************************************************************
        #region alpha animation

        private void StartAlphaAnimation(int whichNumberOfFrames)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 15;

            alphaAnimationLength = whichNumberOfFrames;
            alphaAnimationFrame = 0;
            alphaAnimationActive = true;

            targetAlphaAnim.SetAnimationValues(alpha, targetAlpha, 0, alphaAnimationLength);
        }
        private void StartAlphaAnimation(int whichNumberOfFrames, int whichDelay)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 15;

            alphaAnimationLength = whichNumberOfFrames + whichDelay;
            alphaAnimationFrame = 0;
            alphaAnimationActive = true;

            targetAlphaAnim.SetAnimationValues(alpha, targetAlpha, whichDelay, alphaAnimationLength);
        }

        private void StopAlphaAnimation()
        {
            alphaAnimationActive = false;
        }

        #endregion alpha animation
        
        // ****************************************************************************************************************************************************************
        #region value animation

        private void StartValueAnimation(int whichNumberOfFrames)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 15;

            valueAnimationLength = whichNumberOfFrames;
            valueAnimationFrame = 0;
            valueAnimationActive = true;

            targetValueAnim.SetAnimationValues(value, targetValue, valueAnimationFrame, valueAnimationLength);
        }

        private void StartValueAnimation(int whichNumberOfFrames, int whichDelay)
        {
            if (whichNumberOfFrames == -1)
                whichNumberOfFrames = 15;

            valueAnimationLength = whichNumberOfFrames + whichDelay;
            valueAnimationFrame = 0;
            valueAnimationActive = true;

            targetValueAnim.SetAnimationValues(value, targetValue, whichDelay, valueAnimationLength);
        }
        private void StopValueAnimation()
        {
            valueAnimationActive = false;
        }
        #endregion value animation

        // ****************************************************************************************************************************************************************

    }
}