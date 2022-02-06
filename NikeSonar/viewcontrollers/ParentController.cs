using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace NikeSonar
{
	partial class ParentController : UITabBarController
	{
		public ParentController (IntPtr handle) : base (handle)
		{
		   
		}
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            //TabBarController.TabBar.Hidden = true;
            TabBarController.SelectedViewController = NikeSonar.ViewControllers.MainViewController;
        }
	}
}
