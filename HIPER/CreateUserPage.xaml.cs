using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class CreateUserPage : ContentPage
    {
        
        public CreateUserPage()
        {
            InitializeComponent();
        }

        private async void saveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isFirstNameEntryEmpty = string.IsNullOrEmpty(firstNameEntry.Text);
            bool isLastNameEntryEmpty = string.IsNullOrEmpty(lastNameEntry.Text);
            bool isEmailEntryEmpty = string.IsNullOrEmpty(emailEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);

            if (isFirstNameEntryEmpty || isLastNameEntryEmpty || isEmailEntryEmpty || isPasswordEntryEmpty) {

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
                        CreatedDate = DateTime.Now
                    };

                    // Save on Azure
                    await App.client.GetTable<UserModel>().InsertAsync(user);
                    await DisplayAlert("Success", "user successfully created. Please login.", "Ok");
                    await Navigation.PopAsync();
                }catch(Exception ex)
                {
                    await DisplayAlert("Error", "Something went wrong", "Ok");
                }
 
            }
        }
    }
}
