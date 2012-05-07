
using MonoTouch.UIKit;
using System;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace iBloodPressure
{
	public delegate void RowSelectedEventHandler(MonoTouch.Foundation.NSIndexPath indexPath);
	partial class RootViewController : UITableViewController
	{
		private SQLiteConnection db;
				
		public RootViewController (IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			//Show an edit button
			NavigationItem.LeftBarButtonItem = EditButtonItem;
			
			Selector insertObject = new Selector("InsertNewObject");
			UIBarButtonItem AddButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, 
			                                                this, insertObject );
            NavigationItem.RightBarButtonItem = AddButton;
    
			this.Title = "Blood Pressure";

			this.db = new SQLiteConnection(Path.Combine (Environment.GetFolderPath 
			                                             (Environment.SpecialFolder.MyDocuments),
			                                             "bloodPressure.db"));
			this.db.CreateTable<BloodPressure>();
			
			DataSource dataSource = new DataSource(this);
			dataSource.RowSelectedEvent += HandleDataSourceRowSelectedHandler;
			this.TableView.Source = dataSource;
				
			
			
		}

		void HandleDataSourceRowSelectedHandler (MonoTouch.Foundation.NSIndexPath indexPath)
		{
			AddBloodPressureViewController controller = 
				new AddBloodPressureViewController("AddBloodPressureViewController", null);
	      	controller.Connection = this.db;
			BloodPressure bp = 
				db.Table<BloodPressure>().OrderBy(pressure => pressure.TimeStamp).ElementAt(indexPath.Row);
			controller.PressureObject = bp;
			controller.SavedEvent += HandleControllerSavedEvent;
			NavigationController.PushViewController(controller, true);
		}
		
		public static IEnumerable<BloodPressure> QueryBloodPressure (SQLiteConnection db)
		{
			
        		return db.Query<BloodPressure>("select * from BloodPressure");
		}
		
		[Export("InsertNewObject")]
		public void InsertNewObject()
		{
			AddBloodPressureViewController controller =
				new AddBloodPressureViewController("AddBloodPressureViewController", null);
	      	controller.Connection = this.db;
			controller.SavedEvent += HandleControllerSavedEvent;
			NavigationController.PushViewController(controller, true);
		}
		void HandleControllerSavedEvent (object sender, EventArgs e)
		{
			AddBloodPressureViewController controller = sender as AddBloodPressureViewController;
			
			controller.SavedEvent -= HandleControllerSavedEvent;
	
			this.TableView.ReloadData();
				
			NavigationController.PopViewControllerAnimated(true);
		}
		
		
		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidUnload ()
		{
			// Release anything that can be recreated in viewDidLoad or on demand.
			// e.g. this.myOutlet = null;
			
			base.ViewDidUnload ();
		}

		class DataSource : UITableViewSource
		{
			RootViewController controller;
			
			public DataSource (RootViewController controller)
			{
				this.controller = controller;
			}

			public override int NumberOfSections (UITableView tableView)
			{
				return 1;
			}

			// Customize the number of rows in the table view
			public override int RowsInSection (UITableView tableview, int section)
			{
        			int count = controller.db.Table<BloodPressure>().Count();							
				return count;
			}

			// Customize the appearance of table view cells.
			public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				string cellIdentifier = "Cell";
				var cell = tableView.DequeueReusableCell (cellIdentifier);
				if (cell == null) {
					cell = new UITableViewCell (UITableViewCellStyle.Subtitle, cellIdentifier);
				}
				
				BloodPressure bp = controller.db.Table<BloodPressure>().OrderBy(pressure => pressure.TimeStamp).ElementAt(indexPath.Row);
					       
				cell.DetailTextLabel.Text
					= "Systolic: " +  bp.Systolic.ToString() + ", Diastolic: " + bp.Diastolic.ToString() + ", Pulse: " + bp.PulsePerMin.ToString();
				
				cell.TextLabel.Text = bp.TimeStamp.ToString();			
				return cell;
			}

			
			// Override to support conditional editing of the table view.
			public override bool CanEditRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				// Return false if you do not want the specified item to be editable.
				return true;
			}
			
			
			// Override to support editing the table view.
			public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				if (editingStyle == UITableViewCellEditingStyle.Delete) {
					BloodPressure bp = controller.db.Table<BloodPressure>().OrderBy(pressure => pressure.TimeStamp).ElementAt(indexPath.Row);
				
					this.controller.db.Delete<BloodPressure>(bp);
					controller.TableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
				} 
				
			}
					
			// Override to support conditional rearranging of the table view.
			public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
			{
				// Return false if you do not want the item to be re-orderable.
				return true;
			}
			
			public event RowSelectedEventHandler RowSelectedEvent;

			// Override to support row selection in the table view.
			public override void RowSelected (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
			{
				if (this.RowSelectedEvent != null)
				{
					RowSelectedEvent(indexPath);
				}			
			}
		}
	}
	
}
