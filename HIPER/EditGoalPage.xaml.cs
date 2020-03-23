using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditGoalPage : ContentPage
    {
        GoalModel selectedGoal;
 
        public EditGoalPage(GoalModel selectedGoal)
        {
            InitializeComponent();

            this.selectedGoal = selectedGoal;

            goalNameEntry.Text = selectedGoal.Title;
            goalDescriptionEntry.Text = selectedGoal.Description;
            goalDeadlineEntry.Date = selectedGoal.Deadline;
            goalTargetEntry.Text = selectedGoal.TargetValue;
            goalCurrentEntry.Text = selectedGoal.CurrentValue;
            privateGoalCheckbox.IsChecked = selectedGoal.PrivateGoal;

            goalNameEntry.IsEnabled = !selectedGoal.Completed;
            goalDescriptionEntry.IsEnabled = !selectedGoal.Completed;
            goalDeadlineEntry.IsEnabled = !selectedGoal.Completed;
            goalTargetEntry.IsEnabled = !selectedGoal.Completed;
            goalCurrentEntry.IsEnabled = !selectedGoal.Completed;
            privateGoalCheckbox.IsEnabled = !selectedGoal.Completed;

            deleteGoal.IsVisible = !selectedGoal.Completed;
            updateGoal.IsVisible = !selectedGoal.Completed;
            completeGoal.IsVisible = !selectedGoal.Completed;

            if (selectedGoal.Completed)
            {
                headerText.Text = "COMPLETED GOAL";
            }
            else
            {
                headerText.Text = "EDIT GOAL";
            }   
        }

        private async void updateGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedGoal.Title = goalNameEntry.Text;
            selectedGoal.Description = goalDescriptionEntry.Text;
            selectedGoal.Deadline = DateTime.Parse(goalDeadlineEntry.Date.ToString());
            selectedGoal.TargetValue = goalTargetEntry.Text;
            selectedGoal.PrivateGoal = privateGoalCheckbox.IsChecked;
            selectedGoal.CurrentValue = goalCurrentEntry.Text;

            await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
            await DisplayAlert("Success", "Goal updated", "Ok");
            await Navigation.PopAsync();

        }

        private async void deleteGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            await App.client.GetTable<GoalModel>().DeleteAsync(selectedGoal);
            await DisplayAlert("Success", "Goal deleted", "Ok");
            await Navigation.PopAsync();

        }

        private async void completeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            
            selectedGoal.Completed = true;
            selectedGoal.Closed = true;

            await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
            await DisplayAlert("Congratulations", "Goal goal completed", "Ok");
            await Navigation.PopAsync();

        }
    }
}
