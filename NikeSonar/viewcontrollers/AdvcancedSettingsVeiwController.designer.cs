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
	[Register ("AdvcancedSettingsVeiwController")]
	partial class AdvcancedSettingsVeiwController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch btnOverideSleep { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnSave { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIImageView imgStoreHandle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISlider sliderWaitTime { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel txtGUUID { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtStoreHandle { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel txtWaitTime { get; set; }

		[Action ("btnSave_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void btnSave_TouchUpInside (UIButton sender);

		[Action ("UIButton241_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton241_TouchUpInside (UIButton sender);

		[Action ("UIButton647_TouchUpInside:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UIButton647_TouchUpInside (UIButton sender);

		[Action ("UISlider666_ValueChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UISlider666_ValueChanged (UISlider sender);

		[Action ("UISwitch670_ValueChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void UISwitch670_ValueChanged (UISwitch sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnOverideSleep != null) {
				btnOverideSleep.Dispose ();
				btnOverideSleep = null;
			}
			if (btnSave != null) {
				btnSave.Dispose ();
				btnSave = null;
			}
			if (imgStoreHandle != null) {
				imgStoreHandle.Dispose ();
				imgStoreHandle = null;
			}
			if (sliderWaitTime != null) {
				sliderWaitTime.Dispose ();
				sliderWaitTime = null;
			}
			if (txtGUUID != null) {
				txtGUUID.Dispose ();
				txtGUUID = null;
			}
			if (txtStoreHandle != null) {
				txtStoreHandle.Dispose ();
				txtStoreHandle = null;
			}
			if (txtWaitTime != null) {
				txtWaitTime.Dispose ();
				txtWaitTime = null;
			}
		}
	}
}
