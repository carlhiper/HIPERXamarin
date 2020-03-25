using System;
using System.Collections.Generic;
using HIPER.Helpers;
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
            initPage();

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

            repeatableRB1.IsChecked = (selectedGoal.TargetType == 0) ? true : false;
            repeatableRB2.IsChecked = (selectedGoal.TargetType == 1) ? true : false;
            repeatableRB21.IsChecked = (selectedGoal.WeeklyOrMonthly == 0) ? true : false;
            repeatableRB22.IsChecked = (selectedGoal.WeeklyOrMonthly == 1) ? true : false;

            weekdayPicker.SelectedIndex = selectedGoal.RepeatWeekly;
            dayOfMonthPicker.SelectedIndex = selectedGoal.RepeatMonthly;
          
            if (selectedGoal.Completed)
            {
                headerText.Text = "CLOSED GOAL";
            }
            else
            {
                headerText.Text = "EDIT GOAL";
            }   
        }

        private void initPage()
        {

            weekdayPicker.ItemsSource = App.weekdays;
            dayOfMonthPicker.ItemsSource = App.daysofmonth;
        }

        private async void updateGoal_Clicked(System.Object sender, System.EventArgs e)
        {

            bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
            bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);
            bool isWeeklyCheckedAndEntryFilled = repeatableRB21.IsChecked && (weekdayPicker.SelectedIndex < 0);
            bool isMonthlyCheckedAndEntryFilled = repeatableRB22.IsChecked && (dayOfMonthPicker.SelectedIndex < 0);

            if(isGoalNameEmpty || isGoalDescriptionEmpty || isWeeklyCheckedAndEntryFilled || isMonthlyCheckedAndEntryFilled)
            {
                await DisplayAlert("Error", "All field needs to be entered", "Ok");
            }
            else
            {
                if (repeatableRB2.IsChecked)
                {
                    var startDate = DateHandling.GetStartDate(repeatableRB2.IsChecked && repeatableRB21.IsChecked, repeatableRB2.IsChecked && repeatableRB22.IsChecked, weekdayPicker.SelectedIndex, dayOfMonthPicker.SelectedIndex);
                    selectedGoal.CreatedDate = startDate;
                    selectedGoal.Deadline = DateHandling.GetDeadlineDateForRepeatingGoals(repeatableRB2.IsChecked && repeatableRB21.IsChecked, repeatableRB2.IsChecked && repeatableRB22.IsChecked, startDate);
                }
                else
                {
                    selectedGoal.Deadline = DateTime.Parse(goalDeadlineEntry.Date.ToString());
                }

                selectedGoal.Title = goalNameEntry.Text;
                selectedGoal.Description = goalDescriptionEntry.Text;
                selectedGoal.TargetValue = goalTargetEntry.Text;
                selectedGoal.PrivateGoal = privateGoalCheckbox.IsChecked;
                selectedGoal.CurrentValue = goalCurrentEntry.Text;
                selectedGoal.RepeatWeekly = weekdayPicker.SelectedIndex;
                selectedGoal.RepeatMonthly = dayOfMonthPicker.SelectedIndex;
                selectedGoal.RepeatType = repeatableRB1.IsChecked ? 0 : 1;
                selectedGoal.WeeklyOrMonthly = repeatableRB21.IsChecked ? 0 : 1;

                await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
                await DisplayAlert("Success", "Goal updated", "Ok");
                await Navigation.PopAsync();
            }


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
            await DisplayAlert("Congratulations", "Goal completed", "Ok");
            await Navigation.PopAsync();

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
                }
                else
                {
                    weekdayPicker.IsEnabled = false;
                    dayOfMonthPicker.IsEnabled = true;
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

        private void goalCurrentEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //checkGoalCompleted();
        }

        private void goalTargetEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
         //   checkGoalCompleted();
        }

        private async void checkGoalCompleted()
        {
            if (!string.IsNullOrEmpty(goalCurrentEntry.Text) && !string.IsNullOrEmpty(goalCurrentEntry.Text))
            {
                if (int.Parse(goalCurrentEntry.Text) >= int.Parse(goalTargetEntry.Text))
                {
                    bool setGoalCompleted = await DisplayAlert("Congrats!", "You did it! Would you like to set this goal completed?", "Ok", "Cancel");
                    if (setGoalCompleted)
                    {
                        selectedGoal.Completed = true;
                        selectedGoal.Closed = true;

                        await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
                        await Navigation.PopAsync();
                    }
                }
            }
        }
    }
}
