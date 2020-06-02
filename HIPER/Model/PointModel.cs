using System;
using SQLite;

namespace HIPER.Model
{
    public class PointModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string UserId { get; set; }

        public int Points { get; set; }

        public DateTime RegDate { get; set; }

        public PointModel()
        {
        }
    }
}
