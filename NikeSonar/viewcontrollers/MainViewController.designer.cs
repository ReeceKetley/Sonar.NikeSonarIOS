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
	[Register ("MainViewController")]
	partial class MainViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnScannerToggle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgKeywords { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgNegKeywords { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtKeywords { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtNegKeywords { get; set; }

		[Action ("btnScannerToggle_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnScannerToggle_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnScannerToggle != null) {
				btnScannerToggle.Dispose ();
				btnScannerToggle = null;
			}
			if (imgKeywords != null) {
				imgKeywords.Dispose ();
				imgKeywords = null;
			}
			if (imgNegKeywords != null) {
				imgNegKeywords.Dispose ();
				imgNegKeywords = null;
			}
			if (txtKeywords != null) {
				txtKeywords.Dispose ();
				txtKeywords = null;
			}
			if (txtNegKeywords != null) {
				txtNegKeywords.Dispose ();
				txtNegKeywords = null;
			}
		}
	}
}
