using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.WindowsAzure.MobileServices;
using HIPER.Model;
using System.Collections.Generic;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using Microsoft.WindowsAzure.MobileServices.Sync;
using Microsoft.WindowsAzure.MobileServices.SQLiteStore;

namespace HIPER
{
     public partial class App : Application
    {
        public static string DatabaseLocation = string.Empty;

        public static MobileServiceClient client = new MobileServiceClient(Helpers.Constants.AZURE_WEB_SERVICE);

        public static IMobileServiceSyncTable<UserModel> UserTable;

        public static UserModel loggedInUser = new UserModel();

        public static List<string> donutChartColors = new List<string>() { "#ff7563", "#666666", "#4ca3dd", "#66cdaa", "#daa520", "#468499", "#8b0000", "#f7347a", "#065535" };

        public static List<string> weekdays = new List<string>() { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
        public static List<string> daysofmonth = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13"
                                                                    , "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"
                                                                    , "24", "25", "26", "27", "28"};

        public static List<string> months = new List<string>() { "January", "Febuary", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
        public static List<string> months_short = new List<string>() { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
        public static List<string> numberofsteps = new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

        public static List<string> filterOptions = new List<string>() { "Title", "Last updated", "Deadline", "Performance", "Progress" };

        public static List<string> feedfilterOptions = new List<string>() { "All", "Posts", "Updates"};


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

            //var store = new MobileServiceSQLiteStore(databaseLocation);
            //store.DefineTable<UserModel>();

            //client.SyncContext.InitializeAsync(store);

            //UserTable = client.GetSyncTable<UserModel>();
        }

        protected override void OnStart()
        {

            string androidAppSecret = "ffa750a1-2d72-432c-b96b-dd7b077d0ce0";
            string iOSAppSecret = "bc589ec5-5df5-4bb6-a9de-415153420374";
            AppCenter.Start($"android = {androidAppSecret}; ios = {iOSAppSecret}", typeof(Crashes));

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
