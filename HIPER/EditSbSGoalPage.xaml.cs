using System;
using System.Collections.Generic;
using HIPER.Helpers;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditSbSGoalPage : ContentPage
    {
        GoalModel selectedGoal;

        public EditSbSGoalPage()
        {
            InitializeComponent();

            initPage();
        }


        public EditSbSGoalPage(GoalModel selectedGoal)
        {
            InitializeComponent();
            initPage();

            this.selectedGoal = selectedGoal;

            goalNameEntry.Text = selectedGoal.Title;
            goalDescriptionEntry.Text = selectedGoal.Description;
            goalDeadlineEntry.Date = selectedGoal.Deadline;
            privateGoalCheckbox.IsChecked = selectedGoal.PrivateGoal;


            gridTitle.IsEnabled = !(selectedGoal.Completed || selectedGoal.Closed);
            gridSteps.IsEnabled = !(selectedGoal.Completed || selectedGoal.Closed);
            gridSingle.IsEnabled = !(selectedGoal.Completed || selectedGoal.Closed);
            gridSingle.IsEnabled = !(selectedGoal.Completed || selectedGoal.Closed);

            deleteGoal.IsVisible = !(selectedGoal.Completed || selectedGoal.Closed);
            updateGoal.IsVisible = !(selectedGoal.Completed || selectedGoal.Closed);
            completeGoal.IsVisible = !(selectedGoal.Completed || selectedGoal.Closed);
            closeGoal.IsVisible = !(selectedGoal.Completed || selectedGoal.Closed);

            repeatableRB1.IsChecked = (selectedGoal.RepeatType == 0) ? true : false;
            repeatableRB2.IsChecked = (selectedGoal.RepeatType == 1) ? true : false;
            repeatableRB21.IsChecked = (selectedGoal.WeeklyOrMonthly == 0) ? true : false;
            repeatableRB22.IsChecked = (selectedGoal.WeeklyOrMonthly == 1) ? true : false;

            weekdayPicker.SelectedIndex = selectedGoal.RepeatWeekly;
            dayOfMonthPicker.SelectedIndex = selectedGoal.RepeatMonthly;
            stepbystepPicker.SelectedIndex = selectedGoal.SteByStepAmount;

            if (selectedGoal.TargetType == 1)
            {
                step1CB.IsChecked = selectedGoal.Checkbox1;
                step1CB.IsVisible = true;
                step1entry.Text = selectedGoal.Checkbox1Comment;
                step1entry.IsVisible = true;
                step1label.IsVisible = true;
                step2CB.IsChecked = selectedGoal.Checkbox2;
                step2CB.IsVisible = (selectedGoal.SteByStepAmount > 0) ? true : false;
                step2entry.Text = selectedGoal.Checkbox2Comment;
                step2entry.IsVisible = (selectedGoal.SteByStepAmount > 0) ? true : false;
                step2label.IsVisible = (selectedGoal.SteByStepAmount > 0) ? true : false;
                step3CB.IsChecked = selectedGoal.Checkbox3;
                step3CB.IsVisible = (selectedGoal.SteByStepAmount > 1) ? true : false;
                step3entry.Text = selectedGoal.Checkbox3Comment;
                step3entry.IsVisible = (selectedGoal.SteByStepAmount > 1) ? true : false;
                step3label.IsVisible = (selectedGoal.SteByStepAmount > 1) ? true : false;
                step4CB.IsChecked = selectedGoal.Checkbox4;
                step4CB.IsVisible = (selectedGoal.SteByStepAmount > 2) ? true : false;
                step4entry.Text = selectedGoal.Checkbox4Comment;
                step4entry.IsVisible = (selectedGoal.SteByStepAmount > 2) ? true : false;
                step4label.IsVisible = (selectedGoal.SteByStepAmount > 2) ? true : false;
                step5CB.IsChecked = selectedGoal.Checkbox5;
                step5CB.IsVisible = (selectedGoal.SteByStepAmount > 3) ? true : false;
                step5entry.Text = selectedGoal.Checkbox5Comment;
                step5entry.IsVisible = (selectedGoal.SteByStepAmount > 3) ? true : false;
                step5label.IsVisible = (selectedGoal.SteByStepAmount > 3) ? true : false;
                step6CB.IsChecked = selectedGoal.Checkbox6;
                step6CB.IsVisible = (selectedGoal.SteByStepAmount > 4) ? true : false;
                step6entry.Text = selectedGoal.Checkbox6Comment;
                step6entry.IsVisible = (selectedGoal.SteByStepAmount > 4) ? true : false;
                step6label.IsVisible = (selectedGoal.SteByStepAmount > 4) ? true : false;
                step7CB.IsChecked = selectedGoal.Checkbox7;
                step7CB.IsVisible = (selectedGoal.SteByStepAmount > 5) ? true : false;
                step7entry.Text = selectedGoal.Checkbox7Comment;
                step7entry.IsVisible = (selectedGoal.SteByStepAmount > 5) ? true : false;
                step7label.IsVisible = (selectedGoal.SteByStepAmount > 5) ? true : false;
                step8CB.IsChecked = selectedGoal.Checkbox8;
                step8CB.IsVisible = (selectedGoal.SteByStepAmount > 6) ? true : false;
                step8entry.Text = selectedGoal.Checkbox8Comment;
                step8entry.IsVisible = (selectedGoal.SteByStepAmount > 6) ? true : false;
                step8label.IsVisible = (selectedGoal.SteByStepAmount > 6) ? true : false;
                step9CB.IsChecked = selectedGoal.Checkbox9;
                step9CB.IsVisible = (selectedGoal.SteByStepAmount > 7) ? true : false;
                step9entry.Text = selectedGoal.Checkbox9Comment;
                step9entry.IsVisible = (selectedGoal.SteByStepAmount > 7) ? true : false;
                step9label.IsVisible = (selectedGoal.SteByStepAmount > 7) ? true : false;
            }

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
            stepbystepPicker.ItemsSource = App.numberofsteps;
        }


        private async void updateGoal_Clicked(System.Object sender, System.EventArgs e)
        {

            bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
            bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);
            bool isWeeklyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB21.IsChecked && (weekdayPicker.SelectedIndex < 0);
            bool isMonthlyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB22.IsChecked && (dayOfMonthPicker.SelectedIndex < 0);

            if (isGoalNameEmpty || isGoalDescriptionEmpty || isWeeklyCheckedAndEntryFilled || isMonthlyCheckedAndEntryFilled)
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
                selectedGoal.LastUpdatedDate = DateTime.Now;

                selectedGoal.Title = goalNameEntry.Text;
                selectedGoal.Description = goalDescriptionEntry.Text;
                selectedGoal.PrivateGoal = privateGoalCheckbox.IsChecked;
                selectedGoal.TargetValue = (selectedGoal.SteByStepAmount + 1).ToString();
                selectedGoal.CurrentValue = ((step1CB.IsChecked ? 1 : 0) + (step2CB.IsChecked ? 1 : 0) + (step3CB.IsChecked ? 1 : 0) + (step4CB.IsChecked ? 1 : 0) + (step5CB.IsChecked ? 1 : 0) + (step6CB.IsChecked ? 1 : 0) +
                                            (step7CB.IsChecked ? 1 : 0) + (step8CB.IsChecked ? 1 : 0) + (step8CB.IsChecked ? 1 : 0)).ToString();

                selectedGoal.RepeatWeekly = weekdayPicker.SelectedIndex;
                selectedGoal.RepeatMonthly = dayOfMonthPicker.SelectedIndex;
                selectedGoal.SteByStepAmount = stepbystepPicker.SelectedIndex;
                selectedGoal.RepeatType = repeatableRB1.IsChecked ? 0 : 1;
                selectedGoal.WeeklyOrMonthly = repeatableRB21.IsChecked ? 0 : 1;

                selectedGoal.Checkbox1 = step1CB.IsChecked;
                selectedGoal.Checkbox1Comment = step1entry.Text;
                selectedGoal.Checkbox2 = step2CB.IsChecked;
                selectedGoal.Checkbox2Comment = step2entry.Text;
                selectedGoal.Checkbox3 = step3CB.IsChecked;
                selectedGoal.Checkbox3Comment = step3entry.Text;
                selectedGoal.Checkbox4 = step4CB.IsChecked;
                selectedGoal.Checkbox4Comment = step4entry.Text;
                selectedGoal.Checkbox5 = step5CB.IsChecked;
                selectedGoal.Checkbox5Comment = step5entry.Text;
                selectedGoal.Checkbox6 = step6CB.IsChecked;
                selectedGoal.Checkbox6Comment = step6entry.Text;
                selectedGoal.Checkbox7 = step7CB.IsChecked;
                selectedGoal.Checkbox7Comment = step7entry.Text;
                selectedGoal.Checkbox8 = step8CB.IsChecked;
                selectedGoal.Checkbox8Comment = step8entry.Text;
                selectedGoal.Checkbox9 = step9CB.IsChecked;
                selectedGoal.Checkbox9Comment = step9entry.Text;

                await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
                await DisplayAlert("Success", "Goal updated", "Ok");
                await Navigation.PopAsync();
            }

        }

        private async void deleteGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            bool delete = await DisplayAlert("Wait", "Are you sure you want to delete this goal?", "Yes", "No");
            if (delete)
            {
                await App.client.GetTable<GoalModel>().DeleteAsync(selectedGoal);
                await DisplayAlert("Sure", "Lets delete it", "Ok");
                await Navigation.PopAsync();
            }
        }

        private async void completeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            selectedGoal.LastUpdatedDate = DateTime.Now;
            selectedGoal.Completed = true;
            if (selectedGoal.RepeatType == 0)
            {
                selectedGoal.Closed = true;
            }
            selectedGoal.ClosedDate = DateTime.Now;

            await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
            await DisplayAlert("Congratulations", "Goal goal completed", "Ok");
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

        private async void closeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            bool delete = await DisplayAlert("Wait", "Are you sure you want to close this goal?", "Yes", "No");
            if (delete)
            {
                selectedGoal.Closed = true;
                selectedGoal.LastUpdatedDate = DateTime.Now;
                selectedGoal.ClosedDate = DateTime.Now;

                await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
                await DisplayAlert("Goal closed", "", "Ok");
                await Navigation.PopAsync();
            }
        }
    }
}
