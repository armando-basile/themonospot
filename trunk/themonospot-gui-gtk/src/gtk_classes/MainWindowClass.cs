
using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using Gtk;
using Gdk;
using Glade;
using ThemonospotBase;

namespace ThemonospotGuiGtk
{
	
	
	public partial class MainWindowClass
	{
		

		
		private void OpenInfo()
		{
			AboutDialogClass awc = new AboutDialogClass(ref MainWindow);
			// awc.Show();
		}
		
		
		
		
		/// <summary>
		/// Detect method to use
		/// </summary>
		private void OpenAction(string openType)
		{
			
			if (openType == "ScanFile")
			{
				// Try to scan only single file
				OpenFile();
				return;				
			}
			
			if (openType == "ScanFolder")
			{
				// Try to scan all files contained in
				// a specific folder
				OpenFolder(false);
				return;				
			}
			
			if (openType == "ScanFolderSubfolders")
			{
				// Try to scan all files contained in
				// a specific folder and her subfolders
				OpenFolder(true);
				return;
			}
			
		}

		
		
		
		
		
		private void OpenFile()
		{
			string selectedFile = "";
			
			Gtk.FileChooserDialog FileBox = 
				new Gtk.FileChooserDialog(GlobalData.GetLanguageKeyValue("FILEOPEN"), 
                                          MainWindow,
                                          FileChooserAction.Open,
                                          GlobalData.GetLanguageKeyValue("BTNCANCEL"), 
				                          Gtk.ResponseType.Cancel,
				                          GlobalData.GetLanguageKeyValue("BTNOPEN"), 
                                          Gtk.ResponseType.Accept);
                        
            FileBox.WindowPosition= WindowPosition.CenterOnParent;
			FileFilter filter = new FileFilter();
			filter.Name = GlobalData.FileDialogExtensions;
            string[] managedExt = bf.GetManagedExtentions();
			
			for (int j=0; j<managedExt.Length; j++)
			{
				filter.AddPattern("*." + managedExt[j]);
			}
			
			FileBox.AddFilter(filter);
			
            // Manage result of dialog box
            FileBox.Icon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
            FileBox.SetCurrentFolder(bf.DefaultPath);
            int retFileBox = FileBox.Run();
            if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
            {       
                    // path of a right file returned
                    selectedFile = FileBox.Filename.ToString();                         
                    FileBox.Destroy();
                    FileBox.Dispose();
            
            }
            else
            {
                    // nothing returned
                    FileBox.Destroy();
                    FileBox.Dispose();
                    return;
            }

			
			// recall other constructor
			OpenFile(selectedFile);
		}
		
		
		
		
		
		/// <summary>
		/// Try to open a single file
		/// </summary>
		private void OpenFile(string selectedFile)
		{
			// clear tabContainer from all tabs
			tabContainer.Visible = false;			
			for (int g=(tabContainer.NPages-1); g>=0; g--)
			{
				tabContainer.RemovePage(g);
			}
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
			// create new instance of FileInfoEntity
			fiEntity = new List<FileInfoEntity>();
			
			// Extract info
			ScanFile(selectedFile);
			
			// Update Gui
			if (fiEntity.Count > 0)
			{
				if (fiEntity[0].Errors == "")
				{
					// if scan worked ok
					FillListView(fiEntity[0].Video, fiEntity[0].Audio, fiEntity[0].FilePath);
				}
				else
				{
					// if scan generated errors
					ShowWarning("Error Detected",
					            "File: " + fiEntity[0].FileName + "\r\n\r\n" +
					            "Error: " + fiEntity[0].Errors);
				}				
			}
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
			// set visible tabcontainer
			tabContainer.Visible = true;
			tabContainer.ShowAll();
			

		}

		
		
		
		
		
		/// <summary>
		/// Try to open all file contained in a specific
		/// folder (optional also in her subfolders)
		/// </summary>
		private void OpenFolder(bool recursive)
		{
			string selectedFolder = "";
			
			Gtk.FileChooserDialog FileBox = 
				new Gtk.FileChooserDialog(GlobalData.GetLanguageKeyValue("FOLDEROPEN"), 
                                          MainWindow,
                                          FileChooserAction.SelectFolder,
                                          GlobalData.GetLanguageKeyValue("BTNCANCEL"), 
				                          Gtk.ResponseType.Cancel,
				                          GlobalData.GetLanguageKeyValue("BTNOPEN"), 
                                          Gtk.ResponseType.Accept);
                        
            FileBox.WindowPosition= WindowPosition.CenterOnParent;
            
            // Manage result of dialog box
            FileBox.Icon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
            FileBox.SetCurrentFolder(bf.DefaultPath);
            int retFileBox = FileBox.Run();
            if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
            {       
                    // path of a right file returned
                    selectedFolder = FileBox.Filename.ToString();                         
                    FileBox.Destroy();
                    FileBox.Dispose();
            
            }
            else
            {
                    // nothing returned
                    FileBox.Destroy();
                    FileBox.Dispose();
                    return;
            }
			
			// Recall other constructor
			OpenFolder(selectedFolder, recursive);

		}	
		
		
		
		
		
		private void OpenFolder(string selectedFolder, bool recursive)
		{
			// clear tabContainer from all tabs
			tabContainer.Visible = false;			
			for (int g=tabContainer.NPages-1; g>=0; g--)
			{
				tabContainer.RemovePage(g);
			}
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
			
			// Create new instance of ScanningDialog
			cancelScan = false;
			fiEntity = new List<FileInfoEntity>();
			fileScanning = "";
			sdc = new ScanningDialogClass(ref MainWindow);
			sdc.CancelScan += CancelScan;			
			
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
			// Launch StartScanFolder in a new Thread			
			ThreadStart ts = new ThreadStart( delegate { StartScanFolder(selectedFolder, recursive); } );
			Thread scanThread = new Thread(ts);
			scanThread.Start();	
			
			// Loop to wait end of scan
			WaitForScan((double)400);
			
			// Update Gui
			if (fiEntity.Count > 0)
			{
				for (int h=0; h<fiEntity.Count; h++)
				{
					if (fiEntity[h].Errors == "")
					{
						// if scan worked ok
						FillListView(fiEntity[h].Video, fiEntity[h].Audio, fiEntity[h].FilePath);
					}
					else
					{
						// if scan generated errors
						ShowWarning("Error Detected",
						            "File: " + fiEntity[h].FileName + "\r\n\r\n" +
						            "Error: " + fiEntity[h].Errors);
					}
				}
			}
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
			// set visible tabContainer
			tabContainer.Visible=true;
			tabContainer.ShowAll();
			
		}
		
		
		
		


		
		/// <summary>
		/// Generate report file
		/// </summary>
		private void SaveReport()
		{
			if (tabContainer.NPages < 1)
			{
				// If there isn't any selected tab, exit
				return;
			}
			
			
			string selectedFile = "";
			
			Gtk.FileChooserDialog FileBox = 
				new Gtk.FileChooserDialog(GlobalData.GetLanguageKeyValue("FILESAVE"), 
                                          MainWindow,
                                          FileChooserAction.Save,
				                          GlobalData.GetLanguageKeyValue("BTNCANCEL"), 
                                          Gtk.ResponseType.Cancel,
				                          GlobalData.GetLanguageKeyValue("BTNSAVE"),
                                          Gtk.ResponseType.Accept);
                        
            FileBox.WindowPosition= WindowPosition.CenterOnParent;
			FileFilter filter = new FileFilter();
            filter.AddPattern("*.report");
			filter.Name = "Report file (*.report)";
			FileBox.AddFilter(filter);
			
            // Manage result of dialog box
            FileBox.Icon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
            FileBox.SetCurrentFolder(bf.DefaultPath);
            int retFileBox = FileBox.Run();
            if ((ResponseType)retFileBox == Gtk.ResponseType.Accept)
            {       
                    // path of a right file returned
                    selectedFile = FileBox.Filename.ToString();                         
                    FileBox.Destroy();
                    FileBox.Dispose();
            
            }
            else
            {
                    // nothing returned
                    FileBox.Destroy();
                    FileBox.Dispose();
                    return;
            }
			
			
			// generate report
			bf.SaveReportFile(fiEntity[tabContainer.Page].Video,
			                  fiEntity[tabContainer.Page].Audio,
			                  fiEntity[tabContainer.Page].FileName,
			                  selectedFile,
			                  fiEntity[tabContainer.Page].PluginUsed);
			
			// Message of report generated
			MessageDialog dlg = new MessageDialog(MainWindow, 
			                                      DialogFlags.Modal, 
                                                  MessageType.Info, 
			                                      ButtonsType.Ok,
                                                  GlobalData.GetLanguageKeyValue("SAVEREPORTMSG") + "\r\n" +
			                        			  fiEntity[tabContainer.Page].FileName);
            dlg.Title = GlobalData.GetLanguageKeyValue("SAVEREPORTWIN");
            dlg.Run();
            dlg.Destroy();
            return;

		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Scan single multimedia file and add result to 
		/// collection results
		/// </summary>		
		private void ScanFile(string filePath)
		{
			
			// create informations containers
			List<string[]> audio = new List<string[]>();
			List<string[]> video = new List<string[]>();
			string pluginParameter = "";
			
			FileInfoEntity fie = new FileInfoEntity();
			fie.FilePath = filePath;
			
			try
			{
				// Extract info from file using themonospot-base
				bf.GetFileInfo(filePath, ref video, ref audio, ref pluginParameter);
				
			}
			catch(ThemonospotBaseException tbexp)
			{
				ThemonospotLogger.Append("\r\n" + 
				                         tbexp.Method + "\r\n" +
				                         tbexp.Message);
				
				fie.Errors = Path.GetFileName(filePath) + "\r\n" +
					         "internal method [" + tbexp.Method + "]\r\n" +
					         "message: " + tbexp.Message;					
				return;
				                    
			}
			catch(Exception ex)
			{
				ThemonospotLogger.Append("\r\n" + 
				                         "MainWindowClass::ScanFile\r\n\r\n" +
				                         ex.Message + "\r\n\r\n" +
				                         ex.StackTrace);

				fie.Errors = Path.GetFileName(filePath) + "\r\n" +
				             "message: " + ex.Message;
				return;
			}
			
			
			// add scan result to entity
			fie.Audio = audio;
			fie.Video = video;
			fie.PluginUsed = pluginParameter;
			
			fiEntity.Add(fie);
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Add new tab to Notebook object and fill it with 
		/// scan result of multimedia file
		/// </summary>
		private void FillListView(List<string[]> video, List<string[]> audio, string filePath)
		{
			TreeStore store = null;
			
			ScrolledWindow sw = new ScrolledWindow();

			Label lblIndex = 
				new Label(Path.GetFileName(filePath).PadRight(25).Substring(0,25).Trim());
			lblIndex.TooltipText = Path.GetFileName(filePath);
						
			tabContainer.AppendPage(sw, lblIndex);			
			
			TreeView tv = new TreeView();
			tv.HeadersVisible = true;

			tv.AppendColumn ("", new CellRendererPixbuf(), "pixbuf", 0);
			tv.AppendColumn ("Description", new CellRendererText(), "text", 1);
			tv.AppendColumn ("Value", new CellRendererText(), "text", 2);
			
			sw.Add(tv);
			
			store = new TreeStore (typeof (Gdk.Pixbuf), typeof (string), typeof (string));
			TreeIter iter = store.AppendValues (Gdk.Pixbuf.LoadFromResource("video.png"),
			                                    "Video", "");
			
			for (int k=0; k<video.Count; k++)
			{
				store.AppendValues(iter, null, video[k][0], video[k][1]);
			}
			
			
			iter = store.AppendValues (Gdk.Pixbuf.LoadFromResource("sound.png"),
			                           "Audio", "");
			
			for (int k=0; k<audio.Count; k++)
			{
				store.AppendValues(iter, null, audio[k][0], audio[k][1]);
			}

			tv.Model = store;
			tv.ExpandAll();
		}
		

		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Launch scan folder operation
		/// </summary>
		private void StartScanFolder(string folderPath, bool recursive)
		{
			ScanFolder(folderPath, recursive);
			
			// to close ScanDialog
			cancelScan = true;
		}
		
		
		
		
		
		
		/// <summary>
		/// Scan multimedia file contained in a specific folder
		/// and her subfolders 
		/// </summary>
		private void ScanFolder(string folderPath, bool recursive)
		{
			
			if (cancelScan)
			{
				return;
			}
			
			DirectoryInfo di = new DirectoryInfo(folderPath);
			
			// Detect if recursive mode is enabled
			if(recursive)
			{
				foreach (DirectoryInfo sdi in di.GetDirectories()) 
				{
					// Recall CallBack Function ScanFolder
					ScanFolder(sdi.FullName, true);
					
				}
			}
			
			
			foreach (FileInfo fi in di.GetFiles()) 
			{
				// verify Cancel Flag
				if (cancelScan)
				{
					return;
				}
				
				// extract file extension
				string extension = fi.Extension;
				
				// if extension is managed, recall scanfile
				if ((managedExtensions.IndexOf(extension.ToLower() + ";") >= 0) &&
					(extension != ""))
				{
					
					// launch scan of file
					fileScanning = fi.Name;
					ScanFile(fi.FullName);
				}
				
			}
		}
		
		
		
		
		
		
		/// <summary>
		/// Loop to wait end of scanning and update scanning info
		/// on ScanningDialog window
		/// </summary>
		private void WaitForScan(double waitTimeMilliseconds)
		{
			
			
			// loop until scan thread working
			while (cancelScan == false)
			{
				if (fileScanning != sdc.FileName)
				{
					// update ScanDialog if necessary
					sdc.FileName = fileScanning;
					
					while (Gtk.Application.EventsPending ())
                        Gtk.Application.RunIteration ();

					
				}
				
				// wait some milliseconds
				DateTime waitEnd = DateTime.Now.AddMilliseconds(waitTimeMilliseconds);
				while(DateTime.Now <= waitEnd)
				{
					while (Gtk.Application.EventsPending ())
                        Gtk.Application.RunIteration ();

				}
				                       
			}
			
			// close ScanDialog window
			sdc.Close();
			
		}
		

		
		
		
		
		
		/// <summary>
		/// Show warning message box
		/// </summary>
		private void ShowWarning(string title, string message)
		{
			// Message
			MessageDialog dlg = new MessageDialog(MainWindow, 
			                                      DialogFlags.Modal, 
                                                  MessageType.Warning, 
			                                      ButtonsType.Ok,
                                                  message);
            dlg.Title = title;
            dlg.Run();
            dlg.Destroy();
            return;
		}
		
		
		
		
		
		
		
		
		
		
		
		
	}
}
