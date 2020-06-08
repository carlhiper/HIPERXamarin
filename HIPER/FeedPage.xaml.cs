using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using HIPER.Helpers;
using Microcharts;
using SkiaSharp;
using System.Linq;

namespace HIPER
{
    public partial class FeedPage : ContentPage
    {
        bool PointsChartYearly = false;
        //List<GoalModel> activeGoals = new List<GoalModel>();
        private readonly List<ChartEntry> progressEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> pointsEntries = new List<ChartEntry>();

        // for drag to refresh - not used
        private bool _isRefreshing;
        private Command _refreshViewCommand;

        public FeedPage()
        {
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();

            createFeedList();
            PopulateProgressChart();
            PopulateTeamPointsChart();
            GetAlerts();
        }

        private async void PopulateProgressChart()
        {
            progressEntries.Clear();
            int index = 0;
            var activeGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderByDescending(g => g.CreatedDate).ToListAsync();
            foreach (var goal in activeGoals)
            {
                progressEntries.Add(new ChartEntry((int)(goal.Progress * 100)) { Label = goal.Title, ValueLabel = ((int)(goal.Progress * 100)).ToString("D") + "%", Color = SKColor.Parse(App.donutChartColors[index]) });
                index++;
                if (index >= App.donutChartColors.Count)
                    index = 0;
            }
            chartViewProgress.Chart = new RadialGaugeChart() { Entries = progressEntries, MaxValue = 100 };
        }

        private async void GetAlerts()
        {
            var alertGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderBy(g => g.Deadline).Take(3).ToListAsync();
        
            if (alertGoals.Count == 3)
            {
                Label1.Text = "-> " + alertGoals[0].Title + " ends " + GetDifference(alertGoals[0].Deadline.Date);
                Label2.Text = "-> " + alertGoals[1].Title + " ends " + GetDifference(alertGoals[1].Deadline.Date);
                Label3.Text = "-> " + alertGoals[2].Title + " ends " + GetDifference(alertGoals[2].Deadline.Date);
            }
            else if (alertGoals.Count == 2)
            {
                Label1.Text = alertGoals[0].Title + " ends " + GetDifference(alertGoals[0].Deadline.Date);
                Label2.Text = alertGoals[1].Title + " ends " + GetDifference(alertGoals[1].Deadline.Date);
                Label3.IsVisible = false;
            }
            else if (alertGoals.Count == 1)
            {
                Label1.Text = alertGoals[0].Title + " ends" + GetDifference(alertGoals[0].Deadline.Date);
                Label2.IsVisible = false;
                Label3.IsVisible = false;

            }
            else
            {

            }
        }

        private string GetDifference(DateTime Deadline)
        {
            DateTimeOffset today = (DateTimeOffset)DateTime.Now.Date;
            var difference = (DateTimeOffset)Deadline.Date - today;

            if (difference.TotalDays < 1)
                return " today!";
            else
                return " in " + difference.TotalDays.ToString() + " days.";
        }

        private async void PopulateTeamPointsChart()
        {
            var months = 0;
            var firstDayOfThisMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var firstDayOfThisYear = new DateTime(DateTime.Now.Year, 1, 1);
            var firstDayOfMonth = firstDayOfThisMonth.AddMonths(-months);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            DateTime comparisonDate;
            if (PointsChartYearly)
                comparisonDate = firstDayOfThisYear;
            else
                comparisonDate = firstDayOfThisMonth;

            pointsEntries.Clear();
            int index = 0;
            var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).OrderBy(u => u.FirstName).ToListAsync();
            foreach (var user in users)
            {
                var points = await App.client.GetTable<PointModel>().Where(p => p.UserId == user.Id).ToListAsync();
                int point_sum = 0;
                foreach (var point in points)
                {
                    if (point.RegDate > comparisonDate)
                        point_sum += point.Points;
                }

                pointsEntries.Add(new ChartEntry(point_sum) { Label = user.FirstName, ValueLabel = point_sum.ToString("D"), Color = SKColor.Parse(App.donutChartColors[index]) });
                index++;
                if (index >= App.donutChartColors.Count)
                    index = 0;
            }
            chartViewPoints.Chart = new PointChart() { Entries = pointsEntries };
        }

        private async void createFeedList()
        {
            List<FeedModel> feed = new List<FeedModel>();
            List<UserModel> users = new List<UserModel>();

            if (App.loggedInUser.TeamId == null)
            {
                ;
            }
            else
            {
                users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                List<string> challengeIds = new List<string>();

                foreach (UserModel user in users)
                {
                    try
                    {
                        var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).Where(g => g.GoalType > 0).Where(g => g.CreatedDate > DateTime.Now.AddYears(-1)).ToListAsync();
                        foreach (var goal in goals)
                        {
                            string challengeId = goal.ChallengeId;
                            if( challengeId != null)
                            {
                                if (!challengeIds.Contains(challengeId))
                                {
                                    challengeIds.Add(challengeId);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                    }
                }

                // Insert winners of challenges
                foreach (var id in challengeIds)
                {
                    var winnerGoal = (await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == id).OrderBy(g => g.ClosedDate).ToListAsync()).FirstOrDefault();
                    var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == winnerGoal.UserId).ToListAsync()).FirstOrDefault();
                    FeedModel feedItem = new FeedModel();
                    feedItem.IndexDate = winnerGoal.ClosedDate;
                    feedItem.ProfileImageURL = user.ImageUrl;

                    if (winnerGoal.GoalType == 1)
                    {
                        feedItem.FeedItemTitle = "Challenge won!";
                        feedItem.FeedItemPost = user.FirstName + " has won the challenge \"" + winnerGoal.Title + "\"! ";
                    }
                    else if (winnerGoal.GoalType == 2)
                    {
                        feedItem.FeedItemTitle = "Competition won!";
                        feedItem.FeedItemPost = user.FirstName + " has won the competition \"" + winnerGoal.Title + "\"! ";
                    }

                    feed.Add(feedItem);
                }

                foreach (var id in challengeIds)
                {
                    var challenge = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == id).ToListAsync()).FirstOrDefault();
                    var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == challenge.OwnerId).ToListAsync()).FirstOrDefault();
                    var goal = (await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == challenge.Id).ToListAsync()).FirstOrDefault();
                    FeedModel feedItem = new FeedModel();
                    feedItem.IndexDate = challenge.CreatedDate;
                    feedItem.ProfileImageURL = user.ImageUrl;

                    if (goal.GoalType == 1)
                    {
                        feedItem.FeedItemTitle = "New challenge!";
                        feedItem.FeedItemPost = user.FirstName + " created the challenge \"" + goal.Title + "\"! ";
                    }
                    else if (goal.GoalType == 2)
                    {
                        feedItem.FeedItemTitle = "New competition!";
                        feedItem.FeedItemPost = user.FirstName + " created the competition \"" + goal.Title + "\"! ";
                    }

                    feed.Add(feedItem);
                }
                feed.Sort((x, y) => y.IndexDate.CompareTo(x.IndexDate));
                feedCollectionView.ItemsSource = feed;
            }
        }


        void feedFilter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createFeedList();
        }

  

        void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new PostPage());
        }

  
        void buttonYear_Clicked(System.Object sender, System.EventArgs e)
        {
            PointsChartYearly = true;
            PopulateTeamPointsChart();
        }

        void buttonMonth_Clicked(System.Object sender, System.EventArgs e)
        {
            PointsChartYearly = false;
            PopulateTeamPointsChart();
        }
    }
}
