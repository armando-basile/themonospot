
using Gtk;
using Glade;
using System;
using System.IO;
using System.Threading;
using ThemonospotBase;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace ThemonospotGuiGtk
{
	
	
	public class GtkWinMain
	{
		
		private BaseFactory bFactory;
		private string[] availableExtensions;
		private Arguments startArgs;
		
		
		private List<string[]> videoInfo;
		private List<string[]> audioInfo;
		
		
		#region Gtk Objects
		
		[Glade.Widget]	Gtk.Window 				WinMain;
		
		
		#endregion Gtk Objects
		
		
		
		public GtkWinMain(string[] args, ref bool retStart)
		{
			// Flag to check GtkWindow create instance 
			// from static Main
			retStart = true;

			// Adjust args in Arguments object
			startArgs = new Arguments(args);

			// Parse args Arguments
			ParsePassedArgs();
			
			
			// Debug
			ThemonospotLogger.Append("themonospot-gui-gtk release: " + 
			                         Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
									 Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
				   					 Assembly.GetExecutingAssembly().GetName().Version.Build.ToString());
			
			// Base component front end class
			bFactory = new BaseFactory();
			
			// Debug
			ThemonospotLogger.Append("themonospot-base release: " + bFactory.Release);
			
			
			// Get available managed extensions			
			availableExtensions = bFactory.GetManagedExtentions();
			
			// Init graphic gtk objects
			initWindow();
			
			// Wait for User Interface
			while (Gtk.Application.EventsPending ())
        		Gtk.Application.RunIteration ();
			
			
			
			// Test avi file scan
			//string fileToScan = "/windows/J/XviDivX/I mostri oggi/I mostri oggi - CD1.avi";
			//string fileToScan = "/home/armando/Video/cantante_xvid.avi";
			string fileToScan = "/mnt/c_drive/Share/New Folder/cantante_xvid.avi";
			
			ExtractInfoFromFile(fileToScan);
			
		}
		
		
		/// <summary>
		/// Initialize graphic gtk objects
		/// </summary>
		private void initWindow()
		{
			Gtk.Window.DefaultIcon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
			Glade.XML gxml =  new Glade.XML("GtkWinMain.glade", "WinMain");
			
			gxml.Autoconnect(this);
			
			
			
			WinMain.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString();
			WinMain.ShowAll();
			
		}
		
		
		#region Window Objects Methods
		
		
		private void on_WinMain_delete_event(object sender, DeleteEventArgs Args)
		{
			// Debug
			ThemonospotLogger.Append("Exit request\r\n");			
			ThemonospotLogger.Close();
			
			// Dispose base component
			bFactory.Dispose();
			
			
			// Close Application
			Gtk.Application.Quit();
		}
		
		
		
		
		
		#endregion Window Objects Methods
		
		
		
		#region User Methods
		
		/// <summary>
		/// Extract info from file and fill audio/video lists
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/> File path to scan
		/// </param>
		private void ExtractInfoFromFile(string filePath)
		{

			videoInfo = new List<string[]>();
			audioInfo = new List<string[]>();
			
			try
			{
				bFactory.GetFileInfo(filePath,
				                 ref videoInfo,
				                 ref audioInfo);
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
				                         "GtkWinMain::ExtractInfoFromFile\r\n\r\n" +
				                         ex.Message + "\r\n\r\n" +
				                         ex.StackTrace);
			}
			
			

			for(int j=0; j<videoInfo.Count; j++)
			{
				Console.WriteLine(videoInfo[j][0].PadRight(25) +
				                  videoInfo[j][1]);
			}
			

			for(int j=0; j<audioInfo.Count; j++)
			{
				Console.WriteLine(audioInfo[j][0].PadRight(25) +
				                  audioInfo[j][1]);
			}
			
		}
			
			
			
			
			
		/// <summary>
		/// Scan start arguments
		/// </summary>
		private void ParsePassedArgs()
		{
			// Tracefile argument
			if (startArgs["tracefile"] == "true")
			{
				string logFilePath;
				
				if (IsWindows())
				{
					logFilePath =
						Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString()) +
						Path.DirectorySeparatorChar + "themonospot.trace";					
				}
				else
				{
					logFilePath = 
						Environment.GetFolderPath(Environment.SpecialFolder.Personal) + 
						Path.DirectorySeparatorChar + ".themonospot.trace";
				}
				
				
				ThemonospotLogger.TraceFilePath = logFilePath;
				ThemonospotLogger.TraceFile = true;
			}
			
			
			// Add Debug mode
			if (startArgs["debug"] == "true")
			{
				ThemonospotLogger.Console = true;
			}
			else
			{
				ThemonospotLogger.Console = false;
			}
			
		
		}
		
		
		
		
		
		/// <summary>
		/// Return TRUE if operating system is Windows based
		/// </summary>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool IsWindows()
		{
		    PlatformID platform = Environment.OSVersion.Platform;	    
		    return (platform == PlatformID.Win32NT | platform == PlatformID.Win32Windows |
		            platform == PlatformID.Win32S | platform == PlatformID.WinCE); 
		}
			
		
		
		
		#endregion User Methods
		
		
		
	}
}
