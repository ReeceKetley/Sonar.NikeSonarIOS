using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using AlertView;
using Tweetinvi.Controllers.User;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Xamarin.Controls;

namespace NikeSonar
{
    public class TableSource : UITableViewSource
    {
        private List<TaskCell> _items;
        string cellIdentifier = "TableCell";
        public TableSource(List<TaskCell> items)
        {

            _items = items;
        }
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _items.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            //new UIAlertView("Row Selected", _items[indexPath.Row].Title, null, "OK", null).Show();
            //tableView.DeselectRow(indexPath, true); // normal iOS behaviour is to remove the blue highlight
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(cellIdentifier);
            // if there are no cells to reuse, create a new one
            if (cell == null)
                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, cellIdentifier);
            try
            {
                cell.TextLabel.Text = _items[indexPath.Row].Title;
            }
            catch
            {
                cell.TextLabel.Text = _items[indexPath.Row].Title;
            }
            cell.DetailTextLabel.Text = _items[indexPath.Row].Status;
            cell.ImageView.Image = UIImage.FromFile("Images/added.png");
            return cell;
        }
    }
    public partial class BotRunningViewController : UIViewController
    {
        private ReleaseTask task;
        private bool _stopTask;
        private Thread _earlyThread;
        private string _url;
        private IUserStream _userStream;
        public string NikeUsername { get; set; }
        public string NikePassword { get; set; }
        public string NikeSize { get; set; }
        public string EarlyLink { get; set; }
        public string Keywords { get; set; }
        public string NegativeKeywords { get; set; }
        public int totalTasks { get; set; }
        public int completedTask { get; set; }
        public bool bRunning = false;
        public static List<ReleaseTask> Tasks = new List<ReleaseTask>();
        public static List<TaskCell> taskCells = new List<TaskCell>();

        public BotRunningViewController(IntPtr handle)
            : base(handle)
        {
            //TabBarController.TabBar.Hidden = true;
            ViewControllers.BotRunningViewController = this;
        }

        public override bool PrefersStatusBarHidden()
        {
            return true;
        }

        private bool CheckLink()
        {
            HTTP http = new HTTP("Mozilla/5.0 (Windows NT 6.3; WOW64; rv:28.0) Gecko/20100101 Firefox/28.0", "");
            http.Get(_url);
            if ((http.LastStatusCode == HttpStatusCode.OK) && !(http.LastResponse.Contains("NO MATCHES FOUND") || !http.LastResponse.Contains("add-to-cart-form")))
            {
                return true;
            }
            return false;
        }

        private void earlyThread_DoWork()
        {
            while (_stopTask == false)
            {
                //Console.WriteLine("Link Checked");
                if (CheckLink())
                {
                    earlyThread_DoWorkCompleted();
                    //Console.WriteLine("Link Checked");
                    break;
                }
                System.Threading.Thread.Sleep(5000);
            }
        }

        private void earlyThread_DoWorkCompleted()
        {
            foreach (var taskList in Tasks)
            {
                taskList.SetTargetUrl(_url);
            }
            //Console.WriteLine("Setting Target url");
            _stopTask = true;
            _earlyThread = null;
            _url = "";
        }

        public void earlyThread_Start(string url)
        {
            if (url == "")
            {
                return;
            }
            _url = url;
            _stopTask = false;
            _earlyThread = new Thread(earlyThread_DoWork);
            _earlyThread.Start();
            //Console.WriteLine("earlyThread_Start");
        }

        public override void ViewDidLoad()
        {
            tblTasks.BackgroundColor = new UIColor(77, 92, 98, 100);
            //Console.WriteLine(NikeUsername + " " + NikePassword + " " + NikeSize + " " + EarlyLink);
            base.ViewDidLoad();
            NavigationController.SetNavigationBarHidden(true, true);
            TabBarController.TabBar.Hidden = true;
            //Start();
        }

        public void ReloadData()
        {
            tblTasks.ReloadData();
        }

        public int GetTaskCellID(string user)
        {
            try
            {
                return taskCells.FindIndex(a => a.Title == user);
            }
            catch
            {
                return -1;
            }
        }

        public void Start()
        {
            if (EarlyLink != "")
            {

                earlyThread_Start(EarlyLink);
                AlertCenter.Default.PostMessage("Tasks", "Early Link Moniter started");
            }
            else
            {
                TwitterMoniter.IntalizeTwitter();
                TwitterMoniter.Start(this);
                AlertCenter.Default.PostMessage("Tasks", "Twitter Moniter Started");
            }

            foreach (var account in SonarSettings.AccountList)
            {
                TaskCell t = new TaskCell(account.UserName, "Logging in");
                ReleaseTask task = new ReleaseTask(account.Size, account.UserName, account.Password, this, t);
                Tasks.Add(task);
                taskCells.Add(t);
                task.StartAsync();
            }
            totalTasks = Tasks.Count;
            tblTasks.Source = new TableSource(taskCells);
            tblTasks.ReloadData();
        }

        public void SetTaskUrl(string url)
        {
            foreach (var releaseTask in Tasks)
            {
                releaseTask.SetTargetUrl(url);
            }
        }

        public void UpdateLog(string str)
        {
            InvokeOnMainThread(() =>
            {
                txtLog.Text = txtLog.Text + str;
                NSRange range = new NSRange(txtLog.Text.Length - 1, 1);
                txtLog.ScrollRangeToVisible(range);
            });
        }

        public override void ViewDidAppear(bool animated)
        {

        }

        public void GestureFired()
        {
            int i = tblTasks.IndexPathForSelectedRow.Row;
            Console.WriteLine("GestureFired" + i);
            if (i < 0)
            {
                return;
            }
            if (taskCells[tblTasks.IndexPathForSelectedRow.Row].Complete)
            {
                LoadCheckout(Tasks[i].NikeUsername, Tasks[i].NikePassword);
            }
        }

        public string ReadStringSetting(string Setting)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(Setting);
        }

        public void LoadCheckout(string username, string password)
        {
            task = null;
            TabBarController.SelectedIndex = 5;
            var viewController = (RapidCheckoutViewController)TabBarController.ViewControllers[5];
            viewController.Login(username, password);
        }


        partial void UIButton291_TouchUpInside(UIButton sender)
        {
            if (bRunning)
            {
                btnstart.Enabled = true;
                bRunning = false;
                foreach (ReleaseTask ts in Tasks)
                {
                    ts.StopAsync();
                }
                TwitterMoniter.Stop();
            }
                Tasks.Clear();
                taskCells.Clear();
                ReloadData();
            TabBarController.SelectedViewController = ViewControllers.MainViewController;
        }

        partial void UIButton1403_TouchUpInside(UIButton sender)
        {
            btnstart.Enabled = false;
            if (!bRunning)
            {
                bRunning = true;
                Start();
            }
        }

        partial void btnCheckout_Activated(UIBarButtonItem sender)
        {
            if (tblTasks.IndexPathForSelectedRow != null)
            {
                GestureFired();
            }
        }
    }
}
