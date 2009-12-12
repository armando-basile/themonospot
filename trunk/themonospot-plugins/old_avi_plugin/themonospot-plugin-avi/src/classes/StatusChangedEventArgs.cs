
using System;

namespace ThemonospotPluginAvi
{
	
	
	public class StatusChangedEventArgs: EventArgs
	{

		private string _statusDescription = "";
		private int _statusValue = 0;

		public string StatusDescription 
		{
			get {return _statusDescription;	}
			set {_statusDescription = value;}
		}

		public int StatusValue 
		{
			get {return _statusValue;}
			set {_statusValue = value;}
		}		

		
		public StatusChangedEventArgs(string statusDesc, int statusValue)
		{
			_statusDescription = statusDesc;
			_statusValue = statusValue;
		}
		
		
	}
}
