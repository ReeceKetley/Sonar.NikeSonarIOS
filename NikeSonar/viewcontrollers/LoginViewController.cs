using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using Newtonsoft.Json.Linq;
using Xamarin.Controls;
using System.Threading;
using AlertView;


namespace NikeSonar
{
    public partial class LoginViewController : UIViewController
    {
        private bool _isUsernameValid;
        private bool _isPasswordValid;
        private string _username;
        private bool _loggedIn;
        private string _password;
        private string _devId;
        private BackgroundWorker loginCheckWorker;
        private bool _loginChecker;
        public LoginViewController(IntPtr handle)
            : base(handle)
        {
            ViewControllers.LoginViewController = this;
            //TabBarController.TabBar.Hidden = true;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        #region View lifecycle

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override void ViewDidLoad()
        {            
            base.ViewDidLoad();            
            TabBarController.SelectedIndex = 1;
            TabBarController.SelectedIndex = 2;
            TabBarController.SelectedIndex = 3;
            TabBarController.SelectedIndex = 4;
            TabBarController.SelectedIndex = 5;
            TabBarController.SelectedIndex = 6;
            TabBarController.SelectedIndex = 0;
            loginCheckWorker = new BackgroundWorker();
            loginCheckWorker.DoWork += new DoWorkEventHandler(loginWorker_doWork);
            btnLogin.Enabled = false;
            btnLogin.TouchUpInside += btnLogin_TouchUpInside;
            //TabBarController.SelectedIndex = 4;
            txtUsername.EditingChanged += txtUsername_EditingChanged;
            txtUsername.Text = ReadStringSetting("Username");
            if (txtUsername.Text.Length > 0)
            {
                SetTextStatus(imgUsername, "Ok");
                _isUsernameValid = true;
                ScannerButtonToggle();
            }
            txtUsername.ShouldReturn = delegate
            {
                txtUsername.ResignFirstResponder();
                return true;
            };

            txtPassword.EditingChanged += txtPassword_EditingChanged;
            txtPassword.Text = Base64Decode(ReadStringSetting("Password"));
            if (txtPassword.Text.Length > 0)
            {
                SetTextStatus(imgPassword, "Ok");
                _isPasswordValid = true;
                ScannerButtonToggle();
            }
            txtPassword.ShouldReturn = delegate
            {
                txtPassword.ResignFirstResponder();
                return true;
            };


            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);
        }

        void loginWorker_doWork(object sender, DoWorkEventArgs e)
        {
            _loginChecker = true;
            for (; ; )
            {
                if (!SonarSettings.loggedIn)
                {
                    break;
                    loginCheckWorker.CancelAsync();
                }
                Thread.Sleep(60000);
                var loginResponse = ApiLogin.Login(_devId, _username, _password);
                if (loginResponse != LoginResponseCode.Sucess && loginResponse != LoginResponseCode.LinkedDevice)
                {
                    ////Console.WriteLine(_devId + " " + _username + " " + _password + loginResponse.ToString());
                    //Console.WriteLine("Auth check failed.");
                    InvokeOnMainThread(() => TabBarController.SelectedViewController = ViewControllers.LoginViewController);
                    InvokeOnMainThread(() => AlertCenter.Default.PostMessage("API Server", "Authentication error occured please log in"));
                    InvokeOnMainThread(() => txtStat.Text = "API Server: Authentication error please login");
                    _loginChecker = false;
                    break;
                }
                ////Console.WriteLine("Auth check.");
            }
        }

        public string ReadStringSetting(string Setting)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Setting);
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

        private void txtPassword_EditingChanged(object sender, EventArgs e)
        {
            if (txtPassword.Text.Length > 3)
            {
                SetTextStatus(imgPassword, "Ok");
                _isPasswordValid = true;
            }
            else
            {
                SetTextStatus(imgPassword, "Bad");
                _isPasswordValid = false;
            }
            ScannerButtonToggle();
        }

        private void txtUsername_EditingChanged(object sender, EventArgs e)
        {
            if (txtUsername.Text.Length > 4)
            {
                SetTextStatus(imgUsername, "Ok");
                _isUsernameValid = true;
            }
            else
            {
                SetTextStatus(imgUsername, "Bad");
                _isUsernameValid = false;
            }
            ScannerButtonToggle();
        }

        public void ScannerButtonToggle()
        {
            btnLogin.Enabled = _isPasswordValid && _isUsernameValid;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

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



        private bool _loginSuccess;
        private Thread _loginThread;
        private bool _isRunning;
        private object _mutex = new object();
        private LoginResponseCode _loginResponseCode;
        private void LoginThread_DoWork()
        {
            _devId = UIDevice.CurrentDevice.IdentifierForVendor.AsString();
            //_devId = "7CA0454F-779F-4DF9-9799-969B07F8A89D";
            lock (_mutex)
            {
                
            }
            for (int i = 0; i < 5; ++i)
            {
                //Download Page Data
                //Decrypt data
                //Process JSON 
                //Handle Sucess
                var loginResponse = ApiLogin.Login(_devId, _username, _password);
                _loginResponseCode = loginResponse;
                if (loginResponse == LoginResponseCode.Sucess || loginResponse == LoginResponseCode.InvalidCredentials)
                {
                    break;
                } 
                Thread.Sleep(500);
            }
            BeginInvokeOnMainThread(LoginThread_DoWorkCompleted);
        }

        private void LoginThread_DoWorkCompleted()
        {
            _loginThread.Join();
            lock (_mutex)
            {
                //_username = null;
                //_password = null;
                _loginThread = null;
                btnLogin.Enabled = true;
                txtPassword.Enabled = true;
                txtUsername.Enabled = true;
                var loginResponseCode = _loginResponseCode;
                _loginResponseCode = LoginResponseCode.UnkownFail;
                if (loginResponseCode == LoginResponseCode.Sucess || loginResponseCode == LoginResponseCode.LinkedDevice)
                {
                    MBHUDView.DismissCurrentHUD();
                    TabBarController.SelectedViewController = ViewControllers.MainViewController;
                    ViewControllers.APIUser = txtUsername.Text;
                    ViewControllers.APIPass = txtPassword.Text;
                    ViewControllers.SetAPIUserPath();
                    SonarSettings.loggedIn = true;
                    if (!_loginChecker)
                    {
                        loginCheckWorker.RunWorkerAsync();
                    }
                }
                else if (loginResponseCode == LoginResponseCode.InvalidCredentials)
                {
                    MBHUDView.DismissCurrentHUD();
                    AlertCenter.Default.PostMessage("Login Error", "Invalid Credentials");
                }
                else
                {
                    MBHUDView.DismissCurrentHUD();

                    //Console.WriteLine("Maximum number of nike accounts: " + SonarSettings.MaxAccounts);
                    ViewControllers.APIUser = txtUsername.Text;
                    ViewControllers.SetAPIUserPath();
                    //AlertCenter.Default.PostMessage("Login Error", "Unknown Error"); 
                }
                _isRunning = false;
            }
        }

        private void btnLogin_TouchUpInside(object sender, EventArgs e)
        {
            lock (_mutex)
            {
                if (_isRunning)
                {
                    return;
                }
                //TabBarController.SelectedIndex = 4;
                NSUserDefaults.StandardUserDefaults.SetString(txtUsername.Text, "Username");
                NSUserDefaults.StandardUserDefaults.SetString(Base64Encode(txtPassword.Text), "Password");
                NSUserDefaults.StandardUserDefaults.Synchronize();

                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
                btnLogin.Enabled = false;

                _username = txtUsername.Text;
                _password = txtPassword.Text;

                MBHUDView.HudWithBody(
                    body: "Logging in",
                    aType: MBAlertViewHUDType.ActivityIndicator,
                    delay: 30.0f,
                    showNow: true
                    );
                _loginThread = new Thread(LoginThread_DoWork);
                _loginThread.Start();
                _isRunning = true;
            }

            #endregion
        }
    }
}