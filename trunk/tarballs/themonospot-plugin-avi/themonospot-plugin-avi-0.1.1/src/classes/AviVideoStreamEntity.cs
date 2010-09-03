
using System;
using System.Collections.Generic;

namespace ThemonospotPluginAvi
{
	
	
	public class AviVideoStreamEntity
	{
		

		private string 	_codecName = "";
		private string 	_codecDesc = "";
		private string 	_frameSize = "";
		private int 	_frameRate = 0;
		private string 	_videoBitrate = "";
		private long 	_totalTime = 0;
		private long 	_totalFrame = 0;	
		private long 	_dataRate = 0;	
		private double 	_quality = 0;	
		private bool 	_packetBitstream = false;	
		
		private List<string> 	_infoData = new List<string>();	
		private List<string> 	_userData = new List<string>();	
		
		
		
		public string VideoBitrate 
		{
			get {return _videoBitrate;}
			set {_videoBitrate = value;}
		}
		
		public List<string> UserData 
		{
			get {return _userData;}
			set {_userData = value;}
		}
		
		public long TotalTime 
		{
			get {return _totalTime;}
			set {_totalTime = value;}
		}
		
		public long TotalFrame 
		{
			get {return _totalFrame;}
			set {_totalFrame = value;}
		}
		
		public double Quality 
		{
			get {return _quality;}
			set {_quality = value;}
		}
		
		public bool PacketBitstream 
		{
			get {return _packetBitstream;	}
			set {_packetBitstream = value;	}
		}
		
		public List<string> InfoData 
		{
			get {return _infoData;}
			set {_infoData = value;	}
		}
		
		public string FrameSize 
		{
			get {return _frameSize;	}
			set {_frameSize = value;}
		}
		
		public int FrameRate 
		{
			get {return _frameRate;		}
			set {_frameRate = value;	}
		}
		
		public long DataRate 
		{
			get {return _dataRate;	}
			set {_dataRate = value;	}
		}
		
		public string CodecName 
		{
			get {return _codecName;	}
			set {_codecName = value;}
		}
		
		public string CodecDesc 
		{
			get {return _codecDesc;	}
			set {_codecDesc = value;}
		}
		
		
		
	}
}
