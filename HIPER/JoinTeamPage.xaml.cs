using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
using Xamarin.Forms;

namespace HIPER
{
    public partial class JoinTeamPage : ContentPage
    {
        public JoinTeamPage()
        {
            InitializeComponent();
        }

        private async void saveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool isTeamNameEntryEmpty = string.IsNullOrEmpty(teamNameEntry.Text);
                bool isIdentifierEntryEmpty = string.IsNullOrEmpty(identifierEntry.Text);

                if (isTeamNameEntryEmpty || isIdentifierEntryEmpty)
                {
                    await DisplayAlert("Error", "Required fields missing input", "Ok");
                }
                else
                {

                    var team = (await App.client.GetTable<TeamModel>().Where(i => i.Identifier == identifierEntry.Text).ToListAsync()).FirstOrDefault();
                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == team.Id).ToListAsync();


                    if (team != null && users.Count < team.Max_number_users)
                    {
                        if (team.Name.ToUpper() == teamNameEntry.Text.ToUpper())
                        {
                            App.loggedInUser.TeamId = team.Id;
                            await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Error", "Team credentials are wrong", "Ok");
                        }
                    }
                    else
                    {
                        if (users.Count < team.Max_number_users)
                        {
                            await DisplayAlert("Error", "Team already have max number of members", "Ok");
                        }
                        else
                        {
                            await DisplayAlert("Error", "Failed to join team", "Ok");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                        { "Join team page", "Save button" }};
                Crashes.TrackError(ex, properties);
            }
        }
    }
}
