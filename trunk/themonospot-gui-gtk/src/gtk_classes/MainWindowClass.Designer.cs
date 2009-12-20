using System;
using System.IO;
using System.Threading;
using ThemonospotBase;
using System.Reflection;
using System.Collections.Generic;

using Gtk;
using Glade;


namespace ThemonospotGuiGtk
{
	
	
	public partial class MainWindowClass
	{
		
		
		#region Gtk Objects
		
		[Glade.Widget]	Gtk.Window 				MainWindow;
		[Glade.Widget]	Gtk.MenuItem			menuFile;
		[Glade.Widget]	Gtk.MenuItem			menuOptions;
		[Glade.Widget]	Gtk.MenuItem			menuAbout;		
		[Glade.Widget]	Gtk.ImageMenuItem		menuScanFile;
		[Glade.Widget]	Gtk.ImageMenuItem		menuScanFolder;
		[Glade.Widget]	Gtk.ImageMenuItem		menuScanFolderSubfolders;
		[Glade.Widget]	Gtk.ImageMenuItem		menuSaveReport;
		[Glade.Widget]	Gtk.ImageMenuItem		menuExit;
		[Glade.Widget]	Gtk.CheckMenuItem		menuAutoReport;
		[Glade.Widget]	Gtk.ImageMenuItem		menuAboutWindow;
		[Glade.Widget]	Gtk.Frame				grpResult;
		[Glade.Widget]	Gtk.ToolButton			btnScanFile;
		[Glade.Widget]	Gtk.ToolButton			btnScanFolder;
		[Glade.Widget]	Gtk.ToolButton			btnScanFolderSubfolders;
		[Glade.Widget]	Gtk.ToolButton			btnSaveReport;
		[Glade.Widget]	Gtk.ToolButton			btnExit;
		[Glade.Widget]	Gtk.Statusbar			statusBar;
		[Glade.Widget]	Gtk.Label				lblResult;
		[Glade.Widget]	Gtk.Notebook			tabContainer;
		
		
		#endregion Gtk Objects
		
		
		
		// ATTRIBUTES		
		ScanningDialogClass sdc;
		string[] localArgs;
		BaseFactory bf;
		Assembly execAssembly;
		
		List<FileInfoEntity> fiEntity;
		string MainThread = "MainWindowThread";
		string managedExtensions;
		string fileScanning = "";
		bool cancelScan = false;

		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainWindowClass(string[] args)
		{
			localArgs = args;
			
			// Create new instance of Base Component factory class
			if (args.Length > 0)
			{
				if (Convert.ToString(args[0]).ToLower() == "--test")
				{
					bf = new BaseFactory(true);
				}
				else
				{
					bf = new BaseFactory();
				}
			}
			else
			{
				bf = new BaseFactory();
			}
			
			// Update global information in GlobalData static object
			UpdateGlobalObject();
			

			// Update Graphic Objects
			UpdateGraphicObjects();
			
			
			// Add eventhandlers
			UpdateReactors();
			
			
			// Set Top Thread name
			Thread.CurrentThread.Name = MainThread;
			
		}
		
		
		
		// EventHandler for event generated in ScanningDialog
		public void CancelScan(object sender, EventArgs args)
		{
			cancelScan = true;
		}	
		
		
		
		public void ParseArguments()
		{
			// Parse Arguments
			if (localArgs.Length > 0)
			{
				// Disable debug as default
				bf.Console = false;
				

				if (File.Exists(Convert.ToString(localArgs[0])))
				{
					// if is a file					
					OpenFile(Convert.ToString(localArgs[0]));					
					
				}
				else if (Directory.Exists(Convert.ToString(localArgs[0])))
				{
					// if is a folder					
					OpenFolder(Convert.ToString(localArgs[0]), false);					
					
				}
				
				for (int h=0; h<localArgs.Length; h++)
				{
					// Parse all arguments and detect debug
					if (Convert.ToString(localArgs[h]).ToLower() == "debug")
					{
						// if there is debug as argument, re enable debug
						bf.Console = true;
						break;
					}
				}
				
			}	
			
		}
		
		
		
		
		
		private void UpdateGlobalObject()
		{
			// Init language file
			GlobalData.InitLanguage(bf.GetGuiLanguagesPath());

			
			// Update Global informations
			execAssembly = Assembly.GetExecutingAssembly();
			GlobalData.GuiRelease = execAssembly.GetName().Version.Major.ToString() + "." + 
								    execAssembly.GetName().Version.Minor.ToString() + "." + 
				   				    execAssembly.GetName().Version.Build.ToString();
			
			GlobalData.BaseRelease = bf.Release;
			GlobalData.AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName);
			GlobalData.BasePlugins = bf.PluginsAvailable;
			
			// Get AssemblyDescription
			AssemblyDescriptionAttribute adAttr = 
                    (AssemblyDescriptionAttribute)Attribute.GetCustomAttribute(
                    execAssembly, typeof(AssemblyDescriptionAttribute));
			GlobalData.GuiDescription = adAttr.Description;
			
			// Get AssemblyCopyright
			AssemblyCopyrightAttribute acAttr = 
                    (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                    execAssembly, typeof(AssemblyCopyrightAttribute));
			GlobalData.GuiCopyright = acAttr.Copyright;

			// Update managed extensions list
			string[] managedExt = bf.GetManagedExtentions();
			GlobalData.FileDialogExtensions = "Media files (";
			
			foreach (string extension in managedExt)
			{
				GlobalData.FileDialogExtensions += "*." + extension + " ";
				managedExtensions += "." + extension.ToLower() + ";";
			}
			GlobalData.FileDialogExtensions = GlobalData.FileDialogExtensions.Trim() + ")";
		}
		
		
		
		
		private void UpdateGraphicObjects()
		{
			// Set window icon
			Gtk.Window.DefaultIcon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
			
			// Instance glade xml object using glade file
			Glade.XML gxml =  new Glade.XML("MainWindow.glade", "MainWindow");
			
			// Aonnect glade xml object to this Gtk.Window
			gxml.Autoconnect(this);
			
			// Main Window Title
			MainWindow.Title = "Themonospot [Gtk] v" + GlobalData.GuiRelease;
			Gdk.Geometry geo = new Gdk.Geometry();
			geo.MinHeight = 350;
			geo.MinWidth = 650;
			MainWindow.SetGeometryHints(grpResult, geo, Gdk.WindowHints.MinSize);
			
			for (int g=tabContainer.NPages-1; g>=0; g--)
			{
				tabContainer.RemovePage(g);
			}
			
			// LANGUAGE 
			((Label)menuFile.Child).TextWithMnemonic = 
				GlobalData.GetLanguageKeyValue("MAINFILE").Replace("&","_");
			((Label)menuOptions.Child).TextWithMnemonic = 
				GlobalData.GetLanguageKeyValue("MAINOPTIONS").Replace("&","_");
			((Label)menuAbout.Child).TextWithMnemonic = 
				GlobalData.GetLanguageKeyValue("MAINABOUT").Replace("&","_");
			
			((Label)menuScanFile.Child).Text =
				GlobalData.GetLanguageKeyValue("SCANFILE");
			btnScanFile.Label = ((Label)menuScanFile.Child).Text;
			((Label)menuScanFolder.Child).Text =
				GlobalData.GetLanguageKeyValue("SCANFOLDER");
			btnScanFolder.Label = ((Label)menuScanFolder.Child).Text;
			((Label)menuScanFolderSubfolders.Child).Text =
				GlobalData.GetLanguageKeyValue("SCANSUBFOLDERS");
			btnScanFolderSubfolders.Label = ((Label)menuScanFolderSubfolders.Child).Text;
			((Label)menuSaveReport.Child).Text =
				GlobalData.GetLanguageKeyValue("SAVEREPORT");
			btnSaveReport.Label = ((Label)menuSaveReport.Child).Text;
			((Label)menuExit.Child).Text =
				GlobalData.GetLanguageKeyValue("EXIT");
			btnExit.Label = ((Label)menuExit.Child).Text;
			
			((Label)menuAutoReport.Child).Text =
				GlobalData.GetLanguageKeyValue("AUTOREPORT");
			
			((Label)menuAboutWindow.Child).Text =
				GlobalData.GetLanguageKeyValue("INFOABOUT") + " Themonospot [Gtk]";
			
			lblResult.Markup = "<b>" + GlobalData.GetLanguageKeyValue("TITLERESULT") + "</b>";
			

			// STATUS TIP FOR WIDGETS NOT IMPLEMENTED

				
			// LANGUAGE TOOL TIP
			btnScanFile.TooltipText = GlobalData.GetLanguageKeyValue("SCANFILETT");
			menuScanFile.TooltipText = btnScanFile.TooltipText;
			btnScanFolder.TooltipText = GlobalData.GetLanguageKeyValue("SCANFOLDERTT");
			menuScanFolder.TooltipText = btnScanFolder.TooltipText;			
			btnScanFolderSubfolders.TooltipText = GlobalData.GetLanguageKeyValue("SCANSUBFOLDERSTT");
			menuScanFolderSubfolders.TooltipText = btnScanFolderSubfolders.TooltipText;
			btnSaveReport.TooltipText = GlobalData.GetLanguageKeyValue("SAVEREPORTTT");
			menuSaveReport.TooltipText = btnSaveReport.TooltipText;
			btnExit.TooltipText = GlobalData.GetLanguageKeyValue("EXITTT");
			menuExit.TooltipText = btnExit.TooltipText;
			menuAutoReport.TooltipText = GlobalData.GetLanguageKeyValue("AUTOREPORTTT");
			menuAboutWindow.TooltipText = GlobalData.GetLanguageKeyValue("INFOABOUTTT");

			
			MainWindow.Show();
			
		}

		
		
		
		
		
		// Set Event Handlers
		private void UpdateReactors()
		{
			
			// Enable Drop on MainWindow
            TargetEntry[] VtvD = new TargetEntry[]{
                    new TargetEntry("text/uri-list", 0, 0),
                    new TargetEntry("text/plain", 0, 1),
                    new TargetEntry("STRING", 0, 1)
            };
			
			Gtk.Drag.DestSet(MainWindow, 
                             DestDefaults.All, VtvD, 
                             Gdk.DragAction.Copy | 
                             Gdk.DragAction.Move |
                             Gdk.DragAction.Link |
                             Gdk.DragAction.Ask);
            
			MainWindow.DragDataReceived += MainWindow_Selection_Received;
			MainWindow.DeleteEvent += MainWindow_Delete;
			
			menuExit.Activated += MainWindow_Delete;
			btnExit.Clicked += MainWindow_Delete;
			
			menuScanFile.Activated += ActionOpen;
			btnScanFile.Clicked += ActionOpen;
			
			menuScanFolder.Activated += ActionOpen;
			btnScanFolder.Clicked += ActionOpen;
			
			menuScanFolderSubfolders.Activated += ActionOpen;
			btnScanFolderSubfolders.Clicked += ActionOpen;
			
			menuSaveReport.Activated += ActionReport;
			btnSaveReport.Clicked += ActionReport;
			
			menuAutoReport.Activated += ActionAutoReport;
			menuAboutWindow.Activated += ActionInfo;
			
			tabContainer.SwitchPage += TabChanged;
			
		}
		
		
		
		#region REACTORS
		
		
		/// <summary>
		/// Received a Drop on MainWindow
		/// </summary>		
		public void MainWindow_Selection_Received(object sender, DragDataReceivedArgs InArgs)
        {
			// Extract info from received data
            string Data = System.Text.Encoding.UTF8.GetString(InArgs.SelectionData.Data);
            string DropFileName = "";
			
			// remove header and footer of file/folder path
            Data = Data.Replace(@"file://", "");
            int EndLine = Data.IndexOf(Environment.NewLine);
            
            if (EndLine >0)
			{
            	Data = Data.Substring(0, Data.IndexOf(Environment.NewLine));
			}
            
			// Extract correct file/folder path
            Uri TheFile = new Uri(Data);
            DropFileName = Path.GetFullPath(TheFile.LocalPath);
            
			Gtk.Drag.Finish(InArgs.Context,true, false, InArgs.Time);
			
			// Exit is file or folder don't exists
            if (File.Exists(DropFileName))
			{
            	// Is a file 
				OpenFile(DropFileName);
			}
			else if (Directory.Exists(DropFileName))
			{
				// Is a Folder
				OpenFolder(DropFileName, false);
			}
			else
			{
				return;
			}
                
        }

		
		
		
		
		
		// If close main window
		private void MainWindow_Delete(object sender,EventArgs a)
		{
			// close also Gtk Application
			ActionExit();
			
		}
		
		
		
		// Exit
		public void ActionExit()
		{
			Gtk.Application.Quit();
		}

		
		
		
		// recall open action passing action name
		public void ActionOpen(object sender,EventArgs a)
		{
			
			if ((((Widget)sender).Name == "menuScanFile") ||
			    (((Widget)sender).Name == "btnScanFile"))
			{
				// Scan single file
				OpenAction("ScanFile");
			}
			else if ((((Widget)sender).Name == "menuScanFolder") ||
			    (((Widget)sender).Name == "btnScanFolder"))
			{
				// Scan files in a single folder
				OpenAction("ScanFolder");
			}
			else if ((((Widget)sender).Name == "menuScanFolderSubfolders") ||
			    (((Widget)sender).Name == "btnScanFolderSubfolders"))
			{
				// Scan files in a folder and his subfolders
				OpenAction("ScanFolderSubfolders");
			}
			
		}
		
		
		
		// Recall report save method
		public void ActionReport(object sender,EventArgs a)
		{
			SaveReport();
		}
		
		
		
		
		// Open About window
		public void ActionInfo(object sender,EventArgs a)
		{
			OpenInfo();
		}
		
		
		
		
		// Update AutoReport option		
		public void ActionAutoReport(object sender,EventArgs a)
		{
			if (menuAutoReport.Active &&
			    !bf.IsReportAuto)
			{
				bf.IsReportAuto = true;
			}
			else if (!menuAutoReport.Active &&
			    bf.IsReportAuto)
			{
				bf.IsReportAuto = false;
			}
		}
		
		
		
		
		
		
		
		
		public void TabChanged(object sender, SwitchPageArgs args)
		{
			// Change status bar message
			Label tabLabel = (Label)tabContainer.GetTabLabel(tabContainer.CurrentPageWidget);
			statusBar.Push(1, tabLabel.TooltipText);
		}
		
		
		
		
		
		#endregion REACTORS
		
		
		
		
		
		
	}
}
