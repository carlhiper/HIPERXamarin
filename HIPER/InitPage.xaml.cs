using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class InitPage : ContentPage
    {
        public InitPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
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
                            aiLayout.IsVisible = true;
                            ai.IsRunning = true;

                            App.loggedInUser = stored_user;

                            // wait for server connection before continuing
                            string teamName = (await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault().Name;

                            if (!string.IsNullOrEmpty(teamName))
                            {
                                aiLayout.IsVisible = false;
                                ai.IsRunning = false;

                                await Navigation.PushAsync(new HomePage());
                            }

                        }
                        else
                        {
                            await Navigation.PushAsync(new MainPage());
                        }
                    }
                    else
                    {
                        await Navigation.PushAsync(new MainPage());
                    }
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
