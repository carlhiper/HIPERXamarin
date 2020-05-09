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
        List<GoalModel> goals;
        UserModel selectedUser;
        int selectedMonth;
        public StatisticsPage(UserModel user)
        {
            this.selectedUser = user;
            InitializeComponent();
            selectedMonth = 0;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == selectedUser.Id).ToListAsync();

            UpdateStats();
 
        }

        private async void UpdateStats()
        {
            if (goals != null)
            {
                monthLabel.Text = GoalStats.GetMonthName(selectedMonth);
                completionRatioLabel.Text = GoalStats.GetGoalCompletionRatio(goals, selectedMonth).ToString("F0") + "%";
                goalsCreatedLabel.Text = GoalStats.GetCreatedGoals(goals, selectedMonth).ToString();
                goalsClosedLabel.Text = GoalStats.GetClosedGoals(goals, selectedMonth).ToString();
                goalsCompletedLabel.Text = GoalStats.GetCompletedGoals(goals, selectedMonth).ToString();

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

        void prevButton_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedMonth++;
            UpdateStats();

        }

        void nextButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (selectedMonth > 0)
            {
                selectedMonth--;
                UpdateStats();
            }
        }
    }
}
