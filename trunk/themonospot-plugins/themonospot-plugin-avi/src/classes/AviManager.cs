
using System;
using System.IO;
using System.Collections.Generic;


namespace ThemonospotPluginAvi
{
	
	/// <summary>
	/// Class used by plugin to manage avi file 
	/// </summary>
	public partial class AviManager
	{
		

		
		private FileStream ifs;
		private Utility utils = new Utility();
		// private bool recIX = false;
		private long streamPosition = 0;
		
		
		
		#region Properties
		
		
		private string _aviFilePath = "";

		/// <value>
		/// Path of avi file to manage
		/// </value>
		public string AviFilePath 
		{
			get {	return _aviFilePath;	}
		}		
		
		
		
		
		
		
		
		private string _aviFileName = "";

		/// <value>
		/// Name of avi file to manage
		/// </value>
		public string AviFileName
		{
			get {	return _aviFileName;	}
		}		

		
		
		
		

		private long _aviFileSize = 0;

		/// <value>
		/// Size of avi file to manage
		/// </value>
		public long AviFileSize
		{
			get {	return _aviFileSize;	}
		}		

		
		
		
		
		private AviContainerEntity _aviContainer = new AviContainerEntity();
		
		/// <value>
		/// Return AviContainer class filled with all info
		/// </value>
		public AviContainerEntity AviContainer
		{
			get {	return _aviContainer;}
		}
		
		
		
		
		#endregion Properties
		
		
		
		#region Constructors
		
		

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="aviFilePath">
		/// A <see cref="System.String"/> Path of avi file to manage
		/// </param>
		/// <param name="debugMode">
		/// A <see cref="System.Boolean"/>
		/// </param>
		public AviManager(string aviFilePath, bool debugMode)
		{
			_aviFilePath = aviFilePath;
			_aviFileName = Path.GetFileNameWithoutExtension(_aviFilePath);	
			_aviFileSize = new FileInfo(aviFilePath).Length;
			utils.DebugMode = debugMode;
		}
		
		
		
		#endregion Constructors
		
		
		
		
		/// <summary>
		/// Scan avi file and fill AviContainer with info
		/// </summary>
		public void GetInfo()
		{
			
			uint 	FourCC = 0;		// Four-Character Code
			string 	sFourCC = "";	// Four-Character Code in Ascii
			string 	hFourCC = "";	// Four-Character Code in Hexadecimal
			uint	FourCClen = 0;	// Four-Character Code length
			
			
			// Create new instance of AviContainer
			_aviContainer = new AviContainerEntity();
			
			
			
			// Open Stream on file
			ifs = new FileStream(_aviFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

			
			// Reset stream position
			streamPosition=0;
			
			
			
			
			#region Header Checks
			
			
			// Read and check First FourCC in stream
			FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
			
			if ((utils.GetFourccFromInt(FourCC) != AviConstants.RIFF) &&
			    (utils.GetFourccFromInt(FourCC) != AviConstants.RIFX))
			{
				ifs.Close();
				ifs.Dispose();
				ifs = null;
				
				throw new Exception("Error. Not a valid RIFF file\r\n" + 
				                    "[" + FourCC.ToString() + " " + 
				                          utils.GetFourccFromInt(FourCC) + 
				                    "]");
			}
			
			
			
			
			// Read and check file size
			uint dataSize = utils.ReadDWord(ref ifs, ref streamPosition);
			uint fileType = utils.ReadDWord(ref ifs, ref streamPosition);
			
			_aviContainer.DataSize = (long)dataSize;
			
			if (_aviFileSize < (dataSize + 8))
			{
				ifs.Close();
				ifs.Dispose();
				ifs = null;
				
				throw new Exception("Error. Error. Truncated file\r\n" + 
				                    "File Length: " + _aviFileSize.ToString() + "\r\n" +
				                    "RIFF Length: " + (dataSize + 8).ToString() );
			}
			
			
			#endregion Header Checks
			
			// Debug
			utils.DebugWrite("________", "FILE", 0, Convert.ToUInt32(_aviFileSize));

			// Debug
			utils.DebugWrite("________", "RIFF", 0, dataSize);
			utils.DebugWrite(utils.GetFourccFromInt(fileType).PadRight(8, Convert.ToChar("_")) , "TYPE", 0, 0);

			
			
			// Loop until EOF
			while (streamPosition < _aviFileSize)
			{
				// Read Next FourCC and associated length
				FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
				sFourCC = utils.GetFourccFromInt(FourCC);
				hFourCC = FourCC.ToString("X8");
				FourCClen = utils.ReadDWord(ref ifs, ref streamPosition);
				
				// Adjust bytes to read (no odd)
				if ((FourCClen % 2) != 0)
				{
					FourCClen++;
				}
				
				// Debug
				utils.DebugWrite(hFourCC,sFourCC, streamPosition, FourCClen);

				
				
				// Verify if REC_IX is present
				if (sFourCC.ToLower().Substring(3) == "rec" ||
				    sFourCC.ToLower().Substring(2) == "ix" )
				{
					// recIX = true;
					utils.DebugWrite(hFourCC, sFourCC, streamPosition, 0);
				}
				
				
				// Check memory to read
				if ((streamPosition + FourCClen) > _aviFileSize)
				{
					ifs.Close(); 
					ifs.Dispose();
					ifs = null;
					return;
				}
				
				
				
				
				// Parse Area
				if(FourCC == AviConstants.IdMainAVIList)
				{
					// "LIST" section
					ManageLIST(FourCClen);
					
				}
				else if (FourCC == AviConstants.IdAVIOldIndex)
				{
					// "idx1" section
					_aviContainer.IdxOffset = streamPosition;
					ManageIDX1(FourCClen);
					
				}
				else if (FourCC == AviConstants.IdJUNKTag)
				{
					// "JUNK" section
					ManageJUNK(FourCClen);
					
				}
				else
				{
					// unknow section... skip
					utils.StreamSeek(ref ifs, ref streamPosition, FourCClen);
				}
				
				
			} // end while (streamPosition < _aviFileSize)

			
			
			// Close stream
			ifs.Close();
			ifs.Dispose();
			ifs = null;
			
			return;			
			
		} // end function GetInfo()
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Manage LIST under root section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/> Section length
		/// </param>
		private void ManageLIST(uint sectionSize)
		{
			uint iPos = 0;
			
			// Read FourCC of LIST TYPE
			uint FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
			string sFourCC = utils.GetFourccFromInt(FourCC);
			string hFourCC = FourCC.ToString("X8");
			uint listSize = 0;
			
			// Update offsets
			iPos +=4;
			sectionSize -=4;		// we already readed 4CC LIST TYPE
			listSize = sectionSize;
			
			
			// Debug
			utils.DebugWrite(hFourCC, sFourCC, streamPosition, 0);
			//Console.WriteLine(sFourCC);

			
			
			// If MOVI section.. manage it
			if(FourCC == AviConstants.IdAVIMovieData)
			{
				_aviContainer.MoviSize = (long)listSize;
				_aviContainer.MoviOffset = streamPosition;
				ManageMOVI(listSize);
				// utils.StreamSeek(ref ifs, ref streamPosition, listSize);
				iPos +=listSize;
			}

			
			
			// Loop for each sections in LIST
			while (iPos < sectionSize)
			{
			
				// Read FourCC of LIST subitems
				FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
				uint FourCClen = utils.ReadDWord(ref ifs, ref streamPosition);
				
				// Adjust bytes to read (no odd)
				if ((FourCClen % 2) != 0)
				{
					FourCClen++;
				}
				
				sFourCC = utils.GetFourccFromInt(FourCC);
				hFourCC = FourCC.ToString("X8");
				
				iPos +=8;
				
				// Debug
				utils.DebugWrite(hFourCC," " + sFourCC, streamPosition, FourCClen);

				
				
				
				
				
				// Parse LIST ITEMS
				if (FourCC == AviConstants.IdMainAVIList)
				{
					// SUB "LIST" section
					ManageSUBLIST(FourCClen);
					iPos += FourCClen;	
					
				}
				else if (FourCC == AviConstants.IdJUNKTag)
				{
					// "JUNK" section
					ManageJUNK(FourCClen);
					iPos += FourCClen;	
					
				}
				else if (FourCC == AviConstants.IdAVIISFT)
				{
					// "ISFT" section
					ManageISFT(FourCClen, listSize);
					iPos += FourCClen;	
					
				}
				else if (FourCC == AviConstants.IdMainAVIHeader)
				{
					// "avih" section
					ManageAVIH(FourCClen);
					iPos += FourCClen;	 
					
				}
				else
				{
					// Unknow section
					utils.StreamSeek(ref ifs, ref streamPosition, FourCClen);
					iPos += FourCClen;	
					
				}
				
			}
			
			return;
		
		}
		

		
		
		
		
		
		
		
		
		/// <summary>
		/// Manage LIST under another LIST section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/> Section length
		/// </param>
		private void ManageSUBLIST(uint sectionSize)
		{
			
			uint iPos = 0;
			uint listSize = 0;
			
			// Read FourCC of LIST TYPE
			uint FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
			string sFourCC = utils.GetFourccFromInt(FourCC);
			string hFourCC = FourCC.ToString("X8");
			
			// Update offsets
			iPos +=4;
			sectionSize -=4;		// we already readed 4CC LIST TYPE
			listSize = sectionSize;
			
			// Debug			
			utils.DebugWrite(hFourCC, " " +sFourCC, streamPosition, 0);
			//Console.WriteLine(" " + sFourCC);
			
			
			// Loop for each sections in LIST
			while (iPos < sectionSize)
			{
			
				// Read FourCC of LIST subitems
				FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
				uint FourCClen = utils.ReadDWord(ref ifs, ref streamPosition);
				
				// Adjust bytes to read (no odd)
				if ((FourCClen % 2) != 0)
				{
					FourCClen++;
				}
				
				sFourCC = utils.GetFourccFromInt(FourCC);
				hFourCC = FourCC.ToString("X8");
				
				iPos +=8;
				
				// Debug
				utils.DebugWrite(hFourCC,"  " + sFourCC, streamPosition, FourCClen);

				
				
				// Parse SubList items
				if(FourCC == AviConstants.IdAVIStreamHeader)
				{
					// "strh" section
					ManageSTRH(FourCClen);
					iPos +=FourCClen;
				}
				else if(FourCC == AviConstants.IdAVIStreamFormat)
				{
					// "strf" section
					ManageSTRF(FourCClen);
					iPos +=FourCClen;
				}
				else if(FourCC == AviConstants.IdAVIISFT)
				{
					// "ISFT" section
					ManageISFT(FourCClen, listSize);
					iPos +=(listSize-8);
				}
				else if(FourCC == AviConstants.IdJUNKTag)
				{
					// "JUNK" section
					ManageJUNK(FourCClen);
					iPos +=FourCClen;
				}
				else
				{
					// Unknow section
					utils.StreamSeek(ref ifs, ref streamPosition, FourCClen);
					iPos +=FourCClen;
				}	

				
			}

			return;
			
		}

		
		
		
		
		
		
		

		
		/// <summary>
		/// Manage idx1 section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageIDX1(uint sectionSize)
		{
			_aviContainer.IdxSize = sectionSize;
			
			// Skip section
			utils.StreamSeek(ref ifs, ref streamPosition, sectionSize);			
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Read JUNK section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageJUNK(uint sectionSize)
		{
			// Read JUNK section
			byte[] tmpBytes = utils.StreamRead(ref ifs, ref streamPosition, sectionSize);

			string junkString = utils.GetAsciiFromBytes(tmpBytes).Trim();
			bool isAlreadyPresent = false;
			
			// Update AviContainer Junk section
			if (junkString != "")
			{
				for (int h=0; h<_aviContainer.JunkData.Count; h++)
				{
					if (_aviContainer.JunkData[h] == junkString)
					{
						isAlreadyPresent = true;
						break;
					}
				}
				
				if (!isAlreadyPresent)
				{
					_aviContainer.JunkData.Add(junkString);
				}
			}
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Read avih section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageAVIH(uint sectionSize)
		{
			// Read avih section
			byte[] tmpBytes =  utils.StreamRead(ref ifs, ref streamPosition, sectionSize);
			
			// Load info in AviContainer
			_aviContainer.AviHeader.loadDataStructure(tmpBytes);
			
			return;
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Manage strh section (stream header)
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageSTRH(uint sectionSize)
		{
			// Read avih section
			byte[] tmpBytes =  utils.StreamRead(ref ifs, ref streamPosition, sectionSize);
			
			// Load info in AviContainer
			AviStreamEntity ase = new AviStreamEntity();
			ase.AviStreamHeader.loadDataStructure(tmpBytes);
			ase.Offset = streamPosition - sectionSize - 4;
			
			// Add AviStream to AviContainer
			_aviContainer.AviStreams.Add(ase);
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Manage strf (stream format header)
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageSTRF(uint sectionSize)
		{
			byte[] tmpBytes = null;
			
			// Get FourCC type of current stream
			int fccType = 
				_aviContainer.AviStreams[_aviContainer.AviStreams.Count-1].AviStreamHeader.FccType;
			
			// Parse stream
			if (fccType == AviConstants.StreamtypeVIDEO)
			{
				// "vids" section
				tmpBytes = utils.StreamRead(ref ifs, ref streamPosition, sectionSize);
				BITMAPINFOHEADER vsh = new BITMAPINFOHEADER();
				vsh.Offset = streamPosition - sectionSize - 24;
				vsh.loadDataStructure(tmpBytes);
				
				// Add AviVideoStream to AviContainer 
				_aviContainer.AviStreams[_aviContainer.AviStreams.Count-1].AviVideoStreamHeader.Add(vsh);
				
			}
			else if (fccType == AviConstants.StreamtypeAUDIO)
			{
				// "auds" section
				tmpBytes = utils.StreamRead(ref ifs, ref streamPosition, sectionSize);
				WAVEFORMATEX ash = new WAVEFORMATEX();				
				ash.loadDataStructure(tmpBytes);
				
				// Add AviAudioStream to AviContainer 
				_aviContainer.AviStreams[_aviContainer.AviStreams.Count-1].AviAudioStreamHeader.Add(ash);
				
			}
			else
			{
				// Skip unknow section
				utils.StreamSeek(ref ifs, ref streamPosition, sectionSize);			
			}
			
			
		}
		
		
		
		
		

		
		
		
		
		/// <summary>
		/// Manage ISFT section
		/// </summary>
		/// <param name="sectionSize">
		/// A <see cref="System.UInt32"/>
		/// </param>
		/// <param name="listLen">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageISFT(uint sectionSize, uint listLen)
		{
			// Read ISFT section
			byte[] tmpBytes =  utils.StreamRead(ref ifs, ref streamPosition, sectionSize);
			bool isAlreadyPresent = false;
			
			string isftString = utils.GetAsciiFromBytes(tmpBytes).Trim();
			
			// Update AviContainer ISFT section
			if (isftString != "")
			{				
				for (int h=0; h<_aviContainer.ISFTData.Count; h++)
				{
					if (_aviContainer.ISFTData[h] == isftString)
					{
						isAlreadyPresent = true;
						break;
					}
				}
				
				if (!isAlreadyPresent)
				{
					_aviContainer.ISFTData.Add(isftString);
				}
			}
			
			// skip other sections after ISFT in LIST
			if ((listLen - 8) > sectionSize)
			{
				utils.StreamSeek(ref ifs, ref streamPosition, (listLen - 8) - sectionSize);
			}
			
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Manage MOVI section
		/// </summary>
		/// <param name="listLen">
		/// A <see cref="System.UInt32"/>
		/// </param>
		private void ManageMOVI(uint listLen)
		{
			uint iPos = 0;
			uint FourCC = 0;
			uint FourCClen = 0;
			string sFourCC = "";
			// string hFourCC = "";
			
			long frameCount = 0;
			
			// Loop for each sections in MOVI LIST
			while (iPos < listLen)
			{
				
				// Read FourCC of MOVI LIST subitems
				FourCC = utils.ReadDWord(ref ifs, ref streamPosition);
				FourCClen = utils.ReadDWord(ref ifs, ref streamPosition);
				
				// Adjust bytes to read (no odd)
				if ((FourCClen % 2) != 0)
				{
					FourCClen++;
				}
				
				sFourCC = utils.GetFourccFromInt(FourCC);
				iPos +=8;
				
				// Debug
				// hFourCC = FourCC.ToString("X8");
				// utils.DebugWrite(hFourCC," " + sFourCC, streamPosition, FourCClen);
				
				
				
				// Parse SubItems (with FourCC = xxdb or xxdc)
				if (sFourCC.Substring(2,2) == "dc" || 
				    sFourCC.Substring(2,2) == "db")
				{
					frameCount ++;
					
					if (frameCount == 1)
					{
						ParseDCuserdata(FourCClen);
						iPos += FourCClen;
					}
					else
					{
						ParseDCvopdata(FourCClen);	    			
						iPos += FourCClen;
					}

					// scan only first 200 xxdc or xxdb frames in MOVI chunk
					if (frameCount >= 200)
	    			{
						utils.StreamSeek(ref ifs, ref streamPosition, (listLen - iPos));
						iPos = listLen;
	    			}
					
				}
				else
				{
					// skip other sections
					utils.StreamSeek(ref ifs, ref streamPosition, FourCClen);
					iPos += FourCClen;
				}
				
				
			}
			
			
			return;
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Extract UserData info from DC subarea
		/// </summary>
		/// <param name="dcSubareaSize">
		/// A <see cref="System.UInt32"/> length of subarea
		/// </param>
		private void ParseDCuserdata(uint dcSubareaSize)
		{
			string outValue="";
			int sPoint = 0; 
			int ePoint = 0;
			
			
			byte[] dcBuffer = utils.StreamRead(ref ifs, ref streamPosition, dcSubareaSize);
			
			bool isAlreadyPresent = false;

			// Find UserData ... START			
			sPoint =  utils.CompareByteArray(dcBuffer, AviConstants.UserDataBytes, 0);
			
			// Loop to find END POINT
			while ((sPoint < (int)dcSubareaSize) && sPoint >= 0)
			{
				// Find another UserDataBytes
				ePoint = utils.CompareByteArray(dcBuffer, AviConstants.UserDataBytes, (sPoint + 3));
				
				if (ePoint < 0)
				{
					// Find VOLStartBytes
					ePoint = utils.CompareByteArray(dcBuffer, AviConstants.VOLStartBytes , (sPoint + 3));
				}
				
				if (ePoint < 0)
				{
					// Find VOPStartBytes
					ePoint = utils.CompareByteArray(dcBuffer, AviConstants.VOPStartBytes , (sPoint + 3));
				}
				
				if (ePoint < 0)
				{
					// from sPoint to end of Byte Array
					outValue = utils.GetHexFromBytes(dcBuffer, (sPoint+4), (int)(dcSubareaSize - (sPoint+3)) );
					outValue = utils.GetAsciiFromHex(outValue);
					
					isAlreadyPresent = false;
					for (int h=0; h<_aviContainer.MoviUserData.Count; h++)
					{
						if (_aviContainer.MoviUserData[h] == outValue)
						{
							isAlreadyPresent = true;
							break;
						}
					}
					
					if (!isAlreadyPresent)
					{
						_aviContainer.MoviUserData.Add(outValue);
					}

					break;
				}
				else
				{
					// from sPoint to ePoint
					outValue = utils.GetHexFromBytes(dcBuffer, (sPoint+4), (int)(ePoint - (sPoint+4)) );					
					outValue = utils.GetAsciiFromHex(outValue);					
					
					isAlreadyPresent = false;
					for (int h=0; h<_aviContainer.MoviUserData.Count; h++)
					{
						if (_aviContainer.MoviUserData[h] == outValue)
						{
							isAlreadyPresent = true;
							break;
						}
					}
					
					if (!isAlreadyPresent)
					{
						_aviContainer.MoviUserData.Add(outValue);
					}
					
					sPoint = utils.CompareByteArray(dcBuffer, AviConstants.UserDataBytes, ePoint);
				}
			}
			
			// Find UserData ... END

			
			dcBuffer = null;
			
			
		}
		
		

		
		
		
		
		
		
		
		/// <summary>
		/// Extract VOP count in to DC subarea (Packet Bitstream Detect)
		/// </summary>
		/// <param name="dcSubareaSize">
		/// A <see cref="System.UInt32"/> length of subarea
		/// </param>
		private void ParseDCvopdata(uint dcSubareaSize)
		{
			int sPoint = 0; 
			int vopCount = 0;
			
			byte[] dcBuffer = utils.StreamRead(ref ifs, ref streamPosition, dcSubareaSize);

			
			// Find Packed Bitstream ... START
			sPoint =  utils.CompareByteArray(dcBuffer, AviConstants.VOPStartBytes, 0);
			
			while (sPoint < (dcSubareaSize-2) && sPoint >= 0)
			{
				vopCount++;
				sPoint =  utils.CompareByteArray(dcBuffer, AviConstants.VOPStartBytes, sPoint+3);
			}
			
			// Find Packed Bitstream ... END
			
			// Packet Bitstream Detect
			if (vopCount > 1)
			{
				_aviContainer.PBitstream = true;				
			}

			dcBuffer = null;

						
		}
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Get audio type
		/// </summary>
		/// <param name="audioVal">
		/// A <see cref="System.String"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		public string GetAudioType(string audioVal)
	    {
	    	// return name of codec audio	    	
			if (audioVal == "0055")	
			{
				return "0x" + audioVal + " (MP3)";
			}
	    	else if (audioVal == "0001")
			{
				return "0x" + audioVal + " (PCM)";
			}
			else if (audioVal == "2001")
			{
				return "0x" + audioVal + " (DTS)";
			}
			else if (audioVal == "000A")
			{
				return "0x" + audioVal + " (WMA9)";
			}
			else if (audioVal == "0030")
			{
				return "0x" + audioVal + " (Dolby AC2)";
			}
			else if (audioVal == "0050")
			{
				return "0x" + audioVal + " (MPEG)";
			}
			else if (audioVal == "2000")
			{
				return "0x" + audioVal + " (AC3)";
			}
			else
			{
				return "0x" + audioVal + " (?)";
			}
	    }
		
		
		
		
		
		
		
	}
}
