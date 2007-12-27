
using System;

namespace themonospot_Base_Main
{
	
	[Serializable]
	public class clsConfiguration
	{
		
		string my_defaultPath;
		
		public clsConfiguration()
		{
		}
		
		public string defaultPath
        {
        	get { return my_defaultPath; }
        	set { my_defaultPath = value; }
        }
		
	}
}
