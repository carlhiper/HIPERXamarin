using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditUserPage : ContentPage
    {
        User selectedUser;

        public EditUserPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<User>();
                var user = conn.Table<User>().ToArray();
                selectedUser = user.GetValue(0) as User;

                firstNameEntry.Text = selectedUser.firstName;
                lastNameEntry.Text = selectedUser.lastName;
                emailEntry.Text = selectedUser.email;
                passwordEntry.Text = selectedUser.password;
            }
        }

   /*     void updateUserButton_Clicked(System.Object sender, System.EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                selectedUser.firstName = firstNameEntry.Text;
                selectedUser.lastName = lastNameEntry.Text;
                selectedUser.email = emailEntry.Text;
                selectedUser.password = passwordEntry.Text;
                
                conn.CreateTable<User>();
                int rows = conn.Update(selectedUser);

                if (rows > 0)
                {
                    DisplayAlert("Success!", "Profile updated", "Ok");
                }
                else
                {
                    DisplayAlert("Failure!", "Something went wrong, please try again", "Ok");
                }
            }
        }*/

        void saveProfile_Clicked(System.Object sender, System.EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                selectedUser.firstName = firstNameEntry.Text;
                selectedUser.lastName = lastNameEntry.Text;
                selectedUser.email = emailEntry.Text;
                selectedUser.password = passwordEntry.Text;

                conn.CreateTable<User>();
                int rows = conn.Update(selectedUser);

                if (rows > 0)
                {
                    DisplayAlert("Success!", "Profile updated", "Ok");
                }
                else
                {
                    DisplayAlert("Failure!", "Something went wrong, please try again", "Ok");
                }
                Navigation.PopAsync();
            }
        }
    }
}
