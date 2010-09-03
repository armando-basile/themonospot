
using System;
using System.IO;

namespace ThemonospotBase
{
	
	/// <summary>
	/// Manage language files to use with gui
	/// </summary>
	public class LanguageManager
	{
		
		// ATTRIBUTES
		private string langFilePath;
		private string currentLanguage;
		private SettingsManager sm;
		
		
		
		
		// PROPERTIES
		
		public string CurrentLanguage 
		{
			get
			{
				return currentLanguage;	
			}
		}
		
		
		
		// CONSTRUCTOR
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="languageFilesFolder">
		/// path of language files folder
		/// </param>
		public LanguageManager(string languageFilesFolder)
		{
			
			// Language of operating system
			string systemLanguage = 
				System.Globalization.CultureInfo.CurrentCulture.NativeName.ToLower();			
			systemLanguage = 
				systemLanguage.Substring(0, systemLanguage.IndexOf("(") - 1).Trim();
			
			// Check for language file
			if (!File.Exists(languageFilesFolder +
			                 Path.DirectorySeparatorChar + systemLanguage + ".xml"))
			{
				// If there is a language file for the
				// same language of operating system language, use it
				currentLanguage = "english";
			}
			else
			{
				// Use standard english language file
				currentLanguage = systemLanguage;
			}
			
			// define language file path for current language
			langFilePath = languageFilesFolder +
			                 Path.DirectorySeparatorChar + currentLanguage + ".xml";
			
			// create an instance of SettingsManager class
			// and open language file.
			sm = new SettingsManager(langFilePath);		
			
			
		}
		
		
		
		/// <summary>
		/// Get key value from language file
		/// </summary>
		/// key name
		/// </param>
		/// <returns>
		/// Return key value
		/// </returns>
		public string GetKeyValue(string KeyName)
		{
			return sm.ReadStringInner("language", KeyName, "").Replace("&amp;","&");
		}
		
		
		
	}
}
