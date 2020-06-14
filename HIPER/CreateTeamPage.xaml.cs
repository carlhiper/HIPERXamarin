using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using Xamarin.Forms;
using HIPER.Helpers;

namespace HIPER
{
    public partial class CreateTeamPage : ContentPage
    {
        public CreateTeamPage()
        {
            InitializeComponent();
        }

        private static string uniqueIdentifier(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        } 

        private async void saveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool isTeamNameEntryEmpty = string.IsNullOrEmpty(teamNameEntry.Text);
                bool isCompanyEntryEmpty = string.IsNullOrEmpty(companyEntry.Text);

                if (isTeamNameEntryEmpty || isCompanyEntryEmpty)
                {
                    await DisplayAlert("Error", "Required fields missing input", "Ok");
                }
                else
                {
                    TeamModel team = new TeamModel()
                    {
                        Name = teamNameEntry.Text,
                        Company = companyEntry.Text,
                        Organisation_number = orgNumberEntry.Text,
                        Identifier = uniqueIdentifier(Constants.UNIQUE_IDENTIFIER_LENGTH),
                        CreatedDate = DateTime.Now,
                        Max_number_users = Constants.MAX_TEAM_MEMBERS,
                        Administrator_id = App.loggedInUser.id,
                        Active = true
                    };


                    // Save team on Azure
                    await App.client.GetTable<TeamModel>().InsertAsync(team);

                    // Read team id from Azure
                    var updatedTeam = (await App.client.GetTable<TeamModel>().Where(t => t.Identifier == team.Identifier).ToListAsync()).FirstOrDefault();

                    // Update User with TeamId
                    App.loggedInUser.TeamId = updatedTeam.Id;
                    await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);

                    await DisplayAlert("Success", "Team created", "Ok");
                    await Navigation.PopAsync();

                }
            }
            catch (NullReferenceException nre)
            {
            }
            catch (Exception ex)
            {

            }
        }
    }
}
