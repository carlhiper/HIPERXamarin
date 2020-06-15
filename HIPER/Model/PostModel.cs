using System;
using System.Runtime.Serialization;
using SQLite;


namespace HIPER.Model
{
    [DataContract]
    public class PostModel
    {

        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string UserId { get; set; }

        [DataMember]
        public string Post { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

    }
}
