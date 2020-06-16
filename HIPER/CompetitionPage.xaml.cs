using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;
using HIPER.Controllers;
using System.Linq;

namespace HIPER
{
    public partial class CompetitionPage : ContentPage
    {
        public CompetitionPage()
        {
            InitializeComponent();
        }

        private async void addGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new AddCompetitionPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            createGoalsList(filter.SelectedIndex);

            filter.ItemsSource = App.filterOptions;
        }

        private async void competitionCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedGoal = competitionCollectionView.SelectedItem as GoalModel;
            await Navigation.PushAsync(new GoalDetailPage(selectedGoal, true));
        }

        private void showClosedSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }

        private async void createGoalsList(int filter)
        {
            List<GoalModel> activeCompetitions = new List<GoalModel>();
            List<GoalModel> closedCompetitions = new List<GoalModel>();
            List<GoalModel> competitions = new List<GoalModel>();

            var challenges = await App.client.GetTable<ChallengeModel>().Where(c => c.OwnerId == App.loggedInUser.Id).ToListAsync();
        
            if (challenges != null)
            {
                foreach (var c in challenges)
                {
                    var goal = (await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == c.Id).ToListAsync()).FirstOrDefault();
                    if (goal != null)
                        if (goal.GoalType == 2)
                            competitions.Add(goal);

                }
            }



          
            //Sorting
            if (filter == 0)
            {
                competitions.Sort((x, y) => x.Title.CompareTo(y.Title));
            }
            else if (filter == 1)
            {
                competitions.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
            }
            else if (filter == 2)
            {
                competitions.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
            }
            else if (filter == 3)
            {
                competitions.Sort((x, y) => y.PerformanceIndicator.CompareTo(x.PerformanceIndicator));
            }
            else if (filter == 4)
            {
                competitions.Sort((x, y) => y.Progress.CompareTo(x.Progress));
            }
            else
            {
                competitions.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
            }

            if (showClosedSwitch.IsToggled)
            {
                foreach (var item in competitions)
                {
                    if (item.Closed || item.Completed || item.ClosedDate < DateTime.Now)
                    {
                        closedCompetitions.Add(item);
                    }
                }
                competitionCollectionView.ItemsSource = closedCompetitions;
            }
            else
            {
                foreach (var item in competitions)
                {
                    if (!item.Closed && !item.Completed && !(item.ClosedDate < DateTime.Now))
                    {
                        activeCompetitions.Add(item);
                    }
                }
                competitionCollectionView.ItemsSource = activeCompetitions;
            }
        }

        void filter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }
    }
}
