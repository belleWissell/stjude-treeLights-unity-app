using System; // for Math.pow

namespace AnimationEngine
{
    public class AEEaseInExpoOneStage
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
                animatedValue = EaseInExpo((currentFrame - cuePoint0), defaultVal, (targetVal - defaultVal), (cuePoint1 - cuePoint0));
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

      
        private static float EaseInExpo(float t, float b, float c, float d)
        {
            return (t == 0) ? b : c * (float)Math.Pow(2, 10 * (t / d - 1)) + b;
        }
        
    }
}
