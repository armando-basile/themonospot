
using System;

namespace ThemonospotPluginAvi
{
	
	
	public class AviAudioStreamEntity
	{
		
		private string _code = "";
		
		public string Code 
		{
			get {return _code;}
			set {_code = value;}
		}		

		
		private string _streamType = "";
		
		public string StreamType 
		{
			get {return _streamType;}
			set {_streamType = value;}
		}		

		
		
		
		private string _bitrate = "";
		
		public string Bitrate 
		{
			get {return _bitrate;}
			set {_bitrate = value;}
		}		
		
		
		
		private int _hz = 0;
		
		public int Hz
		{
			get {return _hz;}
			set {_hz = value;}
		}	
		
	
		
		
		private int _channels = 0;
		
		public int Channels
		{
			get {return _channels;}
			set {_channels = value;}
		}	
		
		
	}
}
