namespace AnimationEngine
{
    public class AEEaseInOutQuintOneStage
    {
        private float defaultVal, targetVal; // animate from defaul to target and back
        private float cuePoint0, cuePoint1; // begin animation at 0, end at 1

        private float animatedValue;

        public void SetAnimationValues(float startValue, float endValue, int que0, int que1)
        {
            defaultVal = startValue;
            targetVal = endValue;

            cuePoint0 = (float)que0;
            cuePoint1 = (float)que1;
        }

        
        public float GetAnimatedValue(int currentFrameInt)
        {

            float currentFrame = (float)currentFrameInt;
            //unsafe
            //{
                if ((currentFrame >= cuePoint0) & (currentFrame < cuePoint1))
                {
                    animatedValue = EaseInOutQuint((currentFrame - cuePoint0), defaultVal, (targetVal - defaultVal), (cuePoint1 - cuePoint0));
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
        public float ReturnFinalValue()
        {
            return targetVal;
        }
        private float EaseInOutQuint(float t, float b, float c, float d)
        {
            if ((t /= d / 2f) < 1f) return c / 2f * t * t * t * t * t + b;
            return c / 2f * ((t -= 2f) * t * t * t * t + 2f) + b;
        }
    }
}