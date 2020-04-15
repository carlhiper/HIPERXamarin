using System;
using SQLite;

namespace HIPER.Model
{
    public class ChallengeModel
    {
        [PrimaryKey]
        public string Id { get; set; }


        public string UserId { get; set; }


        public string GoalId { get; set; }

    }
}
