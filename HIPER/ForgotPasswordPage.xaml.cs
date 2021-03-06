﻿using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using System.Linq;
using Microsoft.AppCenter.Crashes;

namespace HIPER
{
    public partial class ForgotPasswordPage : ContentPage
    {
        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

    

        private async void showButton_Clicked(System.Object sender, System.EventArgs e)
        {
            ai.IsRunning = true;
            aiLayout.IsVisible = true;

            try
            {

                var user = (await App.client.GetTable<UserModel>().Where(u => u.Email == emailEntry.Text).ToListAsync()).FirstOrDefault();

                if (user != null)
                {
                    var source = user.UserPassword;
                    int length = source.Length;
                    source = source.Remove(2, length - 4);
                    for (int i = 0; i < length - 4; i++)
                    {
                        source = source.Insert(2, "*");
                    }
                    password.Text = source;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Connection error", ex.ToString(), "Ok");
                var properties = new Dictionary<string, string> {
                        { "Forgot password", "Show button" }};
                Crashes.TrackError(ex, properties);

            }
            aiLayout.IsVisible = false;
            ai.IsRunning = false;

        }
    }
}
