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
        
        public MainPage()
        {
            InitializeComponent();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Local database
        /*    using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
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
            } */
        }

        void createUserButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateUserPage());
        }

        private async void loginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            
            bool isLoginNameEntryEmpty = string.IsNullOrEmpty(loginNameEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);

            if (isLoginNameEntryEmpty || isPasswordEntryEmpty)
            {

            }
            else
            {
                var user = (await App.client.GetTable<UserModel>().Where(u => u.Email == loginNameEntry.Text).ToListAsync()).FirstOrDefault();

                if (user != null)
                {
                    App.loggedInUser = user;
                    if (user.UserPassword == passwordEntry.Text)
                    {
                        
                        await Navigation.PushAsync(new HomePage());
                    }
                    else
                    {
                        await DisplayAlert("Error", "Email or password are incorrect", "Ok");
                    }
                }else
                {
                    await DisplayAlert("Error", "Failed to login", "Ok");
                }
            }
        }
    }
}
∑