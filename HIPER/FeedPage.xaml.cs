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

            List<FeedModel> feed = new List<FeedModel>();


            // Get users in team
            var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
           
       
            
            foreach (UserModel user in users)
            {
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();

                foreach (var goal in goals)
                {
                    if (!goal.PrivateGoal && !goal.Completed) { 
                        FeedModel feedItem = new FeedModel();
                        feedItem.GoalTitle = goal.Title;
                        feedItem.UserName = user.FirstName + " " + user.LastName;
                        feedItem.Progress = goal.Progress;
                        feedItem.Hipes = goal.Hipes;
                        feed.Add(feedItem);
                    }
                }
            }

            feedCollectionView.ItemsSource = feed;
        }
    }
}
