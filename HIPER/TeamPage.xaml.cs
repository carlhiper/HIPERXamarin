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
        public TeamPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try { 
                firstName.Text = App.loggedInUser.FirstName;
                lastName.Text = App.loggedInUser.LastName;
                company.Text = App.loggedInUser.Company;
                email.Text = App.loggedInUser.Email;

                var team = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault();
                if (team != null) { 
                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == team.Id).ToListAsync();
                    if (users != null) { 
                        teamCollectionView.ItemsSource = users;

                        teamNameLabel.Text = "TEAM " + team.Name.ToUpper();
                        teamIdentifierLabel.Text = team.Identifier;

                        createTeam.IsVisible = false;
                        joinTeam.IsVisible = false;
                        buttonGrid.IsVisible = false;
                    }
                }
                else
                {
                    createTeam.IsVisible = true;
                    joinTeam.IsVisible = true;
                    buttonGrid.IsVisible = true;
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
        }

        void joinTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new JoinTeamPage());
        }

        void createTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateTeamPage());
        }
    }
}
