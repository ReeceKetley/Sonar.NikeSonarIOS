using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using Xamarin.Controls;

namespace NikeSonar
{
    partial class AddAccountsViewController : UITableViewController
    {
        #region Public Constructors

        public AddAccountsViewController(IntPtr handle)
            : base(handle)
        {
            ViewControllers.AddAccountsViewController = this;
        }

        #endregion Public Constructors

        #region Overides
        public override bool PrefersStatusBarHidden()
        {
            return true;
        }
        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if (segue.Identifier == "TaskSegue")
            { // set in Storyboard
                var navctlr = segue.DestinationViewController as TaskDetailViewController;
                if (navctlr != null)
                {
                    var source = tblUsers.Source as RootTableSource;
                    var rowPath = tblUsers.IndexPathForSelectedRow;
                    var item = source.GetItem(rowPath.Row);
                    navctlr.SetTask(this, item); // to be defined on the TaskDetailViewController
                }
            }
        }
        public override void ViewDidLoad()
        {
            tblUsers.AllowsMultipleSelection = false;
            NavigationController.SetNavigationBarHidden(true, true);
            if (SonarSettings.AccountList == null)
            {
                SonarSettings.AccountList = new List<NikeStoreAccounts>();
            }
            tblUsers.Source = new RootTableSource(SonarSettings.AccountList.ToArray());
        }
        #endregion Overides

        #region Public Methods

        public void CreateTask()
        {
            // first, add the task to the underlying data
            if (SonarSettings.AccountList.Count >= SonarSettings.MaxAccounts)
            {
                AlertCenter.Default.PostMessage("Account Limit Reached", "Delete existing account or upgrade your tier");
                return;
            }
            var newId = 0;
            if (SonarSettings.AccountList.Count <= 0)
            {
                newId = 1;
            }
            else
            {
                newId = SonarSettings.AccountList[SonarSettings.AccountList.Count - 1].Id + 1;
            }
            var newChore = new NikeStoreAccounts { Id = newId };
            SonarSettings.AccountList.Add(newChore);

            // then open the detail view to edit it
            var detail = Storyboard.InstantiateViewController("detail") as TaskDetailViewController;
            detail.SetTask(this, newChore);
            NavigationController.PushViewController(detail, true);
            tblUsers.ReloadData();
            //Console.WriteLine("List Total:" + SonarSettings.AccountList.Count);
        }

        public void DeleteTask(NikeStoreAccounts account)
        {
            var oldTask = SonarSettings.AccountList.Find(t => t.Id == account.Id);
            SonarSettings.AccountList.Remove(oldTask);
            NavigationController.PopViewControllerAnimated(true);
            tblUsers.Source = new RootTableSource(SonarSettings.AccountList.ToArray());
            tblUsers.ReloadData();
            //Console.WriteLine(SonarSettings.AccountList.Count);
        }

        public void EditSelected()
        {
            //Console.WriteLine("Gesture Fired");
            if (tblUsers.IndexPathForSelectedRow.Row != -1 || tblUsers.IndexPathForSelectedRow.Row != null)
            {
                var detail = Storyboard.InstantiateViewController("detail") as TaskDetailViewController;
                var acc = SonarSettings.AccountList[tblUsers.IndexPathForSelectedRow.Row];
                SonarSettings.AccountList.Remove(acc);
                detail.SetTask(this, acc);
                NavigationController.PushViewController(detail, true);
            }
        }

        public void SaveTask(NikeStoreAccounts account)
        {
            var oldTask = SonarSettings.AccountList.Find(t => t.Id == account.Id);
            SonarSettings.AccountList.Remove(oldTask);
            SonarSettings.AccountList.Add(account);
            NavigationController.PopViewControllerAnimated(true);
            tblUsers.Source = new RootTableSource(SonarSettings.AccountList.ToArray());
            tblUsers.ReloadData();
            //Console.WriteLine(account.UserName);
        }

        #endregion Public Methods

        #region Private Methods

        partial void btnBack_Activated(UIBarButtonItem sender)
        {
            TabBarController.SelectedViewController = ViewControllers.MainViewController;
            SonarSettings.UpdateAccounts(SonarSettings.AccountList);
        }

        partial void btnCreateNew_Activated(UIBarButtonItem sender)
        {
            CreateTask();
        }

        partial void btnEdit_Activated(UIBarButtonItem sender)
        {
            if (tblUsers.IndexPathForSelectedRow == null || tblUsers.IndexPathForSelectedRow.Row < 0)
            {
                return;
            }
            EditSelected();
        }

        partial void UIBarButtonItem1308_Activated(UIBarButtonItem sender)
        {
            if (tblUsers.IndexPathForSelectedRow == null || tblUsers.IndexPathForSelectedRow.Row < 0)
            {
                return;
            }
            SonarSettings.AccountList.RemoveAt(tblUsers.IndexPathForSelectedRow.Row);
            SonarSettings.UpdateAccounts(SonarSettings.AccountList);
            tblUsers.Source = new RootTableSource(SonarSettings.AccountList.ToArray());
            tblUsers.ReloadData();
        }

        #endregion Private Methods
    }
}
