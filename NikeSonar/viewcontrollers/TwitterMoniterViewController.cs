using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Drawing;
using AlertView;
using Tweetinvi;
using Tweetinvi.WebLogic;

namespace NikeSonar
{
    partial class TwitterMoniterViewController : UIViewController
    {
        private Tweetinvi.Core.Interfaces.Credentials.ITemporaryCredentials _applicationCredentials;
        public TwitterMoniterViewController(IntPtr handle)
            : base(handle)
        {
            ViewControllers.TwitterMoniterViewController = this;
        }

        public override void ViewDidAppear(bool animated)
        {
            webView.LoadHtmlString("<body bgcolor=rgb(33,47,53)>", null);
            if (NSUserDefaults.StandardUserDefaults.BoolForKey("TwitterSet"))
            {
                var alert = MBAlertView.AlertWithBody(
                  body: "Twitter is already configured are you sure you want to continue?",
                  buttonTitle: "Cancel",
                  handler: () => TabBarController.SelectedViewController = ViewControllers.MainViewController
                );

                alert.AddButtonWithText(
                  text: "Continue",
                  bType: MBAlertViewItemType.Destructive,
                  handler: () => Console.WriteLine("")
                );
                alert.AddToDisplayQueue();
            }
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        partial void btnGetDetails_TouchUpInside(UIButton sender)
        {
            _applicationCredentials = CredentialsCreator.GenerateApplicationCredentials("lMnRNIbYV8zAkm7T9qoNy90GM", "8cmnvehHa0iWpJh99Axq1eTuC1ukaMVwsliEEll38Ip4r8whPl");
            var url = CredentialsCreator.GetAuthorizationURL(_applicationCredentials);
            webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));
            webView.LoadFinished += new EventHandler(webView_LoadFinished);
            //Console.WriteLine(url);
        }

        void webView_LoadFinished(object sender, EventArgs e)
        {
            //Console.WriteLine("Page loaded");
            if (webView.Request.Url.ToString() == "https://api.twitter.com/oauth/authorize")
            {
                string code = webView.EvaluateJavascript("document.getElementsByTagName(\"code\")[0].innerHTML");
                //Console.WriteLine(webView.Request.Url + " - " + code);
                FinishCredintals(code);
            }
        }

        void FinishCredintals(string code)
        {
            var newCredentials = CredentialsCreator.GetCredentialsFromVerifierCode(code, _applicationCredentials);
            TwitterCredentials.ApplicationCredentials = newCredentials;
            //Console.WriteLine("Access Token = {0}", newCredentials.AccessToken);
            //Console.WriteLine("Access Token Secret = {0}", newCredentials.AccessTokenSecret);
            NSUserDefaults.StandardUserDefaults.SetString(newCredentials.ConsumerKey, "ConsumerKey");
            NSUserDefaults.StandardUserDefaults.SetString(newCredentials.ConsumerSecret, "ConsumerSecret");
            NSUserDefaults.StandardUserDefaults.SetString(newCredentials.AccessToken, "AccessToken");
            NSUserDefaults.StandardUserDefaults.SetString(newCredentials.AccessTokenSecret, "AccessTokenSecret");
            NSUserDefaults.StandardUserDefaults.SetBool(true, "TwitterSet");
            NSUserDefaults.StandardUserDefaults.Synchronize();
            webView.LoadHtmlString("<body bgcolor=rgb(33,47,53)>", null);
            MBHUDView.HudWithBody(
                body: "TwitterAPI Configured!",
                aType: MBAlertViewHUDType.ImagePositive,
                delay: 4.0f,
                showNow: true
                );
            TabBarController.SelectedViewController = ViewControllers.MainViewController;
        }
    }
}
