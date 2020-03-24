using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.MobileServices;
using HIPER.Model;
using System.Collections.Generic;

namespace HIPER
{
     public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;

        public static MobileServiceClient client =new MobileServiceClient("https://hiper-app-webapp.azurewebsites.net");

        public static UserModel loggedInUser = new UserModel();

        public static List<string> weekdays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        public static List<string> daysofmonth = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13"
                                                                    , "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"
                                                                    , "24", "25", "26", "27", "28", "End of month"};

        public static List<string> numberofsteps = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            DatabaseLocation = databaseLocation;
        }

        protected override void OnStart()
        {
      
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
