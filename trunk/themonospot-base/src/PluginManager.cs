
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using ThemonospotPlugins;


namespace ThemonospotBase
{
	
	/// <summary>
	/// Manage load/unload and getinfo of plugin assembly
	/// </summary>
	public class PluginManager
	{
		
		private Dictionary<string, AppDomain> appDomainsLoaded = 
			new Dictionary<string, AppDomain>();

		
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
		public List<ThemonospotPluginEntity> LoadPlugins(string pluginsPath)
		{
			string assemblyFullPath = "";
			string assemblyName = "";
			
			List<ThemonospotPluginEntity> pe = new List<ThemonospotPluginEntity>();
			DirectoryInfo pluginDirInfo = new DirectoryInfo(pluginsPath);
			
			if (!Directory.Exists(pluginsPath))
			{
				return pe;
			}
			
			
			
			foreach (DirectoryInfo di in pluginDirInfo.GetDirectories("themonospot-plugin*"))
			{
				// set local variables
				assemblyFullPath = di.FullName + Path.DirectorySeparatorChar + di.Name +  ".dll";
				assemblyName = di.Name;
				
				// create AppDomainSetup for this assembly
				AppDomainSetup ads = new AppDomainSetup();				
				ads.ApplicationName = assemblyName;
				ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
				ads.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;

				// create AppDomain specific for this assembly and 
				// add this AppDomain to AppDomains List
				appDomainsLoaded.Add(assemblyName, 
					                 AppDomain.CreateDomain(assemblyName, null, ads));
				
				// Extract Plugin info from assembly
				ThemonospotPluginEntity ntpe = GetPluginInfo(assemblyFullPath);
				
				// Update Loaded plugins List
				pe.Add(ntpe);

			}
			
			return pe;
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get info about specific assembly
		/// </summary>
		/// <param name="fullPath">
		/// A <see cref="System.String"/> assembly file path
		/// </param>
		/// <returns>
		/// A <see cref="ThemonospotPluginEntity"/> class contains assembly info
		/// </returns>
		public ThemonospotPluginEntity GetPluginInfo(string fullPath)
		{
			
			// themonospot plugin entity
			ThemonospotPluginEntity tpe = new ThemonospotPluginEntity();
			
			string pluginFolderName = (new FileInfo(fullPath)).Directory.Name;
			
			// ceate instance of AssemblyLoader
			AssemblyLoader pluginLoader = (AssemblyLoader)appDomainsLoaded[pluginFolderName].
				CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, 
				                        typeof(AssemblyLoader).FullName);

			
			// Create instance and Get Plugin Info
			pluginLoader.DebugMode = ThemonospotLogger.Console;
			pluginLoader.CreateInstance(fullPath, "IThemonospotPlugin");
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
		public void GetFileInfo(string pluginPath, 
		                        string filePathToScan, 
		                    ref List<string[]> videoInfoList, 
		                    ref List<string[]> audioInfoList)
		{
			
			string pluginFolderName = new FileInfo(pluginPath).Directory.Name;
			

			// ceate instance of AssemblyLoader
			AssemblyLoader pluginLoader = (AssemblyLoader)appDomainsLoaded[pluginFolderName].
				CreateInstanceAndUnwrap(typeof(AssemblyLoader).Assembly.FullName, 
				                        typeof(AssemblyLoader).FullName);


			// Create instance and Get Info of file scanned
			pluginLoader.DebugMode = ThemonospotLogger.Console;
			pluginLoader.CreateInstance(pluginPath, "IThemonospotPlugin");			
			pluginLoader.GetFileInfo(filePathToScan, out videoInfoList, out audioInfoList);
			
		}
		
		
		
		
		/// <summary>
		/// Unload plugins loaded
		/// </summary>
		public void UnloadPlugins()
		{
			foreach (AppDomain adl in appDomainsLoaded.Values) 
			{
				appDomainsLoaded.Remove(adl.FriendlyName);
				AppDomain.Unload(adl);				
			}			
		}

		
		
		
		
		
	}
}
