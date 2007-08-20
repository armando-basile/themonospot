using System;
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

		bool flagAbort = false;
		
		int stdBufferSize = 4096;

		Window pWin;
		clsThemonospotBase baseObject;
		
		
		/// <summary>
		/// <p> Create new instance of monoSPOTwait.</p>
		/// </summary>
		/// <param name="args">parameters passed to the application</param>
		public monoSPOTwait(ref Gtk.Window pW, 
		                    ref clsThemonospotBase baseObj)
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
			topWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
			                  this.baseObject.Release() + 
							  " - Save file";
			topWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");		
		}
		
		private void on_cmdCancel_clicked(object sender, EventArgs a)
		{
			Console.WriteLine("on_cmdCancel_clicked");			
			flagAbort = true;
		}
		
		static private void on_topWindow_delete_event(object sender, DeleteEventArgs args)
		{
			Console.WriteLine("on_topWindow_delete_event");			
		}
		
		public void saveAvi()
		{
			string tmpFName = "";
			int newTotalBytes = 0;
			
			Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Save file as...", 
			                                topWindow,
			                                FileChooserAction.Save, 
			                                "Cancel", Gtk.ResponseType.Cancel,
                                            "Accept", Gtk.ResponseType.Accept);
			
			FileBox.WindowPosition= WindowPosition.CenterAlways;
			
			// Manage result of dialog box
			FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT_16.png");
			int retFileBox = FileBox.Run();
			if (retFileBox == Gtk.ResponseType.Accept.value__)
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
				this.callBackFunction();
				return;
			}
			
			this.topWindow.WindowPosition= WindowPosition.CenterAlways;
			this.topWindow.ShowAll();
			
    		// Write data before MOVI chunk
    		myPBar.Adjustment.Lower = 0;
    		myPBar.Adjustment.Upper = baseObject.totProgressItems;
            myPBar.Adjustment.Value = 0;

			while (Gtk.Application.EventsPending ())
        		Gtk.Application.RunIteration ();

            while (baseObject.saveFlag == true)
			{
                
                if (baseObject.redrawInfo == true)
                {
                    myPBar.Adjustment.Lower = 0;
    		        myPBar.Adjustment.Upper = baseObject.totProgressItems;
                    myPBar.Adjustment.Value = 0;
                    lblInfo.Markup = baseObject.saveInfo;
                }
                
                myPBar.Adjustment.Value = baseObject.saveStatus;
                
                while (Gtk.Application.EventsPending ())
        		    Gtk.Application.RunIteration ();
                
			    
			
			
			}
			
			
			/*
			// Begin Write new avi file
			inFile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			outFile = new FileStream(tmpFName, FileMode.Create,FileAccess.Write, FileShare.None);
			
			
			FileInfo fi = new FileInfo(filename);
			long filmsize = fi.Length;
			long filmoffset = 0;
			long diffBytes = 0;
			byte[] tmpBytes;
			int totMoviItems = Convert.ToInt32(_idxSize / 16) + 3000;
			
			framesOffset = new long[ totMoviItems ];
			framesSize = new long[ totMoviItems ];
			int bufferSize = stdBufferSize;
			
			// Write data before MOVI chunk
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)_moviOffset;
			flagAbort=false;
			
			Console.WriteLine("Write Header START");
			lblInfo.Markup = "update file header... wait please";
			while (filmoffset < _moviOffset)
			{
				if ((filmoffset + bufferSize) > _moviOffset)
					bufferSize = (int)(_moviOffset - filmoffset);
				
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			Console.WriteLine("Write Header END");
			
			
			// Write new MOVI chunk from old
			if (writeMoviChunk() != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
			}
			
			
			
			// Write data before IDX1 chunk
			diffBytes = (_idxOffset - inFile.Position);
			bufferSize = stdBufferSize;
			
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT_1 START");
			lblInfo.Markup = "update file content... wait please";
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 1 END");
			
			
			
			// Write new IDX1 chunk from new MOVI created
			if (writeIdx1Chunk() != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
			}
			
			
			// Write data after IDX1 chunk			
			diffBytes = (filmsize - inFile.Position);
			bufferSize = stdBufferSize;
			
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT 2 START");
			lblInfo.Markup = "update file content... wait please";
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 2 END");
			
			// Close output
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			
			// Read File Infos
			fi = new FileInfo(tmpFName);
			newTotalBytes = (int)(fi.Length - 8);
			fi = null;
			
			// Update MOVI Size and File Size
			outFile = new FileStream(tmpFName, FileMode.Open,FileAccess.Write, FileShare.None);
			outFile.Seek(4,SeekOrigin.Begin);
			outFile.Write(intToByteArray(newTotalBytes),0,4);
			outFile.Seek(_moviOffset - 8,SeekOrigin.Begin);
			outFile.Write(intToByteArray((int)(_moviSizeNew + 4)),0,4);
						
			// Close all streams and return to Main Window
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			inFile.Close();
			inFile.Dispose();
			inFile = null;
			
			
			*/
			
			this.topWindow.Destroy();
			this.topWindow.Dispose();			
			
		}
		
	
		
		
		
		
		

	}
}
