using System;
using SQLite;

namespace HIPER.Model
{
    public class GoalModel
    {
        [PrimaryKey]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string TeamId { get; set; }

        [MaxLength(40)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        public DateTime Deadline { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ClosedDate { get; set; }

        public string TargetValue { get; set; }

        public string CurrentValue { get; set; }

        public bool PrivateGoal { get; set; }

        public float Progress {
            get {
                    var isCurrentValueNumerical = float.TryParse(CurrentValue, out float n);
                    var isTargetValueNumerical = float.TryParse(TargetValue, out float d);
                    if (isCurrentValueNumerical && isTargetValueNumerical && (d > 0.0f))
                    { return n / d; }
                    else { return 0.0f; }
                }
            set {; }
        }

        public bool Completed { get; set; }

        public bool Closed { get; set; }

        public int Hipes { get; set; }

        public GoalModel()
        {

        }
    }
}
