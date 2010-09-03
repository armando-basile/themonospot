
using System;
using log4net;
using log4net.Config;

namespace ThemonospotComponents
{


	public class Utilities
	{

		public Utilities ()
		{
		}
		
		
		
		
		
		/// <summary>
		/// Configure Log4Net
		/// </summary>
		public void Log4NetConfig(bool isConsoleEnabled, string traceFilePath)
		{
			if (isConsoleEnabled)
			{
				// Add console output
				log4net.Appender.ConsoleAppender ca = new log4net.Appender.ConsoleAppender();
				ca.Layout = new log4net.Layout.PatternLayout("%-5level %message%newline");
				ca.Threshold = log4net.Core.Level.All;
				log4net.Config.BasicConfigurator.Configure(ca);
				ca.ActivateOptions();
			}
			
			if (traceFilePath != "")
			{
				// Add file output
				log4net.Appender.RollingFileAppender rfa = new log4net.Appender.RollingFileAppender();
				rfa.AppendToFile = true;
				rfa.File = traceFilePath;
				rfa.MaxFileSize = 4096000;
				rfa.MaxSizeRollBackups = 2;
				rfa.Threshold = log4net.Core.Level.All;
				rfa.Layout = new log4net.Layout.PatternLayout("%-5level %date{HH:mm:ss,fff} " + 
				                                              "[%thread] %logger (%file:%line) " + 
				                                              "%newline%message%newline%newline");
				log4net.Config.BasicConfigurator.Configure(rfa);
				rfa.ActivateOptions();
			}
			
			// Set Root level
			log4net.Repository.Hierarchy.Hierarchy h = 
				(log4net.Repository.Hierarchy.Hierarchy)log4net.LogManager.GetRepository ();
      		log4net.Repository.Hierarchy.Logger rootLogger = h.Root;
			rootLogger.Level = log4net.Core.Level.All;
			
		}
		
		
		
		
		
		/// <summary>
		/// Return TRUE if operating system is Windows based
		/// </summary>
		public bool IsWindows()
		{
		    PlatformID platform = Environment.OSVersion.Platform;	    
		    return (platform == PlatformID.Win32NT | platform == PlatformID.Win32Windows |
		            platform == PlatformID.Win32S | platform == PlatformID.WinCE); 
		}
		
		
		
		/// <summary>
		/// Formatting string for avi text table
		/// </summary>
		public string GetRow(string id, string id2, long position, uint length)
		{
			return id.PadRight(20, Convert.ToChar(".")) + " " +
				   position.ToString("#,###").PadLeft(13) + " " +
				   length.ToString("#,###").PadLeft(13) + "  " +
				   "0x" + id2;
		}
		
		
		/// <summary>
		/// Formatting string for text table
		/// </summary>
		public string GetRow(string id, long position, long length)
		{
			return id.PadRight(10) + " " +
				   position.ToString("#,##0").PadLeft(16) + " " +
				   length.ToString("#,##0").PadLeft(16);
		}
		
		
		/// <summary>
		/// Formatting string for text table
		/// </summary>
		public string GetRow(string id, long position, uint length)
		{
			return GetRow(id, position, length);
		}
		
		
	}
}
