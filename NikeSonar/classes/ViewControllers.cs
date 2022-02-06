using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using MonoTouch.UIKit;
using SlideDownMenu;

namespace NikeSonar
{
    class ViewControllers
    {
        public static string APIServer = "http://nikesonar.com/";
        public static string APIUser = "";
        public static string APIPass = "";
        public static string DeviceID = "";
        public static string APIUserPath = APIServer + "api/userfiles/" + APIUser + "/";
        public static string APIAccountsFile = "";
        public static LoginViewController LoginViewController;
        public static MainViewController MainViewController;
        public static AdvcancedSettingsVeiwController AdvcancedSettingsVeiwController;
        public static BotRunningViewController BotRunningViewController;
        public static RapidCheckoutViewController RapidCheckoutViewController;
        public static TwitterMoniterViewController TwitterMoniterViewController;
        public static AddAccountsViewController AddAccountsViewController;

        public static void SetAPIUserPath()
        {
            APIUserPath = APIServer + "api/userfiles/" + APIUser + "/";
        }

        public static void CreateMenu(UIViewController controller)
        {
            var item0 = new MenuItem("Menu", UIImage.FromBundle("images/menu.png"), (menuItem) =>
            {
                Console.WriteLine("Item: {0}", menuItem);
            });
            item0.Tag = 0;

            var item1 = new MenuItem("Bot Configuration", UIImage.FromBundle("images/bot.png"), (menuItem) =>
            {
                controller.TabBarController.SelectedViewController = ViewControllers.MainViewController;
            });
            item1.Tag = 1;

            var item2 = new MenuItem("Nike Accounts", UIImage.FromBundle("images/Nikelid.png"), (menuItem) =>
            {
                controller.TabBarController.SelectedViewController = ViewControllers.AddAccountsViewController;
            });
            item2.Tag = 2;

            var item3 = new MenuItem("Twitter Configuration", UIImage.FromBundle("images/twitter.png"), (menuItem) =>
            {
                controller.TabBarController.SelectedViewController = ViewControllers.TwitterMoniterViewController;
            });
            item3.Tag = 3;

            var item4 = new MenuItem("Advanced Configuration", UIImage.FromBundle("images/setting.png"), (menuItem) =>
            {
                controller.TabBarController.SelectedViewController = ViewControllers.AdvcancedSettingsVeiwController;
            });
            item4.Tag = 4;

            var item5 = new MenuItem("Logout", UIImage.FromBundle("images/logout.png"), (menuItem) =>
            {
                SonarSettings.loggedIn = false;
                controller.TabBarController.SelectedViewController = ViewControllers.LoginViewController;
            });
            item5.Tag = 5;

            var slideMenu = new SlideMenu(new List<MenuItem> { item0, item1, item2, item3, item4, item5 });
            List<MenuItemView> itemViews = (List<MenuItemView>)typeof(SlideMenu).GetField("itemViews", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(slideMenu);
            Console.WriteLine(itemViews.Count);
            foreach (var itemView in itemViews)
            {
                if (itemView.Item == item0)
                {
                    continue;
                }
                itemView.MenuItemDidAction = delegate(MenuItemView view)
                {
                    slideMenu.ToggleMenu();
                };
            }
            controller.View.AddSubview(slideMenu);      
        }
    }
}
