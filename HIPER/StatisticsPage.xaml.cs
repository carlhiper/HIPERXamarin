using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class StatisticsPage : ContentPage
    {
        UserModel selectedUser;
        public StatisticsPage(UserModel user)
        {
            this.selectedUser = user;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            List<GoalModel> goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == selectedUser.Id).ToListAsync();

            if (goals != null)
            {
                
                completionRatioLabel.Text = GoalStats.GetGoalCompletionRatio(goals, 1).ToString("F0") + "%";
                goalsCreatedLabel.Text = GoalStats.GetCreatedGoals(goals, 1).ToString();
                goalsClosedLabel.Text = GoalStats.GetClosedGoals(goals, 1).ToString();
                goalsCompletedLabel.Text = GoalStats.GetCompletedGoals(goals, 1).ToString();

                int createdChallenges = 0;
                foreach (GoalModel goal in goals)
                {
                    var challenge = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == goal.ChallengeId).ToListAsync()).FirstOrDefault();
                    if (challenge != null)
                    {
                        if (challenge.OwnerId == goal.UserId)
                        {
                            createdChallenges++;
                        }

                    }
                }
                challengesCreatedLabel.Text = createdChallenges.ToString();

            }
        }
    }
}
