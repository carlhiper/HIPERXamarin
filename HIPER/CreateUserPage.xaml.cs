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

        void saveButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isFirstNameEntryEmpty = string.IsNullOrEmpty(firstNameEntry.Text);
            bool isLastNameEntryEmpty = string.IsNullOrEmpty(lastNameEntry.Text);
            bool isEmailEntryEmpty = string.IsNullOrEmpty(emailEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);

            if (isFirstNameEntryEmpty || isLastNameEntryEmpty || isEmailEntryEmpty || isPasswordEntryEmpty) {

                // Write code
                DisplayAlert("Field missing input!", "Please fill in all fields and try again.", "Ok");
            }
            else
            {
                User user = new User()
                {
                    firstName = firstNameEntry.Text,
                    lastName = lastNameEntry.Text,
                    company = companyEntry.Text,
                    email = emailEntry.Text,
                    password = passwordEntry.Text,
                    createdDate = DateTime.Now
                };

                // Save locally
                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation)) { 
                    conn.CreateTable<User>();
                    int rows = conn.Insert(user);
     
                    if (rows > 0)
                    {
                        DisplayAlert("User successfully saved!", "Return to login page to login", "Ok");
                    }
                    else
                    {
                        DisplayAlert("User not saved!", "Something went wrong, please try again", "Ok");
                    }
                }
                Navigation.PopAsync();
            }
        }
    }
}
