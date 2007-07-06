using System;
using System.IO;
using System.Text;
using System.Threading;
using Utility;
using System.Collections;


namespace monoSpotMain
{


	public class monoSpotParserException : Exception
	{
        public monoSpotParserException()
        	: base()
        {
        }

        public monoSpotParserException(string message)
            : base(message)
        {
        }
	}
	
	
	
	
	
	
	
	
	
	/// <summary>
	/// MONOSPOTPARSER - Parser Avi File
	/// </summary>
	public class monoSpotParser
    {   
		
		
		
		#region CONSTANTS

        public const int DWORDSIZE = 4;
        public const int TWODWORDSSIZE = 8;
        public static readonly string RIFF4CC = "RIFF";
        public static readonly string RIFX4CC = "RIFX";
        public static readonly string LIST4CC = "LIST";

		// Known file types
		public static readonly int ckidAVI = Utility.cEncoding.ToFourCC("AVI ");
		public static readonly int ckidWAV = Utility.cEncoding.ToFourCC("WAVE");
		public static readonly int ckidRMID = Utility.cEncoding.ToFourCC("RMID");

        #endregion

		
		
		
		
		// Generic Object
		private AVIMAINHEADER myAviHeader = new AVIMAINHEADER();
		private AVISTREAMHEADER[] myAviStreamHeader = new AVISTREAMHEADER[0];
		private BITMAPINFOHEADER[] myAviBitMap = new BITMAPINFOHEADER[0];
		private WAVEFORMATEX[] myAviWave = new WAVEFORMATEX[0];
		private string[] myUData = new string[0];
		
		private string _m_filename;
		private string _m_shortname;
		private long _m_filesize;
		private long _m_posStream;
		private long _m_MoviSize=0;
		private long _m_MoviStart=0;
		private FileStream aviStreamReader;
		
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

		public string m_filename
		{	get {	return _m_filename; }		}
		
		public string m_shortname
		{	get {	return _m_shortname; }		}
		
		public long m_filesize
		{	get {	return _m_filesize; }		}
		
		public long m_MoviSize
		{	get {	return _m_MoviSize; }		}
				
		public long m_MoviStart
		{	get {	return _m_MoviStart; }		}

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
		
		#endregion
		
		
		
		public monoSpotParser()
		{
		}
		
		/// <summary>
		/// Parse the selected file 
		/// </summary>
		public void OpenAviFile(string FileName)
		{	
						
			// File Not Found...
			if (File.Exists(FileName) != true)
				throw new monoSpotParserException("File (" + FileName + ") Not Found...");				
			
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
			sFourCC = Utility.cEncoding.FromFourCC(FourCC);
			Console.WriteLine("FourCC ".PadRight(20,(char)46) + sFourCC + " (" + dataSize.ToString("#,###.##") + ")");
			
			
			if (sFourCC != RIFF4CC && 
			    sFourCC != RIFX4CC)
			{
				// No Riff File
				aviStreamReader.Close(); aviStreamReader = null;
				throw new monoSpotParserException("Error. Not a valid RIFF file");
			}
			
			// Check File Size
			if (_m_filesize < (dataSize + 8) )
			{
				// No Riff File
				aviStreamReader.Close(); aviStreamReader = null;
				throw new monoSpotParserException("Error. Truncated file");
			}
			
			#endregion

			
			byte[] tmpByteArray;			
			int byteToRead;
			int readBytes;
			long byteOfList;
			int byteOfINFO=0;
			int byteOfINFOReaded=0;
			string strType = "";
			
			// Loop until EOF
			while (_m_posStream < _m_filesize )
			{
				FourCC = readDWord();				
				sFourCC = Utility.cEncoding.FromFourCC(FourCC);
				hexFourCC = FourCC.ToString("X8");
				byteToRead = readDWord();
				
				// Adjust bytes to read (no odd)
				if (byteToRead % 2 != 0)
					byteToRead++;
					
				_m_posStream +=8;
				
				Console.WriteLine("FourCC ".PadRight(20,(char)46) + sFourCC + " (" + byteToRead.ToString("#,###.##") + ") [" + hexFourCC + "]" + "    filepos=" + _m_posStream.ToString("#,###.00") );
				
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
					sFourCC = Utility.cEncoding.FromFourCC(FourCC);
					hexFourCC = FourCC.ToString("X8");
					byteOfList = byteToRead;
					Console.WriteLine("LIST type ".PadRight(20,(char)46) + sFourCC + "[" + hexFourCC + "]"+ "    filepos=" + _m_posStream.ToString("#,###.00") );
					
					
					// Verify FourCC
					if (FourCC == AviRiffData.ckidAVIMovieData)
					{
						// skip "movi" section 6D 6F 76 69
						_m_MoviStart =_m_posStream; 
						_m_posStream += byteOfList-4;
						aviStreamReader.Seek(byteOfList-4, SeekOrigin.Current);
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
						strType = cEncoding.FromFourCC(tmpSH.fccType);
						Console.WriteLine("STREAM TYPE ".PadRight(20,(char)46) + cEncoding.FromFourCC(tmpSH.fccType) );
					}					
					else if(FourCC == AviRiffData.ckidStreamFormat)
					{
						// "strf"
						if (strType == "vids")
						{
							tmpByteArray = new byte[40];
							readBytes = aviStreamReader.Read(tmpByteArray,0,40);
							_m_posStream +=readBytes;
							
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
						cEncoding myEnc = new cEncoding();
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
						cEncoding myEnc = new cEncoding();
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
			string Frame_Size = "";
			string Total_Time = "";
			string Frame_Rate = "";
			string Total_Frames = "";
			string Video_Data_Rate = "";
			Hashtable informations = new Hashtable();
			Console.WriteLine(myAviStreamHeader.Length);
			for (k=0; k < headerStreams.Length; k++ ) {
				Console.WriteLine(k);
				if(cEncoding.FromFourCC(headerStreams[k].fccType)
				!= "vids" ) 
					continue;
				long totalTime = 0;
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
			
			informations.Add("Video:", cEncoding.FromFourCC(videoStreams[0].biCompression));
			informations.Add("Frame Size:", Frame_Size );
			informations.Add("Average Video Bitrate:", AverageVideoBitRate.ToString() + " Kb/Sec");	       
			informations.Add("Avi file size:", ((m_filesize / 1024).ToString("#,### KB")));
			informations.Add("Total Time:", Total_Time );
			informations.Add("Frame Rate:", Frame_Rate) ;
			informations.Add("Total Frames:", Total_Frames);
			informations.Add("Video Data Rate:", Video_Data_Rate );
			informations.Add("Video Quality:", videoQuality.ToString("#,###.##") );
			
			if (userData.Length >0 )
				for (l=0; l<userData.Length;l++)
					informations.Add("Info Data[" + l + "]:", userData[l].ToString());

			return informations;
	    }
	    
	    public Hashtable GetAudioInformations () {
			int k;
			int iA=0;
			double sizOfAudio=0;
			int blockPerSec=0;
			Hashtable informations = new Hashtable();
			
			for (k=0; k < headerStreams.Length; k++ ) {
				if(cEncoding.FromFourCC(headerStreams[k].fccType)
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



		
	}
	
	
	
}
