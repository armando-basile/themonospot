using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization.Formatters.Soap;
using Gtk;
using Glade;
using System.IO;
using Utility;

namespace monoSpotMain {

	public class monoSPOT {
		
		[Glade.Widget]			Window MonoSPOTWindow;    
		[Glade.Widget]			FileChooserButton FilenameChooser;
		[Glade.Widget]			Button ExportButton;
		[Glade.Widget]			Button infoButton;
		[Glade.Widget]			Image VideoImage;
		[Glade.Widget]			Image AudioImage;
		[Glade.Widget]			TreeView VideoTreeView;
		[Glade.Widget]			TreeView AudioTreeView;

		// Create new instance of monoSPOT
		public monoSPOT() 
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
			
			MonoSPOTWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
	                  " v" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
	                  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
	                  "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			
			MonoSPOTWindow.DeleteEvent += On_MainWindow_Delete;
			MonoSPOTWindow.ShowAll();
		}
    
		// Entry Point
		public static void Main (string[] args)
		{
			Gtk.Application.Init();
			new monoSPOT();
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
			monoSpotParser parser = null;
			
			if (File.Exists(filename) != true)
				return;
			
			try 
			{
				parser = new monoSpotParser();
				parser.OpenAviFile(filename);
			} 
			catch (monoSpotParserException e) 
			{
				MessageDialog dlg = new MessageDialog (MonoSPOTWindow, DialogFlags.Modal, 
				                                       MessageType.Warning, ButtonsType.Ok,
				                                       "Error: " + e.Message);
				dlg.Title = "monoSpotParser Error";
				dlg.Run();
				dlg.Destroy();
				return;		   
			} 
			catch (Exception e) 
			{
				MessageDialog dlg = new MessageDialog (MonoSPOTWindow, DialogFlags.Modal, 
				                                       MessageType.Warning, ButtonsType.Ok,
				                                       "Error: " + e.Message);
				dlg.Title = "Generic Error";
				dlg.Run();
				dlg.Destroy();
				return;	
			}

			if (parser == null)
				return;

			ListStore informations = new ListStore(typeof(string), typeof(string));
			
			foreach (DictionaryEntry entry in parser.GetVideoInformations())
				informations.AppendValues(entry.Key, entry.Value);
			
			VideoTreeView.Model = informations;
			
			informations = new ListStore(typeof(string), typeof(string));
			
			foreach (DictionaryEntry entry in parser.GetAudioInformations())
				informations.AppendValues(entry.Key, entry.Value);
			
			AudioTreeView.Model = informations;
		}


		private void on_infoButton_button_release_event(object sender,EventArgs a)
		{
			Console.WriteLine("info...");
			monoSPOTinfo infoWin = new monoSPOTinfo(this.MonoSPOTWindow, DialogFlags.DestroyWithParent);
		}
		
		// The On_MainWindow_Delete method is used when we want quit.
		private void On_MainWindow_Delete(object sender,EventArgs a)
    	{
        	Gtk.Application.Quit();
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
			    	
			    	
			    	// Video Stream
			    	TreeIter tmpIter = new TreeIter();
			    	VideoTreeView.Model.GetIterFirst(out tmpIter);
			    	string tmpItem1 = (string) VideoTreeView.Model.GetValue(tmpIter, 0);
					string tmpItem2 = (string) VideoTreeView.Model.GetValue(tmpIter, 1);
					outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
					
					while(VideoTreeView.Model.IterNext(ref tmpIter)) 
					{
		  				tmpItem1 = (string) VideoTreeView.Model.GetValue(tmpIter,0);
		  				tmpItem2 = (string) VideoTreeView.Model.GetValue(tmpIter,1);
						outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
					}
		    		
		    		outputStream.WriteLine("");
		    		
		    		// Audio Stream
		    		AudioTreeView.Model.GetIterFirst(out tmpIter);
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
		  			outputStream.WriteLine("Report generated by themonospot v" + 
		  									Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
				                  			"." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
				                  			"." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString());
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
