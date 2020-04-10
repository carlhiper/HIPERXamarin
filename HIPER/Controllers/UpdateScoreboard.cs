﻿using System;
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
                        goal.ClosedDate = DateTime.Now;
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

                        if (today > goal.Deadline)
                        {
                            goal.Closed = true;
                            goal.ClosedDate = DateTime.Now;
                            
                            await App.client.GetTable<GoalModel>().UpdateAsync(goal);

                            GoalModel newGoal = new GoalModel()
                            {
                                Title = goal.Title,
                                Description = goal.Description,
                                TargetValue = goal.TargetValue,
                                PrivateGoal = goal.PrivateGoal,
                                UserId = App.loggedInUser.Id,
                                CurrentValue = "0",
                                ClosedDate = DateTime.MaxValue,
                                CreatedDate = goal.Deadline.AddDays(1),
                                Progress = 0,
                                TargetType = goal.TargetType,
                                RepeatType = goal.RepeatType,
                                WeeklyOrMonthly = goal.WeeklyOrMonthly,
                                RepeatWeekly = goal.RepeatWeekly,
                                RepeatMonthly = goal.RepeatMonthly,
                                SteByStepAmount = goal.SteByStepAmount,
                                LastUpdatedDate = DateTime.Now,
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

                            if (goal.WeeklyOrMonthly == 0)  //Weekly
                            {
                                DateTime newDeadline = goal.Deadline;
                                while (newDeadline < DateTime.Now)
                                {
                                    newDeadline.AddDays(7);
                                }
                                newGoal.Deadline = newDeadline;
                            }
                            else
                            {                          //Monthly
                                DateTime newDeadline = goal.Deadline;
                                while (newDeadline < DateTime.Now)
                                {
                                    newDeadline.AddMonths(1);
                                    // newDeadline.AddDays(DateTime.DaysInMonth(DateTime.Today.Year, DateTime.Today.Month));
                                }
                                newGoal.Deadline = newDeadline;
                            }
          
                            await App.client.GetTable<GoalModel>().InsertAsync(newGoal);
                        }
                    }
                }
            }
            catch(NullReferenceException nre)
            {
            }
            catch(Exception ex)
            {


            }
        }
    }
}
