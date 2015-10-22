
using System;
using System.Threading;
using Qyoto;
using System.IO;
using ThemonospotBase;
using ThemonospotComponents;
using System.Reflection;
using System.Collections.Generic;
using log4net;

namespace ThemonospotGuiQt
{
	
	
	public partial class MainWindowClass: QMainWindow
	{
		
		// ATTRIBUTES		
		Ui.MainWindow mainwindow_Ui;
		string[] localArgs;
		ScanningDialogClass sdc;		
		Assembly execAssembly;
		
		List<FileInfoEntity> fiEntity;
		string MainThread = "MainWindowThread";
		string managedExtensions;
		string fileScanning = "";
		bool cancelScan = false;

		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(MainWindowClass));
		
		
		// CONSTRUCTOR
		public MainWindowClass(string[] args)
		{
			localArgs = args;
			
			// Update global information in GlobalData static object
			UpdateGlobalObject();
			
			// Create new mainwindow_Ui object
			mainwindow_Ui = new Ui.MainWindow();
			
			// Configure layout of this new QMainWindow with 
			// mainwindow_Ui objects and data
			mainwindow_Ui.SetupUi(this);
			
			// Update Graphic Objects
			UpdateGraphicObjects();
			
			// set autoreport flag
			mainwindow_Ui.actionAutoReport.Checked = GlobalData.bFactory.IsReportAuto;

			mainwindow_Ui.tabContainer.Clear();

			// Add eventhandlers
			UpdateReactors();
			
			
			// Set Top Thread name
			Thread.CurrentThread.Name = MainThread;
			
		}
		
		
		
	
		private void UpdateGlobalObject()
		{
			
			// Init language file
			GlobalData.InitLanguage( GlobalData.bFactory.GetGuiLanguagesPath());
			
			// Update Global informations
			execAssembly = Assembly.GetExecutingAssembly();
			GlobalData.GuiRelease = execAssembly.GetName().Version.ToString(3);
			
			GlobalData.BaseRelease = GlobalData.bFactory.Release;
			GlobalData.AppPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().FullName);

			
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
			string[] managedExt = GlobalData.bFactory.GetManagedExtentions();
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
			// Main Window Title
			this.WindowTitle = "Themonospot [Qt] v" + GlobalData.GuiRelease;
			
			// LANGUAGE 
			mainwindow_Ui.menu_File.Title = GlobalData.GetLanguageKeyValue("MAINFILE");
			mainwindow_Ui.actionScanFile.Text = GlobalData.GetLanguageKeyValue("SCANFILE");
			mainwindow_Ui.actionScanFolder.Text = GlobalData.GetLanguageKeyValue("SCANFOLDER");
			mainwindow_Ui.actionScanFolderSubfolders.Text = GlobalData.GetLanguageKeyValue("SCANSUBFOLDERS");
			mainwindow_Ui.actionSaveReport.Text = GlobalData.GetLanguageKeyValue("SAVEREPORT");
			mainwindow_Ui.actionExit.Text = GlobalData.GetLanguageKeyValue("EXIT");
			mainwindow_Ui.menu_Options.Title = GlobalData.GetLanguageKeyValue("MAINOPTIONS");
			mainwindow_Ui.menu_About.Title = GlobalData.GetLanguageKeyValue("MAINABOUT");
			mainwindow_Ui.actionAutoReport.Text = GlobalData.GetLanguageKeyValue("AUTOREPORT");
			mainwindow_Ui.actionInfoAbout.Text = GlobalData.GetLanguageKeyValue("INFOABOUT") + " Themonospot [Qt]";
			mainwindow_Ui.grpResult.Title = GlobalData.GetLanguageKeyValue("TITLERESULT");
			
			// LANGUAGE STATUS TIP
			mainwindow_Ui.actionScanFile.StatusTip = GlobalData.GetLanguageKeyValue("SCANFILETT");
			mainwindow_Ui.actionScanFolder.StatusTip = GlobalData.GetLanguageKeyValue("SCANFOLDERTT");
			mainwindow_Ui.actionScanFolderSubfolders.StatusTip = GlobalData.GetLanguageKeyValue("SCANSUBFOLDERSTT");
			mainwindow_Ui.actionSaveReport.StatusTip = GlobalData.GetLanguageKeyValue("SAVEREPORTTT");
			mainwindow_Ui.actionExit.StatusTip = GlobalData.GetLanguageKeyValue("EXITTT");
			mainwindow_Ui.actionAutoReport.StatusTip = GlobalData.GetLanguageKeyValue("AUTOREPORTTT");
			mainwindow_Ui.actionInfoAbout.StatusTip = GlobalData.GetLanguageKeyValue("INFOABOUTTT");
			
				
			// LANGUAGE TOOL TIP
			mainwindow_Ui.actionScanFile.ToolTip = mainwindow_Ui.actionScanFile.StatusTip;
			mainwindow_Ui.actionScanFolder.ToolTip = mainwindow_Ui.actionScanFolder.StatusTip;
			mainwindow_Ui.actionScanFolderSubfolders.ToolTip = mainwindow_Ui.actionScanFolderSubfolders.StatusTip;
			mainwindow_Ui.actionSaveReport.ToolTip = mainwindow_Ui.actionSaveReport.StatusTip;
			mainwindow_Ui.actionExit.ToolTip = mainwindow_Ui.actionExit.StatusTip;
			mainwindow_Ui.actionAutoReport.ToolTip =  mainwindow_Ui.actionAutoReport.StatusTip;
			mainwindow_Ui.actionInfoAbout.ToolTip = mainwindow_Ui.actionInfoAbout.StatusTip;
			
			
			
			
			
		}
			
		
		
		
		
		
		private void UpdateReactors()
		{
			// Drop enable
			this.AcceptDrops = true;
			
			// Configure events reactors
			Connect( mainwindow_Ui.actionExit, SIGNAL("activated()"), this, SLOT("ActionExit()"));
			Connect( mainwindow_Ui.actionScanFile, SIGNAL("activated()"), this, SLOT("ActionOpen()"));
			Connect( mainwindow_Ui.actionScanFolder, SIGNAL("activated()"), this, SLOT("ActionOpen()"));
			Connect( mainwindow_Ui.actionScanFolderSubfolders, SIGNAL("activated()"), this, SLOT("ActionOpen()"));
			Connect( mainwindow_Ui.actionSaveReport, SIGNAL("activated()"), this, SLOT("ActionReport()"));
			Connect( mainwindow_Ui.actionInfoAbout, SIGNAL("activated()"), this, SLOT("ActionInfo()"));
			Connect( mainwindow_Ui.actionAutoReport, SIGNAL("activated()"), this, SLOT("ActionAutoReport()"));
			Connect( mainwindow_Ui.tabContainer, SIGNAL("currentChanged(int)"), this, SLOT("TabChanged(int)"));
						
		}
		
		
		
		
		
		#region Q_SLOTS
		
		
		[Q_SLOT]
		public void ActionExit()
		{
			QApplication.Quit();
		}

		
		
		
		[Q_SLOT]		
		public void ActionOpen()
		{
			// retrieve action name
			QAction qws = (QAction)Sender();
			
			// recall open action passing action name
			OpenAction(qws.ObjectName);
			
		}
		
		
		
		[Q_SLOT]
		public void ActionReport()
		{
			SaveReport();
		}
		
		
		
		
		[Q_SLOT]		
		public void ActionInfo()
		{
			OpenInfo();
			
		}
		
		
		
		
		// Update AutoReport option
		[Q_SLOT]
		public void ActionAutoReport()
		{
			if (mainwindow_Ui.actionAutoReport.Checked == true &&
			    GlobalData.bFactory.IsReportAuto != true)
			{
				GlobalData.bFactory.IsReportAuto = true;
			}
			else if (mainwindow_Ui.actionAutoReport.Checked == false &&
			    GlobalData.bFactory.IsReportAuto != false)
			{
				GlobalData.bFactory.IsReportAuto = false;
			}
			
		}
		
		
		
		
		
		
		
		[Q_SLOT]
		public void TabChanged(int tabIndex)
		{
			mainwindow_Ui.statusbar.ShowMessage(mainwindow_Ui.tabContainer.TabToolTip(tabIndex),20000);
		}
		
		
		
		
		
		#endregion Q_SLOTS
		
		
		
		// First step to drop process
		protected override void DragEnterEvent(QDragEnterEvent dragEvent)
		{
			// detect drop data type
			QMimeData qmd = dragEvent.MimeData();
			if (qmd.HasText() || qmd.HasUrls() || qmd.HasFormat("STRING"))
			{
				dragEvent.AcceptProposedAction();
			}
			else
			{
				dragEvent.Ignore();
			}			
		}
		
		// Final drop action
		protected override void DropEvent(QDropEvent dropEvent)
		{
			QMimeData qmd = dropEvent.MimeData();
			
			if (qmd.HasText() || qmd.HasUrls() || qmd.HasFormat("STRING"))
			{
				dropEvent.Accept();
			}
			else
			{
				dropEvent.Ignore();
				return;
			}
			
			// parse data dropped
			string dataIN = qmd.Text();
			
			if (dataIN.IndexOf(@"file://") != 0)
			{
				// isn't path
				return;
			}
			
			// cut file:// from path
			dataIN = dataIN.Substring(7);
			
			if (File.Exists(dataIN))
			{
				// open file
				OpenFile(dataIN);
			}
			else if (Directory.Exists(dataIN))
			{
				// open folder
				OpenFolder(dataIN, false);
			}
			
		}
		
		
		
		

		
		
		// EventHandler for event generated in ScanningDialog
		public void CancelScan(object sender, EventArgs args)
		{
			cancelScan = true;
		}				
		
		
		// Parse  command line arguments to detect
		// startup operation
		public void ParseAguments()
		{
			// Parse Arguments
			if (localArgs.Length > 0)
			{
				// Check for last command argument
				if (File.Exists(Convert.ToString(localArgs[localArgs.Length-1])))
				{
					// if is a file					
					OpenFile(Convert.ToString(localArgs[localArgs.Length-1]));					
					
				}
				else if (Directory.Exists(Convert.ToString(localArgs[localArgs.Length-1])))
				{
					// if is a folder					
					OpenFolder(Convert.ToString(localArgs[localArgs.Length-1]), false);					
					
				}
				
				
			}	
			
		}
		
		
		
		
		
		
		

		
		
	}
}
