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
            Navigation.PushAsync(new AddGoalPage(user));
        }


        private async void createGoalsList(int filter)
        {
            List<GoalModel> activeGoals = new List<GoalModel>();
            List<GoalModel> closedGoals = new List<GoalModel>();

            if (user != null)
            {
                var goals = await App.client.GetTable<GoalModel>().Where(g => g.UserId == user.Id).ToListAsync();

           
                //Sorting
                if (filter == 0)
                {
                    goals.Sort((x, y) => x.Title.CompareTo(y.Title));
                }
                else if (filter == 1)
                {
                    goals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }
                else if (filter == 2)
                {
                    goals.Sort((x, y) => x.Deadline.CompareTo(y.Deadline));
                }
                else if (filter == 3)
                {
                    goals.Sort((x, y) => y.PerformanceIndicator.CompareTo(x.PerformanceIndicator));
                }
                else if (filter == 4)
                {
                    goals.Sort((x, y) => y.Progress.CompareTo(x.Progress));
                }
                else
                {
                    goals.Sort((x, y) => y.LastUpdatedDate.CompareTo(x.LastUpdatedDate));
                }

                if (completedSwitch.IsToggled)
                {
                    foreach (var item in goals)
                    {
                        if (item.Closed)
                        {
                            closedGoals.Add(item);
                        }
                    }
                    userDetailCollectionView.ItemsSource = closedGoals;
                }
                else
                {
                    foreach (var item in goals)
                    {
                        if (!item.Closed)
                        {
                            activeGoals.Add(item);
                        }
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
