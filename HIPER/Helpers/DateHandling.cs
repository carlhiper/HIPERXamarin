using System;
namespace HIPER.Helpers
{
    public static class DateHandling
    {
        public static DateTime GetStartDate(bool checkboxesForWeekly, bool checkboxesForMonthly, int weekday, int dayOfMonth )
        {

            var createdDate = DateTime.Today;
            DateTime startDate = createdDate;
            if (checkboxesForWeekly && weekday >= 0)
            {
                var diff = weekday - (int)DateTime.Today.DayOfWeek + 2;
                if (diff > 0)
                {
                    diff -= 7;
                }
                startDate = createdDate.AddDays((double)diff);
            }
            else if (checkboxesForMonthly && dayOfMonth >= 0)
            {
                var diff = dayOfMonth + 2 - DateTime.Today.Day;
                if (diff > 0)
                {
                    diff = diff - DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month - 1);
                }
                startDate = createdDate.AddDays((double)diff);
            }

            return startDate;
        }

        public static DateTime GetDeadlineDateForRepeatingGoals(bool checkboxesForWeekly, bool checkboxesForMonthly, DateTime startDate)
        {
            DateTime deadLine;

            if (checkboxesForWeekly)      // weekly
            {
                deadLine = startDate.AddDays(6);
            }
            else if (checkboxesForMonthly)    //monthly
            {
                if (startDate.Month < DateTime.Now.Month) {
                    deadLine = startDate.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month - 1) - 1);
                }
                else
                {
                    deadLine = startDate.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month) - 1);
                }
            }
            else
            {
                deadLine = DateTime.MaxValue;
            }
            return deadLine;
        }
    }
}
