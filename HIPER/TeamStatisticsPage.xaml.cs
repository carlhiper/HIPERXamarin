using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class TeamStatisticsPage : ContentPage
    {
        TeamModel selectedTeam;

        public TeamStatisticsPage(TeamModel team)
        {
            this.selectedTeam = team;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            float completedGoalsRatio = 0;
            int goalsCreated = 0;
            int goalsCompleted = 0;
            int goalsClosed = 0;
            int challengesCreated = 0;
            List<UserModel> users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == selectedTeam.Id).ToListAsync();

            foreach(UserModel selectedUser in users)
            {
                List<GoalModel> goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == selectedUser.Id).ToListAsync();

                if (goals != null)
                {

                    completedGoalsRatio += GoalStats.GetGoalCompletionRatio(goals, 1)/((float)users.Count);
                    goalsCreated += GoalStats.GetCreatedGoals(goals, 1);
                    goalsClosed += GoalStats.GetClosedGoals(goals, 1);
                    goalsCompleted += GoalStats.GetCompletedGoals(goals, 1);

                    foreach (GoalModel goal in goals)
                    {
                        var challenge = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == goal.ChallengeId).ToListAsync()).FirstOrDefault();
                        if (challenge != null)
                        {
                            if (challenge.OwnerId == goal.UserId)
                            {
                                challengesCreated++;
                            }

                        }
                    }

                }
            }
            completionRatioLabel.Text = completedGoalsRatio.ToString("F0") + "%";
            goalsCreatedLabel.Text = goalsCreated.ToString();
            goalsClosedLabel.Text = goalsClosed.ToString();
            goalsCompletedLabel.Text = goalsCompleted.ToString();
            challengesCreatedLabel.Text = challengesCreated.ToString();

        }
    }
}
