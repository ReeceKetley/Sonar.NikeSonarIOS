using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Xamarin.Controls;
using SlideDownMenu;

namespace NikeSonar
{
    public partial class MainViewController : UIViewController
    {
        #region Private Fields

        private bool _isKeywordsValid;
        private bool _isSizeValid;

        #endregion Private Fields

        #region Public Constructors

        public MainViewController(IntPtr handle)
            : base(handle)
        {
            ViewControllers.MainViewController = this;
          
        }

        #endregion Public Constructors

        #region Overides

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }
        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TabBarController.TabBar.Hidden = true;
            SetupTextBoxs();
            // Menu
            ViewControllers.CreateMenu(this);
            SonarSettings.LoadAccounts();
        }

        #endregion Public overides

        #region Private Methods

        private void ScannerButtonToggle()
        {
            btnScannerToggle.Enabled = _isKeywordsValid;
        }
        private void SetupTextBoxs()
        {
            btnScannerToggle.Enabled = false;
            txtKeywords.ShouldReturn = delegate
            {
                txtKeywords.ResignFirstResponder();
                return true;
            };
            txtKeywords.EditingChanged += new EventHandler(txtKeywords_EditingChanged);
            txtNegKeywords.ShouldReturn = delegate
            {
                txtNegKeywords.ResignFirstResponder();
                return true;
            };
            txtNegKeywords.EditingChanged += new EventHandler(txtNegKeywords_EditingChanged);
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);
        }
        private void SetTextStatus(UIImageView obj, string status)
        {
            status = status.ToLower();
            if (status == "missing")
            {
                obj.Image = UIImage.FromFile("Images/Missing.png");
            }
            else if (status == "ok")
            {
                obj.Image = UIImage.FromFile("Images/Ok.png");
            }
            else if (status == "bad")
            {
                obj.Image = UIImage.FromFile("Images/Bad.png");
            }
        }

        // Control Events
        private void txtKeywords_EditingChanged(object sender, EventArgs e)
        {
            if (txtKeywords.Text.Length > 3)
            {
                SetTextStatus(imgKeywords, "Ok");
                _isKeywordsValid = true;
            }
            else
            {
                SetTextStatus(imgKeywords, "Bad");
                _isKeywordsValid = false;
            }
            ScannerButtonToggle();
        }
        private void txtNegKeywords_EditingChanged(object sender, EventArgs e)
        {
            if (txtKeywords.Text.Length > 0)
            {
                SetTextStatus(imgNegKeywords, "Ok");
            }
            else
            {
                SetTextStatus(imgNegKeywords, "Missing");
            }
        }
        partial void btnScannerToggle_TouchUpInside(UIButton sender)
        {
            if (txtKeywords.Text.Contains("http://"))
            {
                ViewControllers.BotRunningViewController.EarlyLink = txtKeywords.Text.Trim();
            }
            else
            {
                ViewControllers.BotRunningViewController.EarlyLink = "";
                ViewControllers.BotRunningViewController.Keywords = txtKeywords.Text.Trim();
            }
            if (txtNegKeywords.Text != "")
            {
                ViewControllers.BotRunningViewController.NegativeKeywords = txtNegKeywords.Text.Trim();
            }
            if (!SonarSettings.AccountsLoaded)
            {
                if (NSUserDefaults.StandardUserDefaults.StringForKey("TwitterHandle") == null &&
                    NSUserDefaults.StandardUserDefaults.StringForKey("TwitterHandle") == "")
                {
                    AlertCenter.Default.PostMessage("Store Twitter Handle Error", "Check store twitter handle in advanced settings");
                    return;
                }
                AlertCenter.Default.PostMessage("Missing Nike Accounts", "No nike accounts please add some.");
                return;
            }
            //ViewControllers.BotRunningViewController.Start();
            TabBarController.SelectedViewController = ViewControllers.BotRunningViewController;

        }
        #endregion Private Methods
    }
}