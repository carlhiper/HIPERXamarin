using System;
using System.Collections.Generic;
using HIPER.Model;
using System.Linq;

using Xamarin.Forms;

namespace HIPER
{

    public partial class LeaderBoardPage : ContentPage
    {
        GoalModel goal;
        List<LeaderBoardModel> competitors = new List<LeaderBoardModel>();

        public LeaderBoardPage(GoalModel selectedGoal)
        {
            this.goal = selectedGoal;
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                if (!string.IsNullOrEmpty(goal.ChallengeId))
                {
                    var goals = await App.client.GetTable<GoalModel>().Where(g => g.ChallengeId == goal.ChallengeId).ToListAsync();

                    if (goals != null)
                    {
                        

                        foreach (GoalModel g in goals)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.id == g.UserId).ToListAsync()).FirstOrDefault();

                            LeaderBoardModel competitor = new LeaderBoardModel()
                            {
                                Name = user.FirstName + " " + user.LastName,
                                Progress = g.Progress

                            };
                           
                            competitors.Add(competitor);
                  
                        }
                        competitors.Sort((x, y) => y.Progress.CompareTo(x.Progress));

                        for (int i = 1 ; i <=  competitors.Count; i++)
                        {
                            competitors[i - 1].Placing = i;
                        }
                    }
                    leaderboardCollectionView.ItemsSource = competitors;
                }
            }
            catch(Exception ex)
            {


            }
        }
    }
}
