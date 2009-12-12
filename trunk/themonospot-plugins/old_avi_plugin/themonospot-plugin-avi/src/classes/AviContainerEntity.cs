
using System;
using System.Collections.Generic;

namespace ThemonospotPluginAvi
{
	
	
	public class AviContainerEntity
	{

		List<AviVideoStreamEntity> _videoStreams = new List<AviVideoStreamEntity>();
		List<AviAudioStreamEntity> _audioStreams = new List<AviAudioStreamEntity>();
		
		List<string> _junkData = new List<string>();
		List<string> _isftData = new List<string>();
		List<string> _moviUserData = new List<string>();
		bool _pBitstream = false;
		long _dataSize = 0;
		long _moviSize = 0;
		long _moviOffset = 0;
		long _idxSize = 0;
		long _idxOffset = 0;
		
		
		AVIMAINHEADER _aviHeader = new AVIMAINHEADER();
		List<AviStreamEntity> _aviStreams = new List<AviStreamEntity>();
		
		
		public List<AviAudioStreamEntity> AudioStreams 
		{
			get {return _audioStreams; }
			set {_audioStreams = value;}
		}

		public List<AviVideoStreamEntity> VideoStreams 
		{
			get {return _videoStreams;}
			set {_videoStreams = value;}
		}
		
		public List<string> JunkData
		{
			get {return _junkData;}
			set {_junkData = value;}
		}
		
		
		public AVIMAINHEADER AviHeader
		{
			get {return _aviHeader;}
			set {_aviHeader = value;}
		}

		public List<AviStreamEntity> AviStreams 
		{
			get {return _aviStreams;}
			set {_aviStreams = value;}
		}

		public List<string> ISFTData 
		{
			get {return _isftData;	}
			set {_isftData = value;	}
		}

		public List<string> MoviUserData 
		{
			get {return _moviUserData;	}
			set {_moviUserData = value;	}
		}

		public bool PBitstream 
		{
			get {return _pBitstream;	}
			set {_pBitstream = value;	}
		}

		public long DataSize 
		{
			get {return _dataSize;	}
			set {_dataSize = value;	}
		}
		
		public long MoviSize 
		{
			get {return _moviSize;	}
			set {_moviSize = value;	}
		}
		

		public long MoviOffset 
		{
			get {return _moviOffset;	}
			set {_moviOffset = value;	}
		}		
		
		
		public long IdxSize 
		{
			get {return _idxSize;	}
			set {_idxSize = value;	}
		}


		public long IdxOffset 
		{
			get {return _idxOffset;	}
			set {_idxOffset = value;	}
		}
		
	}
}
