using System;
using System.Collections;
using System.Reflection;
using Gtk;
using Glade;
using System.IO;

using themonospot_Base_Main;
using themonospot_Gui_Main;

namespace themonospot_Gui_Main {

	public class tmsMAIN {
		
		[Glade.Widget]			Window MonoSPOTWindow;    
		[Glade.Widget]			FileChooserButton FilenameChooser;
		[Glade.Widget]			Button ExportButton;
		[Glade.Widget]			Button infoButton;
		[Glade.Widget]			Button ChangeUserData;
		[Glade.Widget]			Image VideoImage;
		[Glade.Widget]			Image AudioImage;
		[Glade.Widget]			TreeView VideoTreeView;
		[Glade.Widget]			TreeView AudioTreeView;

		
		// monoSPOTwait SaveWindow;
		clsThemonospotBase baseObject = new clsThemonospotBase();
		
		string releaseGUI = "";
		string releaseBASE = "";
		
		string userDataToChange = "";
		bool rec_ix = false;
		
		// codec info used
		string ASH = "";
		string VSH = "";

		// Create new instance of monoSPOT
		public tmsMAIN(string[] args) 
		{			
			Window.DefaultIcon =	Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			Glade.XML gxml =  new Glade.XML("monoSPOT.glade", "MonoSPOTWindow");
			gxml.Autoconnect(this);
			
			// Filter for File Chooser
			FileFilter filter = new Gtk.FileFilter();
			filter.AddPattern("*.avi");
			filter.AddPattern("*.xvid");
			filter.AddPattern("*.divx");			
			filter.Name = "avi, divx, xvid files";
			FilenameChooser.AddFilter(filter);
			FilenameChooser.SelectionChanged += FilenameChooser_selection_changed_cb;
			
			VideoImage.FromPixbuf =	Gdk.Pixbuf.LoadFromResource("video.png");
			AudioImage.FromPixbuf =	Gdk.Pixbuf.LoadFromResource("sound.png");
			
			// setting audio and video treeview
			VideoTreeView.AppendColumn ("Information", new CellRendererText(), "text", 0);
			VideoTreeView.AppendColumn ("Description", new CellRendererText(), "text", 1);
			VideoTreeView.Model = new Gtk.ListStore (typeof (string), typeof (string));
			
			AudioTreeView.AppendColumn ("Information", new CellRendererText(), "text", 0);
			AudioTreeView.AppendColumn ("Description", new CellRendererText(), "text", 1);
			AudioTreeView.Model = new Gtk.ListStore (typeof (string), typeof (string));
			
			releaseGUI = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
				         Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
					     Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			
			releaseBASE = baseObject.Release();
			
			MonoSPOTWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
	                  " v" + releaseBASE;
			
			
			if (Directory.Exists(baseObject.defaultPath) == true)
				FilenameChooser.SetCurrentFolder(baseObject.defaultPath);
			
			MonoSPOTWindow.DeleteEvent += on_MainWindow_Delete;
			MonoSPOTWindow.ShowAll();
			
			Console.WriteLine("themonospot-base component version: " + baseObject.Release());
			
			// If there is a path to process in a command line
			if (args.Length > 0)
			{
				Console.WriteLine("Path: ".PadRight(20,(char)46) + args[0]);
				
				if (File.Exists(args[0]) == false)
				{	
					Console.WriteLine("Path not found...:");
					// No Path Founded				
					MessageDialog Dlg;
					Dlg=new MessageDialog(null ,DialogFlags.Modal,MessageType.Error,ButtonsType.Ok,
							"Path (" + args[0] + ") Not found... !!!");
					Dlg.Title = "Error detected";
					Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");					
					Dlg.Run();
					Dlg.Destroy();	
					Dlg = null;
				}
				else
				{
					FilenameChooser.SetFilename(args[0]);
					parseFile(FilenameChooser.Filename);
				}
			}
			
			
		}
    
		// Entry Point
		public static void Main (string[] args)
		{
			Gtk.Application.Init();
			new tmsMAIN(args);
			Gtk.Application.Run();
		}
		
		// An avi file was selected
		public void FilenameChooser_selection_changed_cb(object sender, EventArgs args)
		{
			Console.WriteLine(FilenameChooser.Filename);
			parseFile(FilenameChooser.Filename);
		}
		
		
		public void RescanButton_clicked_cb(object sender, EventArgs args) 
		{
			if(FilenameChooser.Filename != null)
				parseFile(FilenameChooser.Filename);

		}
		
		// Scan the file
		private void parseFile (string filename) 
		{
		
		    string backError = "";
		    ChangeUserData.Sensitive=false;
		    Hashtable VInfo = new Hashtable();
		    Hashtable AInfo = new Hashtable();
		    
		    if (File.Exists(filename) != true)
		        return;
		    
		    bool resultParse = baseObject.parseAviFile(filename, 
		                                               ref backError,
		                                               ref VInfo, 
		                                               ref AInfo, 
		                                               ref ASH, 
		                                               ref VSH);
		    
		    if (resultParse == false)
		    {
		        MessageDialog dlg = new MessageDialog (MonoSPOTWindow, DialogFlags.Modal, 
				                                       MessageType.Warning, ButtonsType.Ok,
				                                       "Error: " + backError);
				dlg.Title = "themonospot Parser Error";
				dlg.Run();
				dlg.Destroy();
				return;
		    
		    }

		    rec_ix = baseObject.rec_ix;
		    
		    ListStore informations = new ListStore(typeof(string), typeof(string));
			
			informations.AppendValues("Video:", VInfo["Video:"].ToString());			
			informations.AppendValues("Frame Size:", VInfo["Frame Size:"].ToString());
			informations.AppendValues("Average Video Bitrate:", VInfo["Average Video Bitrate:"].ToString()); 
			informations.AppendValues("Avi file size:", VInfo["Avi file size:"].ToString());
			informations.AppendValues("Total Time:", VInfo["Total Time:"].ToString());
			informations.AppendValues("Frame Rate:", VInfo["Frame Rate:"].ToString());
			informations.AppendValues("Total Frames:", VInfo["Total Frames:"].ToString());
			informations.AppendValues("Video Data Rate:", VInfo["Video Data Rate:"].ToString());
			informations.AppendValues("Video Quality:", VInfo["Video Quality:"].ToString());
			informations.AppendValues("Packet Bitstream:", VInfo["Packet Bitstream:"].ToString());
				
			for (int k=0; k<8; k++)
				if (VInfo["Info Data[" + k + "]:"] != null)
					informations.AppendValues("Info Data[" + k + "]:", VInfo["Info Data[" + k + "]:"].ToString());

			for (int k=0; k<8; k++)
			    if (VInfo["User Data[" + k + "]:"] != null)
				    informations.AppendValues("User Data[" + k + "]:", VInfo["User Data[" + k + "]:"].ToString());
			
			VideoTreeView.Model = informations;
			
			informations = new ListStore(typeof(string), typeof(string));
			
			foreach (DictionaryEntry entry in AInfo)
				informations.AppendValues(entry.Key, entry.Value);
			
			AudioTreeView.Model = informations;
			
			
			if (baseObject.userDataToChange.Trim() != "")
			    ChangeUserData.Sensitive = true;

			return;			
		}


		private void on_Change4ccButton_clicked(object sender,EventArgs a)
		{
			if (VideoTreeView.Model.IterNChildren() == 0)
				return;
            
			string tmpVSH = VSH;
			string tmpASH = ASH;
			int retSelect = 0;
			tms4CC newFourCC = new tms4CC(this.MonoSPOTWindow, 
			                              DialogFlags.DestroyWithParent, 
			                              ref tmpVSH, 
			                              ref tmpASH, 
			                              out retSelect);
			
			if (retSelect != 1)
				return;
			
			// Are you sure ?
			if ( tmpVSH != VSH || tmpASH != ASH )
			{
				Gtk.MessageDialog FourCCdialog = new MessageDialog(this.MonoSPOTWindow, 
				                                     			   DialogFlags.DestroyWithParent, 
				                                     			   MessageType.Question,
				                                     			   ButtonsType.YesNo, 
				                                     			   "Are you sure to update your avi file with new FourCC values ?");
				
				FourCCdialog.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				FourCCdialog.Title = "Change FourCC Values";
				ResponseType retFourCCdialog = (ResponseType)FourCCdialog.Run();				
				FourCCdialog.Destroy();
				FourCCdialog.Dispose();
				FourCCdialog = null;
				
				if (retFourCCdialog == ResponseType.Yes)
				{
					baseObject.updateFourCC(tmpASH, tmpVSH);
					parseFile(FilenameChooser.Filename);
					
				}
				
			}

		}
		
		private void on_InfoButton_button_release_event(object sender,EventArgs a)
		{
			Console.WriteLine("info...");
			tmsINFO infoWin = new tmsINFO(this.MonoSPOTWindow, DialogFlags.DestroyWithParent, releaseBASE);
		}
		



		// The on_MainWindow_Delete method is used when we want quit.
		private void on_MainWindow_Delete(object sender,EventArgs a)
    	{
        	Gtk.Application.Quit();
        }
		



		
		private void on_ChangeUserData_clicked(object sender, EventArgs a)
		{
			
			// Verify if REC or IX?? are presents
			if (rec_ix == true)
			{
				Gtk.MessageDialog NotProcessable = new MessageDialog(this.MonoSPOTWindow, 
			                                     			   DialogFlags.DestroyWithParent, 
			                                     			   MessageType.Warning,
			                                     			   ButtonsType.Ok, 
			                                     			   "Founded REC chunk or IX?? chunk. Export file is not possible");
			
				NotProcessable.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				NotProcessable.Title = "Save File";
				NotProcessable.Run();				
				NotProcessable.Destroy();
				NotProcessable.Dispose();
				NotProcessable = null;
				return;
			
			}
		
			// Open Save Window
			
			SaveWindow = new monoSPOTwait(ref this.MonoSPOTWindow, 
			                              ref this.baseObject);
			SaveWindow.callBackFunction += this.callBackValues;
			SaveWindow.saveAvi();
			
			return;
		}
		
		
		// Export the scan result
		private void on_ExportButton_clicked(object sender,EventArgs a)
		{
			if (VideoTreeView.Model.IterNChildren() > 0)
			{
				// New dialog to save file 
				Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Insert file path to export scan result...", 
				                                this.MonoSPOTWindow,
				                                FileChooserAction.Save, 
				                                "Cancel", Gtk.ResponseType.Cancel,
	                                            "Accept", Gtk.ResponseType.Accept);
				
				// Manage result of dialog box
				FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				int retFileBox = FileBox.Run();
				
				if (retFileBox == Gtk.ResponseType.Accept.value__)
				{	
		    		// Save File
		    		DateTime Oggi = DateTime.Now; 
		    		StreamWriter outputStream = new StreamWriter(FileBox.Filename , false);
		    		outputStream.WriteLine(("Avi File:").PadRight(25)+ Path.GetFileName(FilenameChooser.Filename));
		    		outputStream.WriteLine(("Analisys Date:").PadRight(25)+ Oggi.ToString("dd-MM-yyyy HH:mm:ss") );
		    		outputStream.WriteLine("-----------------------------------------------------------------------------");
			    	
			    	string tmpItem1 = "";
			    	string tmpItem2 = "";
			    	// Video Stream
			    	TreeIter tmpIter = new TreeIter();
			    	bool isValidIter = VideoTreeView.Model.GetIterFirst(out tmpIter);
			    	
			    	if (isValidIter == true)
			    	{
				    	tmpItem1 = Convert.ToString(VideoTreeView.Model.GetValue(tmpIter, 0));
				    	Console.WriteLine(tmpItem1);
						tmpItem2 = Convert.ToString(VideoTreeView.Model.GetValue(tmpIter, 1));
						Console.WriteLine(tmpItem2);
						outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
						
						while(VideoTreeView.Model.IterNext(ref tmpIter)) 
						{
			  				tmpItem1 = (string) VideoTreeView.Model.GetValue(tmpIter,0);
			  				tmpItem2 = (string) VideoTreeView.Model.GetValue(tmpIter,1);
							outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
						}
			    		
			    		outputStream.WriteLine("");
			    	}
		    		
		    		// Audio Stream
		    		isValidIter = AudioTreeView.Model.GetIterFirst(out tmpIter);
		    		if (isValidIter == true)
			    	{
				    	tmpItem1 = (string) AudioTreeView.Model.GetValue(tmpIter,0);
			  			tmpItem2 = (string) AudioTreeView.Model.GetValue(tmpIter,1);
			  			outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
		
						while(AudioTreeView.Model.IterNext(ref tmpIter)) 
						{
			  				tmpItem1 = (string) AudioTreeView.Model.GetValue(tmpIter,0);
			  				tmpItem2 = (string) AudioTreeView.Model.GetValue(tmpIter,1);
			  				outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
						}
			  			
			  			outputStream.WriteLine(""); outputStream.WriteLine(""); outputStream.WriteLine("");
					}
		  			outputStream.WriteLine("Report generated by themonospot v" + releaseBASE + " --- Gtk# Gui v" + releaseGUI);
					outputStream.WriteLine("project web site: http://www.integrazioneweb.com/themonospot");
		  			
		    		outputStream.Close();
		    		outputStream.Dispose();
				}
	
				
				FileBox.Destroy();
				FileBox.Dispose();
				return;
			
			}
		}


	}

}