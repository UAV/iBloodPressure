
using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.ObjCRuntime;
using SQLite;

namespace iBloodPressure
{
	public partial class AddBloodPressureViewController : UIViewController 
	{
		private bool systolicEnabled;
		private bool diastolicEnabled;
		private bool pulsePerMinEnabled;
		public SQLiteConnection Connection {get; set;}
		public  BloodPressure PressureObject {get;set;}
		
		#region Constructors

		// The IntPtr and initWithCoder constructors are required for controllers that need 
		// to be able to be created from a xib rather than from managed code

		public AddBloodPressureViewController (IntPtr handle) : base(handle)
		{
			Initialize ();
		}
		
		public AddBloodPressureViewController (string nibName, MonoTouch.Foundation.NSBundle bundle) : base(nibName, bundle)
		{
			Initialize ();
		}

		[Export("initWithCoder:")]
		public AddBloodPressureViewController (NSCoder coder) : base(coder)
		{
			Initialize ();
		}

		public AddBloodPressureViewController () : base("AddBloodPressureViewController", null)
		{
			Initialize ();
		}

		void Initialize ()
		{
		}
	
		
		#endregion
	
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			TextFieldDelegate textFieldDelegate = new TextFieldDelegate();
		    systolic.Delegate = textFieldDelegate;
			diastolic.Delegate = textFieldDelegate;
			pulsePerMin.Delegate = textFieldDelegate;
			textFieldDelegate.EditingStartedEvent += HandleTextFieldEditingStartedEvent;
			Selector saveNewObject = new Selector("saveNewObject");
			UIBarButtonItem saveButton = new UIBarButtonItem(UIBarButtonSystemItem.Save, this, saveNewObject) ;
			
			if (PressureObject != null)
			{
				systolicEnabled = true;
				diastolicEnabled = true;
				pulsePerMinEnabled = true;
				systolic.Text = PressureObject.Systolic.ToString();
				diastolic.Text = PressureObject.Diastolic.ToString();
				pulsePerMin.Text = PressureObject.PulsePerMin.ToString();
				this.Title = PressureObject.TimeStamp.ToString();
				saveButton.Enabled = true;				
			}
			else
			{
				systolicEnabled = false;
				diastolicEnabled = false;
				diastolicEnabled = false;
				diastolicEnabled = false;
				pulsePerMinEnabled = false;
				this.Title = "New";	
				saveButton.Enabled = false;		
				
			}
			NavigationItem.RightBarButtonItem = saveButton;				
		}

		void HandleTextFieldEditingStartedEvent (UITextField textField)
		{
			if (textField == systolic)
				systolicEnabled = true;
			else if (textField == diastolic)
					diastolicEnabled =  true;
			else if (textField == pulsePerMin)
				pulsePerMinEnabled = true;
			NavigationItem.RightBarButtonItem.Enabled = systolicEnabled && diastolicEnabled && pulsePerMinEnabled;
		}
		
		partial void ResignToKeboard (MonoTouch.UIKit.UIButton sender)
		{
			systolic.ResignFirstResponder();
			diastolic.ResignFirstResponder();
			pulsePerMin.ResignFirstResponder();
		}
		
		public event EventHandler SavedEvent;
		
		[MonoTouch.Foundation.Export("saveNewObject")]
		private void SaveNewObject()
		{
			this.ResignToKeboard(null);
			if (PressureObject == null)
			{
				PressureObject = new BloodPressure();
				PressureObject.Systolic = int.Parse(systolic.Text);
				PressureObject.Diastolic = int.Parse(diastolic.Text);
				PressureObject.PulsePerMin = int.Parse(pulsePerMin.Text);		
				// Save
	    			Connection.Insert(PressureObject);
			}
			else
			{
				PressureObject.Systolic = int.Parse(systolic.Text);
				PressureObject.Diastolic = int.Parse(diastolic.Text);
				PressureObject.PulsePerMin = int.Parse(pulsePerMin.Text);
				Connection.Update(PressureObject);
			}
			
			if (this.SavedEvent != null)
			{
				SavedEvent(this, EventArgs.Empty);
			}
			
		}
		
		class TextFieldDelegate : UITextFieldDelegate
		{
			public event TextFieldEditingStartedEvent EditingStartedEvent;
			public TextFieldDelegate ()
			{

			}
			
			public override void EditingStarted (UITextField textField)
			{
				if (this.EditingStartedEvent != null)
				{
					this.EditingStartedEvent(textField);
				}
			}

		}
		
	}
	
	public delegate void TextFieldEditingStartedEvent(UITextField textField);
}
