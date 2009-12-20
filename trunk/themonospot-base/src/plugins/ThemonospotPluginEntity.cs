
using System;

namespace ThemonospotBase
{
	
	/// <summary>
	/// Describe Plugin info
	/// </summary>
	public class ThemonospotPluginEntity
	{

		#region Properties
		
		
		private string _description = "";
		
		/// <value>
		/// Assembly description
		/// </value>
		public string Description
		{
			get {return _description;}
			set {_description = value;}
		}

		
		
		
		
		
		private string _fileName = "";
		
		/// <value>
		/// Assembly filename without .dll
		/// </value>
		public string FileName
		{
			get {return _fileName;}
			set {_fileName = value;}
		}
		

		
		
		private string _folderPath = "";
		
		/// <value>
		/// Assembly folder path
		/// </value>
		public string FolderPath
		{
			get {return _folderPath;}
			set {_folderPath = value;}
		}
		
		
		
		

		
		private string _managedExtensions = "";

		/// <value>
		/// Managed multimedia file extentions
		/// </value>
		public string ManagedExtensions
		{
			get {return _managedExtensions;}
			set {_managedExtensions = value;}
		}

		
		
		

		private string _nameSpace = "";

		/// <value>
		/// Get NameSpace name
		/// </value>
		public string NameSpace
		{
			get {return _nameSpace;}
			set {_nameSpace = value;}
		}
		
		
		
		
		private string _release = "";

		/// <value>
		/// Assembly release
		/// </value>
		public string Release
		{
			get {return _release;}
			set {_release = value;}
		}
		
		
		
		
		
		
		
		
		
		
		
		#endregion Properties
		
		
	}
}
