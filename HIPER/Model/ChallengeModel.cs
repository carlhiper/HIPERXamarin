using System;
using SQLite;

namespace HIPER.Model
{
    public class ChallengeModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string OwnerId { get; set; }

        public string InitialGoalId { get; set; }

        public DateTime CreatedDate { get; set; }


    }
}
