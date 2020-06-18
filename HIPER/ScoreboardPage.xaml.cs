using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;
using HIPER.Controllers;
using System.Linq;

namespace HIPER
{
    public partial class ScoreboardPage : ContentPage
    {
        List<GoalModel> activeGoals = new List<GoalModel>();
        List<GoalModel> closedGoals = new List<GoalModel>();

        public ScoreboardPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            createGoalsList(filter.SelectedIndex);

            filter.ItemsSource = App.filterOptions;

            CheckChat();
        }

        void goalCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedGoal = goalCollectionView.SelectedItem as GoalModel;
            Navigation.PushAsync(new GoalDetailPage(selectedGoal, false));
        }

        private void showClosedSwitch_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }

        private async void createGoalsList(int filter)
        {
            activeGoals.Clear();
            closedGoals.Clear();

            if (showClosedSwitch.IsToggled)
            {
                DateTime earliestDate = DateTime.Now.AddMonths(-15);
                closedGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (g.Closed || g.ClosedDate < DateTime.Now) && (g.ClosedDate > earliestDate)).OrderByDescending(g => g.ClosedDate).Take(500).ToListAsync();

                //Sorting
                if (filter == 0)
                {
                    closedGoals.Sort((x, y) => x.Title.CompareTo(y.Title));
                }
                else if (filter == 1)
                {
                    closedGoals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }
                else if (filter == 2)
                {
                    closedGoals.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
                }
                else if (filter == 3)
                {
                    closedGoals.Sort((x, y) => y.PerformanceIndicator.CompareTo(x.PerformanceIndicator));
                }
                else if (filter == 4)
                {
                    closedGoals.Sort((x, y) => y.Progress.CompareTo(x.Progress));
                }
                else
                {
                    closedGoals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }

                goalCollectionView.ItemsSource = closedGoals;
            }
            else
            {
                activeGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == App.loggedInUser.Id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderByDescending(g => g.CreatedDate).ToListAsync();

                //Sorting
                if (filter == 0)
                {
                    activeGoals.Sort((x, y) => x.Title.CompareTo(y.Title));
                }
                else if (filter == 1)
                {
                    activeGoals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }
                else if (filter == 2)
                {
                    activeGoals.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
                }
                else if (filter == 3)
                {
                    activeGoals.Sort((x, y) => y.PerformanceIndicator.CompareTo(x.PerformanceIndicator));
                }
                else if (filter == 4)
                {
                    activeGoals.Sort((x, y) => y.Progress.CompareTo(x.Progress));
                }
                else
                {
                    activeGoals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }

                goalCollectionView.ItemsSource = activeGoals;
            }
        }

        void filter_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createGoalsList(filter.SelectedIndex);
        }

        private async void addOwnGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddGoalPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        private async void AddNewGoalButton_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                await Navigation.PushAsync(new AddGoalPage());
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.ToString(), "Ok");
            }
        }

        private async void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new PostPage());

        }

        private async void CheckChat()
        {
            if (App.loggedInUser.TeamId != null)
            {
                ChatButton.IsEnabled = true;
                var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                List<PostModel> postCollection = new List<PostModel>();
                foreach (var user in users)
                {
                    var post = (await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).OrderByDescending(p => p.CreatedDate).ToListAsync()).FirstOrDefault();
                    if (post != null)
                    {
                        postCollection.Add(post);
                    }
                }
                postCollection.Sort((x, y) => y.CreatedDate.CompareTo(x.CreatedDate));
                if (postCollection[0].CreatedDate > App.loggedInUser.LastViewedPostDate)
                {
                    ChatButton.IconImageSource = "chat_ex.png";
                }
                else
                {
                    ChatButton.IconImageSource = "chat.png";
                }
            }
            else
            {
                ChatButton.IsEnabled = false;
            }
        }
    }
}
