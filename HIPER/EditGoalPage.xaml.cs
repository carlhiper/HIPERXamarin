using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class EditGoalPage : ContentPage
    {
        GoalModel selectedGoal;
        ChallengeModel challenge;

        public EditGoalPage(GoalModel inputGoal)
        {
            InitializeComponent();

            this.selectedGoal = inputGoal;

            weekdayPicker.ItemsSource = App.weekdays;
            dayOfMonthPicker.ItemsSource = App.daysofmonth;
            stepbystepPicker.ItemsSource = App.numberofsteps;

            goalNameEntry.Text = selectedGoal.Title;
            goalDescriptionEntry.Text = selectedGoal.Description;
            goalDeadlineEntry.Date = selectedGoal.Deadline;
            goalTargetEntry.Text = selectedGoal.TargetValue;
            goalCurrentEntry.Text = selectedGoal.CurrentValue;
            privateGoalCheckbox.IsChecked = selectedGoal.PrivateGoal;

            repeatableRB1.IsChecked = (selectedGoal.RepeatType == 0) ? true : false;
            repeatableRB2.IsChecked = (selectedGoal.RepeatType == 1) ? true : false;
            repeatableRB21.IsChecked = (selectedGoal.WeeklyOrMonthly == 0) ? true : false;
            repeatableRB22.IsChecked = (selectedGoal.WeeklyOrMonthly == 1) ? true : false;

            if (!string.IsNullOrEmpty(selectedGoal.ChallengeId))
            {
                privateGoalCheckbox.IsVisible = false;
                privateGoalLabel.IsVisible = false;
            }

            weekdayPicker.SelectedIndex = selectedGoal.RepeatWeekly;
            dayOfMonthPicker.SelectedIndex = selectedGoal.RepeatMonthly;

            if (selectedGoal.TargetType == 1)
            {
                gridSteps.IsVisible = true;
                gridLabel.IsVisible = true;
                aimHighLabel.IsVisible = false;
                gridAimHigh.IsVisible = false;

                stepbystepPicker.SelectedIndex = selectedGoal.StepByStepAmount;
                step1CB.IsChecked = selectedGoal.Checkbox1;
                step1CB.IsVisible = true;
                step1CB.IsEnabled = false;
                step1entry.Text = selectedGoal.Checkbox1Comment;
                step1entry.IsVisible = true;
                step1label.IsVisible = true;
                step2CB.IsChecked = selectedGoal.Checkbox2;
                step2CB.IsVisible = (selectedGoal.StepByStepAmount > 0) ? true : false;
                step2CB.IsEnabled = false;
                step2entry.Text = selectedGoal.Checkbox2Comment;
                step2entry.IsVisible = (selectedGoal.StepByStepAmount > 0) ? true : false;
                step2label.IsVisible = (selectedGoal.StepByStepAmount > 0) ? true : false;
                step3CB.IsChecked = selectedGoal.Checkbox3;
                step3CB.IsVisible = (selectedGoal.StepByStepAmount > 1) ? true : false;
                step3CB.IsEnabled = false;
                step3entry.Text = selectedGoal.Checkbox3Comment;
                step3entry.IsVisible = (selectedGoal.StepByStepAmount > 1) ? true : false;
                step3label.IsVisible = (selectedGoal.StepByStepAmount > 1) ? true : false;
                step4CB.IsChecked = selectedGoal.Checkbox4;
                step4CB.IsVisible = (selectedGoal.StepByStepAmount > 2) ? true : false;
                step4CB.IsEnabled = false;
                step4entry.Text = selectedGoal.Checkbox4Comment;
                step4entry.IsVisible = (selectedGoal.StepByStepAmount > 2) ? true : false;
                step4label.IsVisible = (selectedGoal.StepByStepAmount > 2) ? true : false;
                step5CB.IsChecked = selectedGoal.Checkbox5;
                step5CB.IsVisible = (selectedGoal.StepByStepAmount > 3) ? true : false;
                step5CB.IsEnabled = false;
                step5entry.Text = selectedGoal.Checkbox5Comment;
                step5entry.IsVisible = (selectedGoal.StepByStepAmount > 3) ? true : false;
                step5label.IsVisible = (selectedGoal.StepByStepAmount > 3) ? true : false;
                step6CB.IsChecked = selectedGoal.Checkbox6;
                step6CB.IsVisible = (selectedGoal.StepByStepAmount > 4) ? true : false;
                step6CB.IsEnabled = false;
                step6entry.Text = selectedGoal.Checkbox6Comment;
                step6entry.IsVisible = (selectedGoal.StepByStepAmount > 4) ? true : false;
                step6label.IsVisible = (selectedGoal.StepByStepAmount > 4) ? true : false;
                step7CB.IsChecked = selectedGoal.Checkbox7;
                step7CB.IsVisible = (selectedGoal.StepByStepAmount > 5) ? true : false;
                step7CB.IsEnabled = false;
                step7entry.Text = selectedGoal.Checkbox7Comment;
                step7entry.IsVisible = (selectedGoal.StepByStepAmount > 5) ? true : false;
                step7label.IsVisible = (selectedGoal.StepByStepAmount > 5) ? true : false;
                step8CB.IsChecked = selectedGoal.Checkbox8;
                step8CB.IsVisible = (selectedGoal.StepByStepAmount > 6) ? true : false;
                step9CB.IsEnabled = false;
                step8entry.Text = selectedGoal.Checkbox8Comment;
                step8entry.IsVisible = (selectedGoal.StepByStepAmount > 6) ? true : false;
                step8label.IsVisible = (selectedGoal.StepByStepAmount > 6) ? true : false;
                step9CB.IsChecked = selectedGoal.Checkbox9;
                step9CB.IsVisible = (selectedGoal.StepByStepAmount > 7) ? true : false;
                step9CB.IsEnabled = false;
                step9entry.Text = selectedGoal.Checkbox9Comment;
                step9entry.IsVisible = (selectedGoal.StepByStepAmount > 7) ? true : false;
                step9label.IsVisible = (selectedGoal.StepByStepAmount > 7) ? true : false;

            }
            else
            {
                gridSteps.IsVisible = false;
                gridLabel.IsVisible = false;
                aimHighLabel.IsVisible = true;
                gridAimHigh.IsVisible = true;
                goalCurrentEntry.IsEnabled = false;
            }

            if (!string.IsNullOrEmpty(selectedGoal.ChallengeId))
            {
                headerText.Text = "UPDATE CHALLENGE";
            }
            else
            {
                headerText.Text = "EDIT GOAL";
            }
        }

        private async void saveGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
            bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);
            bool isWeeklyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB21.IsChecked && (weekdayPicker.SelectedIndex < 0);
            bool isMonthlyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB22.IsChecked && (dayOfMonthPicker.SelectedIndex < 0);

            if (isGoalNameEmpty || isGoalDescriptionEmpty || isWeeklyCheckedAndEntryFilled || isMonthlyCheckedAndEntryFilled)
            {
                await DisplayAlert("Error", "All field needs to be entered", "Ok");
                return;
            }

            if (!string.IsNullOrEmpty(selectedGoal.ChallengeId))
            {
                try
                {
                    var goalsToUpdate = await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == selectedGoal.ChallengeId).ToListAsync();
                    foreach (GoalModel g in goalsToUpdate)
                    {
                        CopyViewToGoal(g);

                        await App.client.GetTable<GoalModel>().UpdateAsync(g);

                        if (g.UserId == App.loggedInUser.Id)
                        {
                            CopyViewToGoal(selectedGoal);
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                await DisplayAlert("Success", "Challenge updated", "Ok");
                await Navigation.PopAsync();

            }
            else
            {
                CopyViewToGoal(selectedGoal);
                await App.client.GetTable<GoalModel>().UpdateAsync(selectedGoal);
                await DisplayAlert("Success", "Goal updated", "Ok");
                await Navigation.PopAsync();
            }
        }

        private void CopyViewToGoal(GoalModel selectedGoal)
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
            selectedGoal.RepeatWeekly = weekdayPicker.SelectedIndex;
            selectedGoal.RepeatMonthly = dayOfMonthPicker.SelectedIndex;
            selectedGoal.RepeatType = repeatableRB1.IsChecked ? 0 : 1;
            selectedGoal.WeeklyOrMonthly = repeatableRB21.IsChecked ? 0 : 1;
    
            if (selectedGoal.TargetType == 1)
            {
                selectedGoal.StepByStepAmount = stepbystepPicker.SelectedIndex;
                selectedGoal.TargetValue = (selectedGoal.StepByStepAmount + 1).ToString();
                selectedGoal.Checkbox1Comment = step1entry.Text;
                //selectedGoal.Checkbox1 = step1CB.IsChecked;
                selectedGoal.Checkbox2Comment = step2entry.Text;
                //selectedGoal.Checkbox2 = step2CB.IsChecked;
                selectedGoal.Checkbox3Comment = step3entry.Text;
                //selectedGoal.Checkbox3 = step3CB.IsChecked;
                selectedGoal.Checkbox4Comment = step4entry.Text;
                //selectedGoal.Checkbox4 = step4CB.IsChecked;
                selectedGoal.Checkbox5Comment = step5entry.Text;
                //selectedGoal.Checkbox5 = step5CB.IsChecked;
                selectedGoal.Checkbox6Comment = step6entry.Text;
                //selectedGoal.Checkbox6 = step6CB.IsChecked;
                selectedGoal.Checkbox7Comment = step7entry.Text;
                //selectedGoal.Checkbox7 = step7CB.IsChecked;
                selectedGoal.Checkbox8Comment = step8entry.Text;
                //selectedGoal.Checkbox8 = step8CB.IsChecked;
                selectedGoal.Checkbox9Comment = step9entry.Text;
                //selectedGoal.Checkbox9 = step9CB.IsChecked;
            }
            else
            {
                selectedGoal.TargetValue = goalTargetEntry.Text;
            }
        }


        private async void deleteGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            if (!string.IsNullOrEmpty(selectedGoal.ChallengeId))
            {
                bool delete = await DisplayAlert("Wait", "Are you sure you want to delete this challenge? It will be removed for all participants", "Yes", "No");
                if (delete)
                {
                    try
                    {
                        var goalsToDelete = await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == selectedGoal.ChallengeId).ToListAsync();
                        foreach (GoalModel goal in goalsToDelete)
                        {
                            await App.client.GetTable<GoalModel>().DeleteAsync(goal);
                        }

                        var challengeToDelete = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == selectedGoal.ChallengeId).ToListAsync()).FirstOrDefault();
                        await App.client.GetTable<ChallengeModel>().DeleteAsync(challengeToDelete);
                    }
                    catch (Exception ex)
                    {
                    }
                    await DisplayAlert("Success", "Challenge deleted", "Ok");
                    Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                    await Navigation.PopAsync();
                }
            }
            else
            {
                try
                {
                    bool delete = await DisplayAlert("Wait", "Are you sure you want to delete this goal?", "Yes", "No");
                    if (delete)
                    {
                        await App.client.GetTable<GoalModel>().DeleteAsync(selectedGoal);
                        await DisplayAlert("Sure", "Lets delete it", "Ok");
                        Navigation.RemovePage(Navigation.NavigationStack[Navigation.NavigationStack.Count - 2]);
                        await Navigation.PopAsync();

                    }
                }
                catch (Exception ex)
                {

                }
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

        private void goalCurrentEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //checkGoalCompleted();
        }

        private void goalTargetEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            //   checkGoalCompleted();
        }

        void stepbystepPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            step1CB.IsVisible = (stepbystepPicker.SelectedIndex > -1);
            step1entry.IsVisible = (stepbystepPicker.SelectedIndex > -1);
            step1label.IsVisible = (stepbystepPicker.SelectedIndex > -1);
            step2CB.IsVisible = (stepbystepPicker.SelectedIndex > 0) ? true : false;
            step2entry.IsVisible = (stepbystepPicker.SelectedIndex > 0) ? true : false;
            step2label.IsVisible = (stepbystepPicker.SelectedIndex > 0) ? true : false;
            step3CB.IsVisible = (stepbystepPicker.SelectedIndex > 1) ? true : false;
            step3entry.IsVisible = (stepbystepPicker.SelectedIndex > 1) ? true : false;
            step3label.IsVisible = (stepbystepPicker.SelectedIndex > 1) ? true : false;
            step4CB.IsVisible = (stepbystepPicker.SelectedIndex > 2) ? true : false;
            step4entry.IsVisible = (stepbystepPicker.SelectedIndex > 2) ? true : false;
            step4label.IsVisible = (stepbystepPicker.SelectedIndex > 2) ? true : false;
            step5CB.IsVisible = (stepbystepPicker.SelectedIndex > 3) ? true : false;
            step5entry.IsVisible = (stepbystepPicker.SelectedIndex > 3) ? true : false;
            step5label.IsVisible = (stepbystepPicker.SelectedIndex > 3) ? true : false;
            step6CB.IsVisible = (stepbystepPicker.SelectedIndex > 4) ? true : false;
            step6entry.IsVisible = (stepbystepPicker.SelectedIndex > 4) ? true : false;
            step6label.IsVisible = (stepbystepPicker.SelectedIndex > 4) ? true : false;
            step7CB.IsVisible = (stepbystepPicker.SelectedIndex > 5) ? true : false;
            step7entry.IsVisible = (stepbystepPicker.SelectedIndex > 5) ? true : false;
            step7label.IsVisible = (stepbystepPicker.SelectedIndex > 5) ? true : false;
            step8CB.IsVisible = (stepbystepPicker.SelectedIndex > 6) ? true : false;
            step8entry.IsVisible = (stepbystepPicker.SelectedIndex > 6) ? true : false;
            step8label.IsVisible = (stepbystepPicker.SelectedIndex > 6) ? true : false;
            step9CB.IsVisible = (stepbystepPicker.SelectedIndex > 7) ? true : false;
            step9entry.IsVisible = (stepbystepPicker.SelectedIndex > 7) ? true : false;
            step9label.IsVisible = (stepbystepPicker.SelectedIndex > 7) ? true : false;
        }
    }
}
