using System;


namespace AAMVC
{
    public class OutputChannelSequencer
    {
        public bool outputIsTurnedOn= false;
        
        public bool isActive = false;
        private float startDelay = 0.0f;
        private float totalActiveTime = 4.5f; // make this large to just play until global timeout
        private bool doBlink = false;
        private float blinkOnTime = 1.0f; 
        private float blinkOffTime = 0.5f;


        private float seqStepMs = 250; // update every quarter second?
        private float seqCounter; // this is counting up in seconds
        private System.Timers.Timer seqTimer;

        private bool waitingForBlink = false;
        private float blinkCounter = 0.0f;
        
        private bool dirty = false;
        public bool isDirty()
        {
            bool valueToReturn = dirty;
            if (dirty)
            {
                dirty = false;
            }
            return valueToReturn;
        }
        
        public void initialize(bool whichActive, float whichDelay, float whichOnTime, bool whichBlink, float whichBlinkOn,
            float whichBlinkOff)
        {
            isActive = whichActive;
            startDelay = whichDelay;
            totalActiveTime = whichOnTime;
            doBlink = whichBlink;
            blinkOnTime = whichBlinkOn;
            blinkOffTime = whichBlinkOff;
            
            seqTimer = new System.Timers.Timer();
            seqTimer.Interval = seqStepMs;
            seqTimer.AutoReset = true;
            seqTimer.Elapsed += new System.Timers.ElapsedEventHandler(seqTimer_Elapsed);
        }


        public void startSequence()
        {
            if (isActive)
            {
                seqCounter = 0;
                blinkCounter = 0f;
                seqTimer.Start();
                if (startDelay <= 0)
                {
                    activateOutput(); // turn on right away
                }
                
            }
        }
        
        
        public void stopSequence()
        {
            if (isActive)
            {
                seqCounter = 0;
                seqTimer.Stop();
                deactivateOutput();
               
            }
        }

        private void activateOutput()
        {
            if (!outputIsTurnedOn)
            {
                dirty = true;
                outputIsTurnedOn = true;
            }
        }
        
        private void deactivateOutput()
        {
            if (outputIsTurnedOn)
            {
                dirty = true;
                outputIsTurnedOn = false;
            }
        }
        
        private void seqTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            seqCounter += seqStepMs / 1000f;

            if (seqCounter < totalActiveTime) // have we met the end of the total sequence time
            {
                if (seqCounter > startDelay) // have we passed the delay
                {
                    if (!doBlink)
                    {
                        // time to turn on
                        activateOutput();
                    }
                    else // we're blinking
                    {
                        blinkCounter += seqStepMs / 1000f;
                        if (!waitingForBlink) // we are in the ON period
                        {
                            if (blinkCounter > blinkOnTime) // turn off and reset counter
                            {
                                deactivateOutput();
                                blinkCounter = 0f;
                                waitingForBlink = true;
                            }
                            else
                            {
                                activateOutput();
                            }
                        }
                        else // we are in the OFF period
                        {
                            if (blinkCounter > blinkOffTime) // turn on and reset
                            {
                                activateOutput();
                                blinkCounter = 0f;
                                waitingForBlink = false;
                            }
                            else
                            {
                                deactivateOutput();
                            }
                        }
                    }
                }
            }
            else // total sequence time limit reached 
            {
                // time to turn off
                deactivateOutput();
                
            }

        }

    }
}