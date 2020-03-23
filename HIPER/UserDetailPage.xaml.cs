using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class UserDetailPage : ContentPage
    {
        UserModel user;

        public UserDetailPage()
        {
            InitializeComponent();
        }

        public UserDetailPage(UserModel selectedUser)
        {
            InitializeComponent();
            this.user = selectedUser;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            titleLabel.Text = (user.FirstName + " " + user.LastName).ToUpper();

            createGoalsList();
        }

        void addGoalButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddGoalPage(user));
        }


        private async void createGoalsList()
        {
            List<GoalModel> activeGoals = new List<GoalModel>();
            List<GoalModel> closedGoals = new List<GoalModel>();
            if (user != null) { 
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();

                if (completedSwitch.IsToggled)
                {
                    foreach (var item in goals)
                    {
                        if (item.Closed)
                        {
                            closedGoals.Add(item);
                        }
                    }
                    userDetailCollectionView.ItemsSource = closedGoals;
                }
                else
                {
                    foreach (var item in goals)
                    {
                        if (!item.Closed)
                        {
                            activeGoals.Add(item);
                        }
                    }
                    userDetailCollectionView.ItemsSource = activeGoals;
                }
            }
        }

        void completedSwitch_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList();
        }
    }
}
