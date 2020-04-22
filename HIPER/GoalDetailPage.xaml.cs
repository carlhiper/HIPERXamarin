using System;
using System.Collections.Generic;
using HIPER.Model;
using Xamarin.Forms;
using HIPER.Controllers;
using System.Linq;

namespace HIPER
{
    public partial class GoalDetailPage : ContentPage
    {
        bool constructorRunning;
        GoalModel goal;
        ChallengeModel challenge;
        List<LeaderBoardModel> competitors = new List<LeaderBoardModel>();

        public GoalDetailPage(GoalModel inputGoal)
        {
            this.goal = inputGoal;
            InitializeComponent();

            UpdateDetailView();
            UpdateLeaderBoard();

        }

        private void UpdateDetailView()
        {
            constructorRunning = true;
            headerText.Text = goal.Title;
            goalDescriptionLabel.Text = goal.Description;
            goalTargetLabel.Text = goal.TargetValue;
            goalCurrentEntry.Text = goal.CurrentValue;
            deadlineLabel.Text = goal.Deadline.ToShortDateString();


            if (goal.Completed || goal.Closed)
            {
                gridAimHigh.IsEnabled = false;
                gridSteps.IsEnabled = false;
                completeGoal.IsVisible = false;
                closeGoal.IsVisible = false;
            }

            if (string.IsNullOrEmpty(goal.ChallengeId))
            {
                leaderboardCollectionView.IsVisible = false;
                leaderBoardLabel.IsVisible = false;
                cardFrame.BorderColor = Color.FromHex(HIPER.Helpers.Constants.HIPER_PEACH);
                challengeImage.IsVisible = false;
            }
            else
            {
                leaderboardCollectionView.IsVisible = true;
                leaderBoardLabel.IsVisible = true;
                cardFrame.BorderColor = Color.FromHex(HIPER.Helpers.Constants.CHALLENGE_GOLD);
                challengeImage.IsVisible = true;
            }

            if (goal.TargetType == 1)
            {
                gridAimHigh.IsVisible = false;
                aimHighLabel.IsVisible = false;
                gridSteps.IsVisible = true;
                stepsLabel.IsVisible = true;

                step1CB.IsChecked = goal.Checkbox1;
                step1CB.IsVisible = true;
                step1entry.Text = goal.Checkbox1Comment;
                step1entry.IsVisible = true;
                step1label.IsVisible = true;
                step2CB.IsChecked = goal.Checkbox2;
                step2CB.IsVisible = (goal.SteByStepAmount > 0) ? true : false;
                step2entry.Text = goal.Checkbox2Comment;
                step2entry.IsVisible = (goal.SteByStepAmount > 0) ? true : false;
                step2label.IsVisible = (goal.SteByStepAmount > 0) ? true : false;
                step3CB.IsChecked = goal.Checkbox3;
                step3CB.IsVisible = (goal.SteByStepAmount > 1) ? true : false;
                step3entry.Text = goal.Checkbox3Comment;
                step3entry.IsVisible = (goal.SteByStepAmount > 1) ? true : false;
                step3label.IsVisible = (goal.SteByStepAmount > 1) ? true : false;
                step4CB.IsChecked = goal.Checkbox4;
                step4CB.IsVisible = (goal.SteByStepAmount > 2) ? true : false;
                step4entry.Text = goal.Checkbox4Comment;
                step4entry.IsVisible = (goal.SteByStepAmount > 2) ? true : false;
                step4label.IsVisible = (goal.SteByStepAmount > 2) ? true : false;
                step5CB.IsChecked = goal.Checkbox5;
                step5CB.IsVisible = (goal.SteByStepAmount > 3) ? true : false;
                step5entry.Text = goal.Checkbox5Comment;
                step5entry.IsVisible = (goal.SteByStepAmount > 3) ? true : false;
                step5label.IsVisible = (goal.SteByStepAmount > 3) ? true : false;
                step6CB.IsChecked = goal.Checkbox6;
                step6CB.IsVisible = (goal.SteByStepAmount > 4) ? true : false;
                step6entry.Text = goal.Checkbox6Comment;
                step6entry.IsVisible = (goal.SteByStepAmount > 4) ? true : false;
                step6label.IsVisible = (goal.SteByStepAmount > 4) ? true : false;
                step7CB.IsChecked = goal.Checkbox7;
                step7CB.IsVisible = (goal.SteByStepAmount > 5) ? true : false;
                step7entry.Text = goal.Checkbox7Comment;
                step7entry.IsVisible = (goal.SteByStepAmount > 5) ? true : false;
                step7label.IsVisible = (goal.SteByStepAmount > 5) ? true : false;
                step8CB.IsChecked = goal.Checkbox8;
                step8CB.IsVisible = (goal.SteByStepAmount > 6) ? true : false;
                step8entry.Text = goal.Checkbox8Comment;
                step8entry.IsVisible = (goal.SteByStepAmount > 6) ? true : false;
                step8label.IsVisible = (goal.SteByStepAmount > 6) ? true : false;
                step9CB.IsChecked = goal.Checkbox9;
                step9CB.IsVisible = (goal.SteByStepAmount > 7) ? true : false;
                step9entry.Text = goal.Checkbox9Comment;
                step9entry.IsVisible = (goal.SteByStepAmount > 7) ? true : false;
                step9label.IsVisible = (goal.SteByStepAmount > 7) ? true : false;
            }
            else
            {
                gridAimHigh.IsVisible = true;
                aimHighLabel.IsVisible = true;
                gridSteps.IsVisible = false;
                stepsLabel.IsVisible = false;
            }
            constructorRunning = false;
        }

  
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            UpdateDetailView();

            if (goal.Closed || goal.Completed)
            {
                editGoal.IsEnabled = false;
            }
            else if (!string.IsNullOrEmpty(goal.ChallengeId))
            {
                challenge = (await App.client.GetTable<ChallengeModel>().Where(c => c.Id == goal.ChallengeId).ToListAsync()).FirstOrDefault();
                if ((challenge.OwnerId == App.loggedInUser.Id))
                {
                    editGoal.IsEnabled = true;
                }
                else
                {
                    editGoal.IsEnabled = false;
                }
            }
            else
            {
                editGoal.IsEnabled = true;
            }
            UpdateLeaderBoard();
   
        }



        private async void UpdateLeaderBoard()
        {
            try
            {
                if (!string.IsNullOrEmpty(goal.ChallengeId))
                {
                    competitors.Clear();
                    var goals = await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == goal.ChallengeId).ToListAsync();
                    
                    if (goals != null)
                    {
                        foreach (GoalModel g in goals)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == g.UserId).ToListAsync()).FirstOrDefault();

                            LeaderBoardModel competitor = new LeaderBoardModel()
                            {
                                Name = user.FirstName + " " + user.LastName,
                                Progress = g.Progress,
                                Completed = g.Completed
                            };

                            competitors.Add(competitor);

                        }
                        competitors.Sort((x, y) => y.Progress.CompareTo(x.Progress));

                        for (int i = 1; i <= competitors.Count; i++)
                        {
                            competitors[i - 1].Placing = i;
                        }
                        leaderboardCollectionView.ItemsSource = competitors;
                        leaderboardCollectionView.HeightRequest = competitors.Count * 50 + 20;

                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        protected override bool OnBackButtonPressed()
        {

            return base.OnBackButtonPressed();
        }

 
        void editGoal_Clicked(System.Object sender, System.EventArgs e) {

            Navigation.PushAsync(new EditGoalPage(goal));

        }

        private async void goalCurrentEntry_TextChanged(System.Object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!constructorRunning)
            {
                try
                {
                    goal.CurrentValue = goalCurrentEntry.Text;

                    await App.client.GetTable<GoalModel>().UpdateAsync(goal);
                }
                catch(Exception ex)
                {

                }
            }   
        }

        private async void completeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            goal.Completed = true;
            goal.LastUpdatedDate = DateTime.Now;
            if (goal.RepeatType == 0)
            {
                goal.Closed = true;
            }
            goal.ClosedDate = DateTime.Now;

            await App.client.GetTable<GoalModel>().UpdateAsync(goal);
            await DisplayAlert("Congratulations", "Goal completed", "Ok");
            await Navigation.PopAsync();
        }

        private async void closeGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            bool delete = await DisplayAlert("Wait", "Are you sure you want to close this goal?", "Yes", "No");
            if (delete)
            {
                goal.Closed = true;
                goal.LastUpdatedDate = DateTime.Now;
                goal.ClosedDate = DateTime.Now;

                await App.client.GetTable<GoalModel>().UpdateAsync(goal);
                await DisplayAlert("Goal closed", "", "Ok");
                await Navigation.PopAsync();
            }
        }


        private async void step1CB_CheckedChanged(System.Object sender, Xamarin.Forms.CheckedChangedEventArgs e)
        {
            if (!constructorRunning)
            {
                goal.Checkbox1 = step1CB.IsChecked;
                goal.Checkbox2 = step2CB.IsChecked;
                goal.Checkbox3 = step3CB.IsChecked;
                goal.Checkbox4 = step4CB.IsChecked;
                goal.Checkbox5 = step5CB.IsChecked;
                goal.Checkbox6 = step6CB.IsChecked;
                goal.Checkbox7 = step7CB.IsChecked;
                goal.Checkbox8 = step8CB.IsChecked;
                goal.Checkbox9 = step9CB.IsChecked;

                goal.TargetValue = (goal.SteByStepAmount + 1).ToString();
                goal.CurrentValue = ((step1CB.IsChecked ? 1 : 0) + (step2CB.IsChecked ? 1 : 0) + (step3CB.IsChecked ? 1 : 0) + (step4CB.IsChecked ? 1 : 0) + (step5CB.IsChecked ? 1 : 0) + (step6CB.IsChecked ? 1 : 0) +
                (step7CB.IsChecked ? 1 : 0) + (step8CB.IsChecked ? 1 : 0) + (step8CB.IsChecked ? 1 : 0)).ToString();

                await App.client.GetTable<GoalModel>().UpdateAsync(goal);
            }
        }

    }
}
