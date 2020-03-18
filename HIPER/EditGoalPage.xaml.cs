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

            goalNameEntry.Text = selectedGoal.title;
            goalDescriptionEntry.Text = selectedGoal.description;
            goalDeadlineEntry.Date = selectedGoal.deadline;
            goalTargetEntry.Text = selectedGoal.targetValue;
            goalCurrentEntry.Text = selectedGoal.currentValue;
            privateGoalCheckbox.IsChecked = selectedGoal.privateGoal;

            goalNameEntry.IsEnabled = !selectedGoal.completed;
            goalDescriptionEntry.IsEnabled = !selectedGoal.completed;
            goalDeadlineEntry.IsEnabled = !selectedGoal.completed;
            goalTargetEntry.IsEnabled = !selectedGoal.completed;
            goalCurrentEntry.IsEnabled = !selectedGoal.completed;
            privateGoalCheckbox.IsEnabled = !selectedGoal.completed;

            deleteGoal.IsVisible = !selectedGoal.completed;
            updateGoal.IsVisible = !selectedGoal.completed;
            completeGoal.IsVisible = !selectedGoal.completed;

            if (selectedGoal.completed)
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
            selectedGoal.title = goalNameEntry.Text;
            selectedGoal.description = goalDescriptionEntry.Text;
            selectedGoal.deadline = goalDeadlineEntry.Date;
            selectedGoal.targetValue = goalTargetEntry.Text;
            selectedGoal.privateGoal = privateGoalCheckbox.IsChecked;
            selectedGoal.currentValue = goalCurrentEntry.Text;

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
            
            selectedGoal.completed = true;

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
