
using System;

namespace ThemonospotPluginAvi
{
	
	
	/// <summary>
	/// AVIMAINHEADER - Structure of Main Header ('avih')
	/// </summary>
	public class AVIMAINHEADER
    {   
		
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
		
		public int DwMicroSecPerFrame 
		{	get {	return _dwMicroSecPerFrame;	}	}
	
		public int DwMaxBytesPerSec 
		{	get {	return _dwMaxBytesPerSec;	}	}

		public int DwPaddingGranularity 
		{	get {	return _dwPaddingGranularity;	}	}
	
		public int DwFlags 
		{	get {	return _dwFlags;	}	}
	
		public int DwTotalFrames 
		{	get {	return _dwTotalFrames;	}	}
	
		public int DwInitialFrames 
		{	get {	return _dwInitialFrames;	}	}
	
		public int DwStreams 
		{	get {	return _dwStreams;	}	}
	
		public int DwSuggestedBufferSize 
		{	get {	return _dwSuggestedBufferSize;	}	}
	
		public int DwWidth 
		{	get {	return _dwWidth;	}	}
	
		public int DwHeight 
		{	get {	return _dwHeight;	}	}
	
		public int DwReserved0 
		{	get {	return _dwReserved0;	}	}
	
		public int DwReserved1 
		{	get {	return _dwReserved1;	}	}
	
		public int DwReserved2 
		{	get {	return _dwReserved2;	}	}
	
		public int DwReserved3 
		{	get {	return _dwReserved3;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			Utility myEnc = new Utility();
			
			// Fill data in the structure
			_dwMicroSecPerFrame = (int)myEnc.GetIntFromBytes(dataIN,0,4) ;
	        _dwMaxBytesPerSec = (int)myEnc.GetIntFromBytes(dataIN,4,4) ;
	        _dwPaddingGranularity = (int)myEnc.GetIntFromBytes(dataIN,8,4) ;
	        _dwFlags = (int)myEnc.GetIntFromBytes(dataIN,12,4) ;
	        _dwTotalFrames = (int)myEnc.GetIntFromBytes(dataIN,16,4) ;
	        _dwInitialFrames = (int)myEnc.GetIntFromBytes(dataIN,20,4) ;
	        _dwStreams = (int)myEnc.GetIntFromBytes(dataIN,24,4) ;
	        _dwSuggestedBufferSize = (int)myEnc.GetIntFromBytes(dataIN,28,4) ;
	        _dwWidth = (int)myEnc.GetIntFromBytes(dataIN,32,4) ;
	        _dwHeight = (int)myEnc.GetIntFromBytes(dataIN,36,4) ;
	        _dwReserved0 = (int)myEnc.GetIntFromBytes(dataIN,40,4) ;
			_dwReserved1 = (int)myEnc.GetIntFromBytes(dataIN,44,4) ;
			_dwReserved2 = (int)myEnc.GetIntFromBytes(dataIN,48,4) ;
			_dwReserved3 = (int)myEnc.GetIntFromBytes(dataIN,52,4) ;
			return;
		}
	
	}
	
}
