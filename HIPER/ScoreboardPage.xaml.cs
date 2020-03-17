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
            var selectedGoal = goalCollectionView.SelectedItem as Goal;

            Navigation.PushAsync(new EditGoalPage(selectedGoal));
        }

        private void showCompletedSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList();
        }

        private void createGoalsList()
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Goal>();
                var goals = conn.Table<Goal>().ToList();
                List<Goal> activeGoals = new List<Goal>();
                List<Goal> closedGoals = new List<Goal>();

                if (showCompletedSwitch.IsToggled)
                {
                    foreach (var item in goals)
                    {
                        if (item.completed)
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
                        if (!item.completed)
                        {
                            activeGoals.Add(item);
                        }
                    }
                    goalCollectionView.ItemsSource = activeGoals;
                }
            }
        }
    }
}
