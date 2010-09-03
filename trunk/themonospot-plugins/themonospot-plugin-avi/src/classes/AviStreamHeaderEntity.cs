
using System;
using ThemonospotComponents;

namespace ThemonospotPluginAvi
{
	
	/// <summary>
	/// AVISTREAMHEADER - Structure of Stream Header ('strh')
	/// </summary>
	public class AVISTREAMHEADER
    {   
		
		#region Private Variables

        private int 	_fccType;      			// stream type codes
        private int 	_fccHandler;
        private int 	_dwFlags;
        private ushort 	_wPriority;
        private ushort 	_wLanguage;
        private int 	_dwInitialFrames;
        private int 	_dwScale;
        private int 	_dwRate;				// dwRate/dwScale is stream tick rate in ticks/sec
        private int 	_dwStart;
        private int 	_dwLength;
        private int 	_dwSuggestedBufferSize;
        private int 	_dwQuality;
        private int 	_dwSampleSize;
      	private ushort 	_left;
        private ushort 	_top;
        private ushort 	_right;
        private ushort 	_bottom;
        
		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int FccType 
		{	get {	return _fccType;	}	}
	
		public int FccHandler 
		{	get {	return _fccHandler;	}	}

		public int DwFlags 
		{	get {	return _dwFlags;	}	}
	
		public ushort WPriority 
		{	get {	return _wPriority;	}	}
	
		public ushort WLanguage 
		{	get {	return _wLanguage;	}	}
	
		public int DwInitialFrames 
		{	get {	return _dwInitialFrames;	}	}
	
		public int DwScale 
		{	get {	return _dwScale;	}	}
		
		public int DwRate 
		{	get {	return _dwRate;	}	}
	
		public int DwStart 
		{	get {	return _dwStart;	}	}
	
		public int DwLength 
		{	get {	return _dwLength;	}	}
	
		public int DwSuggestedBufferSize 
		{	get {	return _dwSuggestedBufferSize;	}	}
	
		public int DwQuality 
		{	get {	return _dwQuality;	}	}
	
		public int DwSampleSize 
		{	get {	return _dwSampleSize;	}	}
	
		public ushort Left 
		{	get {	return _left;	}	}

		public ushort Top
		{	get {	return _top;	}	}

		public ushort Right
		{	get {	return _right;	}	}

		public ushort Bottom
		{	get {	return _bottom;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			BytesManipulation myEnc = new BytesManipulation();
			
			// Fill data in the structure
			_fccType = (int)myEnc.GetIntFromBytes(dataIN,0,4) ;
	        _fccHandler = (int)myEnc.GetIntFromBytes(dataIN,4,4) ;
	        _dwFlags = (int)myEnc.GetIntFromBytes(dataIN,8,4) ;
	        _wPriority = (ushort)myEnc.GetIntFromBytes(dataIN,12,2) ;
	        _wLanguage = (ushort)myEnc.GetIntFromBytes(dataIN,14,2) ;
	        _dwInitialFrames = (int)myEnc.GetIntFromBytes(dataIN,16,4) ;
	        _dwScale = (int)myEnc.GetIntFromBytes(dataIN,20,4) ;
	        _dwRate = (int)myEnc.GetIntFromBytes(dataIN,24,4) ;
	        _dwStart = (int)myEnc.GetIntFromBytes(dataIN,28,4) ;	        
	        _dwLength = (int)myEnc.GetIntFromBytes(dataIN,32,4) ;
	        _dwSuggestedBufferSize = (int)myEnc.GetIntFromBytes(dataIN,36,4) ;	        
	        _dwQuality = (int)myEnc.GetIntFromBytes(dataIN,40,4) ;
	        _dwSampleSize = (int)myEnc.GetIntFromBytes(dataIN,44,4);
	        _left = (ushort)myEnc.GetIntFromBytes(dataIN,48,2) ;
	      	_top = (ushort)myEnc.GetIntFromBytes(dataIN,50,2) ;
	        _right = (ushort)myEnc.GetIntFromBytes(dataIN,52,2) ;
	        _bottom = (ushort)myEnc.GetIntFromBytes(dataIN,54,2) ;
	        
			return;
		}
	
	}
	
	
	
	
}
