using System;
using System.Collections.Generic;
using HIPER.Model;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditUserPage : ContentPage
    {

        public EditUserPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            firstNameEntry.Text = App.loggedInUser.FirstName;
            lastNameEntry.Text = App.loggedInUser.LastName;
            companyEntry.Text = App.loggedInUser.Company;
            emailEntry.Text = App.loggedInUser.Email;
            passwordEntry.Text = App.loggedInUser.UserPassword;
        }



        private async void saveProfile_Clicked(System.Object sender, System.EventArgs e)
        {
            try { 
                App.loggedInUser.FirstName = firstNameEntry.Text;
                App.loggedInUser.LastName = lastNameEntry.Text;
                App.loggedInUser.Company = companyEntry.Text;
                App.loggedInUser.Email = emailEntry.Text;
                App.loggedInUser.UserPassword = passwordEntry.Text;

                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                await DisplayAlert("Success", "Profile updated", "Ok");
                await Navigation.PopAsync();
            }catch(NullReferenceException nre)
            {
                await DisplayAlert("Failure!", "Something went wrong, please try again", "Ok");
            }
            catch(Exception ex)
            {
                await DisplayAlert("Failure!", "Something went wrong, please try again", "Ok");
            }
        }

        async void Button_Clicked(System.Object sender, System.EventArgs e)
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsPickPhotoSupported) {
                await DisplayAlert("Error", "This is not supported on your device", "Ok");
                return;
            }

            var mediaOptions = new PickMediaOptions()
            {
                PhotoSize = PhotoSize.Medium

            };
            var selectedImageFile = await CrossMedia.Current.PickPhotoAsync(mediaOptions);
            if(selectedImageFile == null)
            {
                await DisplayAlert("Error", "There was an error trying to get your image file", "Ok");
                return;
            }
            profileImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
        }
    }
}
