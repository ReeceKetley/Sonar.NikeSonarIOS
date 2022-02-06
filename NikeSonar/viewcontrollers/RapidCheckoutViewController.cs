using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using AlertView;


namespace NikeSonar
{
    partial class RapidCheckoutViewController : UIViewController
    {
        private string _nikeUsername;
        private string _nikePassword;
        private string _loginUrl = "http://m.nike.com/us/en_us/?l=shop,login_register";
        private string _logoutUrl = "https://www.nike.com/profile/services/logout";
        private string _nikeHome = "http://m.nike.com/us/en_us/";
        private string _nikeCart =
            "https://secure-store.nike.com/us/checkout/mobile/cart.jsp?country=US&country=US&l=cart";

        public int iStage = 0;
        public bool bLoaded;
        public UIWebView webUi;
        public RapidCheckoutViewController(IntPtr handle)
            : base(handle)
        {
            ViewControllers.RapidCheckoutViewController = this;
            //TabBarController.TabBar.Hidden = true;
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override void ViewDidAppear(bool animated)
        {
            UIScrollView scrollView;
            TabBarController.TabBar.Hidden = true;
            btnFinish.TouchUpInside += new EventHandler(btnFinish_TouchUpInside);
            NavigationController.SetNavigationBarHidden(true, true);
            base.ViewDidAppear(animated);
            RectangleF newBounds = this.View.Bounds; newBounds.Y = newBounds.Y + 82; newBounds.Height = newBounds.Height - 205;
            webUi = new UIWebView(newBounds); webUi.ScrollView.ContentInset = new UIEdgeInsets(-60, 0, 0, 0);
            webUi.ScrollView.ScrollEnabled = true;
            webUi.ContentMode = UIViewContentMode.ScaleToFill;
            View.AddSubview(webUi);
            webUi.LoadFinished += new EventHandler(webUi_LoadFinished);
            Thread checkThread = new Thread(CheckoutThread);
            checkThread.Start();
            webUi.ScrollView.ScrollEnabled = true;
        }

        public void CheckoutThread()
        {
            for (; ; )
            {
                if (iStage == 0)
                {
                    bLoaded = false;
                    InvokeOnMainThread(() =>
                    {
                        webUi.UserInteractionEnabled = false;
                        webUi.LoadRequest(new NSUrlRequest(new NSUrl(_logoutUrl)));
                        ++iStage;
                    });
                }
                if (iStage == 1 && bLoaded)
                {
                    bLoaded = false;
                    InvokeOnMainThread(() =>
                    {
                        webUi.LoadRequest(new NSUrlRequest(new NSUrl(_loginUrl)));
                        MBHUDView.DismissCurrentHUD();
                        webUi.UserInteractionEnabled = true;
                        ++iStage;
                    });
                }
                if (iStage == 3 && bLoaded)
                {
                    MBHUDView.DismissCurrentHUD();
                    webUi.UserInteractionEnabled = true;
                }
            }
        }

        void webUi_LoadFinished(object sender, EventArgs e)
        {
            if (webUi.Request.Url.ToString() == _loginUrl)
            {
                webUi.EvaluateJavascript("$(\"input[name=email]:first\").val(\"" + _nikeUsername + "\");");
                webUi.EvaluateJavascript("$(\"input[name=password]:first\").val(\"" + _nikePassword + "\");");
                webUi.UserInteractionEnabled = true;
                ++iStage;
            }
            if (webUi.Request.Url.ToString() == _nikeHome)
            {
                webUi.LoadRequest(new NSUrlRequest(new NSUrl(_nikeCart)));
                ++iStage;
                webUi.UserInteractionEnabled = true;
            }
            if (webUi.Request.Url.ToString() == _nikeCart)
            {
                MBHUDView.DismissCurrentHUD();
            }
            bLoaded = true;
        }


        public void Login(string nikeUsername, string nikePassword)
        {
            _nikeUsername = nikeUsername;
            _nikePassword = nikePassword;
            Console.WriteLine("RapidCheckout");
            iStage = 0;
        }


        void btnFinish_TouchUpInside(object sender, EventArgs e)
        {
            TabBarController.SelectedViewController = ViewControllers.BotRunningViewController;
        }

        partial void UIButton1536_TouchUpInside(UIButton sender)
        {
            iStage = 0;
        }
    }
}
