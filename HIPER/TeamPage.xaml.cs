using System;
using System.Collections.Generic;
using System.Linq;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;
using HIPER.Helpers;
using Microsoft.AppCenter.Crashes;
using System.Threading.Tasks;

namespace HIPER
{
    public partial class TeamPage : ContentPage
    {
        TeamModel viewedTeam = new TeamModel();
        int IndexOfViewedTeam = 0;
        List<TeamModel> teams = new List<TeamModel>();
        List<TeamsModel> teamsModelObj = new List<TeamsModel>();
        int index = 0;

        public TeamPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTeamView();

            CheckChat();
        }

        private async Task LoadTeams()
        {
            try
            {
                teamsModelObj = await App.client.GetTable<TeamsModel>().Where(t => t.UserId == App.loggedInUser.Id).ToListAsync();
                if (teamsModelObj.Count > 0)
                {
                    teams.Clear();
                    index = 0;
                    foreach (var t in teamsModelObj)
                    {
                        var t_n = (await App.client.GetTable<TeamModel>().Where(t_id => t_id.Id == t.TeamId).ToListAsync()).FirstOrDefault();
                        if(t_n != null)
                        {
                            teams.Add(t_n);
                            if (App.loggedInUser.TeamId == t.TeamId)
                                IndexOfViewedTeam = index;
                            index++;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "Team page", "LoadTeams" }};
                Crashes.TrackError(ex, properties);
            }
        }

        private async Task LoadTeamView()
        {
            try
            {
                await LoadTeams();

                viewedTeam = teams[IndexOfViewedTeam]; //(await App.client.GetTable<TeamModel>().Where(t => t.Id == App.loggedInUser.TeamId).ToListAsync()).FirstOrDefault();
                if (viewedTeam != null)
                {
                    teamStats.IsEnabled = true;
                    if (viewedTeam.Administrator_id == App.loggedInUser.Id)
                    {
                        teamStats.IsVisible = true;
                        competitionButton.IsVisible = true;
                    }
                    else
                    {
                        teamStats.IsVisible = false;
                        competitionButton.IsVisible = false;
                    }

                    var teamAdmin = (await App.client.GetTable<UserModel>().Where(u => u.Id == viewedTeam.Administrator_id).ToListAsync()).FirstOrDefault();

                    var teammembers = await App.client.GetTable<TeamsModel>().Where(t => t.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    List<UserModel> users = new List<UserModel>();
                    if (teammembers.Count > 0)
                    {
                        foreach (var member in teammembers)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == member.UserId).ToListAsync()).FirstOrDefault();
                            users.Add(user);
                        }
                    }
                    //var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == viewedTeam.Id).ToListAsync();
                    if (users != null)
                    {
                        users.RemoveAt(users.FindIndex(a => a.Id == App.loggedInUser.Id));

                        teamCollectionView.ItemsSource = users;

                        teamNameLabel.Text = viewedTeam.Name.ToUpper();
                        teamAdminLabel.Text = "Administrator: " + teamAdmin.FirstName + " " + teamAdmin.LastName;
                        teamIdentifierLabel.Text = "Identifier: " + viewedTeam.Identifier;
                        registerredLabel.Text = "Registerred: " + String.Format("{0:yyyy-MM-dd}", viewedTeam.CreatedDate);

                    }
                }
                else
                {
                    teamStats.IsEnabled = false;
                    competitionButton.IsEnabled = false;
                    teamStats.IsVisible = false;
                    competitionButton.IsVisible = false;
                    membersButton.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "Team page", "LoadTeamView" }};
                Crashes.TrackError(ex, properties);

            }
            UpdateButtons();


        }

        private void UpdateButtons()
        {
            if (teams.Count > 1)
            {
                arrow_left.IsVisible = true;
                arrow_right.IsVisible = true;
            }
            else
            {
                arrow_left.IsVisible = false;
                arrow_right.IsVisible = false;
            }

            if (teams.Count > 0)
            {
                leaveTeam.IsVisible = true;
            }
            else
            {
                leaveTeam.IsVisible = false;
            }

            if (teams.Count < Helpers.Constants.MAX_TEAMS)
            {
                createTeam.IsVisible = true;
                joinTeam.IsVisible = true;
            }
            else
            {
                createTeam.IsVisible = false;
                joinTeam.IsVisible = false;
            }
        }

        void teamCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            if (App.loggedInUser.Id == viewedTeam.Administrator_id)
            {
                var selectedUser = teamCollectionView.SelectedItem as UserModel;
                Navigation.PushAsync(new UserDetailPage(selectedUser));
            }
        }

        void joinTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new JoinTeamPage());
        }

        void createTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateTeamPage());
        }

        private async void leaveTeam_Clicked(System.Object sender, System.EventArgs e)
        {
            try
            {
                if (viewedTeam.Administrator_id == App.loggedInUser.Id)
                {
                    var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == viewedTeam.Id).ToListAsync();
                    if (users.Count > 1)
                    {
                        await DisplayAlert("Failure", "You are team administrator and can not leave as long as there are members in your team", "Ok");
                    }
                    else
                    {
                        bool deleteTeam = await DisplayAlert("Warning", "You are about to delete your team. Are you sure?", "Yes", "No");
                        if (deleteTeam)
                        {

                            await App.client.GetTable<TeamsModel>().DeleteAsync(teamsModelObj[IndexOfViewedTeam]);

                            // Remove team id from user
                            if (teams.Count > 1)
                            {
                                if (IndexOfViewedTeam == 0)
                                    IndexOfViewedTeam++;
                                else
                                    IndexOfViewedTeam--;

                                App.loggedInUser.TeamId = teams[IndexOfViewedTeam].Id;
                                teamNameLabel.Text = teams[IndexOfViewedTeam].Name.ToUpper();
                                teamIdentifierLabel.Text = teams[IndexOfViewedTeam].Identifier;
                            }
                            else
                            {
                                App.loggedInUser.TeamId = null;
                                teamNameLabel.Text = "";
                                teamIdentifierLabel.Text = "";
                            }

                      
                            await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                            // Delete team from databases
                            await App.client.GetTable<TeamModel>().DeleteAsync(viewedTeam);
                            await LoadTeamView();


                            await DisplayAlert("Success", "You have left the team", "Ok");
                        }
                    }
                }
                else
                {
                    bool leaveTeam = await DisplayAlert("Warning", "You are about to leave your team. Are you sure?", "Yes", "No");
                    if (leaveTeam)
                    {
                        await App.client.GetTable<TeamsModel>().DeleteAsync(teamsModelObj[IndexOfViewedTeam]);

                        // Remove team id from user
                        if (teams.Count > 1)
                        {
                            if (IndexOfViewedTeam == 0)
                                IndexOfViewedTeam++;
                            else
                                IndexOfViewedTeam--;

                            App.loggedInUser.TeamId = teams[IndexOfViewedTeam].Id;
                            teamNameLabel.Text = teams[IndexOfViewedTeam].Name.ToUpper();
                            teamIdentifierLabel.Text = teams[IndexOfViewedTeam].Identifier;
                        }
                        else
                        {
                            App.loggedInUser.TeamId = null;
                            teamNameLabel.Text = "";
                            teamIdentifierLabel.Text = "";
                        }

                        await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);

                        await LoadTeamView();


                        await DisplayAlert("Success", "You have left the team", "Ok");
                    }
                }
            }
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                    { "Team page", "OnAppearing" }};
                Crashes.TrackError(ex, properties);

            }
        }

        private async void CheckChat()
        {
            try
            {
                if (App.loggedInUser.TeamId != null)
                {
                    ChatButton.IsEnabled = true;
                    List<UserModel> users = new List<UserModel>();
                    var teammembers = await App.client.GetTable<TeamsModel>().Where(t => t.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    if (teammembers.Count > 0)
                    {
                        foreach (var member in teammembers)
                        {
                            var user = (await App.client.GetTable<UserModel>().Where(u => u.Id == member.UserId).ToListAsync()).FirstOrDefault();
                            users.Add(user);
                        }
                    }
                    //var users = await App.client.GetTable<UserModel>().Where(u => u.TeamId == App.loggedInUser.TeamId).ToListAsync();
                    List<PostModel> postCollection = new List<PostModel>();
                    foreach (var user in users)
                    {
                        var post = (await App.client.GetTable<PostModel>().Where(p => p.UserId == user.Id).Where(p2 => p2.TeamId == App.loggedInUser.TeamId).OrderByDescending(p => p.CreatedDate).ToListAsync()).FirstOrDefault();
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
            catch (Exception ex)
            {
                var properties = new Dictionary<string, string> {
                { "Team page", "Check chat" }};
                Crashes.TrackError(ex, properties);
            }
        }

        void teamStats_Clicked(System.Object sender, System.EventArgs e)
        {
            if(viewedTeam != null)
                Navigation.PushAsync(new StatisticsPage(viewedTeam));
        }

        void competitionButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CompetitionPage());
        }

        private async void ChatButton_Clicked(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new PostPage());
        }

        private async void arrow_left_Clicked(System.Object sender, System.EventArgs e)
        {
            if (teams.Count > 1)
            {
                IndexOfViewedTeam--;
                if (IndexOfViewedTeam < 0)
                    IndexOfViewedTeam = teams.Count-1;
                viewedTeam = teams[IndexOfViewedTeam];
                App.loggedInUser.TeamId = viewedTeam.Id;
                //App.loggedInUser.LastViewedPostDate = teamsModelObj[IndexOfViewedTeam].LastViewedPostDate;
                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                await LoadTeamView();
                CheckChat();
            }
        }

        private async void arrow_right_Clicked(System.Object sender, System.EventArgs e)
        {
            if (teams.Count > 1)
            {
                IndexOfViewedTeam++;
                if (IndexOfViewedTeam >= teams.Count)
                    IndexOfViewedTeam = 0;
                viewedTeam = teams[IndexOfViewedTeam];
                App.loggedInUser.TeamId = viewedTeam.Id;
               // App.loggedInUser.LastViewedPostDate = teamsModelObj[IndexOfViewedTeam].LastViewedPostDate;
                await App.client.GetTable<UserModel>().UpdateAsync(App.loggedInUser);
                await LoadTeamView();
                CheckChat();
            }
        }
    }
}
