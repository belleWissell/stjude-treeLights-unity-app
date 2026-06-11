namespace SupportClasses
{
    public class StoryData
    {
        private static int maxNumberOfStories = 200;
        private static int maxNumberOfContentInAStory = 200;
        public int actualNumberOfStories = 0;

        public string[] storyYears = new string[maxNumberOfStories];
        public string[] storyTitles = new string[maxNumberOfStories];

        public int[,] contentStoryCollection = new int[maxNumberOfStories, maxNumberOfContentInAStory];
        public int[] numberOfContentInStory = new int[maxNumberOfStories];
        
        public StoryData()
        {
            
        }
        
        public void addContent(int whichContentID, string whichYear, string whichTitle)
        {
            // check to see if story already exists
            int i, j;
            bool doAddNewStory = true;
            int foundStoryMatch = -1;
            
            for (i = 0; i < actualNumberOfStories; ++i)
            {
                if (whichYear == storyYears[i])
                {
                    if (whichTitle == storyTitles[i])
                    {
                        doAddNewStory = false;
                        foundStoryMatch = i;
                    }
                }
            }

            if (doAddNewStory)
            {
                storyYears[actualNumberOfStories] = whichYear;
                storyTitles[actualNumberOfStories] = whichTitle;
                contentStoryCollection[actualNumberOfStories, 0] = whichContentID; // first entry in story collection
                numberOfContentInStory[actualNumberOfStories] += 1; // add to count of content in that story
                System.Diagnostics.Debug.WriteLine("[storyData] CREATING story, adding "+whichContentID+" to story " + actualNumberOfStories);
                System.Diagnostics.Debug.WriteLine("[storyData] story "+actualNumberOfStories+" has " + numberOfContentInStory[actualNumberOfStories] +" stories");

                actualNumberOfStories += 1; // add new story
                if (actualNumberOfStories > maxNumberOfStories)
                    actualNumberOfStories = maxNumberOfStories - 1;
            }
            else // add content to existing story
            {
                contentStoryCollection[foundStoryMatch, numberOfContentInStory[foundStoryMatch]] = whichContentID;
                numberOfContentInStory[foundStoryMatch] += 1; // add to count of content in that story
                if (numberOfContentInStory[foundStoryMatch] > maxNumberOfContentInAStory)
                    numberOfContentInStory[foundStoryMatch] = maxNumberOfContentInAStory -1;
                
                System.Diagnostics.Debug.WriteLine("[storyData] adding "+whichContentID+" to story " + foundStoryMatch);
                System.Diagnostics.Debug.WriteLine("[storyData] story "+foundStoryMatch+" has " + numberOfContentInStory[foundStoryMatch] +" stories");
            }
        }

        public int getNumberOfContentInStory(int whichStoryID)
        {
            return numberOfContentInStory[whichStoryID];
        }

        public int getContentIdForStory(int whichStoryID, int whichContentIndex)
        {
            return contentStoryCollection[whichStoryID, whichContentIndex]; 
        }

        public string getTitleForStory(int whichStoryID)
        {
            return storyTitles[whichStoryID];
        }

        public string getDateForStory(int whichStoryID)
        {
            return storyYears[whichStoryID];
        }
        
    }
}