using System;
using System.ComponentModel;

namespace HIPER.Model
{
    public class FeedModel 
    {
        public string FeedItemTitle { get; set; }

        public string FeedItemPost { get; set; }

        public string FeedItemIndicator { get; set; }

        public string ProfileImageURL { get; set; }

        public DateTime IndexDate { get; set; }

        public string UserId { get; set; }

        //public DateTimeOffset TimeAgo{ get ; set ; }

    }
}
