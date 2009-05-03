using System;
using System.Collections;
using System.Collections.Generic;
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
		[Glade.Widget]			Button Change4ccButton;
		[Glade.Widget]			Image VideoImage;
		[Glade.Widget]			Image AudioImage;
		[Glade.Widget]			TreeView VideoTreeView;
		[Glade.Widget]			TreeView AudioTreeView;
		[Glade.Widget]			Label lblAvi;
		[Glade.Widget]			Label lblFile;
		[Glade.Widget]			Label lblAudioTop;
		[Glade.Widget]			Label lblVideoTop;
		[Glade.Widget]			Label lblAboutBT;
		[Glade.Widget]			Label lblExportBT;
		[Glade.Widget]			Label lblFourccBT;
		[Glade.Widget]			Label lblUserdataBT;
		[Glade.Widget]			Label lblRescanBT;

		
		// monoSPOTwait SaveWindow;
		clsThemonospotBase baseObject = new clsThemonospotBase();
		
		string releaseGUI = "";
		string releaseBASE = "";
		static string StartUpError = "";
		string LOCALE_NATIVE_NAME = "";
			
		string userDataToChange = "";
		bool rec_ix = false;
		
		// codec info used
		string ASH = "";
		string VSH = "";

		// Create new instance of monoSPOT
		public tmsMAIN(string[] args) 
		{			
			Window.DefaultIcon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			Glade.XML gxml =  new Glade.XML("monoSPOT.glade", "MonoSPOTWindow");
			gxml.Autoconnect(this);

			LOCALE_NATIVE_NAME = System.Globalization.CultureInfo.CurrentCulture.NativeName.ToLower();
			LOCALE_NATIVE_NAME = LOCALE_NATIVE_NAME.Substring(0, LOCALE_NATIVE_NAME.IndexOf("(") - 1).Trim();
				
			clsLanguages.LanguageName = LOCALE_NATIVE_NAME;
			StartUpError = clsLanguages.Init();
			
			if (StartUpError != "")
			{
				Console.WriteLine("StartUpError = " + StartUpError);
				return;
			}
			
			Console.WriteLine("System language = " + clsLanguages.LanguageName + "\r\nAvailable language = " + clsLanguages.LanguageSet);
			
			// Filter for File Chooser
			FileFilter filter = new Gtk.FileFilter();
			filter.AddPattern("*.avi");
			filter.AddPattern("*.Avi");
			filter.AddPattern("*.AVI");
			filter.AddPattern("*.xvid");
			filter.AddPattern("*.Xvid");
			filter.AddPattern("*.XVID");
			filter.AddPattern("*.divx");
			filter.AddPattern("*.Divx");
			filter.AddPattern("*.DIVX");
			filter.AddPattern("*.mkv");
			filter.AddPattern("*.Mkv");
			filter.AddPattern("*.MKV");
			
			filter.Name = "avi, divx, mkv, xvid files";
			FilenameChooser.AddFilter(filter);
			FilenameChooser.SelectionChanged += FilenameChooser_selection_changed_cb;
			
			VideoImage.FromPixbuf =	Gdk.Pixbuf.LoadFromResource("video.png");
			AudioImage.FromPixbuf =	Gdk.Pixbuf.LoadFromResource("sound.png");
			
			// setting audio and video treeview
			VideoTreeView.AppendColumn (clsLanguages.VCOL1NAME, new CellRendererText(), "text", 0);
			VideoTreeView.AppendColumn (clsLanguages.VCOL2NAME, new CellRendererText(), "text", 1);
			VideoTreeView.Model = new Gtk.ListStore (typeof (string), typeof (string));
			
			AudioTreeView.AppendColumn (clsLanguages.ACOL1NAME, new CellRendererText(), "text", 0);
			AudioTreeView.AppendColumn (clsLanguages.ACOL2NAME, new CellRendererText(), "text", 1);
			AudioTreeView.Model = new Gtk.ListStore (typeof (string), typeof (string));
			
			releaseGUI = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
				         Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
					     Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			
			releaseBASE = baseObject.Release();
			
			MonoSPOTWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
	                  " v" + releaseBASE;
			
			// Update GUI with User Language
			lblAvi.Markup = "<b>" + clsLanguages.DESCFILE + "</b>";
			lblFile.Markup = clsLanguages.LBLFILE;
			lblAudioTop.Markup = "<b>" + clsLanguages.DESCAUDIO + "</b>";
			lblVideoTop.Markup = "<b>" + clsLanguages.DESCVIDEO + "</b>";
			lblAboutBT.MarkupWithMnemonic = clsLanguages.ABOUTBT;
			lblExportBT.MarkupWithMnemonic = clsLanguages.EXPORTBT;
			lblFourccBT.MarkupWithMnemonic = clsLanguages.FOURCCBT;
			lblUserdataBT.MarkupWithMnemonic = clsLanguages.UDATABT;			
			lblRescanBT.MarkupWithMnemonic = clsLanguages.RESCANBT;
			FilenameChooser.Title = clsLanguages.FCTITLE;
			
			Gtk.Tooltips tips1 = new Gtk.Tooltips();
			tips1.SetTip(ExportButton, clsLanguages.TTTEXPORT, null);
			tips1.SetTip(Change4ccButton, clsLanguages.TTTFOURCC, null);
			tips1.SetTip(ChangeUserData, clsLanguages.TTTUDATA, null);
			
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
			
			if (StartUpError != "")
				return;
				
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
		    List<clsInfoItem> VInfo = new List<clsInfoItem>();
		    List<clsInfoItem> AInfo = new List<clsInfoItem>();
		    
		    if (File.Exists(filename) != true)
		        return;
		    
		    string FNameExt = filename.ToLower().Substring(filename.Length - 3);
		    
		    bool resultParse = baseObject.parseMovieFile(filename, 
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
			
			if (baseObject.userDataToChange.Trim() != "")
			    ChangeUserData.Sensitive = true;
			
			if (FNameExt != "avi")
				Change4ccButton.Sensitive = false;
			
			
		    ListStore informations = new ListStore(typeof(string), typeof(string));

			for (int j=0; j<VInfo.Count; j++)
			    informations.AppendValues(VInfo[j].ItemName, VInfo[j].ItemValue);
			
			VideoTreeView.Model = informations;
			

			informations = new ListStore(typeof(string), typeof(string));
			
			for (int j=0; j<AInfo.Count; j++)
			    informations.AppendValues(AInfo[j].ItemName, AInfo[j].ItemValue);

			AudioTreeView.Model = informations;

			if (baseObject.autoReport)
				SaveReport(filename + ".report");
				
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
				                                     			   clsLanguages.FOURCCSURE);
				
				FourCCdialog.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				FourCCdialog.Title = clsLanguages.FOURCCTITLE;
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
			                                     			   clsLanguages.UDATAERROR);
			
				NotProcessable.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				NotProcessable.Title = clsLanguages.UDATATITLE;
				NotProcessable.Run();				
				NotProcessable.Destroy();
				NotProcessable.Dispose();
				NotProcessable = null;
				return;
			
			}
		
			// Open Save Window
			
			tmsWAIT SaveWindow = new tmsWAIT(ref this.MonoSPOTWindow, ref this.baseObject);			
			SaveWindow.saveAvi();
			
			return;
		}
		
		
		// Export the scan result
		private void on_ExportButton_clicked(object sender,EventArgs a)
		{
			if (VideoTreeView.Model.IterNChildren() > 0)
			{
				// New dialog to save file 
				Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog(clsLanguages.EXPORTTITLE, 
				                                this.MonoSPOTWindow,
				                                FileChooserAction.Save, 
				                                clsLanguages.BTCANCEL, Gtk.ResponseType.Cancel,
	                                            clsLanguages.BTACCEPT, Gtk.ResponseType.Accept);
				
				// Manage result of dialog box
				FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				FileBox.SetCurrentFolder(baseObject.defaultPath);
				int retFileBox = FileBox.Run();
				
				if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
					SaveReport(FileBox.Filename);
				
				FileBox.Destroy();
				FileBox.Dispose();
				return;
			
			}
		}
		
		
		private void SaveReport(string ReportPath)
		{			
			// Save File
			DateTime Oggi = DateTime.Now; 
			StreamWriter outputStream = new StreamWriter(ReportPath , false);
			outputStream.WriteLine(("File:").PadRight(25)+ Path.GetFileName(ReportPath));
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


	}

}
