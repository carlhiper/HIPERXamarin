using System;
using System.Collections.Generic;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace HIPER
{
    public partial class PostPage : ContentPage
    {
        public PostPage()
        {
            InitializeComponent();
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

                        var posts = await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).ToListAsync();
                        foreach (var post in posts)
                        {
                            if (!string.IsNullOrEmpty(post.Post))
                            {
                                FeedModel feedItem = new FeedModel();
                                feedItem.IndexDate = post.CreatedDate;
                                feedItem.ProfileImageURL = user.ImageUrl;
                                feedItem.FeedItemTitle = user.FirstName + " " + user.LastName;
                                feedItem.FeedItemPost = post.Post;
                                feedItem.UserId = user.Id;
                                feed.Add(feedItem);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        var properties = new Dictionary<string, string> {
                        { "Post page", "Create feed list" }};
                        Crashes.TrackError(ex, properties);

                    }
                }

                feed.Sort((x, y) => y.IndexDate.CompareTo(x.IndexDate));
                feedCollectionView.ItemsSource = feed;

                //Update last viewed post date
                App.loggedInUser.LastViewedPostDate = feed[0].IndexDate;
                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);

            }
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
                    //await DisplayAlert("Success", "Post inserted", "Ok");
                }
                else
                {
                    await DisplayAlert("Error", "Please write a post", "Ok");
                }

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Post page", "Post button clicked" }};
                Crashes.TrackError(ex, properties);

            }
            PostEntry.Text = "";
            createFeedList();
        }
    }

}
