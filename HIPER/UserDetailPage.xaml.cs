using System;
using System.Collections.Generic;
using HIPER.Helpers;
using HIPER.Model;
using Xamarin.Forms;

namespace HIPER
{
    public partial class UserDetailPage : ContentPage
    {
        UserModel user;
        List<GoalModel> activeGoals = new List<GoalModel>();
        List<GoalModel> closedGoals = new List<GoalModel>();


        public UserDetailPage()
        {
            InitializeComponent();
        }

        public UserDetailPage(UserModel selectedUser)
        {
            InitializeComponent();
            this.user = selectedUser;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            titleLabel.Text = (user.FirstName + " " + user.LastName).ToUpper();
            companyLabel.Text = user.Company;
            emailLabel.Text = user.Email;
            profileImage.Source = user.ImageUrl;

            filterPicker.ItemsSource = App.filterOptions;
            createGoalsList(1);
        }

        void addGoalButton_Clicked(System.Object sender, System.EventArgs e)
        {
            if (activeGoals != null)
            {
                if (activeGoals.Count < HIPER.Helpers.Constants.MAX_ACTIVE_GOALS)
                {
                    Navigation.PushAsync(new AddGoalPage(user));
                }
                else
                {
                    DisplayAlert("Failure", "The user has max allowed active goals. Please upgrade to premium to be able to add more goals", "Ok");
                }
            }
        }


        private async void createGoalsList(int filter)
        {
            activeGoals.Clear();
            closedGoals.Clear();

            if (user != null)
            {
            
                if (completedSwitch.IsToggled)
                {
                    closedGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.id && (g.Closed || g.ClosedDate < DateTime.Now)).OrderByDescending(g => g.ClosedDate).ToListAsync();

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

                    userDetailCollectionView.ItemsSource = closedGoals;
                }
                else
                {
                    activeGoals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.id && (!g.Closed && !g.Completed && g.ClosedDate > DateTime.Now)).OrderByDescending(g => g.CreatedDate).ToListAsync();

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

                    userDetailCollectionView.ItemsSource = activeGoals;
                }
            }
        }

        void completedSwitch_PropertyChanged(System.Object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            createGoalsList(1);
        }

        private async void removeFromTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            bool kickout = await DisplayAlert("Warning", "Are you sure you want to kick this user out from your team?", "Yes", "No");

            if (kickout)
            {
                user.TeamId = null;
                await App.client.GetTable<UserModel>().UpdateAsync(user);
                await Navigation.PopAsync();
            }
        }
        void statsButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new StatisticsPage(user));
        }

        void filterPicker_SelectedIndexChanged(System.Object sender, System.EventArgs e)
        {
            createGoalsList(filterPicker.SelectedIndex);
        }

    }
}
