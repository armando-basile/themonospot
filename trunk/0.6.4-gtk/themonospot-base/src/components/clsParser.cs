using System;
using System.IO;
using System.Text;
using System.Threading;
using Utility;
using System.Collections;


namespace themonospot_Base_Main
{


	public class themonospotParserException : Exception
	{
        public themonospotParserException()
        	: base()
        {
        }

        public themonospotParserException(string message)
            : base(message)
        {
        }
	}
	
	
	
	
	
	
	
	
	
	/// <summary>
	/// themonospotParser - Parser Avi File
	/// </summary>
	public class themonospotParser
    {   
		
		
		
		#region CONSTANTS

        public const int DWORDSIZE = 4;
        public const int TWODWORDSSIZE = 8;
        public static readonly string RIFF4CC = "RIFF";
        public static readonly string RIFX4CC = "RIFX";
        public static readonly string LIST4CC = "LIST";

		// Known file types
		public static readonly int ckidAVI = Utility.clsEncoding.ToFourCC("AVI ");
		public static readonly int ckidWAV = Utility.clsEncoding.ToFourCC("WAVE");
		public static readonly int ckidRMID = Utility.clsEncoding.ToFourCC("RMID");

        #endregion

		
		
		
		
		// Generic Object
		private AVIMAINHEADER myAviHeader = new AVIMAINHEADER();
		private AVISTREAMHEADER[] myAviStreamHeader = new AVISTREAMHEADER[0];
		private BITMAPINFOHEADER[] myAviBitMap = new BITMAPINFOHEADER[0];
		private WAVEFORMATEX[] myAviWave = new WAVEFORMATEX[0];
		private string[] myUData = new string[0];
		private string[] myMUData = new string[0];
		
		private string _m_filename;
		private string _m_shortname;
		private string _udToChange = "";
		private long _m_fourCC_AviStreamHeader;
		private long _m_fourCC_AviVideoHeader;
		private long _m_filesize;
		private long _m_posStream;
		private long _m_MoviSize=0;
		private long _m_MoviSizeNew=0;
		private long _m_MoviStart=0;
		private long _m_IdxSize=0;
		private long _m_IdxStart=0;		
		
		private FileStream aviStreamReader = null;
		private FileStream outFile = null;
		
		private bool pbDetect = false;
		private bool _rec_ix_presence = false;
		
		private int stdBufferSize = 4096;
		private int newTotalBytes = 0;
		
		// FileStream inFile = null;
		
		
		long[] framesOffset = new long[0];
		long[] framesSize = new long[0];
		
		public string udToChange
		{	
			get	{	return _udToChange;}
			set	{	_udToChange = value ;}
		}
		
		public bool rec_ix_presence
		{	get	{	return _rec_ix_presence;}	}

		public long fourCC_AVISTREAMHEADER_offset
		{	get	{	return _m_fourCC_AviStreamHeader;}	}

		public long fourCC_AVIVIDEOHEADER_offset
		{	get	{	return _m_fourCC_AviVideoHeader;}	}

		public AVIMAINHEADER headerFile
		{	get	{	return myAviHeader;}	}
		
		public BITMAPINFOHEADER[] videoStreams
		{	get	{	return myAviBitMap;}	}

		public WAVEFORMATEX[] audioStreams
		{	get	{	return myAviWave;}	}		
		
		public AVISTREAMHEADER[] headerStreams
		{	get {	return myAviStreamHeader;}	}

		public string[] userData
		{	get	{	return myUData;}	}		

		public string[] MOVIuserData
		{	get	{	return myMUData;}	}		

		public string m_filename
		{	get {	return _m_filename; }		}
		
		public string m_shortname
		{	get {	return _m_shortname; }		}
		
		public long m_filesize
		{	get {	return _m_filesize; }		}
		
		public long m_MoviSize
		{	
			get {	return _m_MoviSize; }		
			set {	_m_MoviSize = value; }		
		}
				
		public long m_MoviStart
		{	
			get {	return _m_MoviStart; }
			set {	_m_MoviStart = value; }		
		}

		public long m_IdxSize
		{	
			get {	return _m_IdxSize; }		
			set {	_m_IdxSize = value; }		
		}
				
		public long m_IdxStart
		{	
			get {	return _m_IdxStart; }
			set {	_m_IdxStart = value; }
		}
		
		#region Methods to add items at structures
		
		private void addNew_AVISTREAMHEADER()
		{
			int j;
			
			if (myAviStreamHeader == null)
			{
				// There isn't any elements				
				myAviStreamHeader = new AVISTREAMHEADER[1];
			}
			else
			{
				// There is another element
				AVISTREAMHEADER[] tmpASR;
				tmpASR = new AVISTREAMHEADER[myAviStreamHeader.Length + 1];
				
				for (j=0; j<myAviStreamHeader.Length; j++)
				{	tmpASR[j] = myAviStreamHeader[j];	}
				
				myAviStreamHeader = tmpASR;
			}
			
			return;
		}
		
		private void addNew_BITMAPINFOHEADER()
		{
			int j;
			
			if (myAviBitMap == null)
			{
				// There isn't any elements				
				myAviBitMap = new BITMAPINFOHEADER[1];
			}
			else
			{
				// There is another element	
				BITMAPINFOHEADER[] tmpASR;
				tmpASR = new BITMAPINFOHEADER[myAviBitMap.Length + 1];
				
				for (j=0; j<myAviBitMap.Length; j++)
				{	tmpASR[j] = myAviBitMap[j];	}
				
				myAviBitMap = tmpASR;
			}
			
			return;
		}

		private void addNew_WAVEFORMATEX()
		{
			int j;
			
			if (myAviWave == null)
			{
				// There isn't any elements				
				myAviWave = new WAVEFORMATEX[1];
			}
			else
			{
				// There is another element
				WAVEFORMATEX[] tmpASR;
				tmpASR = new WAVEFORMATEX[myAviWave.Length + 1];
				
				for (j=0; j<myAviWave.Length; j++)
				{	tmpASR[j] = myAviWave[j];	}
				
				myAviWave = tmpASR;
			}
			
			return;
		}
		
		private void addNew_STRING()
		{
			int j;
			if (myUData == null)
			{
				// There isn't any elements				
				myUData = new string[1];
			}
			else
			{
				// There is another element
				string[] tmpASR;
				tmpASR = new string[myUData.Length + 1];
				
				for (j=0; j<myUData.Length; j++)
				{	tmpASR[j] = myUData[j];	}
				
				myUData = tmpASR;
			}
			
			return;			
		}
		
		private void addNew_moviSTRING()
		{
			int j;
			if (myMUData == null)
			{
				// There isn't any elements				
				myMUData = new string[1];
			}
			else
			{
				// There is another element
				string[] tmpASR;
				tmpASR = new string[myMUData.Length + 1];
				
				for (j=0; j<myMUData.Length; j++)
				{	tmpASR[j] = myMUData[j];	}
				
				myMUData = tmpASR;
			}
			
			return;			
		}

		#endregion
		
		
		
		public themonospotParser()
		{
		}
		
		/// <summary>
		/// Parse the selected file 
		/// </summary>
		public void OpenAviFile(string FileName)
		{	
						
			// File Not Found...
			if (File.Exists(FileName) != true)
				throw new themonospotParserException("File (" + FileName + ") Not Found...");				
			
			// Read File Infos
			FileInfo fi = new FileInfo(FileName);
			_m_filename = fi.FullName;
			_m_shortname = fi.Name;
			_m_filesize = fi.Length;
			
			// DEBUG
			Console.WriteLine(""); Console.WriteLine(""); Console.WriteLine("");
			Console.WriteLine("_m_filename  = " + _m_filename);
			Console.WriteLine("_m_shortname = " + _m_shortname);
			Console.WriteLine("_m_filesize  = " + _m_filesize.ToString("#,###.##"));
			
			// Open the streamer
			aviStreamReader = new FileStream(_m_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			_m_posStream = 0;
			
			#region Verify Integrity 
			
			// Detect File Format
			int FourCC = readDWord();
			_m_posStream +=4;
			string sFourCC;
			string hexFourCC;
			int dataSize = readDWord();
			int fileType = readDWord();
			
			
			_m_posStream +=8;
			
			// Check FourCC Header			
			sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
			Console.WriteLine("FourCC ".PadRight(20,(char)46) + sFourCC + " (" + dataSize.ToString("#,###.##") + ")");
			
			
			if (sFourCC != RIFF4CC && 
			    sFourCC != RIFX4CC)
			{
				// No Riff File
				aviStreamReader.Close(); aviStreamReader = null;
				throw new themonospotParserException("Error. Not a valid RIFF file");
			}
			
			// Check File Size
			if (_m_filesize < (dataSize + 8) )
			{
				// No Riff File
				aviStreamReader.Close(); aviStreamReader = null;
				throw new themonospotParserException("Error. Truncated file");
			}
			
			#endregion

			
			byte[] tmpByteArray;			
			int byteToRead;
			int readBytes;
			long byteOfList;
			int byteOfINFO=0;
			int byteOfINFOReaded=0;
			string strType = "";
			
			_m_fourCC_AviVideoHeader = 0;
			_m_fourCC_AviStreamHeader = 0;
			
			// Loop until EOF
			while (_m_posStream < _m_filesize )
			{
				FourCC = readDWord();				
				sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
				hexFourCC = FourCC.ToString("X8");
				byteToRead = readDWord();
				
				// Adjust bytes to read (no odd)
				// TO VERIFY *********************************** 
				if (byteToRead % 2 != 0)
					byteToRead++;
					
				_m_posStream +=8;
				
				Console.WriteLine("FourCC ".PadRight(20,(char)46) + sFourCC + " (" + byteToRead.ToString("#,###.##") + ") [" + hexFourCC + "]" + "    filepos=" + _m_posStream.ToString("#,###.00") );
				
				
				// Check REC? or IX?? FourCC presence
				if (sFourCC.ToLower().Substring(3) == "rec" ||
				    sFourCC.ToLower().Substring(2) == "ix" )
				{
					_rec_ix_presence = true;
				}
				
				// Check memory
				if (_m_posStream + byteToRead > _m_filesize)
				{
					aviStreamReader.Close(); 
					aviStreamReader = null;
					return;
				}
				
				// Parse FourCC
				if (sFourCC == LIST4CC)
				{
					// LIST section 
					FourCC = readDWord();
					_m_posStream += 4;
					sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
					hexFourCC = FourCC.ToString("X8");
					byteOfList = byteToRead;
					Console.WriteLine("LIST type ".PadRight(20,(char)46) + sFourCC + "[" + hexFourCC + "]"+ "    filepos=" + _m_posStream.ToString("#,###.00") );
					
					
					// Verify FourCC
					if (FourCC == AviRiffData.ckidAVIMovieData)
					{
						// skip "movi" section 6D 6F 76 69
						_m_MoviStart = aviStreamReader.Position;
						_m_posStream += byteOfList-4;
						// aviStreamReader.Seek(byteOfList-4, SeekOrigin.Current);
						parseMoviChunk(byteOfList-4);
						_m_MoviSize = byteOfList-4;						
						 
					}
					else if (FourCC == AviRiffData.ckidINFOList )
					{
						// INFO section
						byteOfINFOReaded=0;
						byteOfINFO=(int)(byteOfList-4);
					}

					
				}
				else
				{
					// Other TAGS
					if(FourCC == AviRiffData.ckidMainAVIHeader)
					{
						// "avih"
						tmpByteArray = new byte[56];
						readBytes = aviStreamReader.Read(tmpByteArray,0,56);
						_m_posStream +=readBytes;
						myAviHeader.loadDataStructure(tmpByteArray);
					
					}
					else if(FourCC == AviRiffData.ckidAVIStreamHeader)
					{
						// "strh"
						tmpByteArray = new byte[56];
						readBytes = aviStreamReader.Read(tmpByteArray,0,56);						
						_m_posStream +=56;
						
						// Update Array of Stream Headers
						addNew_AVISTREAMHEADER();
						AVISTREAMHEADER tmpSH = new AVISTREAMHEADER();
						tmpSH.loadDataStructure(tmpByteArray);						
						myAviStreamHeader[ myAviStreamHeader.Length-1 ] = tmpSH;
						strType = clsEncoding.FromFourCC(tmpSH.fccType);
						Console.WriteLine("STREAM TYPE ".PadRight(20,(char)46) + clsEncoding.FromFourCC(tmpSH.fccType) );
						Console.WriteLine("STREAM HEARER ".PadRight(20,(char)46) + clsEncoding.FromFourCC(tmpSH.fccHandler) );
						
						// fourCC_AviStreamHeader position
						if (_m_fourCC_AviVideoHeader == 0)
							_m_fourCC_AviStreamHeader = _m_posStream - 52;
						
					}					
					else if(FourCC == AviRiffData.ckidStreamFormat)
					{
						// "strf"
						if (strType == "vids")
						{
							tmpByteArray = new byte[40];
							readBytes = aviStreamReader.Read(tmpByteArray,0,40);
							_m_posStream +=readBytes;
							
							// fourCC_AviVideoHeader position
							_m_fourCC_AviVideoHeader = _m_posStream - 24;
						
							// Update Array of Stream Format Video
							addNew_BITMAPINFOHEADER();
							BITMAPINFOHEADER tmpBMP = new BITMAPINFOHEADER();
							tmpBMP.loadDataStructure(tmpByteArray);
							myAviBitMap[ myAviBitMap.Length-1 ] = tmpBMP;							 
							
						}
						else if (strType == "auds")
						{
							tmpByteArray = new byte[byteToRead];
							readBytes = aviStreamReader.Read(tmpByteArray,0,byteToRead);
							_m_posStream +=readBytes;
							
							// Update Array of Stream Format Video
							addNew_WAVEFORMATEX();
							WAVEFORMATEX tmpWFR = new WAVEFORMATEX();
							tmpWFR.loadDataStructure(tmpByteArray);						
							myAviWave[ myAviWave.Length-1 ] = tmpWFR;							
							string strAudioType = tmpWFR.wFormatTag.ToString("X4");

						}
						else						
						{
							// other FourCC in a stream list
							aviStreamReader.Seek(byteToRead, SeekOrigin.Current);
							_m_posStream +=byteToRead;
						}
						
					}					
					else if(FourCC == AviRiffData.ckidAVIOldIndex)
					{
						// "idx1"
						// parseIdxChunk(byteToRead);
						_m_IdxStart = aviStreamReader.Position;
						_m_IdxSize = byteToRead;
						aviStreamReader.Seek(byteToRead, SeekOrigin.Current);
						_m_posStream +=byteToRead;
					}
					else if(FourCC == AviRiffData.ckidJUNKTag)
					{
						// "JUNK"
						// Skip 
						tmpByteArray = new byte[byteToRead];
						readBytes = aviStreamReader.Read(tmpByteArray,0,byteToRead);						
						_m_posStream +=readBytes;
						clsEncoding myEnc = new clsEncoding();
						string theStrData = myEnc.getAsciiFromArray(tmpByteArray);
						if (theStrData.Trim() != "")
						{	
							addNew_STRING();
							myUData[myUData.Length-1] = theStrData;
							Console.WriteLine("JUNKDATA ".PadRight(20,(char)46) + myEnc.getAsciiFromArray(tmpByteArray));							
						}
						// aviStreamReader.Seek(byteToRead, SeekOrigin.Current);
					}
					else if(FourCC == AviRiffData.ckidAVIISFT)
					{
						// "ISFT"
						tmpByteArray = new byte[byteToRead];
						readBytes = aviStreamReader.Read(tmpByteArray,0,byteToRead);
						_m_posStream +=readBytes;
						byteOfINFOReaded += byteToRead+8;
						addNew_STRING();				
						clsEncoding myEnc = new clsEncoding();
						myUData[myUData.Length-1] = myEnc.getAsciiFromArray(tmpByteArray);
						Console.WriteLine("ISFTDATA ".PadRight(20,(char)46) + myEnc.getAsciiFromArray(tmpByteArray));
						
						// Check remaining byte 
						if ((byteOfINFO - byteOfINFOReaded) < 8 )
							aviStreamReader.Seek((byteOfINFO - byteOfINFOReaded), SeekOrigin.Current);
							_m_posStream +=(byteOfINFO - byteOfINFOReaded);
					}
					else
					{
						// other FourCC
						aviStreamReader.Seek(byteToRead, SeekOrigin.Current);
						_m_posStream +=byteToRead;
					}
					
				}
				

			}	// end while (_m_posStream < dataSize)

			
			// Close the streamer
			aviStreamReader.Close(); aviStreamReader = null;
			
			return;
		}
		
		
		// Parse MOVI Chunk to extract xxdc or xxdb subarea
		private void parseMoviChunk(long MoviChunkSize)
		{
			long tmpMoviPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;
			
			pbDetect=false;
			
			while (tmpMoviPointer < MoviChunkSize)
			{
				FourCC = readDWord();
				sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
				byteOfRead = readDWord();
				
				// Adjust bytes to read (no odd)
				if ((byteOfRead % 2) != 0 )
					byteOfRead++;				
				
				tmpMoviPointer += 8;
				
				// Console.WriteLine(sFourCC + " Founded, size = " + byteOfRead.ToString());
					
				if (sFourCC.Substring(2,2) == "dc" || sFourCC.Substring(2,2) == "db")
	    		{
					
					frameCount ++;
					
	    			if (frameCount == 1)
	    				parseDCuserdata(byteOfRead);
	    			else
	    				parseDCvopdata(byteOfRead);	    			
	    			
	    			tmpMoviPointer += byteOfRead;
	    			
		    		// scan only first 100 xxdc or xxdb frames in MOVI chunk
		    		
		    		if (frameCount >= 200)
	    			{
	    				aviStreamReader.Seek(MoviChunkSize - tmpMoviPointer, SeekOrigin.Current);
	    				tmpMoviPointer += (MoviChunkSize - tmpMoviPointer);
	    			}				

				}
	    		else
	    		{
	    			//Console.WriteLine(sFourCC + " Founded, size = " + byteOfRead.ToString());
	    			aviStreamReader.Seek(byteOfRead, SeekOrigin.Current);
	    			tmpMoviPointer += byteOfRead;
	    		}

	    		
			}
			
		}
		
		// Extract UserData info from DC subarea
		private void parseDCuserdata(long DCsubareaSize)
		{
			//aviStreamReader.Seek(DCsubareaSize, SeekOrigin.Current);
			
			clsEncoding myEnc = new clsEncoding();
			string outValue="";
			int sPoint = 0; 
			int ePoint = 0;
			
			byte[] dcBuffer = new byte[(int)DCsubareaSize];
			aviStreamReader.Read(dcBuffer,0,(int)DCsubareaSize);
			
			// Find UserData ... START			
			sPoint =  myEnc.compareBytesArray(dcBuffer, AviRiffData.UserDataBytes, 0);
			
			while (sPoint < DCsubareaSize && sPoint >= 0)
			{
				
				ePoint = myEnc.compareBytesArray(dcBuffer, AviRiffData.UserDataBytes, sPoint + 3);
				
				if (ePoint < 0)
					ePoint = myEnc.compareBytesArray(dcBuffer, AviRiffData.VOLStartBytes , sPoint + 3);
				
				if (ePoint < 0)
					ePoint = myEnc.compareBytesArray(dcBuffer, AviRiffData.VOPStartBytes , sPoint + 3);
				
				if (ePoint < 0)
				{
					// from sPoint to end of Byte Array
					outValue = myEnc.getHexFromBytes(dcBuffer,sPoint+4, ((int)DCsubareaSize - (sPoint+3)));
					addNew_moviSTRING();
					myMUData[myMUData.Length-1] = myEnc.getAsciiFromHex(outValue);
					Console.WriteLine("UD founded".PadRight(20,(char)46) + myEnc.getAsciiFromHex(outValue));
					break;
				}
				else
				{
					// from sPoint to ePoint
					outValue = myEnc.getHexFromBytes(dcBuffer,sPoint+4, (ePoint - (sPoint+4)));
					addNew_moviSTRING();
					myMUData[myMUData.Length-1] = myEnc.getAsciiFromHex(outValue);
					Console.WriteLine("UD founded".PadRight(20,(char)46) + myEnc.getAsciiFromHex(outValue));
					sPoint = myEnc.compareBytesArray(dcBuffer, AviRiffData.UserDataBytes, ePoint);
				}
			}
			
			// Find UserData ... END
			
			dcBuffer = null;
			
		}
		
		

		// Extract VOP count in to DC subarea (Packet Bitstream Detect)
		private void parseDCvopdata(long DCsubareaSize)
		{
			//aviStreamReader.Seek(DCsubareaSize, SeekOrigin.Current);
			
			clsEncoding myEnc = new clsEncoding();
			//string outValue="";
			int sPoint = 0; 
			//int ePoint = 0;
			int vopCount = 0;
			
			byte[] dcBuffer = new byte[(int)DCsubareaSize];
			aviStreamReader.Read(dcBuffer,0,(int)DCsubareaSize);
			
			// Find Packed Bitstream ... START
			sPoint =  myEnc.compareBytesArray(dcBuffer, AviRiffData.VOPStartBytes, 0);
			
			while (sPoint < (DCsubareaSize-2) && sPoint >= 0)
			{
				vopCount++;
				sPoint =  myEnc.compareBytesArray(dcBuffer, AviRiffData.VOPStartBytes, sPoint+3);
			}
			
			// Find Packed Bitstream ... END
			
			// Debug Packet Bitstream Detect
			// Console.WriteLine("VOP detect".PadRight(20,(char)46) + vopCount.ToString());
			if (vopCount > 1)
				pbDetect = true;
			
			dcBuffer = null;
			
		}
		
		
		
		// Parse IDX_ Chunk to extract index
		private void parseIdxChunk(long IdxChunkSize)
		{
			long tmpIdxPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;
			
			while (tmpIdxPointer < IdxChunkSize)
			{
				FourCC = readDWord();
				sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
				
				// Adjust bytes to read (no odd)
				if ((byteOfRead % 2) != 0 )
					byteOfRead++;
				
				tmpIdxPointer += 4;
				frameCount ++;
				// Console.WriteLine(sFourCC + " Founded, size = " + (byteOfRead).ToString());
				aviStreamReader.Seek(12, SeekOrigin.Current);
    			tmpIdxPointer += 12;				
			}
			
			Console.WriteLine("Total Frame Counted: " + frameCount.ToString() );
		}
		
		
		
		
		
		
		
		/// <summary>
		/// Detect audio codec
		/// </summary>
		/// <param name="audioVal">codec id</param>
		/// <returns>codec string name</returns>
		public string parseAudioType(string audioVal)
	    {
	    	// return name of codec audio
	    	
			if (audioVal == "0055")	return "0x" + audioVal + " (MP3)";
	    	else if (audioVal == "0001")	return "0x" + audioVal + " (PCM)";
			else if (audioVal == "2001")	return "0x" + audioVal + " (DTS)";
			else if (audioVal == "000A")	return "0x" + audioVal + " (WMA9)";
			else if (audioVal == "0030")	return "0x" + audioVal + " (Dolby AC2)";
			else if (audioVal == "0050")	return "0x" + audioVal + " (MPEG)";
			else if (audioVal == "2000")	return "0x" + audioVal + " (AC3)";
			else
				return "0x" + audioVal + " (?)";
	    }
		
		
		
		
		
		public void saveNewAvi(string newFileName, 
		                   ref bool _redrawInfo, 
			               ref string _saveError, 
			               ref double _saveStatus,
			               ref bool _saveFlag, 
			               ref double _totProgressItems, 
			               ref string _saveInfo )
		{
		
			
			// Begin Write new avi file
			aviStreamReader = new FileStream(_m_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			outFile = new FileStream(newFileName, FileMode.Create,FileAccess.Write, FileShare.None);
			
			
			FileInfo fi = new FileInfo(_m_filename);
			long filmsize = fi.Length;
			long filmoffset = 0;
			long diffBytes = 0;
			byte[] tmpBytes;
			int totMoviItems = Convert.ToInt32(_m_IdxSize / 16) + 3000;
			
			framesOffset = new long[ totMoviItems ];
			framesSize = new long[ totMoviItems ];
			int bufferSize = stdBufferSize;
			
			// Write data before MOVI chunk			
			_totProgressItems = (double)_m_MoviStart;
			
			Console.WriteLine("Write Header START");			
			_saveInfo = clsLanguages.EXPMESSAGE1;
			_redrawInfo=true;
			
			Console.WriteLine("filmoffset = " + filmoffset.ToString() + "  - _m_MoviStart = " + _m_MoviStart.ToString() );
			
			while (filmoffset < _m_MoviStart)
			{
				if ((filmoffset + bufferSize) > _m_MoviStart)
					bufferSize = (int)(_m_MoviStart - filmoffset);
				
				tmpBytes = new byte[bufferSize];
				aviStreamReader.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				_saveStatus = (double)filmoffset;
				// myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				
				if (_saveFlag == false)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					aviStreamReader.Close();
					aviStreamReader.Dispose();
					aviStreamReader = null;
					
					return;
				}
				
				filmoffset += bufferSize;				
			}
			Console.WriteLine("Write Header END");
			
			
			// Write new MOVI chunk from old
			if (writeMoviChunk(ref _redrawInfo, ref _saveError, ref _saveStatus, ref _saveFlag, ref _totProgressItems, ref _saveInfo) != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					aviStreamReader.Close();
					aviStreamReader.Dispose();
					aviStreamReader = null;
					
					return;
			}
			
			
			
			// Write data before IDX1 chunk
			diffBytes = (_m_IdxStart - aviStreamReader.Position);
			bufferSize = stdBufferSize;
			
			_saveStatus = 0;
			_totProgressItems = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT_1 START");
			_saveInfo = clsLanguages.EXPMESSAGE2;
			_redrawInfo = true;
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				aviStreamReader.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				_saveStatus = (double)filmoffset;
				// myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				
				if (_saveFlag == false)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					aviStreamReader.Close();
					aviStreamReader.Dispose();
					aviStreamReader = null;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 1 END");
			
			
			
			// Write new IDX1 chunk from new MOVI created
			if (writeIdx1Chunk(ref _redrawInfo, ref _saveError, ref _saveStatus, ref _saveFlag, ref _totProgressItems, ref _saveInfo) != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					aviStreamReader.Close();
					aviStreamReader.Dispose();
					aviStreamReader = null;
					return;
			}
			
			
			// Write data after IDX1 chunk			
			diffBytes = (filmsize - aviStreamReader.Position);
			bufferSize = stdBufferSize;
			
			_saveStatus = 0;			
			_totProgressItems = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT 2 START");
			_saveInfo = clsLanguages.EXPMESSAGE2;
			_redrawInfo = true;
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				aviStreamReader.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				_saveStatus = (double)filmoffset;
				// myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				
				if (_saveFlag == false)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					aviStreamReader.Close();
					aviStreamReader.Dispose();
					aviStreamReader = null;					
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 2 END");
			
			// Close output
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			
			// Read File Infos
			fi = new FileInfo(newFileName);
			newTotalBytes = (int)(fi.Length - 8);
			fi = null;
			
			// Update MOVI Size and File Size
			outFile = new FileStream(newFileName, FileMode.Open,FileAccess.Write, FileShare.None);
			outFile.Seek(4,SeekOrigin.Begin);
			outFile.Write(intToByteArray(newTotalBytes),0,4);
			outFile.Seek(_m_MoviStart - 8,SeekOrigin.Begin);
			outFile.Write(intToByteArray((int)(_m_MoviSizeNew + 4)),0,4);
						
			// Close all streams and return to Main Window
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			aviStreamReader.Close();
			aviStreamReader.Dispose();
			aviStreamReader = null;
			
			_saveFlag=false;
		}
		
		
		
		
		// write MOVI Chunk frames
		private int writeMoviChunk(ref bool _redrawInfo, 
			               		   ref string _saveError, 
			                       ref double _saveStatus,
			                       ref bool _saveFlag, 
			                       ref double _totProgressItems,
			                       ref string _saveInfo  )
		{
			long tmpMoviPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			int sizeOfFrame = 0;
			int newByteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;		
			int lenOfFrame = 0;
			string hexFourCC = "";
			byte[] tmpByteArray = new byte[0];
			int stepGuiUpdate = 1024;
			int stepFrame = 0;

			// Write data before MOVI chunk
			_saveStatus = 0;			
			_totProgressItems = (double)_m_MoviSize;
			_saveInfo = clsLanguages.EXPMESSAGE3;
			_redrawInfo = true;
			
			Console.WriteLine("Write MOVI START (" + _m_MoviStart.ToString() + " SIZE " + _m_MoviSize.ToString() + ")");
			_m_MoviSizeNew = 0;
			
			while (tmpMoviPointer < _m_MoviSize)
			{
				// Exit if Cancel button was pressed
				if (_saveFlag == false)
					return 1;
				
				FourCC = readDWord();
				hexFourCC = FourCC.ToString("X8");
				sFourCC = Utility.clsEncoding.FromFourCC(FourCC);
				byteOfRead = readDWord();
				
				tmpMoviPointer += 8;
				_m_MoviSizeNew += 8;
				
				// Adjust bytes to read (no odd)
				sizeOfFrame = byteOfRead; 
				if ((byteOfRead % 2) != 0 )
					byteOfRead++;				
				
				tmpByteArray = new byte[byteOfRead];
				aviStreamReader.Read(tmpByteArray, 0, byteOfRead);
				
				stepFrame ++;
				
				// Verify frame type
				if (sFourCC.Substring(2,2) == "dc" || sFourCC.Substring(2,2) == "db")
	    		{
	    			// 
	    			tmpByteArray = processFrame(tmpByteArray, ref sizeOfFrame);
	    			newByteOfRead = tmpByteArray.Length;
	    			
	    			framesOffset[frameCount] = outFile.Position;
	    			framesSize[frameCount] = (long)newByteOfRead;
	    			outFile.Write(intToByteArray(FourCC),0, 4);
	    			outFile.Write(intToByteArray(sizeOfFrame),0, 4);
	    			outFile.Write(tmpByteArray, 0, newByteOfRead);
	    			
	    			tmpMoviPointer += byteOfRead;
	    			_m_MoviSizeNew += newByteOfRead;
					
				}
	    		else
	    		{
	    			framesOffset[frameCount] = outFile.Position;	    			
	    			framesSize[frameCount] = (long)byteOfRead;
	    			outFile.Write(intToByteArray(FourCC),0, 4);
	    			outFile.Write(intToByteArray(sizeOfFrame),0, 4);
	    			outFile.Write(tmpByteArray, 0, byteOfRead);
	    			
	    			tmpMoviPointer += byteOfRead;
	    			_m_MoviSizeNew += byteOfRead;
	    		}

	    		frameCount ++;
	    		
	    		if (stepFrame >= stepGuiUpdate)
	    		{
		    		// Update progressbar
		    		_saveStatus = (double)tmpMoviPointer;
					// myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
					
					stepFrame = 0;
				}
	    		
	    		
			}
			Console.WriteLine("Tot Frames: " + frameCount.ToString());
			Console.WriteLine("Write MOVI END");
			
			return 0;
			
		}
		
		// Extract UserData info from DC subarea
		private byte[] processFrame(byte[] inByteArray, ref int frameLength)
		{
			clsEncoding myEnc = new clsEncoding();			
			byte[] tmpByteArray = null;
			byte[] userdataOld = null;
			byte[] outByteArray = null;
			ASCIIEncoding TextEncoding = new ASCIIEncoding();
			
			tmpByteArray = inByteArray;
			userdataOld = TextEncoding.GetBytes(_udToChange);

			int startPos = myEnc.compareBytesArray(tmpByteArray, userdataOld,0);
			
			// int totalFrameBytes = 0;
			int totalFrameBytes, newFrameLength;
			
			if (startPos >= 0)
			{
				// totalFrameBytes = tmpByteArray.Length - _udToChange.Length + 12;
				newFrameLength = frameLength - _udToChange.Length + 12;
				
				// Padded to an even number of bytes but make sure the padding isn't included
                // in the size written to the chunk header or index
				totalFrameBytes = newFrameLength;
				
				if ((totalFrameBytes % 2) != 0)
					totalFrameBytes ++;
				
				// frameLength = totalFrameBytes;
				frameLength = newFrameLength;
				
				outByteArray = new byte[totalFrameBytes];
				Array.Copy(tmpByteArray, outByteArray, startPos);
				Array.Copy(TextEncoding.GetBytes("DivX999b000p"), 0, outByteArray, startPos, 12);
				Array.Copy(tmpByteArray, startPos + _udToChange.Length , outByteArray, startPos + 12, frameLength - _udToChange.Length - startPos + 1);
				
			}
			else
				outByteArray = tmpByteArray;
			
			
			
			return outByteArray;			
		}
		
		
		// Write new Idx1 Chunk
		private int writeIdx1Chunk(ref bool _redrawInfo, 
			                       ref string _saveError, 
			                       ref double _saveStatus,
			                       ref bool _saveFlag, 
			                       ref double _totProgressItems, 
			                       ref string _saveInfo )
		{
			long tmpIdxPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;
			string hexFourCC = "";
			byte[] tmpByteArray = new byte[16];
			byte[] tmpDWordArray = new byte[4];
			int stepGuiUpdate = 256;
			int stepFrame = 0;
			
			// Write data before MOVI chunk
			_saveStatus = 0;			
			_totProgressItems = (double)_m_IdxSize;
			_saveInfo = clsLanguages.EXPMESSAGE4;
			_redrawInfo=true;
			Console.WriteLine("Write IDX START (" + _m_IdxStart.ToString() + " SIZE " + _m_IdxSize.ToString() + ")");
			
			while (tmpIdxPointer < _m_IdxSize)
			{
				// Exit if Cancel button was pressed
				if (_saveFlag == false)
					return 1;
				
				aviStreamReader.Read(tmpByteArray,0,16);
				
				// Offsets are relative to the start of the 'movi' list type
				tmpDWordArray = intToByteArray((int)(framesOffset[frameCount] - (_m_MoviStart - 4)));
				for (int j=0; j<4; j++)
					tmpByteArray[8+j]=tmpDWordArray[j];

				tmpDWordArray = intToByteArray((int)framesSize[frameCount]);
				for (int j=0; j<4; j++)
					tmpByteArray[12+j]=tmpDWordArray[j];
				
				outFile.Write(tmpByteArray,0,16);
				
				tmpIdxPointer += 16;
				
				frameCount ++;
				stepFrame++;
	    		
	    		if (stepFrame >= stepGuiUpdate)
	    		{
		    		// Update progressbar
		    		_saveStatus = (double)tmpIdxPointer;
					// myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				
					stepFrame = 0;
				}
				
			}
			
			
			Console.WriteLine("Tot Frames: " + frameCount.ToString());
			Console.WriteLine("Write IDX END");
			
			return 0;
		}
		
		
		
		
		/// <summary>
		/// Read 2 bytes and return the value
		/// </summary>
		/// <returns></returns>
		private int readWord()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[2];
			readBytes = aviStreamReader.Read(tmpBuffer,0,2);
			retValue = (tmpBuffer[0]) + (tmpBuffer[1]<<8);
			return retValue;
		}
		
		// returns a byte array of length 4
		private byte[] intToByteArray(int i) 
		{
			byte[] dword = new byte[4];
			dword[0] = (byte) (i & 0x00FF);
			dword[1] = (byte) ((i >> 8) & 0x000000FF);
			dword[2] = (byte) ((i >> 16) & 0x000000FF);
			dword[3] = (byte) ((i >> 24) & 0x000000FF);
			return dword;
		}


		/// <summary>
		/// Read 4 bytes and return the value
		/// </summary>
		/// <returns></returns>
		private int readDWord()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[4];
			readBytes = aviStreamReader.Read(tmpBuffer,0,4);
			retValue = tmpBuffer[0]+(tmpBuffer[1]<<8)+(tmpBuffer[2]<<16)+(tmpBuffer[3]<<24);
			return retValue;
		}

		
	    public Hashtable GetVideoInformations () {
			int k, l;
			int iV=0;
			int iA=0;
			double sizOfAudio=0;
			double sizOfVideo=0;
			double sizOfHeader=0;
			double videoQuality=0;
			double WdH=0;
			int framePerSec=1;
			int AverageVideoBitRate = 0;
			string fccDesc = "";
			string Frame_Size = "";
			string Total_Time = "";
			string Frame_Rate = "";
			string Total_Frames = "";
			string Video_Data_Rate = "";
			string Packet_Bitstream = "Off";
			Hashtable informations = new Hashtable();
			Console.WriteLine(myAviStreamHeader.Length);
			for (k=0; k < headerStreams.Length; k++ ) {
				Console.WriteLine(k);
				if(clsEncoding.FromFourCC(headerStreams[k].fccType)
				!= "vids" ) 
					continue;
				long totalTime = 0;
				fccDesc = clsEncoding.FromFourCC(headerStreams[k].fccHandler);
				if (headerFile.dwMicroSecPerFrame > 0)
					totalTime =(long)((long)headerFile.dwTotalFrames *
							  (long) headerFile.dwMicroSecPerFrame);
				totalTime = (long)(totalTime / 1000000.0);
				int hours = (int)(totalTime / 3600);
				totalTime -= (long)(hours * 3600);
				int mins = (int)(totalTime / 60);
				totalTime -= (long)(mins * 60);
				framePerSec = headerStreams[k].dwRate / headerStreams[k].dwScale;
				WdH = videoStreams[0].biWidth;
				WdH /= videoStreams[0].biHeight;
				Frame_Size = 
					videoStreams[0].biWidth.ToString()
					+ " x " +
					videoStreams[0].biHeight.ToString();
				Total_Time = String.Format("{0:00}:{1:00}:{2:00.00#} seconds", hours, mins, totalTime);
				Frame_Rate = String.Format("{0:N2} Frames/Sec", (1000000.0 / headerFile.dwMicroSecPerFrame));
				Total_Frames = String.Format("{0:G}", headerFile.dwTotalFrames  );
				Video_Data_Rate = String.Format("{0:N2} frames/Sec", framePerSec );
				iV++;
			}
			sizOfHeader = headerFile.dwTotalFrames * 8 * (iA+1); 
			sizOfVideo = m_MoviSize - sizOfHeader - sizOfAudio;
			AverageVideoBitRate = (int)((sizOfVideo * framePerSec * 8) /  (headerFile.dwTotalFrames * 1000));
			videoQuality = (0.75 * WdH) * (AverageVideoBitRate / framePerSec);
			
			if (pbDetect == true)
				Packet_Bitstream = "On";

			informations.Add("Video codec:", clsEncoding.FromFourCC(videoStreams[0].biCompression));
			informations.Add("Codec descr:", fccDesc);
			informations.Add("Frame Size:", Frame_Size );
			informations.Add("Average Video Bitrate:", AverageVideoBitRate.ToString() + " Kb/Sec");	       
			informations.Add("Avi file size:", ((m_filesize / 1024).ToString("#,### KB")));
			informations.Add("Total Time:", Total_Time );
			informations.Add("Frame Rate:", Frame_Rate) ;
			informations.Add("Total Frames:", Total_Frames);
			informations.Add("Video Data Rate:", Video_Data_Rate );
			informations.Add("Video Quality:", videoQuality.ToString("#,###.##") );
			informations.Add("Packet Bitstream:", Packet_Bitstream );

			if (userData.Length >0 )
				for (l=0; l<userData.Length;l++)
					informations.Add("Info Data[" + l + "]:", userData[l].ToString());

			if (MOVIuserData.Length >0 )
				for (l=0; l<MOVIuserData.Length;l++)
					informations.Add("User Data[" + l + "]:", MOVIuserData[l].ToString());


			return informations;
	    }
	    
	    public Hashtable GetAudioInformations () {
			int k;
			int iA=0;
			double sizOfAudio=0;
			int blockPerSec=0;
			Hashtable informations = new Hashtable();
			
			for (k=0; k < headerStreams.Length; k++ ) {
				if(clsEncoding.FromFourCC(headerStreams[k].fccType)
				== "vids" )
					continue;
				string aFormat = audioStreams[iA].wFormatTag.ToString("X4");
				string CVBR = "";
				double audioRate = (8.0 * audioStreams[iA].nAvgBytesPerSec) ;
				if (headerStreams[k].dwSampleSize > 0 )
					audioRate /= (double) headerStreams[k].dwSampleSize;
				if(aFormat == "0055") {
					CVBR = "";
					// MP3 CODEC
					informations.Add("Audio " + 
							 (iA+1).ToString() + 
							 ":",
							 parseAudioType(aFormat)
							 + " " + CVBR + " " +
							 String.Format("{0:N2} Kb/Sec",
								       audioRate / 1000.0) + " " + 
							 "- " + audioStreams[iA].nSamplesPerSec + " Hz (" +
							 audioStreams[iA].nChannels.ToString() + " Channels)");
				} else {
					// Other codec
					informations.Add("Audio " + (iA+1).ToString() + ":",
							 parseAudioType(aFormat) + " " +
							 String.Format("{0:N2} Kb/Sec", audioRate / 1000.0) + " " + 
							 "- " + audioStreams[iA].nSamplesPerSec + " Hz (" +
							 audioStreams[iA].nChannels.ToString() + " Channels)");
				}
										
				// Calc Data for AVBitrate
				blockPerSec = headerStreams[k].dwRate / headerStreams[k].dwScale;
					
				double tmpAudio = headerStreams[k].dwLength;
				tmpAudio *= audioStreams[iA].nAvgBytesPerSec;
				tmpAudio /= blockPerSec;
				sizOfAudio += tmpAudio;
				
				// increment total audio streams
				iA++;
					
			}
				
			return informations;
		}
        
        
        public void change4CC(string ASHval, string VSHval, long ASHpos, long VSHpos)
        {
            byte[] tmpASHarray = null;
			byte[] tmpVSHarray = null;
						
			FileStream updaterSW = new FileStream(_m_filename , FileMode.Open ,FileAccess.ReadWrite);
			
			updaterSW.Seek(ASHpos, SeekOrigin.Current);
			tmpASHarray = Utility.clsEncoding.ToFourCCByte(ASHval);
			Console.WriteLine(tmpASHarray[0].ToString("X2") + 
			                  tmpASHarray[1].ToString("X2") + 
			                  tmpASHarray[2].ToString("X2") + 
			                  tmpASHarray[3].ToString("X2"));
			updaterSW.Write(tmpASHarray, 0, 4);

			updaterSW.Seek(VSHpos, SeekOrigin.Begin);
			tmpVSHarray = Utility.clsEncoding.ToFourCCByte(VSHval);
			Console.WriteLine(tmpVSHarray[0].ToString("X2") + 
			                  tmpVSHarray[1].ToString("X2") + 
			                  tmpVSHarray[2].ToString("X2") + 
			                  tmpVSHarray[3].ToString("X2"));
			updaterSW.Write(tmpVSHarray, 0, 4);

			updaterSW.Close();
			updaterSW.Dispose();
			updaterSW = null;
            
			return;
        }


		
	}
	
	
	
}
