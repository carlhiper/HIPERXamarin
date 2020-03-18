using System;
using SQLite;

namespace HIPER.Model
{
    public class TeamModel
    {
        [PrimaryKey, AutoIncrement]
        public double id { get; set; }

        [MaxLength(40)]
        public string name { get; set; }

        [MaxLength(40)]
        public string password { get; set; }

        [MaxLength(40)]
        public string company { get; set; }

        [MaxLength(40)]
        public string organisation_number { get; set; }

        public DateTime createdDate { get; set; }

        public int max_number_users { get; set; }

        public int administrator_id { get; set; }

        public bool active { get; set; }

    }
}
