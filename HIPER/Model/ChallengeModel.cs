﻿using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class ChallengeModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string OwnerId { get; set; }
        [DataMember]
        public string InitialGoalId { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }

    }
}
