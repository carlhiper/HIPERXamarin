using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class PointModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public int Points { get; set; }

        [DataMember]
        public DateTime RegDate { get; set; }

    }
}
