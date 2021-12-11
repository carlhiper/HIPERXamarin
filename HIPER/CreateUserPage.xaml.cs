using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class CreateUserPage : ContentPage
    {
        MediaFile selectedImage;
        public CreateUserPage()
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



        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                await DisplayAlert("Error", "This is not supported on your device", "Ok");
                return;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium
            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if (selectedImageFile == null)
            {
                return;
            }
            selectedImage = selectedImageFile;
            profileImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
        }

        private async void saveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isFirstNameEntryEmpty = string.IsNullOrEmpty(firstNameEntry.Text);
            bool isLastNameEntryEmpty = string.IsNullOrEmpty(lastNameEntry.Text);
            bool isEmailEntryEmpty = string.IsNullOrEmpty(emailEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);
            bool isTeamNameEmpty = string.IsNullOrEmpty(teamNameEntry.Text);

            if (isFirstNameEntryEmpty || isLastNameEntryEmpty || isEmailEntryEmpty || isPasswordEntryEmpty || isTeamNameEmpty) {

                // Write code
                await DisplayAlert("Field missing input!", "Please fill in all fields and try again.", "Ok");
            } else if(passwordEntry.Text != confirmPasswordEntry.Text) {
                await DisplayAlert("Error", "Passwords don't match", "Ok");
            } else if( passwordEntry.Text.Length < 8)
            {
                await DisplayAlert("Error", "The password need to be at least 8 characters", "Ok");
            }
            else
            {
                try
                {



                    UserModel user = new UserModel()
                    {
                        FirstName = firstNameEntry.Text,
                        LastName = lastNameEntry.Text,
                        Company = companyEntry.Text,
                        Email = emailEntry.Text,
                        UserPassword = passwordEntry.Text,
                        CreatedDate = DateTime.Now,
                        LastViewedPostDate = DateTime.Now,
                        Number_logins = 0,
                        Id = null,
                        Imagename = null,
                        ImageUrl = null,
                        Rating = 0,
                        TeamId = null,
                        
                    };


                    // Save on Azure
                    await App.client.GetTable<UserModel>().InsertAsync(user);

                    // Read user id from Azure
                    var updatedUser = (await App.client.GetTable<UserModel>().Where(u => u.Email == user.Email).ToListAsync()).FirstOrDefault();
                    App.loggedInUser = updatedUser;

                    TeamModel team = new TeamModel()
                    {
                        Name = teamNameEntry.Text,
                        Company = companyEntry.Text,
                        Organisation_number = orgNumberEntry.Text,
                        Identifier = uniqueIdentifier(Constants.UNIQUE_IDENTIFIER_LENGTH),
                        CreatedDate = DateTime.Now,
                        Max_number_users = Constants.MAX_TEAM_MEMBERS,
                        Administrator_id = App.loggedInUser.Id,
                        Active = true
                    };

                    if (selectedImage != null)
                    {
                        var account = CloudStorageAccount.Parse(Helpers.Constants.BLOBSTORAGE_IMAGES2);
                        var client = account.CreateCloudBlobClient();
                        var container = client.GetContainerReference("hiperblob");
                        await container.CreateIfNotExistsAsync();

                        // upload new
                        var name = Guid.NewGuid().ToString();
                        var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
                        await blockBlob.UploadFromStreamAsync(selectedImage.GetStream());

                        App.loggedInUser.Imagename = $"{name}.jpg";
                        App.loggedInUser.ImageUrl = blockBlob.Uri.OriginalString;
                    }



                    // Save team on Azure
                    await App.client.GetTable<TeamModel>().InsertAsync(team);

                    // Read team id from Azure
                    var updatedTeam = (await App.client.GetTable<TeamModel>().Where(t => t.Identifier == team.Identifier).ToListAsync()).FirstOrDefault();

                    // Update User with TeamId & photo
                    App.loggedInUser.TeamId = updatedTeam.Id;
                    await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);

                    // Create entry in Teams database
                    TeamsModel teamsModelObj = new TeamsModel()
                    {
                        TeamId = updatedTeam.Id,
                        UserId = App.loggedInUser.Id,
                        LastViewedPostDate = DateTime.Now.Date
                    };
                    await App.client.GetTable<TeamsModel>().InsertAsync(teamsModelObj);

                    await DisplayAlert("Success", "User successfully created. Please login.", "Ok");
                    await Navigation.PushAsync(new HomePage());
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Error", ex.ToString(), "Ok");
                    var properties = new Dictionary<string, string> {
                    { "CreateUserPage", "saveButton_Clicked" }};
                    Crashes.TrackError(ex, properties);
                }
            }
        }
    }
}
