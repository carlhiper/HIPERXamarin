using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using System.Linq;
using Microsoft.AppCenter.Crashes;

namespace HIPER
{
    public partial class FeedPage : ContentPage
    {
        bool PointsChartYearly = false;
        private readonly List<ChartEntry> progressEntries = new List<ChartEntry>();
        private readonly List<ChartEntry> pointsEntries = new List<ChartEntry>();

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
            GetTeamName();
            CheckChat();
        }

        private async void GetTeamName()
        {
            try
            {
                string teamName = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault().Name;
                teamNameLabel.Text = teamName.ToUpper();

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Dashboard page", "Get team name" }};
                Crashes.TrackError(ex, properties);
            }
        }

        private async void CheckChat()
        {
            try
            {
                if (App.loggedInUser.TeamId != null)
                {
                    ChatButton.IsEnabled = true;
                    List<UserModel> users = new List<UserModel>();
                    var teammembers = await App.client.GetTable<TeamsModel>().Where(t => t.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    if (teammembers.Count > 0)
                    {
                        foreach (var member in teammembers)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == member.UserId).ToListAsync()).FirstOrDefault();
                            users.Add(user);
                        }
                    }
                    //var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    List<PostModel> postCollection = new List<PostModel>();
                    foreach (var user in users)
                    {
                        var post = (await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).Where(p2 => p2.TeamId == App.loggedInUser.TeamId).OrderByDescending(p => p.CreatedDate).ToListAsync()).FirstOrDefault();
                        if (post != null)
                        {
                            postCollection.Add(post);
                        }
                    }
                    postCollection.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
                    if (postCollection[0].CreatedDate > App.loggedInUser.LastViewedPostDate)
                    {
                        ChatButton.IconImageSource = "chat_ex.png";
                    }
                    else
                    {
                        ChatButton.IconImageSource = "chat.png";
                    }
                }
                else
                {
                    ChatButton.IsEnabled = false;
                }

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Feed page", "Check chat" }};
                Crashes.TrackError(ex, properties);
            }
        }


        private async void PopulateProgressChart()
        {
            progressEntries.Clear();
            int index = 0;
            var activeGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderByDescending(g => g.CreatedDate).Where(g2 => g2.TeamId == App.loggedInUser.TeamId).ToListAsync();
            foreach (var goal in activeGoals)
            {
                progressEntries.Add(new ChartEntry((int)(goal.Progress * 100)) { Label = goal.Title, ValueLabel = ((int)(goal.Progress * 100)).ToString("D") + "%", Color = SKColor.Parse(App.donutChartColors[index]) });
                index++;
                if (index >= App.donutChartColors.Count)
                    index = 0;
            }
            if (activeGoals.Count == 0)
            {
                progressEntries.Add(new ChartEntry(25) { Label = "My next goal!", ValueLabel = (25).ToString("D") + "%", Color = SKColor.Parse(App.donutChartColors[index]) });
                chartViewProgress.Chart = new RadialGaugeChart() { Entries = progressEntries, MaxValue = 100, LabelTextSize = 24 };
            }
            else
            {
                chartViewProgress.Chart = new RadialGaugeChart() { Entries = progressEntries, MaxValue = 100, LabelTextSize = 24 };
            }
        }

        private async void GetAlerts()
        {
            var alertGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderBy(g => g.Deadline).Where(g2 => g2.TeamId == App.loggedInUser.TeamId).Take(3).ToListAsync();

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
            List<UserModel> users = new List<UserModel>();
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

            var teammembers = await App.client.GetTable<TeamsModel>().Where(t => t.TeamId == App.loggedInUser.TeamId).ToListAsync();
            if (teammembers.Count > 0)
            {
                foreach (var member in teammembers)
                {
                    var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == member.UserId).ToListAsync()).FirstOrDefault();
                    users.Add(user);
                }
            }

          //  var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId != null).Where(u => u.TeamId == App.loggedInUser.TeamId).OrderBy(u => u.FirstName).ToListAsync();
            if (users.Count > 0)
            {
                foreach (var user in users)
                {
                    var points = await App.client.GetTable<PointModel>().Where(p => p.UserId == user.Id).Where(p2 => p2.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    int point_sum = 0;
                    foreach (var point in points)
                    {
                        if (point.RegDate > comparisonDate)
                            point_sum += point.Points;
                    }

                    pointsEntries.Add(new ChartEntry(point_sum) { Label = user.FirstName.Substring(0, 1) + "." + user.LastName.Substring(0, 1), ValueLabel = point_sum.ToString("D"), Color = SKColor.Parse(App.donutChartColors[index]) });
                    index++;
                    if (index >= App.donutChartColors.Count)
                        index = 0;
                }
                chartViewPoints.Chart = new PointChart() { Entries = pointsEntries, LabelTextSize = 24, ValueLabelOrientation = Orientation.Horizontal };
            }
            else
            {
                pointsEntries.Add(new ChartEntry(10) { Label = App.loggedInUser.FirstName.Substring(0, 1) + "." + App.loggedInUser.LastName.Substring(0, 1), ValueLabel = (10).ToString("D"), Color = SKColor.Parse(App.donutChartColors[index]) });
                chartViewPoints.Chart = new PointChart() { Entries = pointsEntries, LabelTextSize = 24, ValueLabelOrientation = Orientation.Horizontal };
            }
        }

        private async void createFeedList()
        {
            List<FeedModel> feed = new List<FeedModel>();
            List<UserModel> users = new List<UserModel>();

            if (App.loggedInUser.TeamId == null)
            {
                FeedModel feedItem = new FeedModel();
                feedItem.FeedItemTitle = "News!";
                feedItem.FeedItemPost = "This is where you will find your latest team information and see how you compare with your team mates.";
                feedItem.IndexDate = DateTime.Now;
                feed.Add(feedItem);
                feedCollectionView.ItemsSource = feed;

            }
            else
            {
                users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                List<string> challengeIds = new List<string>();

                foreach (UserModel user in users)
                {
                    try
                    {
                        var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).Where(g => g.GoalType > 0).Where(g => g.CreatedDate > DateTime.Now.AddYears(-1)).Where(g2 => g2.TeamId == App.loggedInUser.TeamId).ToListAsync();
                        foreach (var goal in goals)
                        {
                            string challengeId = goal.ChallengeId;
                            if (challengeId != null)
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
                        var properties = new Dictionary<string, string> {
                        { "Feedpage", "Create feed list" }};
                        Crashes.TrackError(ex, properties);
                    }
                }

                // Insert winners of challenges
                foreach (var id in challengeIds)
                {
                    var winnerGoal = (await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == id).Where(g => g.Completed == true).OrderBy(g => g.ClosedDate).ToListAsync()).FirstOrDefault();
                    if (winnerGoal != null)
                    {
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
                }

                foreach (var id in challengeIds)
                {
                    try
                    {
                        var challenge = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == id).ToListAsync()).FirstOrDefault();
                        var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == challenge.OwnerId).ToListAsync()).FirstOrDefault();
                        var goal = (await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == challenge.Id).OrderBy(g => g.CreatedDate).ToListAsync()).FirstOrDefault();
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
                    catch(Exception ex)
                    {
                        var properties = new Dictionary<string, string> {
                        { "Feedpage", "create feed 2" }};
                        Crashes.TrackError(ex, properties);
                    }
          
                }
                feed.Sort((x, y) => y.IndexDate.CompareTo(x.IndexDate));
                feedCollectionView.ItemsSource = feed;
            }
        }

        void feedFilter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createFeedList();
        }

        private async void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new PostPage());
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
