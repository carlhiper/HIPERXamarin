using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class FeedPage : ContentPage
    {
        public FeedPage()
        {
            InitializeComponent();
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();

            createFeedList();


        }

        private async void createFeedList()
        {

            List<FeedModel> feed = new List<FeedModel>();
            feedFilter.ItemsSource = App.feedfilterOptions;

            // Get users in team
            var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();


            foreach (UserModel user in users)
            {
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();

                foreach (var goal in goals)
                {
                    if (!goal.PrivateGoal && !goal.Completed && !goal.Closed)
                    {
                        FeedModel feedItem = new FeedModel();
                        feedItem.GoalTitle = goal.Title;
                        feedItem.UserName = user.FirstName + " " + user.LastName;
                        feedItem.Progress = goal.Progress;
                        feedItem.Hipes = goal.Hipes;
                        feedItem.LastUpdated = goal.LastUpdatedDate;
                        feedItem.Deadline = goal.Deadline;
                        feed.Add(feedItem);
                    }
                }
            }

            int filter = feedFilter.SelectedIndex;
            //Sorting
            if (filter == 0)
            {
                feed.Sort((x, y) => x.GoalTitle.CompareTo(y.GoalTitle));
            }
            else if (filter == 1)
            {
                feed.Sort((x, y) => y.LastUpdated.CompareTo(x.LastUpdated));
            }
            else if (filter == 2)
            {
                feed.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
            }
            else if (filter == 3)
            {
                feed.Sort((x, y) => x.UserName.CompareTo(y.UserName));
            }
            else
            {
                feed.Sort((x, y) => y.LastUpdated.CompareTo(x.LastUpdated));
            }

            feedCollectionView.ItemsSource = feed;
        }



        void feedFilter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createFeedList();
        }
    }
}
