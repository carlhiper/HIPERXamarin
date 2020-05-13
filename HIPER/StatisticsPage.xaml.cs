using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Microcharts;
using SkiaSharp;
using Xamarin.Forms;

namespace HIPER
{
 
    public partial class StatisticsPage : ContentPage
    {
        private readonly List<ChartEntry> goalsCompletedEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> goalsCompletionEntries = new List<ChartEntry>();
        List<GoalModel> goals;
        UserModel selectedUser;
        TeamModel selectedTeam;
        int selectedMonth;

        public StatisticsPage(UserModel user)
        {
            this.selectedUser = user;
            InitializeComponent();
            selectedMonth = 0;
        }

        public StatisticsPage(TeamModel team)
        {
            this.selectedTeam = team;
            InitializeComponent();
            selectedMonth = 0;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (selectedTeam != null)
            {
                goals = new List<GoalModel>();
                List<UserModel> users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == selectedTeam.Id).ToListAsync();
                if (users != null)
                {
                    foreach (UserModel u in users)
                    {
                        List<GoalModel> gs = await App.client.GetTable<GoalModel>().Where(g => g.UserId == u.Id).ToListAsync();
                        if (gs != null)
                        {
                            foreach (GoalModel g_s in gs)
                            {
                                goals.Add(g_s);
                            }
                        }
                    }
                }
            }
            else
            {
                goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == selectedUser.Id).Take(150).ToListAsync();
            }


            UpdateStats();

            PopulateCharts();
        }


        void PopulateCharts()
        {
            goalsCompletedEntries.Clear();
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth+4)) { Label = GoalStats.GetShortMonthName(selectedMonth+4), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 4).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth+3)) { Label = GoalStats.GetShortMonthName(selectedMonth+3), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 3).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth+2)) { Label = GoalStats.GetShortMonthName(selectedMonth+2), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 2).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth+1)) { Label = GoalStats.GetShortMonthName(selectedMonth+1), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 1).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth)) { Label = GoalStats.GetShortMonthName(selectedMonth), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth).ToString(), Color = SKColor.Parse("#FF7562") });

            chartViewCompleted.Chart = new LineChart() { Entries = goalsCompletedEntries };

            goalsCompletionEntries.Clear();
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 4)*100) { Label = GoalStats.GetShortMonthName(selectedMonth + 4), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 4)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 3)*100) { Label = GoalStats.GetShortMonthName(selectedMonth + 3), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 3)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 2)*100) { Label = GoalStats.GetShortMonthName(selectedMonth + 2), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 2)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 1)*100) { Label = GoalStats.GetShortMonthName(selectedMonth + 1), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 1)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth)*100) { Label = GoalStats.GetShortMonthName(selectedMonth), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });

            chartViewCompletionRatio.Chart = new LineChart() { Entries = goalsCompletionEntries };

        }

        private async void UpdateStats()
        {
            if (goals != null)
            {
                monthLabel.Text = GoalStats.GetMonthName(selectedMonth);
                completionRatioLabel.Text = GoalStats.GetGoalCompletionRatio(goals, selectedMonth).ToString("F0") + "%";
                goalsCreatedLabel.Text = GoalStats.GetNbrCreatedGoals(goals, selectedMonth).ToString();
                goalsClosedLabel.Text = GoalStats.GetNbrClosedGoals(goals, selectedMonth).ToString();
                goalsCompletedLabel.Text = GoalStats.GetNbrCompletedGoals(goals, selectedMonth).ToString();

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
                challengesCreatedLabel.Text = GoalStats.GetNbrCreatedChallenges(goals, selectedMonth).ToString();

            }
        }

        void prevButton_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedMonth++;
            UpdateStats();
            PopulateCharts();

        }

        void nextButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (selectedMonth > 0)
            {
                selectedMonth--;
                UpdateStats();
                PopulateCharts();
            }
        }
    }
}
