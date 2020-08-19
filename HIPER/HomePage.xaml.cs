using System;
using System.Collections.Generic;
using HIPER.Controllers;
using Xamarin.Forms;

namespace HIPER
{
    public partial class HomePage : TabbedPage
    {
        public HomePage()
        {
            InitializeComponent();
            UpdateScoreboard.checkDeadlines();
            UpdateScoreboard.checkRepeatGoals();
            NavigationPage.SetHasBackButton(this, false);
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }
    }
}
