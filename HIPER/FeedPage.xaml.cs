using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using HIPER.Helpers;

namespace HIPER
{
    public partial class FeedPage : ContentPage
    {

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
    }
}
