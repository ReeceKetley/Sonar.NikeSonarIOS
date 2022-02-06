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
	[Register ("TaskDetailViewController")]
	partial class TaskDetailViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch bEnabled { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnAdd { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnCancel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnTest { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView taskDetail { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtPassword { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtProxy { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtSize { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtUser { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableViewCell txtUsername { get; set; }

		[Action ("btnTest_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnTest_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (bEnabled != null) {
				bEnabled.Dispose ();
				bEnabled = null;
			}
			if (btnAdd != null) {
				btnAdd.Dispose ();
				btnAdd = null;
			}
			if (btnCancel != null) {
				btnCancel.Dispose ();
				btnCancel = null;
			}
			if (btnTest != null) {
				btnTest.Dispose ();
				btnTest = null;
			}
			if (taskDetail != null) {
				taskDetail.Dispose ();
				taskDetail = null;
			}
			if (txtPassword != null) {
				txtPassword.Dispose ();
				txtPassword = null;
			}
			if (txtProxy != null) {
				txtProxy.Dispose ();
				txtProxy = null;
			}
			if (txtSize != null) {
				txtSize.Dispose ();
				txtSize = null;
			}
			if (txtUser != null) {
				txtUser.Dispose ();
				txtUser = null;
			}
			if (txtUsername != null) {
				txtUsername.Dispose ();
				txtUsername = null;
			}
		}
	}
}
