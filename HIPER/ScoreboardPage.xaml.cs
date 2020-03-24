using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class ScoreboardPage : ContentPage
    {
        public ScoreboardPage()
        {
            InitializeComponent();
        }

        void addGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddGoalPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            createGoalsList();
        }
        
        void goalCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedGoal = goalCollectionView.SelectedItem as GoalModel;
            if (selectedGoal.TargetType == 1)
            {
                Navigation.PushAsync(new EditSbSGoalPage(selectedGoal));
            }
            else
            { 
                Navigation.PushAsync(new EditGoalPage(selectedGoal));
            }
        }

        private void showClosedSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList();
        }

        private async void createGoalsList()
        {
            List<GoalModel> activeGoals = new List<GoalModel>();
            List<GoalModel> closedGoals = new List<GoalModel>();
            var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id).ToListAsync();

            
            if (showClosedSwitch.IsToggled)
            {
                foreach (var item in goals)
                {
                    if (item.Closed)
                    {
                        closedGoals.Add(item);
                    }
                }
                goalCollectionView.ItemsSource = closedGoals;
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
                goalCollectionView.ItemsSource = activeGoals;
            }
        }
    }
}
