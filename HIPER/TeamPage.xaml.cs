using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

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

            try {
                userName.Text = App.loggedInUser.FirstName + " " + App.loggedInUser.LastName;
                company.Text = App.loggedInUser.Company;
                email.Text = App.loggedInUser.Email;
                profileImage.Source = App.loggedInUser.ImageUrl;
                team = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault();
                if (team != null) {
                    if (team.Administrator_id == App.loggedInUser.Id)
                    {
                        teamAdmin.Text = "YOU (team admin)";
                    }

                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == team.Id).ToListAsync();
                    if (users != null) {
                        users.RemoveAt(users.FindIndex(a => a.Id == App.loggedInUser.Id));
         
                        teamCollectionView.ItemsSource = users;

                        teamNameLabel.Text = "TEAM " + team.Name.ToUpper();
                        teamIdentifierLabel.Text = "Identifier: " + team.Identifier;

                        createTeam.IsEnabled = false;
                        joinTeam.IsEnabled = false;
                        leaveTeam.IsEnabled = true;
                    }
                }
                else
                {
                    createTeam.IsEnabled = true;
                    joinTeam.IsEnabled = true;
                    leaveTeam.IsEnabled = false;
                }  
            }
            catch(NullReferenceException nre)
            {

            }catch(Exception ex)
            {

            }
        }

        void editProfileButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new EditUserPage());
        }

        void teamCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (App.loggedInUser.Id == team.Administrator_id) { 
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
                    if(users.Count > 1)
                    {
                        await DisplayAlert("Failure", "You are team administrator and can not leave as long as there are members in your team", "Ok");
                    }
                    else
                    {
                        // Remove team id from user
                        App.loggedInUser.TeamId = null;
                        await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                        // Delete team from database
                        await App.client.GetTable<TeamModel>().DeleteAsync(team);
                        await DisplayAlert("Success", "You have left the team", "Ok");
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

            }catch(NullReferenceException nre)
            {

            }catch(Exception ex)
            {

            }
        }
    }
}
