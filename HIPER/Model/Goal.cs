using System;
using SQLite;

namespace HIPER.Model
{
    public class Goal
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        public double userId { get; set; }

        public double teamId { get; set; }

        [MaxLength(20)]
        public string title { get; set; }

        [MaxLength(200)]
        public string description { get; set; }

        public DateTime deadline { get; set; }

        public DateTime createdDate { get; set; }

        public DateTime closedDate { get; set; }

        public string targetValue { get; set; }

        public string currentValue { get; set; }

        public bool privateGoal { get; set; }

        public float progress {
            get {
                    var isCurrentValueNumerical = float.TryParse(currentValue, out float n);
                    var isTargetValueNumerical = float.TryParse(targetValue, out float d);
                    if (isCurrentValueNumerical && isTargetValueNumerical && (d > 0.0f))
                    { return n / d; }
                    else { return 0.0f; }
                }
            set {; }
        }

        public bool completed { get; set; }

        public Goal()
        {

        }
    }
}
