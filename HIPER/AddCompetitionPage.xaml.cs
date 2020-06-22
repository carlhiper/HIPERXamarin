using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Helpers;
using HIPER.Model;
using Microsoft.AppCenter.Crashes;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class AddCompetitionPage : ContentPage
    {
        UserModel user;
        List<UserModel> teammembers;
        ChallengeModel challenge;

        public AddCompetitionPage()
        {
            this.user = App.loggedInUser;
            InitializeComponent();
        }


        protected override void OnAppearing()
        {
            initPage();
            GetCompetitors();
            base.OnAppearing();
        }

        private void initPage()
        {
            weekdayPicker.ItemsSource = App.weekdays;
            dayOfMonthPicker.ItemsSource = App.daysofmonth;
            stepbystepPicker.ItemsSource = App.numberofsteps;
            repeatableRB1.IsChecked = true;
            repeatableRB21.IsChecked = true;
            targetRB1.IsChecked = true;
            step1CB.IsVisible = false;
            step1entry.IsVisible = false;
            step1label.IsVisible = false;
            challengeCollectionView.HeightRequest = 20;
        }


        private async void CreateGoal(string userId, bool accepted, string challengeId)
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

            GoalModel goal = new GoalModel()
            {
                Title = goalNameEntry.Text,
                Description = goalDescriptionEntry.Text,
                Deadline = deadLineDate,
                TargetValue = goalTargetEntry.Text,
                UserId = userId,
                GoalAccepted = accepted,
                ChallengeId = challengeId,
                CurrentValue = "0",
                ClosedDate = DateTime.MaxValue,
                CreatedDate = startDate,
                LastUpdatedDate = DateTime.Now,
                Progress = 0,
                GoalType = 2,
                TargetType = targetRB1.IsChecked ? 0 : 1,
                RepeatType = repeatableRB1.IsChecked ? 0 : 1,
                WeeklyOrMonthly = repeatableRB21.IsChecked ? 0 : 1,
                RepeatWeekly = weekdayPicker.SelectedIndex,
                RepeatMonthly = dayOfMonthPicker.SelectedIndex,
                StepByStepAmount = stepbystepPicker.SelectedIndex,

                Checkbox1 = step1CB.IsChecked,
                Checkbox1Comment = step1entry.Text,
                Checkbox2 = step2CB.IsChecked,
                Checkbox2Comment = step2entry.Text,
                Checkbox3 = step3CB.IsChecked,
                Checkbox3Comment = step3entry.Text,
                Checkbox4 = step4CB.IsChecked,
                Checkbox4Comment = step4entry.Text,
                Checkbox5 = step5CB.IsChecked,
                Checkbox5Comment = step5entry.Text,
                Checkbox6 = step6CB.IsChecked,
                Checkbox6Comment = step6entry.Text,
                Checkbox7 = step7CB.IsChecked,
                Checkbox7Comment = step7entry.Text,
                Checkbox8 = step8CB.IsChecked,
                Checkbox8Comment = step8entry.Text,
                Checkbox9 = step9CB.IsChecked,
                Checkbox9Comment = step9entry.Text

            };

            //if (goal.RepeatMonthly == 28) // Last day of month
            //{
            //    goal.RepeatMonthly = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //}

            await App.client.GetTable<GoalModel>().InsertAsync(goal);
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

        private async void GetCompetitors()
        {
            try
            {
                if (teammembers == null)
                {
                    teammembers = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                }
                challengeCollectionView.ItemsSource = teammembers;
            }
            catch (Exception ex) {
                var properties = new Dictionary<string, string> {
                { "AddCompetitionPage", "checkDeadlines" }};
                Crashes.TrackError(ex, properties);
            }
            challengeCollectionView.IsVisible = true;
            challengeCollectionView.HeightRequest = 20 * teammembers.Count;
        }


        private async void startCompetition_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                bool isGoalNameEmpty = string.IsNullOrEmpty(goalNameEntry.Text);
                bool isGoalDescriptionEmpty = string.IsNullOrEmpty(goalDescriptionEntry.Text);
                bool isTargetCheckedAndEntryFilled = targetRB1.IsChecked && string.IsNullOrEmpty(goalTargetEntry.Text);
                bool isStepbyStepCheckedAndEntryFilled = targetRB2.IsChecked && (stepbystepPicker.SelectedIndex < 0);
                bool isWeeklyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB21.IsChecked && (weekdayPicker.SelectedIndex < 0);
                bool isMonthlyCheckedAndEntryFilled = repeatableRB2.IsChecked && repeatableRB22.IsChecked && (dayOfMonthPicker.SelectedIndex < 0);

                if (isGoalNameEmpty || isGoalDescriptionEmpty || isTargetCheckedAndEntryFilled || isStepbyStepCheckedAndEntryFilled || isWeeklyCheckedAndEntryFilled || isMonthlyCheckedAndEntryFilled)
                {
                    await DisplayAlert("Error", "All field needs to be entered", "Ok");
                    return;
                }

                challenge = new ChallengeModel()
                {
                    OwnerId = App.loggedInUser.Id,
                    CreatedDate = DateTime.Now
                };
                await App.client.GetTable<ChallengeModel>().InsertAsync(challenge);

                var challenges = (await App.client.GetTable<ChallengeModel>().Where(c => c.OwnerId == App.loggedInUser.Id).ToListAsync());

                challenges.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
                challenge = challenges[0];

                var challengedUsers = challengeCollectionView.SelectedItems;

                foreach (UserModel user in challengedUsers)
                {
                    CreateGoal(user.Id, true, challenge.Id);
                }

                await DisplayAlert("Success", "Competition started and sent to selected team members", "Ok");
                await Navigation.PopAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Something went wrong, please try again", "Ok");
                var properties = new Dictionary<string, string> {
                { "UpdateScoreboard", "startCompetition_Clicked" }};
                Crashes.TrackError(ex, properties);
            }
        }
    }
}
