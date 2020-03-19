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

      /*  void saveGoal_Clicked(System.Object sender, System.EventArgs e)
        {

        }*/

        void updateGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedGoal.Title = goalNameEntry.Text;
            selectedGoal.Description = goalDescriptionEntry.Text;
            selectedGoal.Deadline = goalDeadlineEntry.Date;
            selectedGoal.TargetValue = goalTargetEntry.Text;
            selectedGoal.PrivateGoal = privateGoalCheckbox.IsChecked;
            selectedGoal.CurrentValue = goalCurrentEntry.Text;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<GoalModel>();
                int rows = conn.Update(selectedGoal);

                if (rows > 0)
                {
                    DisplayAlert("Success", "Goal updated", "Ok");
                }
                else
                {
                    DisplayAlert("Failure", "Goal not update", "Ok");
                }
                Navigation.PopAsync();
            }
        }

        void deleteGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<GoalModel>();
                int rows = conn.Delete(selectedGoal);
                if (rows > 0)
                {
                    DisplayAlert("Success", "Goal deleted", "Ok");
            
                }
                else
                {
                    DisplayAlert("Failure", "Goal not deleted", "Ok") ;
                }
                Navigation.PopAsync();
            }
        }

        void completeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            
            selectedGoal.Completed = true;

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<GoalModel>();
                int rows = conn.Update(selectedGoal);

                if (rows > 0)
                {
                    DisplayAlert("Congratulations!", "Goal completed", "Ok");
                }
                else
                {
                    DisplayAlert("Failure", "Goal could not update", "Ok");
                }
                Navigation.PopAsync();
            }

        }
    }
}
