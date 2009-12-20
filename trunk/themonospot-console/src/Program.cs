using System;
using System.IO;
using System.Collections.Generic;
using ThemonospotBase;
using System.Reflection;

namespace ThemonospotConsole
{
	class Program
	{
		
		private static Arguments appArgs;
		private static BaseFactory bFactory;
		private static List<string> managedExt = new List<string>();
		private static string release;
		private static bool recursive;
		private static bool autoreport;
		private static bool isAutoreportPresent = false;
		
		/// <summary>
		/// Entry point of application
		/// </summary>
		/// <param name="args">
		/// A <see cref="System.String"/>
		/// </param>
		public static void Main(string[] args)
		{
			
			release = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
					Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
				   	Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			
			// Parse parameters
			appArgs = new Arguments(args);
			ParseParameters();
			
			// Manage help request
			if(!string.IsNullOrEmpty(appArgs["help"]) ||
			   !string.IsNullOrEmpty(appArgs["h"]) ||
			   !string.IsNullOrEmpty(appArgs["?"]) ||
			   args.Length == 0)
			{
				GetHelp();
				return;
			}
			
			// Base component instance
			if(!string.IsNullOrEmpty(appArgs["test"]))
			{
				if(Convert.ToString(appArgs["test"]) == "true")
				{
					bFactory = new BaseFactory(true);
				}
				else
				{
					bFactory = new BaseFactory();
				}
			}
			else
			{
				bFactory = new BaseFactory();
			}
			
			bFactory.Console = ThemonospotLogger.Console;
			bFactory.Listener = ThemonospotLogger.Listener;
			bFactory.TraceFile = ThemonospotLogger.TraceFile;
			
			if (ThemonospotLogger.TraceFilePath != "")
			{
				bFactory.TraceFilePath = ThemonospotLogger.TraceFilePath + ".base";
			}
			
			
			// Retrieve managed extensions from loaded plugins
			string[] managedExtArray = bFactory.GetManagedExtentions();
			
			foreach (string str in managedExtArray) 
			{
				managedExt.Add(str);
			}

			
			if(isAutoreportPresent)
			{
				bFactory.IsReportAuto = autoreport;
			}
			
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
				bFactory.GetFileInfo(pathFile,
				                 ref video,
				                 ref audio);
			}
			catch(ThemonospotBaseException tbexp)
			{
				ThemonospotLogger.Append("\r\n" + 
				                         tbexp.Method + "\r\n" +
				                         tbexp.Message);
			}
			catch(Exception ex)
			{
				ThemonospotLogger.Append("\r\n" + 
				                         "Program::ScanFile\r\n\r\n" +
				                         ex.Message + "\r\n\r\n" +
				                         ex.StackTrace);
			}
			
			
			Console.WriteLine("File path:".PadRight(25) + 
			                  pathFile);

			for(int j=0; j<video.Count; j++)
			{
				Console.WriteLine(video[j][0].PadRight(25) +
				                  video[j][1]);
			}
			

			for(int j=0; j<audio.Count; j++)
			{
				Console.WriteLine(audio[j][0].PadRight(25) +
				                  audio[j][1]);
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
			outHelp += "   --tracefile=<tracefile path>".PadRight(40) + "enable trace on specific file\r\n";			
			
			outHelp += "\r\n\r\n";
			outHelp += "Website: http://www.integrazioneweb.com/themonospot/\r\n";
			outHelp += "\r\n";
			
			Console.WriteLine(outHelp);
			
		}
		
		
	}
}