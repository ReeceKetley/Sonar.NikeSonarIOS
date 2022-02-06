using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Text;
using AlertView;
using SlideDownMenu;
using Xamarin.Controls;

namespace NikeSonar
{
    partial class AdvcancedSettingsVeiwController : UIViewController
    {
        #region Private Fields

        private bool _isNikePasswordValid;
        private bool _isNikeUserValid;
        private bool _isTwitterHandleValid;
        private int _waitTime;

        #endregion Private Fields

        #region Public Constructors

        public AdvcancedSettingsVeiwController(IntPtr handle)
            : base(handle)
        {
            //Console.WriteLine("Constructed Constructer");
            ViewControllers.AdvcancedSettingsVeiwController = this;
        }

        #endregion Public Constructors

        #region Public Methods

        public static string Base64Decode(string base64EncodedData)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch
            {
                return "";
            }
        }

        public static string DownloadString(string address)
        {
            try
            {
                WebClient client = new WebClient();
                string reply = client.DownloadString(address);
                //Console.WriteLine(reply);
                return reply;
            }
            catch (Exception exception)
            {
                //Console.WriteLine(exception.Message);
                return "";
            }
        }


        public string ReadStringSetting(string Setting)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Setting);
        }


        public bool IsValidEmail(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public bool IsValidHandle(string twitterHandle)
        {
            Match match = Regex.Match(twitterHandle, "^([A-Za-z0-9_]){3,15}$", RegexOptions.IgnoreCase);
            return match.Success;
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public void ScannerButtonToggle()
        {
            btnSave.Enabled = _isTwitterHandleValid;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TabBarController.TabBar.Hidden = true;
            txtGUUID.Text = Functions.UniqueID;
            if (ReadStringSetting("WaitTime") != "")
            {
                _waitTime = Convert.ToInt32(ReadStringSetting("WaitTime"));
            }
            else
            {
                _waitTime = 30;
            }
            btnSave.Enabled = false;
            txtStoreHandle.EditingChanged += txtTwitterHandle_EditingChanged;
            txtStoreHandle.ShouldReturn = delegate
            {
                txtStoreHandle.ResignFirstResponder();
                return true;
            };
            if (txtStoreHandle.Text.Length > 0)
            {
                SetTextStatus(imgStoreHandle, "Ok");
                _isTwitterHandleValid = true;
                ScannerButtonToggle();
            }
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);
            ViewControllers.CreateMenu(this);
        }

        #endregion Public Methods

        #region Private Methods

        partial void btnSave_TouchUpInside(UIButton sender)
        {
            NSUserDefaults.StandardUserDefaults.SetString(txtStoreHandle.Text.ToString(), "TwitterHandle");
            NSUserDefaults.StandardUserDefaults.SetString(_waitTime.ToString(), "WaitTime");
            NSUserDefaults.StandardUserDefaults.Synchronize();
            TabBarController.SelectedViewController = ViewControllers.MainViewController;
        }

        private void SetTextStatus(UIImageView obj, string status)
        {
            status = status.ToLower();
            if (status == "missing")
            {
                obj.Image = UIImage.FromBundle("images/Missing.png");
            }
            else if (status == "ok")
            {
                obj.Image = UIImage.FromBundle("images/Ok.png");
            }
            else if (status == "bad")
            {
                obj.Image = UIImage.FromBundle("images/Bad.png");
            }
        }

        void txtTwitterHandle_EditingChanged(object sender, EventArgs e)
        {
            if (!IsValidHandle(txtStoreHandle.Text))
            {
                SetTextStatus(imgStoreHandle, "Bad");
                _isTwitterHandleValid = false;
            }
            else
            {
                SetTextStatus(imgStoreHandle, "Ok");
                _isTwitterHandleValid = true;
            }
            ScannerButtonToggle();
        }
        partial void UIButton647_TouchUpInside(UIButton sender)
        {
            TabBarController.SelectedViewController = ViewControllers.MainViewController;
        }

        partial void UISlider666_ValueChanged(UISlider sender)
        {
            _waitTime = Convert.ToInt32(sliderWaitTime.Value);
            txtWaitTime.Text = "Wait time ( " + _waitTime + "/30) (def = 30)";
        }

        partial void UISwitch670_ValueChanged(UISwitch sender)
        {
            sliderWaitTime.Enabled = btnOverideSleep.On;
            NSUserDefaults.StandardUserDefaults.SetBool(btnOverideSleep.On, "OverideSleep");
            NSUserDefaults.StandardUserDefaults.Synchronize();
        }

        #endregion Private Methods

        partial void UIButton241_TouchUpInside(UIButton sender)
        {
            try
            {
                var http = new HTTP("");
                http.Get(
                    Functions.UrlEncode(ViewControllers.APIServer + "Nike\\core.php?c=deactivate&user=" +
                                        ViewControllers.APIUser + "&pass=" + ViewControllers.APIPass + "&device=" +
                                        ViewControllers.DeviceID));
            }
            catch
            {

            }
            SonarSettings.loggedIn = false;
            TabBarController.SelectedViewController = ViewControllers.LoginViewController;
        }
    }
}
