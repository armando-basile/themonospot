using System;
using System.IO;
using System.Collections.Generic;
using ThemonospotBase;
using ThemonospotComponents;
using System.Reflection;
using log4net;
using log4net.Config;

namespace ThemonospotConsole
{
	class Program
	{
		
		private static Arguments appArgs;
		private static Utilities utils = new Utilities();
		private static string traceFolderPath = "";
		//private static BaseFactory bFactory;
		private static List<string> managedExt = new List<string>();
		private static string release;
		private static bool recursive;
		//private static bool autoreport;
		//private static bool isAutoreportPresent = false;
		
		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(Program));
		
		/// <summary>
		/// Entry point of application
		/// </summary>
		/// <param name="args">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Main(string[] args)
		{
			
			release = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			
			
			// Parse parameters
			appArgs = new Arguments(args);
			
			
			// Manage help request
			if(!string.IsNullOrEmpty(appArgs["help"]) ||
			   !string.IsNullOrEmpty(appArgs["h"]) ||
			   !string.IsNullOrEmpty(appArgs["?"]) ||
			   args.Length == 0)
			{
				GetHelp();
				return;
			}
			
			// Setup Log4Net
			Log4NetConfig();
			
			// Init Base Component
			InitBaseComponent();
			
			
			// Retrieve managed extensions from loaded plugins
			string[] managedExtArray = GlobalData.bFactory.GetManagedExtentions();
			
			foreach (string str in managedExtArray) 
			{
				managedExt.Add(str);
			}

			/*
			if(isAutoreportPresent)
			{
				GlobalData.bFactory.IsReportAuto = autoreport;
			}
			*/
			
			string fileToScan = args[args.Length-1].ToString();
			
			if (File.Exists(fileToScan))
			{
				// Scan file
				ScanFile(fileToScan);
				
			}
			else if (Directory.Exists(fileToScan))
			{
				// Scan folder
				ScanFolder(fileToScan);
				
			}

			
		}
		
		
		
		
		
		private static void ScanFile(string pathFile)
		{
			List<string[]> audio = new List<string[]>();
			List<string[]> video = new List<string[]>();
			
			try
			{
				GlobalData.bFactory.GetFileInfo(pathFile,
				                 ref video,
				                 ref audio);
			}
			catch(ThemonospotBaseException tbexp)
			{
				log.Error("\r\n" + tbexp.Method + "\r\n" + tbexp.Message);
			}
			catch(Exception ex)
			{
				log.Error("\r\n" + "Program::ScanFile\r\n\r\n" +
				          ex.Message + "\r\n\r\n" +
				          ex.StackTrace);
			}
			
			
			Console.WriteLine("File path:".PadRight(25) + 
			                  pathFile);

			for(int j=0; j<video.Count; j++)
			{
				Console.WriteLine(video[j][0].PadRight(25) + video[j][1]);
			}
			

			for(int j=0; j<audio.Count; j++)
			{
				Console.WriteLine(audio[j][0].PadRight(25) + audio[j][1]);
			}
			
			// Empty row
			Console.WriteLine("");
			
		}
		
		

		
		
		private static void ScanFolder(string pathFolder)
		{
			DirectoryInfo di = new DirectoryInfo(pathFolder);
			
			// Detect if recursive mode is enabled
			if(recursive)
			{
				foreach (DirectoryInfo sdi in di.GetDirectories()) 
				{
					// Recall CallBack Function ScanFolder
					ScanFolder(sdi.FullName);
				}
			}
			
			
			foreach (FileInfo fi in di.GetFiles()) 
			{
				string extension = fi.Extension;
				if (extension.IndexOf(".") == 0)
				{
					extension = extension.Substring(1);
				}
				
				if(managedExt.Contains(extension.ToLower()))
				{
					ScanFile(fi.FullName);
				}
				
			}
			
			
		}

		
		
		
		
/*
		/// <summary>
		/// Parse application parameters
		/// </summary>
		private static void ParseParameters()
		{
			ThemonospotLogger.TraceFile = false;
			ThemonospotLogger.Console = false;
			ThemonospotLogger.Listener = false;
			
			
			if ((!string.IsNullOrEmpty(appArgs["debug"])) && 
			    (appArgs["debug"] == "true"))
			{
				ThemonospotLogger.Console =true;
			}
			
			
			if (!string.IsNullOrEmpty(appArgs["tracefile"]))
			{
				ThemonospotLogger.TraceFile = true;
				ThemonospotLogger.TraceFilePath = appArgs["tracefile"];
			}
			
			
			if ((!string.IsNullOrEmpty(appArgs["listen"])) && 
			    (appArgs["listen"] == "true"))
			{
				ThemonospotLogger.Listener = true;
			}
			
			
			if ((!string.IsNullOrEmpty(appArgs["recursive"])) && 
			    (appArgs["recursive"] == "true"))
			{
				recursive = true;
			}
			
			
			if ((!string.IsNullOrEmpty(appArgs["autoreport"])) && 
			    (appArgs["autoreport"] == "true"))
			{
				isAutoreportPresent = true;
				autoreport = true;
			}
			else if ((!string.IsNullOrEmpty(appArgs["autoreport"])) && 
			    (appArgs["autoreport"] == "false"))
			{
				isAutoreportPresent = true;
				autoreport = false;
			}
			
		}
*/
		
		/// <summary>
		/// output help message
		/// </summary>
		private static void GetHelp()
		{
			string outHelp = "themonospot-console release: " + release + "\r\n\r\n";
			outHelp += "usage: mono themonospot-console.exe [arguments] <file or folder path>\r\n\r\n";
			outHelp += "accepted arguments: \r\n";
			outHelp += "   --help".PadRight(40) + "obtain this help message\r\n";
			outHelp += "   --autoreport=true".PadRight(40) + "enable automatic report generation after scan\r\n";
			outHelp += "   --debug=true".PadRight(40) + "enable debug mode\r\n";
			outHelp += "   --listen=true".PadRight(40) + "enable listen on default listener\r\n";
			outHelp += "   --recursive=true".PadRight(40) + "enable recursive folders scan\r\n";
            outHelp += "   --test=true".PadRight(40) + "enable use of app folder files\r\n";
			outHelp += "   --trace=true".PadRight(40) + "enable trace on specific file\r\n";			
			
			outHelp += "\r\n\r\n";
			outHelp += "Website: http://www.integrazioneweb.com/themonospot/\r\n";
			outHelp += "\r\n";
			
			Console.WriteLine(outHelp);
			
		}
		
		
		
		
		

		/// <summary>
		/// Configure Log4Net
		/// </summary>
		private static void Log4NetConfig()
		{
			bool l4nConsole = false;
			string l4nFilePath = "";
			
			utils = new Utilities();
			
			if (appArgs["debug"] != null)
			{
				l4nConsole = true;
			}
			
			if (appArgs["trace"] != null)
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
			
			if (appArgs["debug"] != null)
			{
				GlobalData.bFactory.IsConsole = true;
			}

			if (appArgs["recursive"] == "true")
			{
				recursive = true;
			}
			
			if (appArgs["autoreport"] == "true")
			{
				GlobalData.bFactory.IsReportAuto = true;
			}
			else
			{
				GlobalData.bFactory.IsReportAuto = false;
			}
			
			
			if (appArgs["listen"] != null)
			{
				GlobalData.bFactory.IsListener = true;
			}
			
			if (appArgs["test"] != null)
			{
				GlobalData.bFactory.IsTest = true;
			}
			
			if (appArgs["trace"] != null)
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