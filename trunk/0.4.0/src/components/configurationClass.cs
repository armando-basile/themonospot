
using System;

namespace monoSpotMain
{
	
	[Serializable]
	public class configurationClass
	{
		
		string my_defaultPath;
		
		public configurationClass()
		{
		}
		
		public string defaultPath
        {
        	get { return my_defaultPath; }
        	set { my_defaultPath = value; }
        }
		
	}
}
