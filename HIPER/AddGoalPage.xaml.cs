using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class AddGoalPage : ContentPage
    {
        public AddGoalPage()
        {
            InitializeComponent();
        }

        void saveGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
            bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);

            if(isGoalNameEmpty || isGoalDescriptionEmpty)
            {
                DisplayAlert("Not filled", "All field needs to be entered", "Ok");
            }
            else
            {
                GoalModel goal = new GoalModel() {
                    title = goalNameEntry.Text,
                    description = goalDescriptionEntry.Text,
                    deadline = goalDeadlineEntry.Date,
                    targetValue = goalTargetEntry.Text,
                    privateGoal = privateGoalCheckbox.IsChecked,
                    
                };

         
                using( SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<GoalModel>();
                    int rows = conn.Insert(goal);
                    if (rows > 0)
                    {
                        DisplayAlert("Goal saved!", "", "Ok");
                    }
                    else
                    {
                        DisplayAlert("Goal not saved!", "Something went wrong, please try again", "Ok");
                    }
                    
                }
                Navigation.PopAsync();
            }
            
        }
    }
}
