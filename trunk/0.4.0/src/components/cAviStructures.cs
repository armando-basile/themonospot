using System;
using System.Text;
using Utility;

namespace monoSpotMain
{
	#region STRUCTURES CLASSES
	
	/// <summary>
	/// AVIMAINHEADER - Structure of Main Header ('avih')
	/// </summary>
	public class AVIMAINHEADER
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

		private int _dwMicroSecPerFrame;
        private int _dwMaxBytesPerSec;
        private int _dwPaddingGranularity;
        private int _dwFlags;
        private int _dwTotalFrames;
        private int _dwInitialFrames;
        private int _dwStreams;
        private int _dwSuggestedBufferSize;
        private int _dwWidth;
        private int _dwHeight;
        private int _dwReserved0;
		private int _dwReserved1;
		private int _dwReserved2;
		private int _dwReserved3;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int dwMicroSecPerFrame 
		{	get {	return _dwMicroSecPerFrame;	}	}
	
		public int dwMaxBytesPerSec 
		{	get {	return _dwMaxBytesPerSec;	}	}

		public int dwPaddingGranularity 
		{	get {	return _dwPaddingGranularity;	}	}
	
		public int dwFlags 
		{	get {	return _dwFlags;	}	}
	
		public int dwTotalFrames 
		{	get {	return _dwTotalFrames;	}	}
	
		public int dwInitialFrames 
		{	get {	return _dwInitialFrames;	}	}
	
		public int dwStreams 
		{	get {	return _dwStreams;	}	}
	
		public int dwSuggestedBufferSize 
		{	get {	return _dwSuggestedBufferSize;	}	}
	
		public int dwWidth 
		{	get {	return _dwWidth;	}	}
	
		public int dwHeight 
		{	get {	return _dwHeight;	}	}
	
		public int dwReserved0 
		{	get {	return _dwReserved0;	}	}
	
		public int dwReserved1 
		{	get {	return _dwReserved1;	}	}
	
		public int dwReserved2 
		{	get {	return _dwReserved2;	}	}
	
		public int dwReserved3 
		{	get {	return _dwReserved3;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure
			_dwMicroSecPerFrame = myEnc.getIntFromBytes(dataIN,0,4) ;
	        _dwMaxBytesPerSec = myEnc.getIntFromBytes(dataIN,4,4) ;
	        _dwPaddingGranularity = myEnc.getIntFromBytes(dataIN,8,4) ;
	        _dwFlags = myEnc.getIntFromBytes(dataIN,12,4) ;
	        _dwTotalFrames = myEnc.getIntFromBytes(dataIN,16,4) ;
	        _dwInitialFrames = myEnc.getIntFromBytes(dataIN,20,4) ;
	        _dwStreams = myEnc.getIntFromBytes(dataIN,24,4) ;
	        _dwSuggestedBufferSize = myEnc.getIntFromBytes(dataIN,28,4) ;
	        _dwWidth = myEnc.getIntFromBytes(dataIN,32,4) ;
	        _dwHeight = myEnc.getIntFromBytes(dataIN,36,4) ;
	        _dwReserved0 = myEnc.getIntFromBytes(dataIN,40,4) ;
			_dwReserved1 = myEnc.getIntFromBytes(dataIN,44,4) ;
			_dwReserved2 = myEnc.getIntFromBytes(dataIN,48,4) ;
			_dwReserved3 = myEnc.getIntFromBytes(dataIN,52,4) ;
			return;
		}
	
	}


	
	
	
	/// <summary>
	/// AVIEXTHEADER - Structure of AVIEXTHEADER ('dmlh')
	/// </summary>
	public class AVIEXTHEADER
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

        private int 	_dwGrandFrames;
        private int[] 	_dwFuture;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int dwGrandFrames 
		{	get {	return _dwGrandFrames;	}	}
	
		public int[] dwFuture
		{	get {	return _dwFuture;	}	}
	

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN, int dwFutureLen)
		{	
			myEnc = new cEncoding();
			int k;
			
			// Fill data in the structure			
			_dwGrandFrames = myEnc.getIntFromBytes(dataIN,0,4) ;
			if (dwFutureLen > 0)
			{
				_dwFuture = new int[dwFutureLen];
				for (k=0; k<dwFutureLen; k++)
					_dwFuture[k] = dataIN[k+4];
			}
			
			return;
		}
	
	}
	
	
	
	
	
	
	/// <summary>
	/// AVISTREAMHEADER - Structure of Stream Header ('strh')
	/// </summary>
	public class AVISTREAMHEADER
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

        private int 	_fccType;      			// stream type codes
        private int 	_fccHandler;
        private int 	_dwFlags;
        private short 	_wPriority;
        private short 	_wLanguage;
        private int 	_dwInitialFrames;
        private int 	_dwScale;
        private int 	_dwRate;				// dwRate/dwScale is stream tick rate in ticks/sec
        private int 	_dwStart;
        private int 	_dwLength;
        private int 	_dwSuggestedBufferSize;
        private int 	_dwQuality;
        private int 	_dwSampleSize;
      	private short 	_left;
        private short 	_top;
        private short 	_right;
        private short 	_bottom;
        
		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int fccType 
		{	get {	return _fccType;	}	}
	
		public int fccHandler 
		{	get {	return _fccHandler;	}	}

		public int dwFlags 
		{	get {	return _dwFlags;	}	}
	
		public short wPriority 
		{	get {	return _wPriority;	}	}
	
		public short wLanguage 
		{	get {	return _wLanguage;	}	}
	
		public int dwInitialFrames 
		{	get {	return _dwInitialFrames;	}	}
	
		public int dwScale 
		{	get {	return _dwScale;	}	}
		
		public int dwRate 
		{	get {	return _dwRate;	}	}
	
		public int dwStart 
		{	get {	return _dwStart;	}	}
	
		public int dwLength 
		{	get {	return _dwLength;	}	}
	
		public int dwSuggestedBufferSize 
		{	get {	return _dwSuggestedBufferSize;	}	}
	
		public int dwQuality 
		{	get {	return _dwQuality;	}	}
	
		public int dwSampleSize 
		{	get {	return _dwSampleSize;	}	}
	
		public short left 
		{	get {	return _left;	}	}

		public short top
		{	get {	return _top;	}	}

		public short right
		{	get {	return _right;	}	}

		public short bottom
		{	get {	return _bottom;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure
			_fccType = myEnc.getIntFromBytes(dataIN,0,4) ;
	        _fccHandler = myEnc.getIntFromBytes(dataIN,4,4) ;
	        _dwFlags = myEnc.getIntFromBytes(dataIN,8,4) ;
	        _wPriority = (short)myEnc.getIntFromBytes(dataIN,12,2) ;
	        _wLanguage = (short)myEnc.getIntFromBytes(dataIN,14,2) ;
	        _dwInitialFrames = myEnc.getIntFromBytes(dataIN,16,4) ;
	        _dwScale = myEnc.getIntFromBytes(dataIN,20,4) ;
	        _dwRate = myEnc.getIntFromBytes(dataIN,24,4) ;
	        _dwStart = myEnc.getIntFromBytes(dataIN,28,4) ;
	        _dwLength = myEnc.getIntFromBytes(dataIN,32,4) ;
	        _dwSuggestedBufferSize = myEnc.getIntFromBytes(dataIN,36,4) ;
	        _dwQuality = myEnc.getIntFromBytes(dataIN,40,4) ;
	        _dwSampleSize = myEnc.getIntFromBytes(dataIN,44,4) ;
	      	_left = (short)myEnc.getIntFromBytes(dataIN,48,2) ;
	        _top = (short)myEnc.getIntFromBytes(dataIN,50,2) ;
	        _right = (short)myEnc.getIntFromBytes(dataIN,52,2) ;
	        _bottom = (short)myEnc.getIntFromBytes(dataIN,54,2) ;
			return;
		}
	
	}

	
	
	/// <summary>
	/// BITMAPINFOHEADER - Structure of BITMAPINFOHEADER ('strf')
	/// </summary>
	public class BITMAPINFOHEADER
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

	    private int  	_biSize;
	    private int   	_biWidth;
	    private int   	_biHeight;
	    private short  	_biPlanes;
	    private short  	_biBitCount;
	    private int  	_biCompression;
	    private int  	_biSizeImage;
	    private int   	_biXPelsPerMeter;
	    private int   	_biYPelsPerMeter;
	    private int  	_biClrUsed;
	    private int 	_biClrImportant;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int biSize 
		{	get {	return _biSize;	}	}
	
		public int biWidth 
		{	get {	return _biWidth;	}	}

		public int biHeight 
		{	get {	return _biHeight;	}	}
	
		public short biPlanes 
		{	get {	return _biPlanes;	}	}
	
		public short biBitCount 
		{	get {	return _biBitCount;	}	}
	
		public int biCompression 
		{	get {	return _biCompression;	}	}
	
		public int biSizeImage 
		{	get {	return _biSizeImage;	}	}
	
		public int biXPelsPerMeter 
		{	get {	return _biXPelsPerMeter;	}	}
	
		public int biYPelsPerMeter 
		{	get {	return _biYPelsPerMeter;	}	}
	
		public int biClrUsed 
		{	get {	return _biClrUsed;	}	}
	
		public int biClrImportant 
		{	get {	return _biClrImportant;	}	}
	

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure
	    	_biSize = myEnc.getIntFromBytes(dataIN,0,4) ;
	    	_biWidth = myEnc.getIntFromBytes(dataIN,4,4) ;
	    	_biHeight = myEnc.getIntFromBytes(dataIN,8,4) ;
	    	_biPlanes = (short)myEnc.getIntFromBytes(dataIN,12,2) ;
	    	_biBitCount = (short)myEnc.getIntFromBytes(dataIN,14,2) ;
	    	_biCompression = myEnc.getIntFromBytes(dataIN,16,4) ;
	    	_biSizeImage = myEnc.getIntFromBytes(dataIN,20,4) ;
	    	_biXPelsPerMeter = myEnc.getIntFromBytes(dataIN,24,4) ;
	    	_biYPelsPerMeter = myEnc.getIntFromBytes(dataIN,28,4) ;
	    	_biClrUsed = myEnc.getIntFromBytes(dataIN,32,4) ;
	    	_biClrImportant = myEnc.getIntFromBytes(dataIN,36,4) ;
			return;
		}
	
	}
	
	
	
	
	/// <summary>
	/// WAVEFORMATEX - Structure of WAVEFORMATEX
	/// </summary>
	public class WAVEFORMATEX
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

		private short 	_wFormatTag; 
		private short  	_nChannels; 
		private int	  	_nSamplesPerSec; 
		private int	  	_nAvgBytesPerSec; 
		private short  	_nBlockAlign; 
		private short  	_wBitsPerSample; 
		private short  	_cbSize; 
		// Extra
		private short  	_wID;
		private int  	_fdwFlags;		 
		private short  	_nBlockSize; 
		private short  	_nFramesPerBlock; 
		private short  	_nCodecDelay; 
		

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public short wFormatTag 
		{	get {	return _wFormatTag;	}	}
	
		public short nChannels 
		{	get {	return _nChannels;	}	}
	
		public int nSamplesPerSec 
		{	get {	return _nSamplesPerSec;	}	}
	
		public int nAvgBytesPerSec 
		{	get {	return _nAvgBytesPerSec;	}	}
	
		public short nBlockAlign 
		{	get {	return _nBlockAlign;	}	}
	
		public short wBitsPerSample 
		{	get {	return _wBitsPerSample;	}	}
	
		public short cbSize 
		{	get {	return _cbSize;	}	}
	
		// Extra
		public short wID
		{	get {	return _wID;	}	}

		public int fdwFlags 
		{	get {	return _fdwFlags;	}	}

		public short nBlockSize 
		{	get {	return _nBlockSize;	}	}
	
		public short nFramesPerBlock 
		{	get {	return _nFramesPerBlock;	}	}
	
		public short nCodecDelay 
		{	get {	return _nCodecDelay;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure			
			_wFormatTag = (short)myEnc.getIntFromBytes(dataIN,0,2) ;
			_nChannels = (short)myEnc.getIntFromBytes(dataIN,2,2) ;
			_nSamplesPerSec = myEnc.getIntFromBytes(dataIN,4,4) ;
			_nAvgBytesPerSec = myEnc.getIntFromBytes(dataIN,8,4) ;
			_nBlockAlign = (short)myEnc.getIntFromBytes(dataIN,12,2) ;
			_wBitsPerSample = (short)myEnc.getIntFromBytes(dataIN,14,2) ;
			_cbSize = (short)myEnc.getIntFromBytes(dataIN,16,2) ;
			
			// Extra
			if (dataIN.Length > 18)
				_wID = (short)myEnc.getIntFromBytes(dataIN,18,2) ;
			
			if (dataIN.Length > 20)
				_fdwFlags = myEnc.getIntFromBytes(dataIN,20,4) ;				

			if (dataIN.Length > 24)
				_nBlockSize = (short)myEnc.getIntFromBytes(dataIN,24,2) ;

			if (dataIN.Length > 26)
				_nFramesPerBlock = (short)myEnc.getIntFromBytes(dataIN,26,2) ;

			if (dataIN.Length > 28)
				_nCodecDelay = (short)myEnc.getIntFromBytes(dataIN,28,2) ;

			return;
		}
	
	}
	
	
	

	/// <summary>
	/// TIMECODEDATA - Structure of TIMECODEDATA
	/// </summary>
	public class TIMECODEDATA
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

        private short 	_wFrameRate;
        private short 	_wFrameFract;
        private int 	_cFrames;
        private int 	_dwSMPTEflags;
        private int 	_dwUser;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public short wFrameRate 
		{	get {	return _wFrameRate;	}	}
	
		public short wFrameFract 
		{	get {	return _wFrameFract;	}	}
	
		public int cFrames 
		{	get {	return _cFrames;	}	}
	
		public int dwSMPTEflags 
		{	get {	return _dwSMPTEflags;	}	}
	
		public int dwUser 
		{	get {	return _dwUser;	}	}
	

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure			
			_wFrameRate = (short)myEnc.getIntFromBytes(dataIN,0,2) ;
			_wFrameFract = (short)myEnc.getIntFromBytes(dataIN,2,2) ;
			_cFrames = myEnc.getIntFromBytes(dataIN,4,4) ;
			_dwSMPTEflags = myEnc.getIntFromBytes(dataIN,8,4) ;
			_dwUser = myEnc.getIntFromBytes(dataIN,12,4) ;
			return;
		}
	
	}

	
	
	
	
	
	
	/// <summary>
	/// AVIOLDINDEXENTRY - Structure of AVIOLDINDEXENTRY
	/// </summary>
	public class AVIOLDINDEXENTRY
    {   
		// Private Objects
		cEncoding myEnc;
		
		
		#region Private Variables

        private int 	_dwChunkId;        
        private int 	_dwFlags;
        private int 	_dwOffset;
        private int 	_dwSize;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int dwChunkId 
		{	get {	return _dwChunkId;	}	}
	
		public int dwFlags 
		{	get {	return _dwFlags;	}	}
	
		public int dwOffset 
		{	get {	return _dwOffset;	}	}
	
		public int dwSize 
		{	get {	return _dwSize;	}	}
	

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			myEnc = new cEncoding();
			
			// Fill data in the structure			
			_dwChunkId = myEnc.getIntFromBytes(dataIN,0,4) ;
			_dwFlags = myEnc.getIntFromBytes(dataIN,4,8) ;
			_dwOffset = myEnc.getIntFromBytes(dataIN,12,4) ;
			_dwSize = myEnc.getIntFromBytes(dataIN,16,4) ;
			return;
		}
	
	}	
	
	
	
	
	
	
	
	#endregion STRUCTURES CLASSES
	
	
	
	
	
	
	
	
    class AviRiffData
    {
        #region AVI constants

        // AVIMAINHEADER flags
        public static readonly int AVIF_HASINDEX = 0x00000010; // Index at end of file?
        public static readonly int AVIF_MUSTUSEINDEX = 0x00000020;
        public static readonly int AVIF_ISINTERLEAVED = 0x00000100;
        public static readonly int AVIF_TRUSTCKTYPE = 0x00000800; // Use CKType to find key frames
        public static readonly int AVIF_WASCAPTUREFILE = 0x00010000;
        public static readonly int AVIF_COPYRIGHTED = 0x00020000;

        // AVISTREAMINFO flags
        public static readonly int AVISF_DISABLED = 0x00000001;
        public static readonly int AVISF_VIDEO_PALCHANGES = 0x00010000;

        // AVIOLDINDEXENTRY flags
        public static readonly int AVIIF_LIST = 0x00000001;
        public static readonly int AVIIF_KEYFRAME = 0x00000010;
        public static readonly int AVIIF_NO_TIME = 0x00000100;
        public static readonly int AVIIF_COMPRESSOR = 0x0FFF0000;  // unused?

        // TIMECODEDATA flags
        public static readonly int TIMECODE_SMPTE_BINARY_GROUP = 0x07;
        public static readonly int TIMECODE_SMPTE_COLOR_FRAME = 0x08;


        // AVI stream FourCC codes
        public static readonly int streamtypeVIDEO = Utility.cEncoding.ToFourCC("vids");
        public static readonly int streamtypeAUDIO = Utility.cEncoding.ToFourCC("auds");
        public static readonly int streamtypeMIDI = Utility.cEncoding.ToFourCC("mids");
        public static readonly int streamtypeTEXT = Utility.cEncoding.ToFourCC("txts");

        // AVI section FourCC codes
		public static readonly int ckidAVIHeaderList = Utility.cEncoding.ToFourCC("hdrl");
        public static readonly int ckidMainAVIHeader = Utility.cEncoding.ToFourCC("avih");
        public static readonly int ckidODML = Utility.cEncoding.ToFourCC("odml");
        public static readonly int ckidAVIExtHeader = Utility.cEncoding.ToFourCC("dmlh");
        public static readonly int ckidAVIStreamList = Utility.cEncoding.ToFourCC("strl");
        public static readonly int ckidAVIStreamHeader = Utility.cEncoding.ToFourCC("strh");
        public static readonly int ckidAVIStreamData = Utility.cEncoding.ToFourCC("strd");
        public static readonly int ckidStreamFormat = Utility.cEncoding.ToFourCC("strf");
        public static readonly int ckidAVIMovieData = Utility.cEncoding.ToFourCC("movi");
        public static readonly int ckidAVIOldIndex = Utility.cEncoding.ToFourCC("idx1");
		public static readonly int ckidINFOList = Utility.cEncoding.ToFourCC("INFO");
		public static readonly int ckidJUNKTag = Utility.cEncoding.ToFourCC("JUNK");
		public static readonly int ckidAVIISFT = Utility.cEncoding.ToFourCC("ISFT");
		public static readonly int ckidWaveFMT = Utility.cEncoding.ToFourCC("fmt ");
		public static readonly int ckidMovieWaveTrack = Utility.cEncoding.ToFourCC("01wb");
		public static readonly int ckidMovieVideoTrack = Utility.cEncoding.ToFourCC("00dc");

		public static readonly byte[] UserDataBytes = {0x00, 0x00, 0x01, 0xB2}; // User Data 
		public static readonly byte[] VOLStartBytes = {0x00, 0x00, 0x01, 0x20}; // Video Object Layer (VOL)
		public static readonly byte[] VOPStartBytes = {0x00, 0x00, 0x01, 0xB6}; // Video Object Plane (VOP)
		
		// Audio codec
		public static readonly int ckidMP3 = 0x0055;
		

        #endregion


    }
	
	
}
