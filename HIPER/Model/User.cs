using System;
using SQLite;

namespace HIPER.Model
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public double teamId { get; set; }

        [MaxLength(20)]
        public string firstName { get; set; }

        [MaxLength(20)]
        public string lastName { get; set; }

        [MaxLength(40)]
        public string email { get; set; }

        [MaxLength(20)]
        public string password { get; set; }

        [MaxLength(40)]
        public string company { get; set; }

        public DateTime createdDate { get; set; }

        public User()
        {
        }
    }
}
