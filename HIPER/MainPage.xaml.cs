using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HIPER
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        void createUserButton_Clicked(System.Object sender, System.EventArgs e)
        {
            Navigation.PushAsync(new CreateUserPage());
        }
        void loginButton_Clicked(System.Object sender, System.EventArgs e)
        {
            bool isLoginNameEntryEmpty = string.IsNullOrEmpty(loginNameEntry.Text);
            bool isPasswordEntryEmpty = string.IsNullOrEmpty(passwordEntry.Text);

            if (isLoginNameEntryEmpty || isPasswordEntryEmpty)
            {

            }
            else
            {
                Navigation.PushAsync(new HomePage());
            }
        }

    }
}
