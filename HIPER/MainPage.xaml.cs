using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                try
                {
                    conn.CreateTable<UserModel>();
                    var stored_user = conn.Table<UserModel>().ToList().FirstOrDefault();
                    if (stored_user != null)
                    {
                        if (!string.IsNullOrEmpty(stored_user.Id))
                        {
                            App.loggedInUser = stored_user;
                            await Navigation.PushAsync(new HomePage());
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        void createUserButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateUserPage());
        }

        private async void loginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ai.IsRunning = true;
            aiLayout.IsVisible = true;
        
            try
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
                            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                            {
                                try
                                {
                                    conn.CreateTable<UserModel>();
                                    conn.Insert(user);
                                }
                                catch (Exception ex)
                                {

                                }
                            }
                            await Navigation.PushAsync(new HomePage());
                        }
                        else
                        {
                            await DisplayAlert("Error", "Email or password are incorrect", "Ok");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to login", "Ok");
                    }
                }
            }
            catch(Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "MainPage", "Login button" }};
                Crashes.TrackError(ex, properties);

            }
            aiLayout.IsVisible = false;
            ai.IsRunning = false;
        }

        void forgotPasswordButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}