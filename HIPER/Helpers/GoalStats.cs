using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using HIPER.Model;

namespace HIPER.Helpers
{
    public static class GoalStats
    {
        public static float GetGoalCompletionRatio(List<GoalModel> goals, int months)
        {
            int completedGoals = GetNbrCompletedGoals(goals, months);
            int totalGoals = GetNbrClosedGoals(goals, months);

            if (totalGoals > 0)
                return ((float)completedGoals / (float)totalGoals) * 100;
            else
                return 0;
        }

        public static int GetNbrCompletedGoals(List<GoalModel> goals, int months)
        {
            int completedGoals = 0;
            var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfMonth = firstDayOfThisMonth.AddMonths(-months);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            if (goals != null)
            {
                foreach (GoalModel goal in goals)
                {
                    if (goal.ClosedDate.CompareTo(firstDayOfMonth) > 0 && goal.ClosedDate.CompareTo(lastDayOfMonth) <= 0)
                    {
                        if (goal.Completed)
                        {
                            completedGoals += 1;
                        }
                    }
                }
            }
            return completedGoals;
        }
        
        public static int GetNbrClosedGoals(List<GoalModel> goals, int months)
        {
            int closedGoals = 0;
            var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfMonth = firstDayOfThisMonth.AddMonths(-months);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            if (goals != null)
            {
                foreach (GoalModel goal in goals)
                {
                    if (goal.ClosedDate.CompareTo(firstDayOfMonth) > 0 && goal.ClosedDate.CompareTo(lastDayOfMonth) <= 0)
                    {
                        closedGoals += 1;
                    }
                }
            }
            return closedGoals;
        }

        public static int GetNbrCreatedGoals(List<GoalModel> goals, int months)
        {
            int createdGoals = 0;
            var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfMonth = firstDayOfThisMonth.AddMonths(-months);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            if (goals != null)
            {
                foreach (GoalModel goal in goals)
                {
                    if (goal.CreatedDate.CompareTo(firstDayOfMonth) > 0 && goal.CreatedDate.CompareTo(lastDayOfMonth) <= 0)
                    {
                        createdGoals += 1;
                    }
                }
            }
            return createdGoals;
        }

        public static int GetNbrCreatedChallenges(GoalModel goal, int months, string ownerId)
        {
            int createdChallenges = 0;
            var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfMonth = firstDayOfThisMonth.AddMonths(-months);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);


            if (goal.CreatedDate.CompareTo(firstDayOfMonth) > 0 && goal.CreatedDate.CompareTo(lastDayOfMonth) <= 0)
            {
                if (goal.GoalType == 1)
                {
                    if (ownerId == goal.UserId)
                        createdChallenges += 1;
                }
            }
            return createdChallenges;
        }


        public static string GetMonthName(int month)
        {
            int year = DateTime.Now.Year;
            int index = (DateTime.Now.Month - 1 - month);
            while (index < 0)
            {
                index += 12;
                year--;
            }
            return App.months[index] + " " + year.ToString();     
        }
        public static string GetShortMonthName(int month)
        {
            int year = DateTime.Now.Year;
            int index = (DateTime.Now.Month - 1 - month);
            while (index < 0)
            {
                index += 12;
                year--;
            }
            return App.months_short[index] + " " + year.ToString();
        }

        public static string GetWeekNumber(int weeks)
        {
            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = new CultureInfo("en-US");
            Calendar myCal = myCI.Calendar;
            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            var wantedWeekNbr = DateTime.Now.AddDays(-weeks * 7);

            int week = myCal.GetWeekOfYear(wantedWeekNbr, myCWR, myFirstDOW);
        
            return week.ToString();
        }

        public static int GetRecurrentGoalResult(List<GoalModel> goals, int timeBack)
        {
            if (goals != null && timeBack < goals.Count)
            {
                return int.Parse(goals[timeBack].CurrentValue);
            }else
                return 0;
        }
    }
}
