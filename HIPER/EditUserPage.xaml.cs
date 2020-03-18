using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditUserPage : ContentPage
    {
        UserModel selectedUser;

        public EditUserPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<UserModel>();
                var user = conn.Table<UserModel>().ToArray();
                selectedUser = user.GetValue(0) as UserModel;

                firstNameEntry.Text = selectedUser.firstName;
                lastNameEntry.Text = selectedUser.lastName;
                companyEntry.Text = selectedUser.company;
                emailEntry.Text = selectedUser.email;
                passwordEntry.Text = selectedUser.userPassword;
            }
        }



        void saveProfile_Clicked(System.Object sender, System.EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                selectedUser.firstName = firstNameEntry.Text;
                selectedUser.lastName = lastNameEntry.Text;
                selectedUser.company = companyEntry.Text;
                selectedUser.email = emailEntry.Text;
                selectedUser.userPassword = passwordEntry.Text;

                conn.CreateTable<UserModel>();
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
