using System;
using SQLite;

namespace HIPER.Model
{
    public class ChallengeModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string OwnerId { get; set; }

    }
}
