using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        UserModel selectedUser;
        
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<UserModel>();
                var x = conn.Table<UserModel>().Count();
                if (x > 0) {
                    
                    var user = conn.Table<UserModel>().ToArray();
                    selectedUser = user.GetValue(0) as UserModel;
                    loginNameEntry.Text = selectedUser.Email;
                    passwordEntry.Text = selectedUser.UserPassword;
                    //createUserButton.IsEnabled = false;
                }
            }
        }
        void createUserButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateUserPage());
        }
        void loginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isLoginNameEntryEmpty = string.IsNullOrEmpty(loginNameEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);

            if (isLoginNameEntryEmpty || isPasswordEntryEmpty)
            {

            }
            else
            {
                if (loginNameEntry.Text == selectedUser.Email &&
                    passwordEntry.Text == selectedUser.UserPassword)
                {
                    Navigation.PushAsync(new HomePage());
                }
                else
                {
                    DisplayAlert("Failed to login", "Wrong email or password", "Ok");
                }


                
            }
        }

    }
}
