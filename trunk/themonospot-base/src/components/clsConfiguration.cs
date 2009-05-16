
using System;

namespace themonospot_Base_Main
{

	public class clsConfiguration
	{
		
		string my_defaultPath;
		bool _autoReport;
		
		public clsConfiguration()
		{
		}
		
		public string defaultPath
        {
        	get { return my_defaultPath; }
        	set { my_defaultPath = value; }
        }
		
		public bool autoReport
        {
        	get { return _autoReport; }
        	set { _autoReport = value; }
        }
		
		
		public void UpdateConfigFile(string configfilepath)
		{
			ABasile.SettingsManager SM = new ABasile.SettingsManager(configfilepath);
			SM.WriteString("Defaults", "defaultPath", my_defaultPath);
			SM.WriteBool("Defaults", "autoReport", _autoReport);
			SM.Save();
			SM = null;
		}

		public void ReadConfigFile(string configfilepath)
		{
			ABasile.SettingsManager SM = new ABasile.SettingsManager(configfilepath);
			my_defaultPath = SM.ReadString("Defaults", "defaultPath", "");
			_autoReport = SM.ReadBool("Defaults", "autoReport", false);			
			SM = null;
		}
		
	}
}
