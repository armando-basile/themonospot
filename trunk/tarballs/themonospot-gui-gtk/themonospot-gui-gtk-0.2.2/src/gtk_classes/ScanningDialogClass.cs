
using System;
using ThemonospotBase;

namespace ThemonospotGuiGtk
{
	
	
	public class ScanningDialogClass
	{
		
		[Glade.Widget]	Gtk.Dialog			ScanningDialog;
		[Glade.Widget]	Gtk.Label			lblOperation;
		[Glade.Widget]	Gtk.Label			lblFileName;
		[Glade.Widget]	Gtk.Button			btnCancel;
		
		
		
		// EVENTS
		public event CancelScanEventHandler CancelScan;
		
		
		
		
		// PROPERTIES
		public string Operation 
		{
			set 
			{
				lblOperation.Text = value;
			}
			get
			{
				return lblOperation.Text;
			}
		}

		public string FileName 
		{
			set 
			{
				lblFileName.Text = value;
			}
			get
			{
				return lblFileName.Text;
			}
		}		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public ScanningDialogClass(ref Gtk.Window parent)
		{
			
			// Instance glade xml object using glade file
			Glade.XML gxml =  new Glade.XML("ScanningDialog.glade", "ScanningDialog");
			
			// Aonnect glade xml object to this Gtk.Window
			gxml.Autoconnect(this);
			
			ScanningDialog.TransientFor = parent;
			
			// Update Gtk graphic objects
			UpdateGraphicObjects();
			
			// Ipdate Event Handlers
			UpdateReactors();			

			ScanningDialog.Show();
		}
		
		
		
		
		private void UpdateGraphicObjects()
		{
			// Set dialog icon
			ScanningDialog.Icon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
			
			ScanningDialog.Title = GlobalData.GetLanguageKeyValue("SCANWINTITLE");
			lblOperation.Text = GlobalData.GetLanguageKeyValue("SCANDESC");
			lblFileName.Text = "...";
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
		}
		
		
		
		
		
		private void UpdateReactors()
		{
			// Configure events reactors
			
			ScanningDialog.DeleteEvent += ActionCancel;
			btnCancel.Clicked += ActionCancel;
		}
		
		
		
		
		private void ActionCancel(object sender, EventArgs args)
		{
			//When click on Cancel, raise CancelScan Event
			CancelScan(this, new EventArgs());
		}
		


		public void Close()
		{
			ScanningDialog.Destroy();
			ScanningDialog.Dispose();
		}
		
		

	}
}
