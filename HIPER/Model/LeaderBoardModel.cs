using System;
namespace HIPER.Model
{
    public class LeaderBoardModel
    {
        public int Placing { get; set; }

        public string Name { get; set; }

        public float Progress { get; set; }

        public bool Completed { get; set; }

        public bool Accepted { get; set; }
    }
}

//CREATE TABLE ChallengeModel
//(
//	placing INT,
//    name NVARCHAR(256),
//	progress FLOAT,
//    completed BIT,
//    accepted BIT
//);