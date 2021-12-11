using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class UserModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public string TeamId { get; set; }

        [DataMember]
        [MaxLength(40)]
        public string FirstName { get; set; }

        [DataMember]
        [MaxLength(40)]
        public string LastName { get; set; }

        [MaxLength(100)]
        [DataMember]
        public string Email { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string UserPassword { get; set; }

        [MaxLength(40)]
        [DataMember]
        public string Company { get; set; }

        [DataMember]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        public int Number_logins { get; set; }

        [DataMember]
        public float Rating { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string ImageUrl { get; set; }

        [DataMember]
        [MaxLength(255)]
        public string Imagename { get; set; }

        [DataMember]
        public DateTime LastViewedPostDate { get; set; }

    }
}


//CREATE TABLE UserModel
//(
//	id NVARCHAR(36) PRIMARY KEY,
//  teamId NVARCHAR(256),
//	firstname NVARCHAR(256),
//	lastname NVARCHAR(256),
//	email NVARCHAR(256),
//	userPassword NVARCHAR(256),
//	company NVARCHAR(256),
//	createdDate DATETIMEOFFSET,
//  number_logins INT,
//  rating FLOAT,
//  imageUrl NVARCHAR(256),
//	imagename NVARCHAR(256),
//	lastViewedPostDate DATETIMEOFFSET
//);