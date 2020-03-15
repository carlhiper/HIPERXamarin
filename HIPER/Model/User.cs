using System;
using SQLite;

namespace HIPER.Model
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [MaxLength(20)]
        public string firstName { get; set; }

        [MaxLength(20)]
        public string lastName { get; set; }

        [MaxLength(40)]
        public string email { get; set; }

        [MaxLength(20)]
        public string password { get; set; }

        public User()
        {
        }
    }
}
