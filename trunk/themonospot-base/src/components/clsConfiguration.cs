
using System;

namespace themonospot_Base_Main
{
	
	[Serializable]
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
		
		
	}
}
