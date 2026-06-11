using System; // for Math.pow

namespace AnimationEngine
{
    public class AEEaseOutExpoOneStage
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
                animatedValue = EaseOutExpo((currentFrame - cuePoint0), defaultVal, (targetVal - defaultVal), (cuePoint1 - cuePoint0));
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

        // taken from https://gist.github.com/mrhelmut/3b70813cacc6c2e1e9f853b74e124dae
        private static float EaseOutExpo(float t, float b, float c, float d)
        {
            return (t == d) ? b + c : c * (-(float)Math.Pow(2, -10 * t / d) + 1) + b;
        }
        
    }
}