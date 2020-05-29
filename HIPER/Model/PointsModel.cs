using System;
using SQLite;

namespace HIPER.Model
{
    public class PointsModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string UserId { get; set; }

        public int Points { get; set; }

        public DateTime RegDate { get; set; }

        public PointsModel()
        {
        }
    }
}
