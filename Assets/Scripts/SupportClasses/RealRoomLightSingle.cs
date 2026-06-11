using AnimationEngine;


namespace AAMVC.Unity
{


    public class RealRoomLightSingle
    {
        private static int numberOfLightControls = 8;
        public float[] currentLRGBYWYPColor = new float[numberOfLightControls];
        private GeneralPurposeAnimLinear anim;

        public bool dirty = false;
        public bool isDirty()
        {
            bool valueToReturn = dirty;
            if (dirty)
            {
                dirty = false;
            }
            return valueToReturn;
        }

        public RealRoomLightSingle()
        {
            anim = new GeneralPurposeAnimLinear();
            //anim.initAnim(); (this is inherent)
        }
        
        public void Update()
        {
            anim.Update();

            int i;

            if (anim.value != currentLRGBYWYPColor[0])  // value 0
            {
                currentLRGBYWYPColor[0] = anim.value;
                dirty = true;
            }
            
            for (i = 0; i < 3; ++i)
            {
                if (anim.posn[i] != currentLRGBYWYPColor[i+1]) // values 1-3
                {
                    currentLRGBYWYPColor[i+1] = anim.posn[i];
                    dirty = true;
                    
                }
                if (anim.rotn[i] != currentLRGBYWYPColor[i+4]) // values 4-6
                {
                    currentLRGBYWYPColor[i+4] = anim.rotn[i];
                    dirty = true;
                    
                }
            }
            
            
            if (anim.alpha != currentLRGBYWYPColor[7])  // value 7
            {
                currentLRGBYWYPColor[7] = anim.alpha;
                dirty = true;
            }
        }

        public float getCurrentAnimatedValue(int whichLightValue)
        {
            //int valueToReturn = 0;
            float valueToReturn_f = 0f;
            
            switch (whichLightValue)
            {
                case 0:
                    valueToReturn_f = anim.value;
                    break;
                case 1:
                    valueToReturn_f = anim.posn[0];
                    break;
                case 2:
                    valueToReturn_f = anim.posn[1];
                    break;
                case 3:
                    valueToReturn_f = anim.posn[2];
                    break;
                case 4:
                    valueToReturn_f = anim.rotn[0];
                    break;
                case 5:
                    valueToReturn_f = anim.rotn[1];
                    break;
                case 6:
                    valueToReturn_f = anim.rotn[2];
                    break;
                case 7:
                    valueToReturn_f = anim.alpha;
                    break;
            }
            
            
            return valueToReturn_f;

        }
        
        public void adjustColorTo(int[] whichNewValues, int whichLength, int whichDelay) // always send between 0-255 (?)
        {
            if (whichNewValues.Length != 8) // big assumption about the length of the incoming array
                return;
            
            anim.SetNewValue(whichNewValues[0], whichLength, whichDelay);
            
            anim.SetNewPosn(0, whichNewValues[1], whichLength, whichDelay);
            anim.SetNewPosn(1, whichNewValues[2], whichLength, whichDelay);
            anim.SetNewPosn(2, whichNewValues[3], whichLength, whichDelay);
            
            anim.SetNewRotn(0, whichNewValues[4], whichLength, whichDelay);
            anim.SetNewRotn(1, whichNewValues[5], whichLength, whichDelay);
            anim.SetNewRotn(2, whichNewValues[6], whichLength, whichDelay);
            
            anim.SetNewAlpha(whichNewValues[7], whichLength, whichDelay);

        }
    }
}