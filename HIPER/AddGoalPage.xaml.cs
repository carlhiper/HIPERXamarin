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

        private async void saveGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            try {
                bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
                bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);

                if (isGoalNameEmpty || isGoalDescriptionEmpty)
                {
                    await DisplayAlert("Not filled", "All field needs to be entered", "Ok");
                }
                else
                {
                    GoalModel goal = new GoalModel() {
                        Title = goalNameEntry.Text,
                        Description = goalDescriptionEntry.Text,
                        Deadline = DateTime.Parse(goalDeadlineEntry.Date.ToString()),
                        TargetValue = goalTargetEntry.Text,
                        PrivateGoal = privateGoalCheckbox.IsChecked,
                        UserId = App.loggedInUser.Id,
                        CurrentValue = "0",
                        CreatedDate = DateTime.Now,
                        ClosedDate = DateTime.MaxValue,
                        Progress = 0
                    };



                    /*
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

                    }*/

                    await App.client.GetTable<GoalModel>().InsertAsync(goal);
                    await DisplayAlert("Success", "Goal saved", "Ok");
                    await Navigation.PopAsync();
                }
            
            }
            catch(NullReferenceException nre)
            {
                await DisplayAlert("Goal not saved!", "Something went wrong, please try again", "Ok");
            }
            catch(Exception ex) 
            {
                await DisplayAlert("Goal not saved!", "Something went wrong, please try again", "Ok");
            }



        }
    }
}
