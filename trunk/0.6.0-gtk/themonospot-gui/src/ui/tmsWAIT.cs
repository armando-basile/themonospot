using System;
using System.Threading;
using System.Reflection;
using Gtk;
using GLib;
using Glade;
using Pango;
using themonospot_Base_Main;
using themonospot_Gui_Main;

namespace themonospot_Gui_Main
{
	
	
	/// <summary>
	/// themonospot Save new file process window
	/// </summary>
	public class tmsWAIT
	{

		// Form Objects
		[Glade.Widget] Gtk.Window topWindow;
		[Glade.Widget] Gtk.ProgressBar myPBar;
		[Glade.Widget] Gtk.Button cmdCancel;
		[Glade.Widget] Gtk.Label lblInfo;

		int stdBufferSize = 4096;

		Window pWin;
		clsThemonospotBase baseObject;
		
		
		/// <summary>
		/// <p> Create new instance of monoSPOTwait.</p>
		/// </summary>
		/// <param name="args">parameters passed to the application</param>
		public tmsWAIT(ref Gtk.Window pW, ref clsThemonospotBase baseObj)
		{
			// Update local variables
			pWin = pW;
			baseObject = baseObj;	
			
			Glade.XML gxml = new Glade.XML(null, "waitWindow.glade", "topWindow", null);
			gxml.Autoconnect(this);
			this.topWindow.TransientFor = pW;
			this.topWindow.Parent = pW;
			this.topWindow.Modal=true;
			configWinObjects();
		}

		
		/// <summary>
		/// <p> Create new instance of monoSPOT.</p>
		/// </summary>		
		public void configWinObjects()
		{	
			// Window
			//topWindow.SetSizeRequest(625,115);
			topWindow.Resizable=false;
			topWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + " v" +
			                  this.baseObject.Release() + 
							  " - Save file";
			topWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");		
		}
		
		private void on_cmdCancel_clicked(object sender, EventArgs a)
		{
			Console.WriteLine("on_cmdCancel_clicked");			
			baseObject.saveFlag = false;
		}
		
		static private void on_topWindow_delete_event(object sender, DeleteEventArgs args)
		{
			Console.WriteLine("on_topWindow_delete_event");			
		}
		
		public void saveAvi()
		{
			string tmpFName = "";
			
			Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Save file as...", 
			                                topWindow,
			                                FileChooserAction.Save, 
			                                "Cancel", Gtk.ResponseType.Cancel,
                                            "Accept", Gtk.ResponseType.Accept);
			
			FileBox.WindowPosition= WindowPosition.CenterAlways;
			
			// Manage result of dialog box
			FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT_16.png");
			int retFileBox = FileBox.Run();
			if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
			{	
				// path of a right file returned
				tmpFName = FileBox.Filename.ToString();				
				FileBox.Destroy();
				FileBox.Dispose();
			
			}
			else
			{
				// nothing returned
				FileBox.Destroy();
				FileBox.Dispose();
				this.topWindow.Dispose();
				this.topWindow.Destroy();
				this.topWindow=null;				
				return;
			}
			
			this.topWindow.WindowPosition= WindowPosition.CenterAlways;
			this.topWindow.ShowAll();
			
			baseObject.saveFlag=true;
			baseObject.newAviFileName = tmpFName;
			// Lunch new thread for save avi file
			System.Threading.Thread saveThread = new System.Threading.Thread( new System.Threading.ThreadStart( baseObject.resaveAviFile ));
			saveThread.Start();
			
    		// Write data before MOVI chunk
    		myPBar.Adjustment.Lower = 0;
    		myPBar.Adjustment.Upper = 100;
            myPBar.Adjustment.Value = 0;

			while (Gtk.Application.EventsPending ())
        		Gtk.Application.RunIteration ();

            // Loop to wait the end of save 
			while (baseObject.saveFlag == true)
			{
                
                if (baseObject.redrawInfo == true)
                {
                    myPBar.Adjustment.Lower = 0;
    		        myPBar.Adjustment.Upper = baseObject.totProgressItems;
                    myPBar.Adjustment.Value = 0;
                    lblInfo.Markup = baseObject.saveInfo;
                    baseObject.redrawInfo = false;
                }
                
                myPBar.Adjustment.Value = baseObject.saveStatus;
                myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
                
                while (Gtk.Application.EventsPending ())
        		    Gtk.Application.RunIteration ();
                
			}
			
			// Show Error 
			if (baseObject.saveError != "")
			{
				MessageDialog Dlg;
				Dlg=new MessageDialog(this.topWindow, DialogFlags.Modal, MessageType.Error, ButtonsType.Ok,
							baseObject.saveError);
					Dlg.Title = "Error detected";
					Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
					Dlg.Run();
					Dlg.Destroy();	
					Dlg = null;
			}
			
			this.topWindow.Destroy();
			this.topWindow.Dispose();
			
			
			

			
						
			
		}
		
	
		
		
		
		
		

	}
}
