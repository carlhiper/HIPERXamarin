using System;
using System.Collections.Generic;
using HIPER.Helpers;
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
            initPage();
        }

        public AddGoalPage(UserModel user)
        {
            InitializeComponent();
            this.user = user;

            initPage();
        }

        private void initPage()
        {
            weekdayPicker.ItemsSource = App.weekdays;
            dayOfMonthPicker.ItemsSource = App.daysofmonth;
            stepbystepPicker.ItemsSource = App.numberofsteps;
            repeatableRB1.IsChecked = true;
            repeatableRB21.IsChecked = true;
            targetRB1.IsChecked = true;
            targetRB1.IsChecked = true;
        }

        private async void saveGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            try {
                bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
                bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);
                bool isTargetCheckedAndEntryFilled = targetRB1.IsChecked && string.IsNullOrEmpty(goalTargetEntry.Text);
                bool isStepbyStepCheckedAndEntryFilled = targetRB2.IsChecked && (stepbystepPicker.SelectedIndex < 0);
                bool isWeeklyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB21.IsChecked && (weekdayPicker.SelectedIndex < 0);
                bool isMonthlyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB22.IsChecked && (dayOfMonthPicker.SelectedIndex < 0);

                if (isGoalNameEmpty || isGoalDescriptionEmpty || isTargetCheckedAndEntryFilled || isStepbyStepCheckedAndEntryFilled || isWeeklyCheckedAndEntryFilled || isMonthlyCheckedAndEntryFilled)
                {
                    await DisplayAlert("Error", "All field needs to be entered", "Ok");
                }
                else
                {
                    
                    var startDate = DateHandling.GetStartDate(repeatableRB2.IsChecked && repeatableRB21.IsChecked, repeatableRB2.IsChecked && repeatableRB22.IsChecked, weekdayPicker.SelectedIndex, dayOfMonthPicker.SelectedIndex);
                    DateTime deadLineDate;
                    if (repeatableRB2.IsChecked)
                    {
                        deadLineDate = DateHandling.GetDeadlineDateForRepeatingGoals(repeatableRB2.IsChecked && repeatableRB21.IsChecked, repeatableRB2.IsChecked && repeatableRB22.IsChecked, startDate);
                    }
                    else
                    {
                        deadLineDate = DateTime.Parse(goalDeadlineEntry.Date.ToString());
                    }




                    GoalModel goal = new GoalModel() {
                        Title = goalNameEntry.Text,
                        Description = goalDescriptionEntry.Text,
                        Deadline = deadLineDate,
                        TargetValue = goalTargetEntry.Text,
                        PrivateGoal = privateGoalCheckbox.IsChecked,
                        UserId = user.Id,
                        CurrentValue = "0",
                        ClosedDate = DateTime.MaxValue,
                        CreatedDate = startDate,
                        LastUpdatedDate = DateTime.Now,
                        Progress = 0,
                        TargetType = targetRB1.IsChecked? 0 : 1,
                        RepeatType = repeatableRB1.IsChecked? 0 : 1,
                        WeeklyOrMonthly = repeatableRB21.IsChecked? 0 : 1, 
                        RepeatWeekly = weekdayPicker.SelectedIndex,
                        RepeatMonthly = dayOfMonthPicker.SelectedIndex,
                        SteByStepAmount = stepbystepPicker.SelectedIndex
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

        private void radioButtonController()
        {
            if (repeatableRB1.IsChecked)
            {
                repeatableRB21.IsEnabled = false;
                repeatableRB22.IsEnabled = false;
                dayOfMonthPicker.IsEnabled = false;
                weekdayPicker.IsEnabled = false;
                goalDeadlineEntry.IsEnabled = true;
                dayOfMonthPicker.SelectedIndex = -1;
                weekdayPicker.SelectedIndex = -1;
            }
            else if (repeatableRB2.IsChecked)
            {
                repeatableRB21.IsEnabled = true;
                repeatableRB22.IsEnabled = true;
                goalDeadlineEntry.IsEnabled = false;
                if (repeatableRB21.IsChecked)
                {
                    weekdayPicker.IsEnabled = true;
                    dayOfMonthPicker.IsEnabled = false;
                    dayOfMonthPicker.SelectedIndex = -1;
                }
                else
                {
                    weekdayPicker.IsEnabled = false;
                    dayOfMonthPicker.IsEnabled = true;
                    weekdayPicker.SelectedIndex = -1;
                }
            }
        }

        void repeatableRB1_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController();
        }

        void repeatableRB2_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController();
        }

        void repeatableRB21_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController();
        }

        void repeatableRB22_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController();
        }

        private void radioButtonController2()
        {
            if (targetRB1.IsChecked)
            {
                goalTargetEntry.IsEnabled = true;
                stepbystepPicker.IsEnabled = false;
                stepbystepPicker.SelectedIndex = -1;
            }
            else
            {
                goalTargetEntry.IsEnabled = false;
                stepbystepPicker.IsEnabled = true;
                goalTargetEntry.Text = "";
            }

        }

        void targetRB1_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController2();
        }

        void targetRB2_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            radioButtonController2();
        }
    }
}
