
using System;
using System.Collections.Generic;
using ThemonospotBase;

namespace ThemonospotBase
{
	
	
	public static class GlobalData
	{
		// ATTRIBUTES
		
		private static LanguageManager langMan;
		
		
		
		// PROPERTIES
		
		public static string FileDialogExtensions {	get; set;	}
		public static string GuiRelease {	get; set;	}
		public static string BaseRelease {	get; set;	}
		public static string AppPath {	get; set;	}
		public static string GuiDescription {	get; set;	}
		public static string GuiCopyright {	get; set;	}
		public static List<ThemonospotPluginEntity> BasePlugins {	get; set;	}
		

		/// <summary>
		/// Init Language Manager
		/// </summary>
		public static void InitLanguage(string languagesFolder)
		{
			langMan = new LanguageManager(languagesFolder);
		}
		

		/// <summary>
		/// Return value of a specific key
		/// </summary>
		public static string GetLanguageKeyValue(string keyName)
		{
			return langMan.GetKeyValue(keyName);
		}
		
		
		
	}
}




