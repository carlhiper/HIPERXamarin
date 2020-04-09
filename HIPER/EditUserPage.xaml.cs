﻿using System;
using System.Collections.Generic;
using System.IO;
using HIPER.Model;
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
            try { 
                App.loggedInUser.FirstName = firstNameEntry.Text;
                App.loggedInUser.LastName = lastNameEntry.Text;
                App.loggedInUser.Company = companyEntry.Text;
                App.loggedInUser.Email = emailEntry.Text;
                App.loggedInUser.UserPassword = passwordEntry.Text;

                //UploadImage(selectedImage.GetStream());


                var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=hiperimagestorage;AccountKey=IayenlXv3jbB7XVIlgiWC1xyIVUSWZcme4AWFxNR0vFPo+eI7xPzUKqegTtspMUarqBv1jUzWHesPFqciPVxMQ==;EndpointSuffix=core.windows.net");
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("imagecontainer");
                await container.CreateIfNotExistsAsync();

                // delete old image file
                if (App.loggedInUser.ImageName != null)
                {
                    var oldBlockBlob = container.GetBlockBlobReference(App.loggedInUser.ImageName);
                    bool result = await oldBlockBlob.DeleteIfExistsAsync();
                }

                // upload new
                var name = Guid.NewGuid().ToString();
                var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
                await blockBlob.UploadFromStreamAsync(selectedImage.GetStream());

                App.loggedInUser.ImageName = $"{name}.jpg";
                App.loggedInUser.ImageUrl = blockBlob.Uri.OriginalString;


                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                await DisplayAlert("Success", "Profile updated", "Ok");
                await Navigation.PopAsync();
            }
            catch(NullReferenceException nre)
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
            selectedImage = selectedImageFile;
            profileImage.Source = ImageSource.FromStream(() => selectedImageFile.GetStream());
        }

        private async void UploadImage(Stream stream)
        {
            try
            {
                var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=hiperimagestorage;AccountKey=IayenlXv3jbB7XVIlgiWC1xyIVUSWZcme4AWFxNR0vFPo+eI7xPzUKqegTtspMUarqBv1jUzWHesPFqciPVxMQ==;EndpointSuffix=core.windows.net");
                var client = account.CreateCloudBlobClient();
                var container = client.GetContainerReference("imagecontainer");
                await container.CreateIfNotExistsAsync();

                // delete old image file
                if (App.loggedInUser.ImageName != null)
                {
                    var oldBlockBlob = container.GetBlockBlobReference(App.loggedInUser.ImageName);
                    bool result = await oldBlockBlob.DeleteIfExistsAsync();
                }

                // upload new
                var name = Guid.NewGuid().ToString();
                var blockBlob = container.GetBlockBlobReference($"{name}.jpg");
                await blockBlob.UploadFromStreamAsync(stream);

                App.loggedInUser.ImageName = $"{name}.jpg";
                App.loggedInUser.ImageUrl = blockBlob.Uri.OriginalString;
            }
            catch(Exception ex)
            {
                
            }
        }
    }
}
