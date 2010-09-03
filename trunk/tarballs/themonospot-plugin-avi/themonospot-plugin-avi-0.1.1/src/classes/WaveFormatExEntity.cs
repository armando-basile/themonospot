
using System;

namespace ThemonospotPluginAvi
{
	
	

	
	/// <summary>
	/// WAVEFORMATEX - Structure of WAVEFORMATEX
	/// </summary>
	public class WAVEFORMATEX
    {   
		
		
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
		
		public short WFormatTag 
		{	get {	return _wFormatTag;	}	}
	
		public short NChannels 
		{	get {	return _nChannels;	}	}
	
		public int NSamplesPerSec 
		{	get {	return _nSamplesPerSec;	}	}
	
		public int NAvgBytesPerSec 
		{	get {	return _nAvgBytesPerSec;	}	}
	
		public short NBlockAlign 
		{	get {	return _nBlockAlign;	}	}
	
		public short WBitsPerSample 
		{	get {	return _wBitsPerSample;	}	}
	
		public short CbSize 
		{	get {	return _cbSize;	}	}
	
		// Extra
		public short WID
		{	get {	return _wID;	}	}

		public int FdwFlags 
		{	get {	return _fdwFlags;	}	}

		public short NBlockSize 
		{	get {	return _nBlockSize;	}	}
	
		public short NFramesPerBlock 
		{	get {	return _nFramesPerBlock;	}	}
	
		public short NCodecDelay 
		{	get {	return _nCodecDelay;	}	}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			Utility myEnc = new Utility();
			
			// Fill data in the structure			
			_wFormatTag = (short)myEnc.GetIntFromBytes(dataIN,0,2) ;
			_nChannels = (short)myEnc.GetIntFromBytes(dataIN,2,2) ;
			_nSamplesPerSec = (int)myEnc.GetIntFromBytes(dataIN,4,4) ;
			_nAvgBytesPerSec = (int)myEnc.GetIntFromBytes(dataIN,8,4) ;
			_nBlockAlign = (short)myEnc.GetIntFromBytes(dataIN,12,2) ;
			_wBitsPerSample = (short)myEnc.GetIntFromBytes(dataIN,14,2);
			
			// Extra
			if (dataIN.Length > 16)
			{
				_cbSize = (short)myEnc.GetIntFromBytes(dataIN,16,2) ;
			}
			
			if (dataIN.Length > 18)
			{
				_wID = (short)myEnc.GetIntFromBytes(dataIN,18,2) ;
			}
			
			if (dataIN.Length > 20)
			{
				_fdwFlags = (int)myEnc.GetIntFromBytes(dataIN,20,4) ;				
			}

			if (dataIN.Length > 24)
			{
				_nBlockSize = (short)myEnc.GetIntFromBytes(dataIN,24,2) ;
			}

			if (dataIN.Length > 26)
			{
				_nFramesPerBlock = (short)myEnc.GetIntFromBytes(dataIN,26,2) ;
			}

			if (dataIN.Length > 28)
			{
				_nCodecDelay = (short)myEnc.GetIntFromBytes(dataIN,28,2) ;
			}
			
			return;
		}
	
	}
	
	
	
	
}
