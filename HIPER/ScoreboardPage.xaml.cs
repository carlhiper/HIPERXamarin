using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;
using HIPER.Controllers;

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

            createGoalsList(filter.SelectedIndex);

            filter.ItemsSource = App.filterOptions;
        }
        
        void goalCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedGoal = goalCollectionView.SelectedItem as GoalModel;
            Navigation.PushAsync(new GoalDetailPage(selectedGoal));
            //if (selectedGoal.TargetType == 1)
            //{
            //    Navigation.PushAsync(new EditSbSGoalPage(selectedGoal));
            //}
            //else
            //{ 
            //    Navigation.PushAsync(new EditGoalPage(selectedGoal));
            //}
        }

        private void showClosedSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }

        private async void createGoalsList(int filter)
        {
            List<GoalModel> activeGoals = new List<GoalModel>();
            List<GoalModel> closedGoals = new List<GoalModel>();
            var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id).ToListAsync();



            //Sorting
            if(filter == 0)
            {
                goals.Sort((x, y) => x.Title.CompareTo(y.Title));
            }
            else if (filter == 1)
            {
                goals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
            }
            else if (filter == 2)
            {
                goals.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
            }
            else if (filter == 3)
            {
                goals.Sort((x, y) => y.PerformanceIndicator.CompareTo(x.PerformanceIndicator));
            }
            else if (filter == 4)
            {
                goals.Sort((x, y) => y.Progress.CompareTo(x.Progress));
            }
            else
            {
                goals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
            }

            if (showClosedSwitch.IsToggled)
            {
                foreach (var item in goals)
                {
                    if (item.Closed || item.Completed)
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
                    if (!item.Closed && !item.Completed)
                    {
                        activeGoals.Add(item);
                    }
                }
                goalCollectionView.ItemsSource = activeGoals;
            }
        }

        void filter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }
    }
}
