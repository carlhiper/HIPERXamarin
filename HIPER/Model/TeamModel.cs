﻿using System;
using SQLite;

namespace HIPER.Model
{
    public class TeamModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        [MaxLength(40)]
        public string Name { get; set; }

        [MaxLength(40)]
        public string Identifier { get; set; }

        [MaxLength(40)]
        public string Password { get; set; }

        [MaxLength(40)]
        public string Company { get; set; }

        [MaxLength(40)]
        public string Organisation_number { get; set; }

        public DateTime CreatedDate { get; set; }

        public int Max_number_users { get; set; }

        public string Administrator_id { get; set; }

        public bool Active { get; set; }
    }
}
