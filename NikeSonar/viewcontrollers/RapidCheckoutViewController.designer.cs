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
	[Register ("RapidCheckoutViewController")]
	partial class RapidCheckoutViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnFinish { get; set; }

		[Action ("UIButton1536_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton1536_TouchUpInside (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnFinish != null) {
				btnFinish.Dispose ();
				btnFinish = null;
			}
		}
	}
}
