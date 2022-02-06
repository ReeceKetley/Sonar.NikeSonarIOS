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
	[Register ("AddAccountsViewController")]
	partial class AddAccountsViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem btnBack { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem btnCreateNew { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIBarButtonItem btnEdit { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView tblUsers { get; set; }

		[Action ("btnBack_Activated:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnBack_Activated (UIBarButtonItem sender);

		[Action ("btnCreateNew_Activated:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnCreateNew_Activated (UIBarButtonItem sender);

		[Action ("btnEdit_Activated:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnEdit_Activated (UIBarButtonItem sender);

		[Action ("UIBarButtonItem1308_Activated:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIBarButtonItem1308_Activated (UIBarButtonItem sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnBack != null) {
				btnBack.Dispose ();
				btnBack = null;
			}
			if (btnCreateNew != null) {
				btnCreateNew.Dispose ();
				btnCreateNew = null;
			}
			if (btnEdit != null) {
				btnEdit.Dispose ();
				btnEdit = null;
			}
			if (tblUsers != null) {
				tblUsers.Dispose ();
				tblUsers = null;
			}
		}
	}
}
