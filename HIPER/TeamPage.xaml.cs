using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class TeamPage : ContentPage
    {
        public TeamPage()
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
                var selectedUser = user.GetValue(0) as UserModel;
                
                firstName.Text = selectedUser.FirstName;
                lastName.Text = selectedUser.LastName;
                company.Text = selectedUser.Company;
                email.Text = selectedUser.Email;
                //password.Text = selectedUser.password;
            }
        }

        void editProfileButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new EditUserPage());
        }
    }
}
