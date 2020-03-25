using System;
using HIPER.Model;

namespace HIPER.Controllers
{
    public static class UpdateScoreboard
    {

        public static async void checkDeadlines()
        {
            try { 
                var today = DateTime.Now.Date;
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id).ToListAsync();

                foreach(var goal in goals)
                {
                    if ((goal.Deadline < today) && (goal.RepeatType==0) && !goal.Closed)
                    {
                        goal.Closed = true;
                        await App.client.GetTable<GoalModel>().UpdateAsync(goal);
                    }
                }
            }catch(Exception ex)
            {

            }
        }


        public static async void checkRepeatGoals()
        {
            try
            {
                var today = DateTime.Now.Date;
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id).ToListAsync();

                foreach (var goal in goals)
                {
                    if ((goal.RepeatType == 1) && !goal.Closed)
                    {
                        if (goal.WeeklyOrMonthly == 0)  //Weekly
                        {
                            //var repeatday =
                            var cDate = goal.CreatedDate.AddDays(7);
                            if (today > cDate)
                            {


                            }

                            //goal.RepeatWeekly;
                            //goal.CreatedDate;
                        }
                        else
                        {                          //Monthly



                        }

                        goal.Closed = true;
                        await App.client.GetTable<GoalModel>().UpdateAsync(goal);
                    }
                }
            }
            catch(Exception ex)
            {


            }
        }
    }
}
