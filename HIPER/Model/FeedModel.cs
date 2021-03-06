﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HIPER.Model
{
    [DataContract]
    public class FeedModel 
    {
        [DataMember]
        public string FeedItemTitle { get; set; }

        [DataMember]
        public string FeedItemPost { get; set; }

        [DataMember]
        public string FeedItemIndicator { get; set; }

        [DataMember]
        public string ProfileImageURL { get; set; }

        [DataMember]
        public DateTime IndexDate { get; set; }

        [DataMember]
        public string UserId { get; set; }

        //public DateTimeOffset TimeAgo{ get ; set ; }

    }
}
