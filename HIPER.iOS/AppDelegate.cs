using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Foundation;
using Microsoft.WindowsAzure.MobileServices;
using UIKit;

namespace HIPER.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.SetFlags("FastRenderers_Experimental");
            global::Xamarin.Forms.Forms.Init();
            CurrentPlatform.Init();

  
            string dbName = "hiper_db.sqlite";
            string folderPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "..", "Library");
            string fullPath = Path.Combine(folderPath, dbName);

            UINavigationBar.Appearance.TintColor = new UIColor(red: 1.00f, green: 0.46f, blue: 0.39f, alpha: 1.00f);

            // Color of the selected tab icon:
            UITabBar.Appearance.SelectedImageTintColor = new UIColor(red: 1.00f, green: 0.46f, blue: 0.39f, alpha: 1.00f);

            // Color of the selected tab text color:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    TextColor = new UIColor(red: 0.00f, green: 0.00f, blue: 0.00f, alpha: 1.00f)
                },
                UIControlState.Selected);

            // Color of the unselected tab icon & text:
            UITabBarItem.Appearance.SetTitleTextAttributes(
                new UITextAttributes()
                {
                    TextColor = UIColor.FromRGB(0, 0, 0)
                },
                UIControlState.Normal);


            LoadApplication(new App(fullPath));

            return base.FinishedLaunching(app, options);
        }
    }
}
