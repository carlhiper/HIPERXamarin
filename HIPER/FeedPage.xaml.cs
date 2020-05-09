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
       
            // Get users in team
            var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();


            foreach (UserModel user in users)
            {

                try
                {
                    var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();


                    foreach (var goal in goals)
                    {
                        if (!goal.PrivateGoal && goal.Completed)
                        {
                            FeedModel feedItem = new FeedModel();

                            feedItem.IndexDate = goal.ClosedDate;
                   
                            feedItem.ProfileImageURL = user.ImageUrl;
                            feedItem.FeedItemTitle = user.FirstName + " completed a goal!";
                            feedItem.FeedItemPost = "Goal \"" + goal.Title + "\" was successfully completed. ";

                            feed.Add(feedItem);
                        }
                        if(goal.GoalType == 1 && goal.GoalAccepted)
                        {
                            FeedModel feedItem = new FeedModel();

                            feedItem.IndexDate = goal.ClosedDate;

                            feedItem.ProfileImageURL = user.ImageUrl;
                            feedItem.FeedItemTitle =  "Completed challenge!";
                            feedItem.FeedItemPost = user.FirstName + " has completed the challenge \"" + goal.Title + "\". ";

                            feed.Add(feedItem);

                        }
                        else if (goal.GoalType == 2)
                        {
           

                        }
                    }

                    var posts = await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).ToListAsync();
                    foreach (var post in posts)
                    {
                        if (!string.IsNullOrEmpty(post.Post))
                        {
                            FeedModel feedItem = new FeedModel();
                            feedItem.IndexDate = post.CreatedDate;
                            feedItem.ProfileImageURL = user.ImageUrl;
                            feedItem.FeedItemTitle = user.FirstName + " posted";
                            feedItem.FeedItemPost = post.Post;

                            feed.Add(feedItem);
                        }
                    }
                }catch(Exception ex)
                {

                }
             }

            feed.Sort((x, y) => y.IndexDate.CompareTo(x.IndexDate));
            feedCollectionView.ItemsSource = feed;
        }


        void feedFilter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createFeedList();
        }

        private async void PostButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool isPostEmpty = String.IsNullOrEmpty(PostEntry.Text);

                if (!isPostEmpty)
                {

                    PostModel postItem = new PostModel()
                    {
                        Post = PostEntry.Text,
                        UserId = App.loggedInUser.Id,
                        CreatedDate = DateTime.Now
                    };

                    // Save on Azure
                    await App.client.GetTable<PostModel>().InsertAsync(postItem);
                    await DisplayAlert("Success", "Post inserted", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Please write a post", "Ok");
                }

            }catch(Exception ex)
            {

            }
            PostEntry.Text = "";
            createFeedList();
        }
    }
}
