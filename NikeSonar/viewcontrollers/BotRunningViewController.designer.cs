// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace NikeSonar
{
	[Register ("BotRunningViewController")]
	partial class BotRunningViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem btnCheckout { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnstart { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnStop { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView ForthVeiwController { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tblTasks { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView txtLog { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel txtStatus { get; set; }

		[Action ("btnCheckout_Activated:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnCheckout_Activated (UIBarButtonItem sender);

		[Action ("UIButton1403_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton1403_TouchUpInside (UIButton sender);

		[Action ("UIButton291_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton291_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnCheckout != null) {
				btnCheckout.Dispose ();
				btnCheckout = null;
			}
			if (btnstart != null) {
				btnstart.Dispose ();
				btnstart = null;
			}
			if (btnStop != null) {
				btnStop.Dispose ();
				btnStop = null;
			}
			if (ForthVeiwController != null) {
				ForthVeiwController.Dispose ();
				ForthVeiwController = null;
			}
			if (tblTasks != null) {
				tblTasks.Dispose ();
				tblTasks = null;
			}
			if (txtLog != null) {
				txtLog.Dispose ();
				txtLog = null;
			}
			if (txtStatus != null) {
				txtStatus.Dispose ();
				txtStatus = null;
			}
		}
	}
}
