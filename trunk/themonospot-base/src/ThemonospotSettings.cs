
using System;
using System.Reflection;
using System.IO;
using log4net;
using ThemonospotComponents;

namespace ThemonospotBase
{
	
	/// <summary>
	/// Manage settings info for themonospot application
	/// </summary>
	public class ThemonospotSettings
	{
		
		
		private SettingsManager sMan;
		private Utilities utils = new Utilities();
		
		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(ThemonospotSettings));
		
		#region Properties
		
		
		bool _isReportAuto;

		/// <value>
		/// Set if is request a report file generation for each parsed file
		/// </value>
		public bool IsReportAuto
        {
        	get { return _isReportAuto; }
        	set { _isReportAuto = value; }
        }
		
		
		
		
		string _configFilePath = "";
		
		/// <value>
		/// Actual xml configuration file path
		/// </value>
		public string ConfigFilePath
		{
			get {	return _configFilePath;	}
		}
		
		
		
		
		string _defaultPath;
		
		/// <value>
		/// Default path when choose file to parse
		/// </value>
		public string DefaultPath
        {
        	get { return _defaultPath; }
        	set { _defaultPath = value; }
        }


		
		
		
		
		#endregion Properties
	
		
		
		/// <summary>
		/// Create instance of ThemonospotSettings class and setup configuration file path.
		/// </summary>
		public ThemonospotSettings()
		{
			
			// set configuration file path
			if (utils.IsWindows() == true)
			{
				log.Info("OS: Windows");
				_configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar.ToString();				
			}
			else
			{
				log.Info("OS: NON Windows");
				_configFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar.ToString();
			}
			
			_configFilePath += ".themonospot.xml";
			log.Info("configuration file path: " + _configFilePath);

		}
		
		
		

		
		
		/// <summary>
		/// Read configuration file and update local objects
		/// </summary>
		public void ReadConfigFile()
		{
			sMan = new SettingsManager(_configFilePath);
			_defaultPath = sMan.ReadString("Defaults", "DefaultPath", "");
			_isReportAuto = sMan.ReadBool("Defaults", "IsReportAuto", false);			
			sMan = null;
		}
		
		
		
		
		/// <summary>
		/// Update configuration file
		/// </summary>
		public void UpdateConfigFile()
		{
			sMan = new SettingsManager(_configFilePath);
			sMan.WriteString("Defaults", "DefaultPath", _defaultPath);
			sMan.WriteBool("Defaults", "IsReportAuto", _isReportAuto);
			sMan.Save();
			sMan = null;
		}

		
		
		
		
		
	}
}
