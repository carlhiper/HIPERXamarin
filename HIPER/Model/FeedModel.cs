using System;

namespace HIPER.Model
{
    public class FeedModel
    {
        public string FeedItemTitle { get; set; }

        public string FeedItemPost { get; set; }

        public string FeedItemIndicator { get; set; }

        public string UserName { get; set; }

        public string GoalTitle { get; set; }
    
        public float Progress { get; set; }

        public int Hipes { get; set; }

        public string ProfileImageURL { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime LastUpdated { get; set; }

        public DateTime IndexDate { get; set; }

        public FeedModel()
        {
        }
    }
}
