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

        public DateTime LastUpdatedDate { get; set; }

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

        public int PerformanceIndicator {
            get
            {
                TimeSpan totalOffset = Deadline - CreatedDate;
                float totalHours = totalOffset.Days * 24 + totalOffset.Hours;

                TimeSpan dividerOffset;

                if (Completed || Closed)
                {
                    dividerOffset = ClosedDate - CreatedDate;
                }
                else
                {
                    dividerOffset = DateTime.Now - CreatedDate;
                }
                float todayHours = dividerOffset.Days * 24 + dividerOffset.Hours;

                float timePassedDividend = todayHours / totalHours;
                if (timePassedDividend == 0)
                    return 0;

                float performanceIndicator = Progress / timePassedDividend;

                if (performanceIndicator > 2)
                {
                    return 5;
                }else if (performanceIndicator <= 2 && performanceIndicator > 1.25)
                {
                    return 4;
                }else if (performanceIndicator <= 1.25 && performanceIndicator > 0.75)
                {
                    return 3;
                }else if (performanceIndicator <= 0.75 && performanceIndicator > 0.5)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }

            set {; }



        }

        public bool Completed { get; set; }

        public bool Closed { get; set; }

        public int Hipes { get; set; }

        public int TargetType { get; set; }
        // 0-Aim high, 1-step by step

        public int SteByStepAmount { get; set; }

        public int RepeatType { get; set; }
        // 0-Single, 1-Recurrent

        public int WeeklyOrMonthly { get; set; }
        // 0-Weekly, 1-Monthly

        public int RepeatWeekly { get; set; }
        // 0-Monday ....

        public int RepeatMonthly { get; set; }
        // 1,2,3,4

        public bool Checkbox1 { get; set; }
        public bool Checkbox2 { get; set; }
        public bool Checkbox3 { get; set; }
        public bool Checkbox4 { get; set; }
        public bool Checkbox5 { get; set; }
        public bool Checkbox6 { get; set; }
        public bool Checkbox7 { get; set; }
        public bool Checkbox8 { get; set; }
        public bool Checkbox9 { get; set; }
        public string Checkbox1Comment { get; set; }
        public string Checkbox2Comment { get; set; }
        public string Checkbox3Comment { get; set; }
        public string Checkbox4Comment { get; set; }
        public string Checkbox5Comment { get; set; }
        public string Checkbox6Comment { get; set; }
        public string Checkbox7Comment { get; set; }
        public string Checkbox8Comment { get; set; }
        public string Checkbox9Comment { get; set; }

        public GoalModel()
        {

        }
    }
}
