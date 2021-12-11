using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class TeamModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Name { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Identifier { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Password { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Company { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Organisation_number { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public int Max_number_users { get; set; }

        [DataMember]
        public string Administrator_id { get; set; }

        [DataMember]
        public bool Active { get; set; }
    }
}


//CREATE TABLE TeamModel
//(
//	id NVARCHAR(36) PRIMARY KEY,
//    name NVARCHAR(256),
//	identifier NVARCHAR(256),
//	password NVARCHAR(256),
//	company NVARCHAR(256),
//	organisation_number NVARCHAR(256),
//	createdDate DATETIMEOFFSET,
//    max_number_users INT,
//    administrator_id NVARCHAR(256),
//	active BIT
//);