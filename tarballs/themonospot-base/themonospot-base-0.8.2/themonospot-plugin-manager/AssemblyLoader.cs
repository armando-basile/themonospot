
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using ThemonospotPlugins;



namespace System.Reflection
{
	
	
	public class AssemblyLoader: MarshalByRefObject
	{
		
		
		private IThemonospotPlugin pluginInstance = null;
		
		
		#region Properties
		
		private bool _debugMode = false;
		
		public bool DebugMode
		{
			get{return _debugMode;}
			set{_debugMode = value;}
		}
		
		#endregion Properties
		
		
		
		
		
		public AssemblyLoader()
		{
		}
		
		
		
		/// <summary>
		/// Create instance of plugin class and return a reference
		/// </summary>
		/// <param name="assemblyFullPath">
		/// A <see cref="System.String"/> Assembly file path
		/// </param>
		/// <param name="interfaceName"> Interface name implemented
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="args"> Arguments passed to plugin instance constructor
		/// A <see cref="System.Object"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Object"/> Reference to plugin instance
		/// </returns>
		public void CreateInstance(string assemblyFullPath, 
		                           string interfaceName, 
		                    params object[] args)
		{
			try
			{
				Assembly assembly = Assembly.LoadFrom(assemblyFullPath);
				
				foreach (Type type in assembly.GetExportedTypes())
				{
					Type interfaceType = type.GetInterface(interfaceName);					
					
					if (interfaceType != null)
					{
						pluginInstance = (IThemonospotPlugin)Activator.CreateInstance(type, args);
						pluginInstance.DebugMode = _debugMode;
						
					}
				}
				
				return;				
			}
			catch 
			{
				/*
				Console.WriteLine("\r\nPluginManager::CreateInstance\r\n" + 
				                  Ex.Message + "\r\n" + Ex.StackTrace);
				*/
				return;
			}
			
		}

		
		
		/// <summary>
		/// Get Plugin Informations
		/// </summary>
		/// <returns>
		/// A <see cref="T:Dictionary{string, string}"/> Return object
		/// </returns>
		public Dictionary<string, string> GetPluginInfo()
		{
			Dictionary<string, string> pluginInfo = new Dictionary<string, string>();
			
			object[] assemblyAttributes = pluginInstance.GetType().Assembly.GetCustomAttributes(true);
			foreach (Attribute attr in assemblyAttributes) 
			{
				if (attr is AssemblyDescriptionAttribute)
				{
					pluginInfo.Add("Description", ((AssemblyDescriptionAttribute)attr).Description);
				}
			}

			pluginInfo.Add("FileName",  pluginInstance.GetType().Assembly.GetName().Name);
			pluginInfo.Add("FolderPath" , Path.GetDirectoryName(pluginInstance.GetType().Assembly.Location));
			pluginInfo.Add("Release", pluginInstance.GetType().Assembly.GetName().Version.Major.ToString() + "." +
			               pluginInstance.GetType().Assembly.GetName().Version.Minor.ToString() + "." +
			               pluginInstance.GetType().Assembly.GetName().Version.Build.ToString());
			pluginInfo.Add("ManagedExtensions", pluginInstance.ManagedExtensions);
			
			return pluginInfo;
		}
		
		
		
		
		/// <summary>
		/// Get audio/video informations of file to scan
		/// </summary>
		/// <param name="filePathToScan">
		/// A <see cref="System.String"/>
		/// </param>
		/// <param name="videoInformations">
		/// A <see cref="List"/>
		/// </param>
		/// <param name="audioInformations">
		/// A <see cref="List"/>
		/// </param>
		public void GetFileInfo(string filePathToScan, 
		                        out List<string[]> videoInformations, 
		                        out List<string[]> audioInformations)
		{
			// use plugin method
			
			object vList = new List<string[]>();
			object aList = new List<string[]>();
			
			pluginInstance.GetFileInfo(filePathToScan, 
			                       ref vList,
			                       ref aList);
			
			videoInformations = (List<string[]>)vList;
			audioInformations = (List<string[]>)aList;
			
			return;
		}
		
		
		
		
	}
}
