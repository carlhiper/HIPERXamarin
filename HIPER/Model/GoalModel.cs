using System;
using System.Runtime.Serialization;
using SQLite;

namespace HIPER.Model
{
    [DataContract]
    public class GoalModel
    {
        [PrimaryKey]
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public string TeamId { get; set; }
        [DataMember]
        public string ChallengeId { get; set; }
        [DataMember]
        public string RecurrentId { get; set; }
        [DataMember]
        public bool GoalAccepted { get; set; }
        [DataMember]
        [MaxLength(40)]
        public string Title { get; set; }
        [DataMember]
        [MaxLength(255)]
        public string Description { get; set; }
        [DataMember]
        public DateTime Deadline { get; set; }
        [DataMember]
        public DateTime CreatedDate { get; set; }
        [DataMember]
        public DateTime ClosedDate { get; set; }
        [DataMember]
        public DateTime LastUpdatedDate { get; set; }
        [DataMember]
        public string TargetValue { get; set; }
        [DataMember]
        public string CurrentValue { get; set; }
        [DataMember]
        public bool PrivateGoal { get; set; }
        [DataMember]
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
        [DataMember]
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
        [DataMember]
        public bool Completed { get; set; }
        [DataMember]
        public bool Closed { get; set; }
        [DataMember]
        public int Hipes { get; set; }

        // 0-standard, 1-Challenge, 2-Competition
        [DataMember]
        public int GoalType { get; set; }
        [DataMember]
        public int TargetType { get; set; }
        // 0-Aim high, 1-step by step
        [DataMember]
        public int StepByStepAmount { get; set; }
        [DataMember]
        public int RepeatType { get; set; }
        // 0-Single, 1-Recurrent
        [DataMember]
        public int WeeklyOrMonthly { get; set; }
        // 0-Weekly, 1-Monthly
        [DataMember]
        public int RepeatWeekly { get; set; }
        // 0-Monday ....
        [DataMember]
        public int RepeatMonthly { get; set; }
        // 1,2,3,4
        [DataMember]
        public bool Checkbox1 { get; set; }
        [DataMember]
        public bool Checkbox2 { get; set; }
        [DataMember]
        public bool Checkbox3 { get; set; }
        [DataMember]
        public bool Checkbox4 { get; set; }
        [DataMember]
        public bool Checkbox5 { get; set; }
        [DataMember]
        public bool Checkbox6 { get; set; }
        [DataMember]
        public bool Checkbox7 { get; set; }
        [DataMember]
        public bool Checkbox8 { get; set; }
        [DataMember]
        public bool Checkbox9 { get; set; }
        [DataMember]
        public string Checkbox1Comment { get; set; }
        [DataMember]
        public string Checkbox2Comment { get; set; }
        [DataMember]
        public string Checkbox3Comment { get; set; }
        [DataMember]
        public string Checkbox4Comment { get; set; }
        [DataMember]
        public string Checkbox5Comment { get; set; }
        [DataMember]
        public string Checkbox6Comment { get; set; }
        [DataMember]
        public string Checkbox7Comment { get; set; }
        [DataMember]
        public string Checkbox8Comment { get; set; }
        [DataMember]
        public string Checkbox9Comment { get; set; }

    }
}
