using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using SQLite;

namespace HIPER.Model
{

    public class UserModel
    {
        [PrimaryKey]
        [JsonProperty("id")]
        public string id { get; set; }

        public string TeamId { get; set; }

        [MaxLength(40)]
        public string FirstName { get; set; }

        [MaxLength(40)]
        public string LastName { get; set; }

        [MaxLength(100)]
        public string Email { get; set; }

        [MaxLength(40)]
        public string UserPassword { get; set; }

        [MaxLength(40)]
        public string Company { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Number_logins { get; set; }

        [MaxLength(100)]
        public string Picture_path { get; set; }

        public float Rating { get; set; }

        public string ImageUrl { get; set; }

        public string ImageName { get; set; }

        public DateTime LastViewedPostDate { get; set; }

        public UserModel()
        {
        }
    }
}
