
using System;

namespace ThemonospotPluginAvi
{
	
	

	
	
	
	/// <summary>
	/// BITMAPINFOHEADER - Structure of BITMAPINFOHEADER ('strf')
	/// </summary>
	public class BITMAPINFOHEADER
    {   
		
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
		private long	_offset;

		#endregion Private Variables

		
		#region Public Methods to obtain infos
		
		public int BiSize 
		{	get {	return _biSize;	}	}
	
		public int BiWidth 
		{	get {	return _biWidth;	}	}

		public int BiHeight 
		{	get {	return _biHeight;	}	}
	
		public short BiPlanes 
		{	get {	return _biPlanes;	}	}
	
		public short BiBitCount 
		{	get {	return _biBitCount;	}	}
	
		public int BiCompression 
		{	get {	return _biCompression;	}	}
	
		public int BiSizeImage 
		{	get {	return _biSizeImage;	}	}
	
		public int BiXPelsPerMeter 
		{	get {	return _biXPelsPerMeter;	}	}
	
		public int BiYPelsPerMeter 
		{	get {	return _biYPelsPerMeter;	}	}
	
		public int BiClrUsed 
		{	get {	return _biClrUsed;	}	}
	
		public int BiClrImportant 
		{	get {	return _biClrImportant;	}	}

		public long Offset 
		{
			get {	return _offset;	}	
			set {	_offset = value;}	
		}

		#endregion Public Methods to obtain infos
		
		/// <summary>
		/// Load data from byte array into structure
		/// </summary>
		public void loadDataStructure(byte[] dataIN)
		{	
			Utility myEnc = new Utility();
			
			// Fill data in the structure
	    	_biSize = (int)myEnc.GetIntFromBytes(dataIN,0,4) ;
	    	_biWidth = (int)myEnc.GetIntFromBytes(dataIN,4,4) ;
	    	_biHeight = (int)myEnc.GetIntFromBytes(dataIN,8,4) ;
	    	_biPlanes = (short)myEnc.GetIntFromBytes(dataIN,12,2) ;
	    	_biBitCount = (short)myEnc.GetIntFromBytes(dataIN,14,2) ;
	    	_biCompression = (int)myEnc.GetIntFromBytes(dataIN,16,4) ;
	    	_biSizeImage = (int)myEnc.GetIntFromBytes(dataIN,20,4) ;
	    	_biXPelsPerMeter = (int)myEnc.GetIntFromBytes(dataIN,24,4) ;
	    	_biYPelsPerMeter = (int)myEnc.GetIntFromBytes(dataIN,28,4) ;
	    	_biClrUsed = (int)myEnc.GetIntFromBytes(dataIN,32,4) ;
	    	_biClrImportant = (int)myEnc.GetIntFromBytes(dataIN,36,4) ;
			return;
		}
	
	}
	

	
	
	
	
	
	
}
