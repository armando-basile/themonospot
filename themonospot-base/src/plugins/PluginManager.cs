
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using ThemonospotPlugins;
using log4net;


namespace ThemonospotBase
{
	
	/// <summary>
	/// Manage load/unload and getinfo of plugin assembly
	/// </summary>
	public class PluginManager
	{
		

		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(PluginManager));

		
		public PluginManager()
		{
		}
		
		
		
		
		/// <summary>
		/// Create reference for all plugins
		/// </summary>
		/// <param name="pluginsPath">
		/// A <see cref="System.String"/> Plugins root folder
		/// </param>
		/// <returns>
		/// A <see cref="T:List{ThemonospotPluginEntity}"/> List of plugin entity
		/// </returns>
		public void LoadPlugins(string pluginsPath)
		{
			string assemblyName = "";
			
			GlobalData.BasePlugins = new List<ThemonospotPluginEntity>();
			DirectoryInfo pluginDirInfo = new DirectoryInfo(pluginsPath);
			
			if (!Directory.Exists(pluginsPath))
			{
				return;
			}
			
            log.Debug("LoadPlugins: " + pluginsPath);
			
			foreach (DirectoryInfo di in pluginDirInfo.GetDirectories("themonospot-plugin*"))
			{
				// Create new instance
				ThemonospotPluginEntity tpe = new ThemonospotPluginEntity();
				
				// set local variables
				assemblyName = di.Name;
				
				tpe.FolderPath = di.FullName;
				tpe.FileName = assemblyName;
				
				// create AppDomainSetup for this assembly
				AppDomainSetup ads = new AppDomainSetup();				
				ads.ApplicationName = assemblyName;
				ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
				ads.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
								
				// create AppDomain specific for this assembly and 
				// add this AppDomain to entity
				tpe.PluginAppDomain = AppDomain.CreateDomain(assemblyName, null, ads);
				

			
				// ceate instance of AssemblyLoader
				tpe.Loader = (AssemblyLoader)tpe.PluginAppDomain.
					CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, 
				    	                    typeof(AssemblyLoader).FullName);

				// Set debug informations
				tpe.Loader.SetDebug(GlobalData.bFactory.IsConsole, 
				                    GlobalData.bFactory.IsTraceFile,
				                    assemblyName);
				
				// Extract Plugin info from assembly
				// ThemonospotPluginEntity ntpe = GetPluginInfo(assemblyFullPath);
				FillPluginInfo(ref tpe);
				
				// Update Loaded plugins List
				GlobalData.BasePlugins.Add(tpe);
				

			}
			
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get info about specific plugin
		/// </summary>
		private void FillPluginInfo(ref ThemonospotPluginEntity pluginEntity)
		{
/*			
			// themonospot plugin entity
			ThemonospotPluginEntity tpe = new ThemonospotPluginEntity();
			
			string pluginFolderName = (new FileInfo(fullPath)).Directory.Name;
			
			// ceate instance of AssemblyLoader
			AssemblyLoader pluginLoader = (AssemblyLoader)appDomainsLoaded[pluginFolderName].
				CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, 
				                        typeof(AssemblyLoader).FullName);
*/
			
			// Create instance of plugin
			string fullPath = pluginEntity.FolderPath + Path.DirectorySeparatorChar + 
				pluginEntity.FileName + ".dll";
			
			IThemonospotPlugin plugIn = 
				(IThemonospotPlugin)pluginEntity.Loader.CreateInstance(fullPath, 
				                                                       "IThemonospotPlugin");
			
			// Get Plugin Info
			pluginEntity.Description = plugIn.Description;
			pluginEntity.Release = plugIn.Release;
			pluginEntity.ManagedExtensions = plugIn.ManagedExtensions;
			
			log.Info("\r\n" + 
			         "FileName: " + pluginEntity.FileName + "\r\n" +
					 "Description: " + pluginEntity.Description + "\r\n" +
					 "FolderPath: " + pluginEntity.FolderPath + "\r\n" +
					 "ManagedExtensions: " + pluginEntity.ManagedExtensions + "\r\n" +
					 "Release: " + pluginEntity.Release + "\r\n");
			
			
/*
			Dictionary<string, string> pInfo = pluginLoader.GetPluginInfo();
			
			tpe.FileName = pInfo["FileName"];
			tpe.ManagedExtensions = pInfo["ManagedExtensions"];
			tpe.Description = pInfo["Description"];
			tpe.FolderPath = pInfo["FolderPath"];
			tpe.Release = pInfo["Release"];
		
			
			// Debug info
			ThemonospotLogger.Append("FileName: " + tpe.FileName);
			ThemonospotLogger.Append("Description: " + tpe.Description);
			ThemonospotLogger.Append("FolderPath: " + tpe.FolderPath);
			ThemonospotLogger.Append("ManagedExtensions: " + tpe.ManagedExtensions);
			ThemonospotLogger.Append("Release: " + tpe.Release);
			ThemonospotLogger.Append("");
			
			return tpe;
*/
			
			// Delete plugin used object
			plugIn = null;
			
		}
		
		
		

		
		
		
		

		
		
		/// <summary>
		/// Create instance of plugin and scan file to report streams infos
		/// </summary>
		/// <param name="pluginPath">
		/// A <see cref="System.String"/> path of plugin assembly
		/// </param>
		/// <param name="filePathToScan">
		/// A <see cref="System.String"/> path of multimedia file to scan
		/// </param>
		/// <param name="videoInfoList">
		/// A <see cref="List"/> video infos container
		/// </param>
		/// <param name="audioInfoList">
		/// A <see cref="List"/> audio info container
		/// </param>
		public void GetFileInfo(ThemonospotPluginEntity pluginEntity, 
		                        string filePathToScan, 
		                    ref List<string[]> videoInfoList, 
		                    ref List<string[]> audioInfoList)
		{
			
			// Create instance of plugin
			string fullPath = pluginEntity.FolderPath + Path.DirectorySeparatorChar + 
				pluginEntity.FileName + ".dll";
			
			IThemonospotPlugin plugIn = (IThemonospotPlugin)pluginEntity.Loader
				.CreateInstance(fullPath, "IThemonospotPlugin");
			
			// Get informations from media file using plugin
			object vList = new List<string[]>();
			object aList = new List<string[]>();
			
			plugIn.GetFileInfo( filePathToScan, ref vList, ref aList);
			
			videoInfoList = (List<string[]>)vList;
			audioInfoList = (List<string[]>)aList;
		
			// Delete reference to used objects
			vList = null;
			aList = null;
			plugIn = null;
			
			
		}
		
		
		
		
		/// <summary>
		/// Unload plugins loaded
		/// </summary>
		public void UnloadPlugins()
		{
			foreach (ThemonospotPluginEntity tpe in GlobalData.BasePlugins) 
			{
				tpe.Loader = null;
				AppDomain.Unload(tpe.PluginAppDomain);
				tpe.PluginAppDomain = null;
				GlobalData.BasePlugins.Remove(tpe);
			}			
		}

		
		
		
		
		
	}
}
