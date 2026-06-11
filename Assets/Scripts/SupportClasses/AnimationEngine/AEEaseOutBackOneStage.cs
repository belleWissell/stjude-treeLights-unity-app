namespace AnimationEngine
{
    public class AEEaseOutBackOneStage
    {
        private float defaultVal, targetVal; // animate from defaul to target
        private float cuePoint0, cuePoint1; // begin animation at 0, end at 1

        private float animatedValue;

        public void SetAnimationValues(float startValue, float endValue, int que0, int que1)
        {
            defaultVal = startValue;
            targetVal = endValue;

            cuePoint0 = (float)que0;
            cuePoint1 = (float)que1;
        }
        public float ReturnFinalValue()
        {
            return targetVal;
        }
        public float GetAnimatedValue(int currentFrameInt)
        {
            float currentFrame = (float)currentFrameInt;
            //unsafe
            //{
                if ((currentFrame >= cuePoint0) & (currentFrame < cuePoint1))
                {
                    animatedValue = EaseOutBack((currentFrame - cuePoint0), defaultVal, (targetVal - defaultVal), (cuePoint1 - cuePoint0));
                    return animatedValue;
                }
                else if (currentFrame >= cuePoint1)
                {
                    return targetVal;
                }
                else
                {
                    // default with this value
                    return defaultVal;
                }
            //}
        }

        private static float EaseOutBack(float t, float b, float c, float d)
        {
            return c * ((t = t / d - 1.0f) * t * ((2.702f) * t + 1.702f) + 1.0f) + b;
        }        
    }
}