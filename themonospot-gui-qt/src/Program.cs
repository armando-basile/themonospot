
using System;
using System.IO;
using Qyoto;
using ThemonospotComponents;
using ThemonospotBase;
using log4net;
using log4net.Config;

namespace ThemonospotGuiQt
{
	
	
	public class Program: Qt
	{
		private static Arguments arguments;
		private static Utilities utils;
		private static string traceFolderPath;
		
		[STAThread]
		public static void Main(string[] args)
		{
			// Init resource class manager						
			Q_INIT_RESOURCE("ResManager");
			
			// Parse command arguments
			arguments = new Arguments(args);
			
			// Init log4net
			Log4NetConfig();
			
			InitBaseComponent();
			
			
			// Create new Qyoto Application
			new QApplication(args);
			
			// Create new Qyoto Desktop Object
			QDesktopWidget qdw = new QDesktopWidget();
			
			// Create MainWindow class manager
			MainWindowClass mwc = new MainWindowClass(args);
			
			int wWidth = Convert.ToInt32(mwc.Width() / 2);
			int wHeight = Convert.ToInt32(mwc.Height() / 2);
			int dWidth = Convert.ToInt32(qdw.Width() / 2);
			int dHeight = Convert.ToInt32(qdw.Height() / 2);
			
			mwc.Move(dWidth - wWidth, dHeight - wHeight - 20);
			
			mwc.ParseAguments();
			
			mwc.Show();
			
			// Run Qyoto Application
			QApplication.Exec();			
			
		}
		
		
		
		/// <summary>
		/// Configure Log4Net
		/// </summary>
		private static void Log4NetConfig()
		{
			bool l4nConsole = false;
			string l4nFilePath = "";
			
			utils = new Utilities();
			
			if (arguments["debug"] != null)
			{
				l4nConsole = true;
			}
			
			if (arguments["trace"] != null)
			{
				if (utils.IsWindows())
				{
					// Windows
					traceFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
					l4nFilePath = traceFolderPath + Path.DirectorySeparatorChar	+ ".themonospot.log";
				}
				else
				{
					// Linux, others
					traceFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					l4nFilePath = traceFolderPath + Path.DirectorySeparatorChar	+ ".themonospot.log";
				}
				
			}
			
			// configure Log4Net
			utils.Log4NetConfig(l4nConsole, l4nFilePath);
			
		}
		
		
		
		/// <summary>
		/// Create new instance of base factory and set up it
		/// </summary>
		private static void InitBaseComponent()
		{
			GlobalData.bFactory = new BaseFactory();
			
			if (arguments["debug"] != null)
			{
				GlobalData.bFactory.IsConsole = true;
			}

			if (arguments["listen"] != null)
			{
				GlobalData.bFactory.IsListener = true;
			}
			
			if (arguments["test"] != null)
			{
				GlobalData.bFactory.IsTest = true;
			}
			
			if (arguments["trace"] != null)
			{
				GlobalData.bFactory.IsTraceFile = true;
				GlobalData.bFactory.TraceFolderPath = traceFolderPath;
			}
			
            // Init language file
            GlobalData.InitLanguage(GlobalData.bFactory.GetGuiLanguagesPath());

			// Scan plugins
			GlobalData.bFactory.ScanPlugins();
			
		}
		
		
		
		
		
		
	}
}
