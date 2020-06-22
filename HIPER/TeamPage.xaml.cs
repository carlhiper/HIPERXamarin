using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;
using HIPER.Helpers;
using Microsoft.AppCenter.Crashes;

namespace HIPER
{
    public partial class TeamPage : ContentPage
    {
        TeamModel team;

        public TeamPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                team = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault();
                if (team != null)
                {
                    teamStats.IsEnabled = true;
                    if (team.Administrator_id == App.loggedInUser.Id)
                    {
                        teamStats.IsVisible = true;
                        competitionButton.IsVisible = true;
                    }
                    else
                    {
                        teamStats.IsVisible = false;
                        competitionButton.IsVisible = false;
                    }

                    var teamAdmin = (await App.client.GetTable<UserModel>().Where(u => u.Id == team.Administrator_id).ToListAsync()).FirstOrDefault();
                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == team.Id).ToListAsync();
                    if (users != null)
                    {
                        users.RemoveAt(users.FindIndex(a => a.Id == App.loggedInUser.Id));

                        teamCollectionView.ItemsSource = users;

                        teamNameLabel.Text = team.Name.ToUpper();
                        teamAdminLabel.Text = "Administrator: " + teamAdmin.FirstName + " " + teamAdmin.LastName;
                        teamIdentifierLabel.Text = "Identifier: " + team.Identifier;
                        registerredLabel.Text = "Registerred: " + String.Format("{0:yyyy-MM-dd}", team.CreatedDate);

                        createTeam.IsEnabled = false;
                        joinTeam.IsEnabled = false;
                        leaveTeam.IsEnabled = true;
                        createTeam.IsVisible = false;
                        joinTeam.IsVisible = false;
                        leaveTeam.IsVisible = true;
                    }
                }
                else
                {
                    teamStats.IsEnabled = false;
                    createTeam.IsEnabled = true;
                    joinTeam.IsEnabled = true;
                    leaveTeam.IsEnabled = false;
                    competitionButton.IsEnabled = false;
                    teamStats.IsVisible = false;
                    createTeam.IsVisible = true;
                    joinTeam.IsVisible = true;
                    leaveTeam.IsVisible = false;
                    competitionButton.IsVisible = false;
                    membersButton.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "Team page", "OnAppearing" }};
                Crashes.TrackError(ex, properties);

            }

            CheckChat();
        }


        void teamCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (App.loggedInUser.Id == team.Administrator_id)
            {
                var selectedUser = teamCollectionView.SelectedItem as UserModel;
                Navigation.PushAsync(new UserDetailPage(selectedUser));
            }
        }

        void joinTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new JoinTeamPage());
        }

        void createTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateTeamPage());
        }

        private async void leaveTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (team.Administrator_id == App.loggedInUser.Id)
                {
                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == team.Id).ToListAsync();
                    if (users.Count > 1)
                    {
                        await DisplayAlert("Failure", "You are team administrator and can not leave as long as there are members in your team", "Ok");
                    }
                    else
                    {
                        bool deleteTeam = await DisplayAlert("Warning", "You are about to delete your team. Are you sure?", "Yes", "No");
                        if (deleteTeam)
                        {
                            // Remove team id from user
                            App.loggedInUser.TeamId = null;
                            teamNameLabel.Text = "";
                            teamIdentifierLabel.Text = "";
                            createTeam.IsEnabled = true;
                            joinTeam.IsEnabled = true;
                            leaveTeam.IsEnabled = false;

                            await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                            // Delete team from database
                            await App.client.GetTable<TeamModel>().DeleteAsync(team);
                            await DisplayAlert("Success", "You have left the team", "Ok");
                        }
                    }
                }
                else
                {
                    bool leaveTeam = await DisplayAlert("Warning", "You are about to leave your team. Are you sure?", "Yes", "No");
                    if (leaveTeam)
                    {
                        App.loggedInUser.TeamId = null;

                        await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                        await DisplayAlert("Success", "You have left the team", "Ok");
                    }
                }

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "Team page", "OnAppearing" }};
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
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Team page", "Check chat" }};
                Crashes.TrackError(ex, properties);

            }
        }

        void teamStats_Clicked(System.Object sender, System.EventArgs e)
        {
            if(team != null)
                Navigation.PushAsync(new StatisticsPage(team));
        }

        void competitionButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CompetitionPage());
        }

        private async void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new PostPage());
        }
    }
}
