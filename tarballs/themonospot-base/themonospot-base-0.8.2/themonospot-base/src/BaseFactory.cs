
using System;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using ThemonospotPlugins;


namespace ThemonospotBase
{
	
	/// <summary>
	/// Manage all plugin classes and serve gui objects
	/// </summary>
	public class BaseFactory: IDisposable
	{
		
		private List<ThemonospotPluginEntity> pluginsAvailable = 
			new List<ThemonospotPluginEntity>();
		
		private string UsedfileName = "";
		private ThemonospotPluginEntity UsedPlugin = null;
		
		private List<string[]> localVideoInfo;
		private List<string[]> localAudioInfo;
		
		private ThemonospotSettings ts = new ThemonospotSettings();
		private PluginManager pm = new PluginManager();
		
		private string baseAssemblyPath = "";
		private string pluginsAssemblyPath = "";
		private bool isTest = false;
		
		
		
		
		#region Properties
		
		
		/// <value>
		/// return version of this component
		/// </value>
		public string Release
		{
			get	
			{	
				return Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
					Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
				   	Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();				
			}
		}
		
		
		
		
		/// <value>
		/// Get/Set if scan report file is auto generated
		/// </value>
		public bool IsReportAuto
		{
			get 
			{	
				return ts.IsReportAuto;	
			}
			set 
			{	
				ts.IsReportAuto = value;
				ts.UpdateConfigFile();	
			}
		}
		
		

		
		
		/// <value>
		/// Get/Set default path to browse file
		/// </value>
		public string DefaultPath
		{
			get 
			{	
				return ts.DefaultPath;	
			}
			set 
			{	
				ts.DefaultPath = value;
				ts.UpdateConfigFile();	
			}
		}
		
		
		
		
		
		public bool Console
		{
			get {	return ThemonospotLogger.Console; }
			set {	ThemonospotLogger.Console = value; }
		}
		
		

		
		
		public bool Listener
		{
			get {	return ThemonospotLogger.Listener; }
			set {	ThemonospotLogger.Listener = value; }
		}

		
		
		
				
		public bool TraceFile
		{
			get {	return ThemonospotLogger.TraceFile; }
			set {	ThemonospotLogger.TraceFile = value; }
		}

		
		
		
		
		public string TraceFilePath
		{
			get {return  ThemonospotLogger.TraceFilePath; }
			set {ThemonospotLogger.TraceFilePath = value; 	}
		}

		
		
		
		
		
		public List<ThemonospotPluginEntity> PluginsAvailable
		{
			get { return pluginsAvailable;}
		}
		
		
		
		#endregion Properties
		
		
		
		
		/// <summary>
		/// Setup plugins and default values
		/// </summary>
		public BaseFactory()
		{
			baseFactory();
		}
		
		
		
		
		/// <summary>
		/// Constructor for test
		/// </summary>
		public BaseFactory(bool testMode)
		{
			isTest = testMode;
			baseFactory();
		}
		
		
		
		/// <summary>
		/// Init base component
		/// </summary>
		private void baseFactory()
		{
			baseAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());
			
			// Plugins stay into subfolder plugins
			pluginsAssemblyPath = baseAssemblyPath + Path.DirectorySeparatorChar + "plugins";
			
			ts.ReadConfigFile();
			
			pluginsAvailable = pm.LoadPlugins(pluginsAssemblyPath);
		}
		
		
		
		
		
		/// <summary>
		/// Get string array contains all managed file extensions 
		/// from available plugins
		/// </summary>
		/// <returns>
		/// A <see cref="T:string[]"/> Array of extensions
		/// </returns>
		public string[] GetManagedExtentions()
		{
			string[] extensions = new string[0];
			
			string totalExt = "";
			foreach (ThemonospotPluginEntity tp in pluginsAvailable) 
			{
				totalExt += tp.ManagedExtensions;
			}			
			totalExt = totalExt.Replace(" ","");
			totalExt = totalExt.Replace(".","");
			
			if (totalExt.Length > 0)
			{
				if (totalExt.Substring(totalExt.Length-1) == ";")
				{
					totalExt = totalExt.Remove(totalExt.Length-1);
				}
				
				extensions = totalExt.Split(Convert.ToChar(";"));
				
				// Remove duplicate
				ArrayList outExtensions = new ArrayList();
				
				foreach (string ext in extensions) 
				{
					if(!outExtensions.Contains(ext))
					{
						outExtensions.Add(ext);
					}
				}
				extensions = (string[])outExtensions.ToArray(typeof(String));
			}
			
			return extensions;
			
		}

		
		
		
		
		/// <summary>
		/// Scan file with specific plugin and return info
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/>File path to scan
		/// </param>
		/// <param name="videoInfo">Output video stream info
		/// A <see cref="T:List{String[]}"/>
		/// </param>
		/// <param name="audioInfo">
		/// A <see cref="T:List{String[]}"/>Output audio stream info
		/// </param>
		public void GetFileInfo(string filePath, 
		                    ref List<string[]> videoInfo,
		                    ref List<string[]> audioInfo)
		{
			
			ThemonospotPluginEntity tPluginEntity = new ThemonospotPluginEntity();
			
			// File Not Exist
			if (!File.Exists(filePath))
			{
				throw new ThemonospotBaseException("BaseFactory::GetFileInfo",
				                                   "File not exist.");
			}

			
			// There isn't any plugin to manage this file type
			if (!IsPluginPresent(filePath, ref tPluginEntity))
			{
				throw new ThemonospotBaseException("BaseFactory::GetFileInfo",
				                                   "No plugin available to manage this file type.");
			}
			

			// create plugin path string
			string pluginPath = tPluginEntity.FolderPath + 
				Path.DirectorySeparatorChar + tPluginEntity.FileName + ".dll";

			// Create instance of plugin and recall scan method			
			pm.GetFileInfo(pluginPath, filePath, ref videoInfo, ref audioInfo);
			
			// Update current used objects
			UsedPlugin = tPluginEntity;
			UsedfileName = Path.GetFileName(filePath);
			localVideoInfo = videoInfo;
			localAudioInfo = audioInfo;
			
			
			// If IsReportAuto generate report
			if ((IsReportAuto) &&
			    (localVideoInfo.Count > 0))
			{
				SaveReportFile(filePath + ".report");
			}
			
			// Update Default path
			DefaultPath = Path.GetDirectoryName(filePath);
		}
		
		
		

		/// <summary>
		/// Scan file with specific plugin and return info
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/>File path to scan
		/// </param>
		/// <param name="videoInfo">Output video stream info
		/// A <see cref="T:List{String[]}"/>
		/// </param>
		/// <param name="audioInfo">
		/// A <see cref="T:List{String[]}"/>Output audio stream info
		/// </param>
		/// <param name="pluginUsed">
		/// A <see cref="System.String"/>Output plugin used to scan
		/// </param>
		public void GetFileInfo(string filePath, 
		                    ref List<string[]> videoInfo,
		                    ref List<string[]> audioInfo,
		                    ref string pluginUsed)
		{
			// recall other constructor of this method
			GetFileInfo(filePath, ref videoInfo, ref audioInfo);
			
			pluginUsed = UsedPlugin.FileName + " v" +UsedPlugin.Release;
		}
		
		
		
		/// <summary>
		/// Export multimedia file info on text report file
		/// </summary>
		/// <param name="outputFilePath">
		/// Report file path
		/// </param>
		public void SaveReportFile(string outputFilePath)
		{
			
			// recall other construct of this method
			SaveReportFile(localVideoInfo, 
			               localAudioInfo,
			               UsedfileName,
			               outputFilePath,
			               UsedPlugin.FileName + " v" +UsedPlugin.Release);
			
			/*
			StreamWriter sw = new StreamWriter(outputFilePath, false);
			string infoName = "";
			string infoValue = "";
			
			
			DateTime toDay = DateTime.Now; 
			sw.WriteLine(("File:").PadRight(25)+ UsedfileName);
			sw.WriteLine(("Analisys Date:").PadRight(25)+ toDay.ToString("yyyy-MM-dd HH:mm:ss") );
			sw.WriteLine("-----------------------------------------------------------------------------");
	    	
			for(int i=0; i<localVideoInfo.Count; i++)
			{
				infoName = localVideoInfo[i][0];
				infoValue = localVideoInfo[i][1];
				
				sw.WriteLine((infoName + ":").PadRight(25) + infoValue);
			}
			
			sw.WriteLine("");
			
			for(int i=0; i<localAudioInfo.Count; i++)
			{
				infoName = localAudioInfo[i][0];
				infoValue = localAudioInfo[i][1];
				
				sw.WriteLine((infoName + ":").PadRight(25) + infoValue);
			}
			
			sw.WriteLine("");
			
			sw.WriteLine("report generated by:\r\n" + 
			             "  themonospot base component v" + Release + "\r\n" +
			             "  plugin " + UsedPlugin.FileName + " v" +UsedPlugin.Release + "\r\n\r\n" + 
			             "project web site: http://www.integrazioneweb.com/themonospot/");
			
			sw.Close();
			sw.Dispose();
			sw = null;
			
			*/
			
			
		}
		
		
		
		/// <summary>
		/// export multimedia file info on text report file
		/// </summary>
		public void SaveReportFile(List<string[]> video,
		                           List<string[]> audio,
		                           string fileScanned,
		                           string outputFilePath,
		                           string pluginUsed)
		{
			if (outputFilePath.IndexOf(".report") < 0)
			{
				outputFilePath += ".report";
			}
			
			StreamWriter sw = new StreamWriter(outputFilePath, false);
			string infoName = "";
			string infoValue = "";
			
			
			DateTime toDay = DateTime.Now; 
			sw.WriteLine(("File:").PadRight(25) + fileScanned);
			sw.WriteLine(("Analisys Date:").PadRight(25) + toDay.ToString("yyyy-MM-dd HH:mm:ss") );
			sw.WriteLine("-----------------------------------------------------------------------------");
	    	
			for(int i=0; i<video.Count; i++)
			{
				infoName = video[i][0];
				infoValue = video[i][1];
				
				sw.WriteLine((infoName + ":").PadRight(25) + infoValue);
			}
			
			sw.WriteLine("");
			
			for(int i=0; i<audio.Count; i++)
			{
				infoName = audio[i][0];
				infoValue = audio[i][1];
				
				sw.WriteLine((infoName + ":").PadRight(25) + infoValue);
			}
			
			sw.WriteLine("");
			
			sw.WriteLine("report generated by:\r\n" + 
			             "  themonospot base component v" + Release + "\r\n" +
			             "  plugin " + pluginUsed + "\r\n\r\n" + 
			             "project web site: http://www.integrazioneweb.com/themonospot/");
			
			sw.Close();
			sw.Dispose();
			sw = null;
		}
		
		

		
		
		
		/// <summary>
		/// Detect plugin to manage passed file
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/> File path 
		/// </param>
		/// <param name="tpe">
		/// A <see cref="ThemonospotPluginEntity"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/>
		/// </returns>
		private bool IsPluginPresent(string filePath, 
		                         ref ThemonospotPluginEntity tpe)
		{
			tpe = new ThemonospotPluginEntity();
			FileInfo fi = new FileInfo(filePath);
			string fileExt = fi.Extension.ToLower() + ";";
			
			// parse all available plugins
			foreach (ThemonospotPluginEntity pe in pluginsAvailable) 
			{
				// If plugin manage this extension
				if (pe.ManagedExtensions.IndexOf(fileExt) >= 0)
				{
					ThemonospotLogger.Append("founded plugin to manage file: " + pe.FileName);
					tpe = pe;
					return true;
				}
			}
			
			return false;			
		}


		
		
		/// <summary>
		/// Unload all AppDomains
		/// </summary>
		public void Dispose ()
		{
			pm.UnloadPlugins();
		}
		

		
		
		/// <summary>
		/// Return language files path for themonospot gui's
		/// </summary>
		public string GetGuiLanguagesPath()
		{	
			if (IsWindows())
				// On Windows OS
				return baseAssemblyPath + Path.DirectorySeparatorChar + "languages";
			else
			{
				// On Others OS
				if (isTest)
				{
					// Test mode
					pluginsAssemblyPath = baseAssemblyPath + Path.DirectorySeparatorChar + "languages";
				}
				else
				{
					// Not test mode
					pluginsAssemblyPath = 
						Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) +
							Path.DirectorySeparatorChar + "themonospot" +
							Path.DirectorySeparatorChar + "languages";
				}
				
				return pluginsAssemblyPath;
			}
		}
		

		
        private bool IsWindows()
        {
            PlatformID platform = Environment.OSVersion.Platform;           
            return (platform == PlatformID.Win32NT | platform == PlatformID.Win32Windows |
                    platform == PlatformID.Win32S | platform == PlatformID.WinCE);    
        }

		
		
		
	}
}
