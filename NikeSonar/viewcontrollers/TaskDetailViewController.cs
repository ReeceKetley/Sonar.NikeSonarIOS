using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Net;
using System.Text.RegularExpressions;
using Xamarin.Controls;

namespace NikeSonar
{
    partial class TaskDetailViewController : UITableViewController
    {
        NikeStoreAccounts currentTask { get; set; }
        public AddAccountsViewController Delegate { get; set; } // will be used to Save, Delete later

        public TaskDetailViewController(IntPtr handle)
            : base(handle)
        {
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            txtUser.Text = currentTask.UserName;
            txtPassword.Text = currentTask.Password;
            txtSize.Text = currentTask.Size;
            bEnabled.On = currentTask.Active;
            txtUser.EditingChanged += new EventHandler(txtUser_EditingChanged);
        }

        void txtUser_EditingChanged(object sender, EventArgs e)
        {
            
        }

        public bool IsValidEmail(string emailAddress)
        {
            return Regex.IsMatch(emailAddress, @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z", RegexOptions.IgnoreCase);
        }

        public override void ViewDidLoad()
        {
            NavigationController.SetNavigationBarHidden(true, true);
            btnAdd.TouchUpInside += (sender, e) =>
            {
                currentTask.UserName = txtUser.Text;
                currentTask.Password = txtPassword.Text;
                currentTask.Size = txtSize.Text;
                currentTask.Active = bEnabled.On;
                currentTask.Proxy = txtProxy.Text;
                if (IsValidEmail(currentTask.UserName) && currentTask.Password != "" && currentTask.Size != "" && !currentTask.UserName.Contains(",") && !currentTask.Size.Contains(",") && !currentTask.Password.Contains(","))
                {
                    Delegate.SaveTask(currentTask);
                    return;
                }
                AlertCenter.Default.PostMessage("Nike Account Error", "Missing/Wrong Information.");
            };
            btnCancel.TouchUpInside += (sender, e) => Delegate.DeleteTask(currentTask);
            txtUser.ShouldReturn = delegate { txtUser.ResignFirstResponder(); return true; };
            txtPassword.ShouldReturn = delegate { txtPassword.ResignFirstResponder(); return true; };
            txtSize.ShouldReturn = delegate { txtSize.ResignFirstResponder(); return true; };
            var g = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(g);
        }


        // this will be called before the view is displayed
        public void SetTask(AddAccountsViewController d, NikeStoreAccounts task)
        {
            Delegate = d;
            currentTask = task;
        }


        partial void btnTest_TouchUpInside(UIButton sender)
        {
            if (!txtProxy.Text.Contains(":"))
            {
                AlertCenter.Default.PostMessage("Proxy Tester", "Proxy is invalid");
                return;
            }
            string[] txt = txtProxy.Text.Split(':');
            bool OK = false;
            try
            {
                WebClient wc = new WebClient();
                wc.Proxy = new WebProxy(txt[0], Convert.ToInt32(txt[1]));
                wc.DownloadString(
                    "http://help-en-us.nike.com/app/answers/detail/article/payment-options/a_id/1009/p/3897");
                OK = true;
                AlertCenter.Default.PostMessage("Proxy Tester", "Proxy seems to be working. This dosen't mean it wont get banned");
            }
            catch
            {
                AlertCenter.Default.PostMessage("Proxy Tester", "Proxy connection failed");
            }
        }
    }
}
