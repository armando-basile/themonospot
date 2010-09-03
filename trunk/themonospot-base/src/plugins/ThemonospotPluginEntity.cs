
using System;
using System.Reflection;

namespace ThemonospotBase
{
	
	/// <summary>
	/// Describe Plugin info
	/// </summary>
	public class ThemonospotPluginEntity
	{

		#region Properties
		
		
		/// <value>
		/// Assembly description
		/// </value>
		public string Description {get; set;}

		
		
		/// <value>
		/// Assembly filename without .dll
		/// </value>
		public string FileName {get; set;}
		

		
		
		/// <value>
		/// Assembly folder path
		/// </value>
		public string FolderPath {get; set;}		
		
		
		

		/// <value>
		/// Managed multimedia file extentions
		/// </value>
		public string ManagedExtensions {get; set;}

		
/*
		/// <value>
		/// Get NameSpace name
		/// </value>
		public string NameSpace {get; set;}		
*/
		

		/// <value>
		/// Assembly release
		/// </value>
		public string Release {get; set;}
		

		
		/// <summary>
		/// AppDomain to use for this plugin
		/// </summary>
		public AppDomain PluginAppDomain {get; set;}
		
		
		
		
		/// <summary>
		/// AssemblyLoader to use for this plugin
		/// </summary>
		public AssemblyLoader Loader {get; set;}
		
		
		#endregion Properties
		
		
	}
}
