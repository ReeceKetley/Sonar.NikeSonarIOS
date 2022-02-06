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
	[Register ("TwitterMoniterViewController")]
	partial class TwitterMoniterViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnGetDetails { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIWebView webView { get; set; }

		[Action ("btnGetDetails_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnGetDetails_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnGetDetails != null) {
				btnGetDetails.Dispose ();
				btnGetDetails = null;
			}
			if (webView != null) {
				webView.Dispose ();
				webView = null;
			}
		}
	}
}
