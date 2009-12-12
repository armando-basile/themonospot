
using System;
using System.Collections.Generic;

namespace ThemonospotPluginAvi
{
	
	
	public class AviStreamEntity
	{
		private long _offset = 0;
		private AVISTREAMHEADER _aviStreamHeader = new AVISTREAMHEADER();
		private List<BITMAPINFOHEADER> _aviVideoStreamHeader = new List<BITMAPINFOHEADER>();
		private List<WAVEFORMATEX> _aviAudioStreamHeader = new List<WAVEFORMATEX>();

		public long Offset
		{
			get {return _offset;}
			set {_offset = value;}
		}
		
		
		public List<WAVEFORMATEX> AviAudioStreamHeader 
		{
			get {return _aviAudioStreamHeader;	}
			set {_aviAudioStreamHeader = value;	}
		}

		public AVISTREAMHEADER AviStreamHeader 
		{
			get {return _aviStreamHeader;	}
			set {_aviStreamHeader = value;	}
		}

		public List<BITMAPINFOHEADER> AviVideoStreamHeader 
		{
			get {return _aviVideoStreamHeader;	}
			set {_aviVideoStreamHeader = value;	}
		}		
		
		
		
	}
}
