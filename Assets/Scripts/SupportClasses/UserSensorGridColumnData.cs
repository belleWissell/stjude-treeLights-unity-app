namespace AAMVC.CommunicationsAndControl
{
    public class UserSensorGridColumnData
    {
        // **************************************
        // GET/SET has state changed?
        // **************************************
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

        public float highestActivePoint = -1f;
        private float prevHighestActivePoint = -1f;
        public float lowestPoint = -999f;
        
        
        public bool isActive = false;
        private bool prevActive = false;

        public void resetColumn()
        {
            isActive = false;
            highestActivePoint = lowestPoint;
        }
        
        public void updateColumnData(float whichNewHeight)
        {
            isActive = true; // assigning data automatically makes it active

            if (whichNewHeight > highestActivePoint) // new height record for this column
                highestActivePoint = whichNewHeight;
        }

        public void checkForChanges()
        {
            if (highestActivePoint != prevHighestActivePoint) // highest point changed
            {
                prevHighestActivePoint = highestActivePoint;
                dirty = true;
            }

            if (isActive != prevActive) // activation changed
            {
                prevActive = isActive;
                dirty = true;
            }
        }

        public void update()
        {
            // TODO ease between active values of highestActivePoint here
        }
        
    }
}