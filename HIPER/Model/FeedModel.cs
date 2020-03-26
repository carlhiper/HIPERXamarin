using System;
namespace HIPER.Model
{
    public class FeedModel
    {

        public string UserName { get; set; }

        public string GoalTitle { get; set; }
    
        public float Progress { get; set; }

        public int Hipes { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime LastUpdated { get; set; }

        public FeedModel()
        {
        }
    }
}
