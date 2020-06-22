using System;
using System.Collections.Generic;
using System.IO;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
using Microsoft.WindowsAzure.Storage;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditUserPage : ContentPage
    {
        MediaFile selectedImage;
        string oldImageString;

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
            oldImageString = App.loggedInUser.ImageUrl;
            profileImage.Source = App.loggedInUser.ImageUrl;
        }



        private async void saveProfile_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {


                if (passwordEntry.Text.Length < 8)
                {
                    await DisplayAlert("Error", "The password need to be at least 8 characters", "Ok");
                    return;
                }
                else if (string.IsNullOrEmpty(emailEntry.Text))
                {
                    await DisplayAlert("Error", "You need to fill in your email", "Ok");
                    return;
                }

                App.loggedInUser.FirstName = firstNameEntry.Text;
                App.loggedInUser.LastName = lastNameEntry.Text;
                App.loggedInUser.Company = companyEntry.Text;
                App.loggedInUser.Email = emailEntry.Text;
                App.loggedInUser.UserPassword = passwordEntry.Text;


                if (selectedImage != null)
                {

                    var account = CloudStorageAccount.Parse(Helpers.Constants.BLOBSTORAGE_IMAGES);
                    var client = account.CreateCloudBlobClient();
                    var container = client.GetContainerReference("imagecontainer");
                    await container.CreateIfNotExistsAsync();

                    // delete old image file
                    if (App.loggedInUser.Imagename != null)
                    {
                        var oldBlockBlob = container.GetBlockBlobReference(App.loggedInUser.Imagename);
                        bool result = await oldBlockBlob.DeleteIfExistsAsync();
                    }

                    // upload new
                    var name = Guid.NewGuid().ToString();
                    var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
                    await blockBlob.UploadFromStreamAsync(selectedImage.GetStream());

                    App.loggedInUser.Imagename = $"{name}.jpg";
                    App.loggedInUser.ImageUrl = blockBlob.Uri.OriginalString;

                }


                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                await DisplayAlert("Success", "Profile updated", "Ok");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Failure!", "Something went wrong, please try again", "Ok");
                var properties = new Dictionary<string, string> {
                { "EditUserPage", "SaveProfile" }};
                Crashes.TrackError(ex, properties);

            }
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

        private async void UploadImage(Stream stream)
        {
            try
            {
                var account = CloudStorageAccount.Parse(Helpers.Constants.BLOBSTORAGE_IMAGES);
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("imagecontainer");
                await container.CreateIfNotExistsAsync();

                // delete old image file
                if (App.loggedInUser.Imagename != null)
                {
                    var oldBlockBlob = container.GetBlockBlobReference(App.loggedInUser.Imagename);
                    bool result = await oldBlockBlob.DeleteIfExistsAsync();
                }

                // upload new
                var name = Guid.NewGuid().ToString();
                var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
                await blockBlob.UploadFromStreamAsync(stream);

                App.loggedInUser.Imagename = $"{name}.jpg";
                App.loggedInUser.ImageUrl = blockBlob.Uri.OriginalString;
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Edit user", "Upload image" }};
                Crashes.TrackError(ex, properties);

            }
        }
    }
}
