using UnityEngine;
namespace AnimationEngine
{
    public class AEEaseOutElasticFaster
    {
        
        private float defaultVal, targetVal; // animate from default to target
        private float cuePoint0, cuePoint1; // begin animation at 0, end at 1

        private float animatedValue;
        private static float _mathPI = (float)Mathf.PI;

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
                animatedValue = EaseOutElastic((currentFrame - cuePoint0), defaultVal, (targetVal - defaultVal), (cuePoint1 - cuePoint0));
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


        private static float EaseOutElastic(float t, float b, float c, float d)
        {
            if ((t /= d) == 1f) return b + c;
            float p = d * .15f; // reduced this value (oscillates faster)
            float s = p / 4f;
            return (c * (float)Mathf.Pow(3f, -20f * t) * (float)Mathf.Sin((t * d - s) * (2f * _mathPI) / p) + c + b); // increased the power function (oscillates for shorter portion of time)
        }        
    
    }
}