using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class TeamsModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string TeamId { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public DateTime LastViewedPostDate { get; set; }

    }
}
