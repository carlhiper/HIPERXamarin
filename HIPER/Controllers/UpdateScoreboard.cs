using System;
using System.Collections.Generic;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;

namespace HIPER.Controllers
{
    public static class UpdateScoreboard
    {

        public static async void checkDeadlines()
        {
            try { 
                var today = DateTime.Now.Date;
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && !g.Closed).ToListAsync();

                foreach(var goal in goals)
                {
                    if ((goal.Deadline < today) && (goal.RepeatType==0))
                    {
                        goal.Closed = true;
                        goal.ClosedDate = DateTime.Now;
                        await App.client.GetTable<GoalModel>().UpdateAsync(goal);
                    }
                }
            }catch(Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "UpdateScoreboard", "checkDeadlines" }};
                Crashes.TrackError(ex, properties);
            }
        }


        public static async void checkRepeatGoals()
        {
            try
            {
                var today = DateTime.Now.Date;
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && !g.Closed).ToListAsync();

                foreach (var goal in goals)
                {
                    if ((goal.RepeatType == 1))
                    {
                        if (today > goal.Deadline)
                        {
                            goal.Closed = true;
                            if (goal.RecurrentId == null)
                            {
                                goal.RecurrentId = goal.Id;
                            }
  
                            await App.client.GetTable<GoalModel>().UpdateAsync(goal);

                            GoalModel newGoal = new GoalModel()
                            {
                                Title = goal.Title,
                                Description = goal.Description,
                                TargetValue = goal.TargetValue,
                                PrivateGoal = goal.PrivateGoal,
                                UserId = App.loggedInUser.Id,
                                TeamId = App.loggedInUser.TeamId,
                                CurrentValue = "0",
                                ClosedDate = DateTime.MaxValue,
                                LastUpdatedDate = DateTime.Now,
                                RecurrentId = goal.RecurrentId,
                                TargetType = goal.TargetType,
                                GoalType = goal.GoalType,
                                RepeatType = goal.RepeatType,
                                WeeklyOrMonthly = goal.WeeklyOrMonthly,
                                RepeatWeekly = goal.RepeatWeekly,
                                RepeatMonthly = goal.RepeatMonthly,
                                StepByStepAmount = goal.StepByStepAmount,
                                ChallengeId = goal.ChallengeId,
                                GoalAccepted = goal.GoalAccepted,
                                Checkbox1Comment = goal.Checkbox1Comment,
                                Checkbox2Comment = goal.Checkbox2Comment,
                                Checkbox3Comment = goal.Checkbox3Comment,
                                Checkbox4Comment = goal.Checkbox4Comment,
                                Checkbox5Comment = goal.Checkbox5Comment,
                                Checkbox6Comment = goal.Checkbox6Comment,
                                Checkbox7Comment = goal.Checkbox7Comment,
                                Checkbox8Comment = goal.Checkbox8Comment,
                                Checkbox9Comment = goal.Checkbox9Comment,
                            };
                            DateTime newDeadline = goal.Deadline;

                            while (today > newDeadline)
                            {
                         
                                if (goal.WeeklyOrMonthly == 0)  //Weekly
                                {
                                    newDeadline = newDeadline.AddDays(7);
                                    newGoal.Deadline = newDeadline;
                                    newGoal.CreatedDate = newDeadline.AddDays(-6);
                                }
                                else
                                {                          //Monthly
                                    newDeadline = newDeadline.AddMonths(1);
                                    newGoal.Deadline = newDeadline;
                                    newGoal.CreatedDate = (newDeadline.AddMonths(-1)).AddDays(1);
                                }
                                if (today > newDeadline)
                                {
                                    newGoal.Closed = true;
                                    newGoal.ClosedDate = newGoal.CreatedDate;
                                }

                                await App.client.GetTable<GoalModel>().InsertAsync(newGoal);
                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "UpdateScoreboard", "checkRepeatGoals" }};
                Crashes.TrackError(ex, properties);

            }
        }
    }
}
