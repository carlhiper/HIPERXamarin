﻿using System;
using System.Collections.Generic;
using HIPER.Model;
using SQLite;
using Xamarin.Forms;

namespace HIPER
{
    public partial class ScoreboardPage : ContentPage
    {
        public ScoreboardPage()
        {
            InitializeComponent();
        }

        void addGoal_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new AddGoalPage());
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
            {
                conn.CreateTable<Goal>();
                var goals = conn.Table<Goal>().ToList();
                //goalListView.ItemsSource = goals;
                //Not sure if I will use this -> List<Goal> selectedGoals = new List<Goal>;
                goalCollectionView.ItemsSource = goals;
                foreach (var item in goals)
                {
                    if (showCompletedSwitch.IsToggled)
                    {
                        goalCollectionView.IsVisible = item.completed;
                    }
                    else
                    {
                        goalCollectionView.IsVisible = !item.completed;
                    }
                }
            }
        }

        void goalCollectionView_SelectionChanged(System.Object sender, Xamarin.Forms.SelectionChangedEventArgs e)
        {
            var selectedGoal = goalCollectionView.SelectedItem as Goal;

            Navigation.PushAsync(new EditGoalPage(selectedGoal));
        }
    }
}
