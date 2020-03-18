using System;
using SQLite;

namespace HIPER.Model
{
    public class UserModel
    {
        [PrimaryKey, AutoIncrement]
        public double id { get; set; }

        public double teamId { get; set; }

        [MaxLength(40)]
        public string firstName { get; set; }

        [MaxLength(40)]
        public string lastName { get; set; }

        [MaxLength(100)]
        public string email { get; set; }

        [MaxLength(40)]
        public string userPassword { get; set; }

        [MaxLength(40)]
        public string company { get; set; }

        public DateTime createdDate { get; set; }

        public int number_logins { get; set; }

        [MaxLength(100)]
        public string picture_path { get; set; }

        public float rating { get; set; }

        public UserModel()
        {
        }
    }
}
