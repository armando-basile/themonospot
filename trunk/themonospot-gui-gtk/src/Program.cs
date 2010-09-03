
using System;
using System.IO;
using ThemonospotBase;
using ThemonospotComponents;
using System.Reflection;
using System.Collections.Generic;
using log4net;
using log4net.Config;

namespace ThemonospotGuiGtk
{
	
	
	public class Program
	{
		private static Arguments arguments;
		private static Utilities utils;
		private static string traceFolderPath;
		
		[STAThread]
		public static void Main(string[] args)
		{
			
			// Parse command arguments
			arguments = new Arguments(args);
			
			// Init log4net
			Log4NetConfig();
			
			InitBaseComponent();
			
			// Init Gtk Application
			Gtk.Application.Init();

			// Create a new MainWindowClass instance
			MainWindowClass mainWindow = new MainWindowClass(args);
			
			mainWindow.ParseArguments();
			
			
			// Run Gtk Application			
			Gtk.Application.Run();
			
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
			
			// Scan plugins
			GlobalData.bFactory.ScanPlugins();
			
		}
		
		
		
		
		
		
		
		
		
		
		
	}
}
