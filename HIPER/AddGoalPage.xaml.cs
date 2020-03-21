using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class AddGoalPage : ContentPage
    {
        UserModel user;

        public AddGoalPage()
        {
            InitializeComponent();
            this.user = App.loggedInUser;
        }

        public AddGoalPage(UserModel user)
        {
            InitializeComponent();
            this.user = user;
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
                        UserId = user.Id,
                        CurrentValue = "0",
                        CreatedDate = DateTime.Now,
                        ClosedDate = DateTime.MaxValue,
                        Progress = 0
                    };

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
