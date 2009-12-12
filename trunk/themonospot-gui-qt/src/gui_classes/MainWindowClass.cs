
using System;
using System.Threading;
using Qyoto;
using System.IO;
using ThemonospotBase;
using System.Reflection;
using System.Collections.Generic;


namespace ThemonospotGuiQt
{
	
	
	public partial class MainWindowClass: QMainWindow
	{


		
		/// <summary>
		/// Detect method to use
		/// </summary>
		private void OpenAction(string openType)
		{
			
			if (openType == "actionScanFile")
			{
				// Try to scan only single file
				OpenFile();
				return;				
			}
			
			if (openType == "actionScanFolder")
			{
				// Try to scan all files contained in
				// a specific folder
				OpenFolder(false);
				return;				
			}
			
			if (openType == "actionScanFolderSubfolders")
			{
				// Try to scan all files contained in
				// a specific folder and her subfolders
				OpenFolder(true);
				return;
			}
			
		}
		
		
		
		

		
		
		
		
		/// <summary>
		/// Try to open a single file
		/// </summary>
		private void OpenFile()
		{
			string selectedFile = 
				QFileDialog.GetOpenFileName(this, 
			                                GlobalData.GetLanguageKeyValue("FILEOPEN"),
			                                bf.DefaultPath,
			                                GlobalData.FileDialogExtensions);
				
			if (string.IsNullOrEmpty(selectedFile))
			{
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
			mainwindow_Ui.tabContainer.SetVisible(false);
			mainwindow_Ui.tabContainer.Clear();
			QApplication.ProcessEvents();		
			fiEntity = new List<FileInfoEntity>();
			
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
			
			QApplication.ProcessEvents();
			
			// set visible tabcontainer
			mainwindow_Ui.tabContainer.SetVisible(true);

		}
		
		
		
		
		
		/// <summary>
		/// Try to open all file contained in a specific
		/// folder (optional also in her subfolders)
		/// </summary>
		private void OpenFolder(bool recursive)
		{
			string selectedFolder = 
				QFileDialog.GetExistingDirectory(this, 
			                                GlobalData.GetLanguageKeyValue("FOLDEROPEN"),
			                                bf.DefaultPath);
				
			if (string.IsNullOrEmpty(selectedFolder))
			{
				return;
			}
			
			// Recall other constructor
			OpenFolder(selectedFolder, recursive);

		}		
		
		
		
		
		/// <summary>
		/// Try to open all file contained in a specific
		/// folder (optional also in her subfolders)
		/// </summary>
		private void OpenFolder(string selectedFolder, bool recursive)
		{
			
			// clear tabContainer from all tabs
			mainwindow_Ui.tabContainer.SetVisible(false);
			mainwindow_Ui.tabContainer.Clear();
			QApplication.ProcessEvents();
			
			
			// Create new instance of ScanningDialog
			cancelScan = false;
			fiEntity = new List<FileInfoEntity>();
			fileScanning = "";
			sdc = new ScanningDialogClass();
			sdc.CancelScan += CancelScan;			
			sdc.Show();
			
			QApplication.ProcessEvents();
			
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
			
			QApplication.ProcessEvents();
			
			// set visible tabContainer
			mainwindow_Ui.tabContainer.SetVisible(true);
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Generate report file
		/// </summary>
		private void SaveReport()
		{
			if (mainwindow_Ui.tabContainer.CurrentIndex < 0)
			{
				// If there isn't any selected tab, exit
				return;
			}
			
			string selectedFile = 
				QFileDialog.GetSaveFileName(this, 
			                                GlobalData.GetLanguageKeyValue("FILESAVE"),
			                                bf.DefaultPath,
			                                "Report file (*.report)");
				
			if (string.IsNullOrEmpty(selectedFile))
			{
				return;
			}
			
			// generate report
			bf.SaveReportFile(fiEntity[mainwindow_Ui.tabContainer.CurrentIndex].Video,
			                  fiEntity[mainwindow_Ui.tabContainer.CurrentIndex].Audio,
			                  fiEntity[mainwindow_Ui.tabContainer.CurrentIndex].FileName,
			                  selectedFile,
			                  fiEntity[mainwindow_Ui.tabContainer.CurrentIndex].PluginUsed);
			
			QMessageBox.Information(this, 
			                        GlobalData.GetLanguageKeyValue("SAVEREPORTWIN"), 
			                        GlobalData.GetLanguageKeyValue("SAVEREPORTMSG") + "\r\n" +
			                        fiEntity[mainwindow_Ui.tabContainer.CurrentIndex].FileName);
		}
		
		
		
		/// <summary>
		/// Information about
		/// </summary>
		private void OpenInfo()
		{
			AboutDialogClass awc = new AboutDialogClass();
			awc.Show();
			
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
		/// Add new tab to TabWidget object and fill it with 
		/// scan result of multimedia file
		/// </summary>
		private void FillListView(List<string[]> video, List<string[]> audio, string filePath)
		{
			
			// define new grid Layout
			QGridLayout glayout = new QGridLayout();
			
			// define new abstract QWidget
			QWidget fileTab = new QWidget();
			
			// define and add QTreeWidget to created tab
			QTreeWidget tw = new QTreeWidget();
			
			// set columns number and disable sort
			tw.SortingEnabled=false;
			tw.ColumnCount = 2;
			
			// set headers
			List<string> twHeader = new List<string>();
			twHeader.Add("Description");
			twHeader.Add("Value");			
			
			tw.SetHeaderLabels(twHeader);
			tw.SetColumnWidth(0, 200);
			
			// add video info to List
			for (int k=0; k<video.Count; k++)
			{
				// add new QTreeWidgetItem object to our QTreeWidget
				QTreeWidgetItem twi = new QTreeWidgetItem(tw);
				
				
				// add description and value of information
				twi.SetText(0, video[k][0]);
				twi.SetText(1, video[k][1]);
				
				// add video icon to first item of created QTreeWidgetItem
				twi.SetIcon(0,new QIcon(":/main/video_32.png"));
				
			}
			
			
			// add audio info to List
			for (int k=0; k<audio.Count; k++)
			{
				// add new QTreeWidgetItem object to our QTreeWidget
				QTreeWidgetItem twi = new QTreeWidgetItem(tw);
				
				// add description and value of information
				twi.SetText(0, audio[k][0]);
				twi.SetText(1, audio[k][1]);
				
				// add audio icon to first item of created QTreeWidgetItem
				twi.SetIcon(0,new QIcon(":/main/sound.png"));
				
			}
			

			// add QTreeView created to created Grid Layout
			glayout.AddWidget(tw);
			
			// Set Grid Layout created as layout of created tab
			fileTab.SetLayout(glayout);
			
			// add abstract QWidget as tab to tabContainer
			string tabName = Path.GetFileName(filePath).PadRight(25).Substring(0,25).Trim();
			int tabIndex = mainwindow_Ui.tabContainer.AddTab(fileTab, tabName);
			mainwindow_Ui.tabContainer.SetTabToolTip(tabIndex, Path.GetFileName(filePath));

			
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
					QApplication.ProcessEvents();
				}
				
				// wait some milliseconds
				DateTime waitEnd = DateTime.Now.AddMilliseconds(waitTimeMilliseconds);
				while(DateTime.Now <= waitEnd)
				{
					QApplication.ProcessEvents();
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
			QMessageBox.Warning(this, title, message);
		}
		
		
		
		

	}
}
