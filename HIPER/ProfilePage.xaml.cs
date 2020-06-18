using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            userName.Text = App.loggedInUser.FirstName + " " + App.loggedInUser.LastName;
            company.Text = App.loggedInUser.Company;
            email.Text = App.loggedInUser.Email;
            profileImage.Source = App.loggedInUser.ImageUrl;
            registeredDate.Text = "Registerred: " + String.Format("{0:yyyy-MM-dd}", App.loggedInUser.CreatedDate);
            CheckChat();
            //var team = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault();

        }

        private async void CheckChat()
        {
            if (App.loggedInUser.TeamId != null)
            {
                ChatButton.IsEnabled = true;
                var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                List<PostModel> postCollection = new List<PostModel>();
                foreach (var user in users)
                {
                    var post = (await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreatedDate).ToListAsync()).FirstOrDefault();
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

        void viewStatsButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new StatisticsPage(App.loggedInUser));

        }

        void editProfileButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new EditUserPage());
        }

        private async void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new PostPage());
        }

        async void logoutButton_Clicked(System.Object sender, System.EventArgs e)
        {
            var logOut = await DisplayAlert("Warning", "Are you sure you want to log out?", "Yes", "No");

            if (logOut)
            {
                App.loggedInUser.Email = "";
                App.loggedInUser.UserPassword = "";
                await Navigation.PopAsync();

            }
        }
    }
}
