using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using HIPER.Helpers;
using Microcharts;
using SkiaSharp;

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

                foreach (UserModel user in users)
                {
                    try
                    {
                        var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();

                        foreach (var goal in goals)
                        {
                            if (goal.GoalType == 0 && goal.Completed)
                            {
                                FeedModel feedItem = new FeedModel();

                                feedItem.IndexDate = goal.ClosedDate;

                                feedItem.ProfileImageURL = user.ImageUrl;
                                feedItem.FeedItemTitle = user.FirstName + " completed a goal!";
                                feedItem.FeedItemPost = "Goal \"" + goal.Title + "\" was successfully completed. ";

                                feed.Add(feedItem);
                            }else if (goal.GoalType == 1 && goal.Completed)
                            {
                                FeedModel feedItem = new FeedModel();

                                feedItem.IndexDate = goal.ClosedDate;

                                feedItem.ProfileImageURL = user.ImageUrl;
                                feedItem.FeedItemTitle =  "Completed challenge!";
                                feedItem.FeedItemPost = user.FirstName + " has completed the challenge \"" + goal.Title + "\". ";

                                feed.Add(feedItem);

                            }
                            else if (goal.GoalType == 2 && goal.Completed)
                            {
                                FeedModel feedItem = new FeedModel();

                                feedItem.IndexDate = goal.ClosedDate;

                                feedItem.ProfileImageURL = user.ImageUrl;
                                feedItem.FeedItemTitle = "Completed competition!";
                                feedItem.FeedItemPost = user.FirstName + " has completed the competition \"" + goal.Title + "\". ";

                                feed.Add(feedItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

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
