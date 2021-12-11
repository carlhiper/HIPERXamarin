using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Microcharts;
using Microsoft.AppCenter.Crashes;
using SkiaSharp;
using Xamarin.Forms;

namespace HIPER
{

    public partial class StatisticsPage : ContentPage
    {
        int recurrentGoalStartIndex = 0;
        private readonly List<ChartEntry> goalsCompletedEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> goalsCompletionEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> recurrentGoalEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> recurrentGoalEntriesAck = new List<ChartEntry>();
        private readonly List<ChartEntry> teamComparisonEntries = new List<ChartEntry>();
        List<GoalModel> recurrentGoals = new List<GoalModel>();
        List<UserModel> users = new List<UserModel>();
        Dictionary<string, string> recGoals = new Dictionary<string, string>();
        List<string> pickerList = new List<string>();
        List<GoalModel> goals;
        UserModel selectedUser;
        TeamModel selectedTeam;
        int selectedMonth;
        string teamName;

        private struct Teammember
        {
            public string FirstName;
            public string LastName;
            public int CompletedGoals;
            public List<GoalModel> Goals;
        }

        List<Teammember> Teammembers = new List<Teammember>();

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
                try
                {
                    header.Text = selectedTeam.Name.ToUpper() + " PERFORMANCE";
                    goals = new List<GoalModel>();

                    var teammembers = await App.client.GetTable<TeamsModel>().Where(t => t.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    List<UserModel> users = new List<UserModel>();
                    if (teammembers.Count > 0)
                    {
                        foreach (var member in teammembers)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == member.UserId).ToListAsync()).FirstOrDefault();
                            users.Add(user);
                        }
                    }

                    //users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == selectedTeam.Id).ToListAsync();
                    if (users != null)
                    {
                        foreach (UserModel u in users)
                        {
                            List<GoalModel> temp_goals = new List<GoalModel>();
                            List<GoalModel> gs = await App.client.GetTable<GoalModel>().Where(g => g.UserId == u.Id).Where(g2 => g2.TeamId == App.loggedInUser.TeamId).OrderByDescending(g => g.CreatedDate).Take(500).ToListAsync();
                            if (gs != null)
                            {
                                foreach (GoalModel g_s in gs)
                                {
                                    goals.Add(g_s);
                                    temp_goals.Add(g_s);
                                }
                            }
                            Teammember t = new Teammember();
                            t.FirstName = u.FirstName;
                            t.LastName = u.LastName;
                            t.Goals = temp_goals;
                            Teammembers.Add(t);
                        }
                    }

                }
                catch (Exception ex)
                {
                    var properties = new Dictionary<string, string> {
                    { "Statistics page", "OnAppearing" }};
                    Crashes.TrackError(ex, properties);

                }
            }
            else
            {
                // participantsCollectionView.IsVisible = false;
                goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == selectedUser.Id).Where(g2 => g2.TeamId == App.loggedInUser.TeamId).Take(500).ToListAsync();
                teamName = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault().Name;
                if (teamName != null)
                    header.Text = "PERFORMANCE IN " + teamName.ToUpper();
            }

            UpdateStats();

            PopulateStandardCharts();

            if (selectedTeam != null)
            {
                PopulateTeamComparisonChart();
            }
            else
            {
                chartViewTeamComparison.IsVisible = false;
                labelTeamComparison.IsVisible = false;
            }

            // Implementation of recurrent goals chart

            GetRecurrentGoals();
        }

  

        private void GetRecurrentGoals()
        {
            if (selectedTeam == null)
            {
                foreach (var g in goals)
                {
                    if (g.RecurrentId != null)
                    {
                        recurrentGoals.Add(g);
                    }
                }
            }
            else
            {
                foreach (var g in goals)
                {
                    if (g.RecurrentId != null && g.GoalType > 0)
                    {
                        recurrentGoals.Add(g);
                    }
                }
            }

            var sortedRecurrentGoals = recurrentGoals.Where(r => r.Deadline >= DateTime.Now.Date).OrderByDescending(r => r.CreatedDate);

            //Get each unique recurrent goal to create list for picker
            foreach (var g in sortedRecurrentGoals)
            {
                if (!recGoals.ContainsKey(g.Title))
                {
                    recGoals.Add(g.Title, g.RecurrentId);
                }
            }

            if (recGoals.Count > 0)
            {
                var first = recGoals.First();

                RecurrentGoalHandler(first.Value);

                foreach (var item in recGoals)
                {
                    pickerList.Add(item.Key);
                }
                pickRecurrentGoal.ItemsSource = pickerList;
                pickRecurrentGoal.Title = pickerList[0];
                pickRecurrentGoal.SelectedIndex = 0;
            }
            else
            {
                recurrentGoalsFrame.IsVisible = false;
            }
        }

        private void RecurrentGoalHandler(string recurrentId)
        {
            List<GoalModel> ReccurrentGoalList = new List<GoalModel>();

            if (selectedTeam == null)
            {

                ReccurrentGoalList = recurrentGoals.Where(g => g.RecurrentId == recurrentId).ToList();
            }
            else
            {
                //Get all goal in recurrentGoals, that have the same challengeId
                var temp_goal = recurrentGoals.Where(t_g => t_g.RecurrentId == recurrentId).FirstOrDefault();
                ReccurrentGoalList = recurrentGoals.Where(t_g => t_g.ChallengeId == temp_goal.ChallengeId).ToList();
            }

            ReccurrentGoalList.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));

            if (selectedTeam == null)
            {
                PopulateRecurrentGoalChart(ReccurrentGoalList);
            }
            else
            {
                List<GoalModel> FinalAckRecGoals = new List<GoalModel>();
                List<GoalModel> AckRecGoals = new List<GoalModel>();
                List<DateTime> UniqueDates = new List<DateTime>();


                foreach (var g in ReccurrentGoalList)
                {
                    if (!UniqueDates.Contains(g.Deadline))
                    {
                        UniqueDates.Add(g.Deadline);
                        AckRecGoals.Add(g);
                    }
                }

                foreach (var g in AckRecGoals)
                {
                    int sum = 0;
                    foreach (var goal in ReccurrentGoalList)
                    {
                        if (g.Deadline == goal.Deadline)
                            sum += int.Parse(goal.CurrentValue);
                    }
                    //     g.CurrentValue = sum.ToString();
                    GoalModel goalboal = new GoalModel()
                    {
                        CurrentValue = sum.ToString(),
                    };
                    FinalAckRecGoals.Add(goalboal);
                }
                PopulateRecurrentGoalChart(FinalAckRecGoals);
            }
        }


        private void PopulateRecurrentGoalChart(List<GoalModel> recGoal)
        {

            recurrentGoalEntries.Clear();
            recurrentGoalEntriesAck.Clear();
            List<string> participants = new List<string>();

            var count = recGoal.Count;
            count += recurrentGoalStartIndex;
            if (count > 15)
            {
                count = 15;
                recurrentGoalStartIndex--;
            }

            if (count == 0)
            {
                recurrentGoalStartIndex++;
            }

            //if (selectedTeam != null)
            //{
            //    foreach (var g in recGoal)
            //    {
            //        var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == g.UserId).ToListAsync()).FirstOrDefault();
            //        participants.Add(user.FirstName + " " + user.LastName);
            //    }
            //    participantsCollectionView.ItemsSource = participants;
            //}

            int ackValue = 0;

            if (recGoal[0].WeeklyOrMonthly == 1)
            {
                for (int i = (count - 1); i >= 0; i--)
                {
                    recurrentGoalEntries.Add(new ChartEntry(GoalStats.GetRecurrentGoalResult(recGoal, i)) { Label = GoalStats.GetShortMonthName(i), ValueLabel = (GoalStats.GetRecurrentGoalResult(recGoal, i)).ToString("D"), Color = SKColor.Parse("#FF7562") });
                    ackValue += GoalStats.GetRecurrentGoalResult(recGoal, i);
                    recurrentGoalEntriesAck.Add(new ChartEntry(ackValue) { Label = GoalStats.GetShortMonthName(i), ValueLabel = ackValue.ToString("D"), Color = SKColor.Parse("#FF7562") });
                }
            }
            else
            {
                for (int i = (count - 1); i >= 0; i--)
                {
                    recurrentGoalEntries.Add(new ChartEntry(GoalStats.GetRecurrentGoalResult(recGoal, i)) { Label = "Week " + GoalStats.GetWeekNumber(i), ValueLabel = (GoalStats.GetRecurrentGoalResult(recGoal, i)).ToString("D"), Color = SKColor.Parse("#FF7562") });
                    ackValue += GoalStats.GetRecurrentGoalResult(recGoal, i);
                    recurrentGoalEntriesAck.Add(new ChartEntry(ackValue) { Label = "Week " + GoalStats.GetWeekNumber(i), ValueLabel = ackValue.ToString("D"), Color = SKColor.Parse("#FF7562") });
                }
            }
            if (count > 0)
            {
                chartViewRecurrentGoals.Chart = new BarChart() { Entries = recurrentGoalEntries, LabelTextSize = 24 };
                chartViewRecurrentGoalsAck.Chart = new LineChart() { Entries = recurrentGoalEntriesAck, LabelTextSize = 24 };
            }
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
                        createdChallenges += GoalStats.GetNbrCreatedChallenges(goal, selectedMonth, challenge.OwnerId);
                    }
                }
                challengesCreatedLabel.Text = createdChallenges.ToString();
            }
        }

        private void PopulateTeamComparisonChart()
        {
            teamComparisonEntries.Clear();
            int index = 0;
            if (Teammembers.Count > 0)
            {
                foreach (Teammember member in Teammembers)
                {
                    int g_nbr = GoalStats.GetNbrCompletedGoals(member.Goals, selectedMonth);
                    teamComparisonEntries.Add(new ChartEntry(g_nbr) { Label = member.FirstName + " " + member.LastName, ValueLabel = g_nbr.ToString("D"), Color = SKColor.Parse(App.donutChartColors[index]) });
                    index++;
                    if (index >= App.donutChartColors.Count)
                        index = 0;
                }
                chartViewTeamComparison.Chart = new DonutChart() { Entries = teamComparisonEntries, LabelTextSize = 24 };
            }
        }



        void PopulateStandardCharts()
        {
            goalsCompletedEntries.Clear();
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 4)) { Label = GoalStats.GetShortMonthName(selectedMonth + 4), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 4).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 3)) { Label = GoalStats.GetShortMonthName(selectedMonth + 3), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 3).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 2)) { Label = GoalStats.GetShortMonthName(selectedMonth + 2), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 2).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 1)) { Label = GoalStats.GetShortMonthName(selectedMonth + 1), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth + 1).ToString(), Color = SKColor.Parse("#FF7562") });
            goalsCompletedEntries.Add(new ChartEntry(GoalStats.GetNbrCompletedGoals(goals, selectedMonth)) { Label = GoalStats.GetShortMonthName(selectedMonth), ValueLabel = GoalStats.GetNbrCompletedGoals(goals, selectedMonth).ToString(), Color = SKColor.Parse("#FF7562") });

            chartViewCompleted.Chart = new LineChart() { Entries = goalsCompletedEntries, LabelTextSize = 24, ValueLabelOrientation = Orientation.Horizontal };

            goalsCompletionEntries.Clear();
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 4) * 100) { Label = GoalStats.GetShortMonthName(selectedMonth + 4), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 4)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 3) * 100) { Label = GoalStats.GetShortMonthName(selectedMonth + 3), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 3)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 2) * 100) { Label = GoalStats.GetShortMonthName(selectedMonth + 2), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 2)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 1) * 100) { Label = GoalStats.GetShortMonthName(selectedMonth + 1), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth + 1)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });
            goalsCompletionEntries.Add(new ChartEntry(GoalStats.GetGoalCompletionRatio(goals, selectedMonth) * 100) { Label = GoalStats.GetShortMonthName(selectedMonth), ValueLabel = (GoalStats.GetGoalCompletionRatio(goals, selectedMonth)).ToString("F1") + "%", Color = SKColor.Parse("#FF7562") });

            chartViewCompletionRatio.Chart = new LineChart() { Entries = goalsCompletionEntries, LabelTextSize = 24, ValueLabelOrientation = Orientation.Horizontal };
        }




        void prevButton_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedMonth++;
            UpdateStats();
            PopulateStandardCharts();
            PopulateTeamComparisonChart();
        }

        void nextButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (selectedMonth > 0)
            {
                selectedMonth--;
                UpdateStats();
                PopulateStandardCharts();
                PopulateTeamComparisonChart();
            }
        }

        void pickRecurrentGoal_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            var pick = pickRecurrentGoal.SelectedIndex;
            recurrentGoalStartIndex = 0;
            if (pick < recGoals.Count)
            {
                var id = recGoals.ElementAt(pick).Value;
                RecurrentGoalHandler(id);
            }
        }

        void buttonLeft_Clicked(System.Object sender, System.EventArgs e)
        {
            var pick = pickRecurrentGoal.SelectedIndex;
            if (pick >= 0)
            {
                recurrentGoalStartIndex++;
                var id = recGoals.ElementAt(pick).Value;
                RecurrentGoalHandler(id);
            }
        }
        void buttonRight_Clicked(System.Object sender, System.EventArgs e)
        {
            var pick = pickRecurrentGoal.SelectedIndex;
            if (pick >= 0)
            {
                recurrentGoalStartIndex--;
                var id = recGoals.ElementAt(pick).Value;
                RecurrentGoalHandler(id);
            }
        }
    }
}
