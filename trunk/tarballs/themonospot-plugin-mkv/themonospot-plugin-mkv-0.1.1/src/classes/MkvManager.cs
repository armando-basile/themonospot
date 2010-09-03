
using System;
using System.IO;
using System.Collections.Generic;


namespace ThemonospotPluginMkv
{
	
	/// <summary>
	/// Parse Matroska file
	/// </summary>
	public class MkvManager
	{
		
		private FileStream ifs;
		// private FileStream ofs;
		private Utility utils = new Utility();
				
		private long streamPosition = 0;

		private string _mkvFilePath = "";
		private string _mkvFileName = "";
		private long _mkvFileSize = 0;
		
		
		#region Properties
		
		
		private List<MkvTrackEntity> _mkvContainer = new List<MkvTrackEntity>();
		
		public List<MkvTrackEntity> MkvContainer
		{
			get{return _mkvContainer;}			
		}
		
		
		
		#endregion Properties
		
		
		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="mkvFilePath"> Matroska file path
		/// A <see cref="System.String"/>
		/// </param>
		public MkvManager(string mkvFilePath, bool debugMode)
		{
			_mkvFilePath = mkvFilePath;
			_mkvFileName = Path.GetFileNameWithoutExtension(_mkvFilePath);	
			_mkvFileSize = new FileInfo(_mkvFilePath).Length;
			utils.DebugMode = debugMode;
		}
		
		
		
		
		
		/// <summary>
		/// Scan matroska file and fill MkvTracks
		/// </summary>
		public void GetInfo()
		{
			_mkvContainer = new List<MkvTrackEntity>();
			string mkvElement = "";
			long lenElement = 0;
			
			// Open Stream on file
			ifs = new FileStream(_mkvFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

			
			// Reset stream position
			streamPosition=0;

			
			// Debug
			utils.DebugMsg("File path: " + _mkvFilePath + "\r\n" +
			               "File name: " + _mkvFileName + "\r\n" +
						   "File size: " + _mkvFileSize.ToString("#,##0"));
			
			
			// Read first element and len
			mkvElement = ReadElement();
			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
			
			// Debug
			utils.DebugWrite(mkvElement, lenElement, streamPosition);
			
			// Check if Matroska file type
			if(mkvElement != MkvConstants.Header_EBML)
			{
				ifs.Close();
				ifs.Dispose();
				ifs = null;
				throw new Exception("Not a valid Matroska file type");
			}
			
			
			// Skip header
			streamPosition += lenElement;
			ifs.Seek(lenElement, SeekOrigin.Current);
			
			
			// Parse remaining segments
			while (streamPosition < _mkvFileSize)
			{
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				if (mkvElement == MkvConstants.Header_SEGMENT)
				{
					// Founded Segment
					ReadSegment(lenElement);
				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
				}

			}
			
			
			
			// Close stream
			ifs.Close();
			ifs.Dispose();
			ifs = null;
			
			return;	
			
			
			
			
		} // end void GetInfo()
		
		
		
		
		
		
		
		
		
		
		private string GetVInt(string Element)
		{
		    int NewFirstByte = 0;
		    int FirstByte = int.Parse(Element.Substring(0,2),System.Globalization.NumberStyles.AllowHexSpecifier);
		    
		    if ( (FirstByte & 0x80) > 0)
				NewFirstByte = FirstByte & 0x7F;
			else if ( (FirstByte & 0x40) > 0)
				NewFirstByte = FirstByte & 0x3F;
			else if ( (FirstByte & 0x20) > 0)
				NewFirstByte = FirstByte & 0x1F;
			else if ( (FirstByte & 0x10) > 0)
				NewFirstByte = FirstByte & 0x0F;
			else if ( (FirstByte & 0x08) > 0)
				NewFirstByte = FirstByte & 0x07;
			else if ( (FirstByte & 0x04) > 0)
				NewFirstByte = FirstByte & 0x03;
			else if ( (FirstByte & 0x02) > 0)
				NewFirstByte = FirstByte & 0x01;
			else if ( (FirstByte & 0x01) > 0)
				NewFirstByte = 0;
			else
				NewFirstByte = 0;
			
			if (Element.Length > 2)
			    return NewFirstByte.ToString("X2") + Element.Substring(2);
			else
			    return NewFirstByte.ToString("X2");
			
		}
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Read single element
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/>
		/// </returns>
		private string ReadElement()
		{
		    int firstByte = ifs.ReadByte();
		    string tmpElement = firstByte.ToString("X2");
		    
		    int nextBytes = BytesToRead(firstByte);
		    
			// Read all bytes
			byte[] allBytes = new byte[nextBytes];
			ifs.Read(allBytes, 0, nextBytes);
			
			tmpElement += utils.GetHexFromBytes(allBytes);
		    
			
			// Update File OffSet
		    streamPosition += nextBytes+1;
		    
		    return tmpElement;
		}
		
		
		

		
		
		
		
		
		
		/// <summary>
		/// Detect bytes to read 
		/// </summary>
		/// <param name="FirstByteInt">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <returns>
		/// A <see cref="System.Int32"/>
		/// </returns>
		private int BytesToRead(int firstByteInt)
		{
			if ( (firstByteInt & 0x80) > 0)
				return 0;
			else if ( (firstByteInt & 0x40) > 0)
				return 1;
			else if ( (firstByteInt & 0x20) > 0)
				return 2;
			else if ( (firstByteInt & 0x10) > 0)
				return 3;
			else if ( (firstByteInt & 0x08) > 0)
				return 4;
			else if ( (firstByteInt & 0x04) > 0)
				return 5;
			else if ( (firstByteInt & 0x02) > 0)
				return 6;
			else if ( (firstByteInt & 0x01) > 0)
				return 7;
			else
				return 8;
		}

		
		
		
		
		
		
		
		/// <summary>
		/// Read Segment entity
		/// </summary>
		/// <param name="segmentLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void ReadSegment(long segmentLen)
		{
			string mkvElement = "";
			long lenElement = 0;
			bool firstCluster = true;
			
			// Set end of Segment
			long endSegment = streamPosition + segmentLen - 1;
			
			// Loop for all segment sub sections
			while (streamPosition < endSegment)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				if (mkvElement == MkvConstants.Header_TRACK)
				{
					// Founded Tracks
					ReadTracks(lenElement);
				}
				else if ((mkvElement == MkvConstants.Header_CLUSTER) &&
				         firstCluster == true)
				{
					// Founded first Cluster
					ReadCluster(lenElement);
					firstCluster = false;
				}				
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
				}
			}
			
		}
		
		
		
		
		
		
		
		
		/// <summary>
		/// Read cluster section
		/// </summary>
		/// <param name="clusterLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		void ReadCluster(long clusterLen)
		{
			string mkvElement = "";
			long lenElement = 0;
			bool firstBlockGroup = true;
			
			// Set end of Cluster
			long endCluster = streamPosition + clusterLen - 1;
			
			// Loop for all cluster sub sections
			while (streamPosition < endCluster)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				if ((mkvElement == MkvConstants.Header_CLUSTER_BLOCK_GRP) && 
				    firstBlockGroup)
				{
					// First Block Group founded
					ReadBlock(lenElement);
					firstBlockGroup = false;
				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
				}
				
			}
			
		}
		
		
		
		
		
		
		/// <summary>
		/// Read Block
		/// </summary>
		/// <param name="blockLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		void ReadBlock(long blockLen)
		{
			string mkvElement = "";
			long lenElement = 0;
			
			
			// Set end of Block
			long endBlock = streamPosition + blockLen - 1;
			
			// Loop for all block sub sections
			while (streamPosition < endBlock)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				

				
/*				
				// Read Block
				byte[] tmpBuffer;
				tmpBuffer = new byte[(int)lenElement];
				ifs.Read(tmpBuffer, 0, (int)lenElement);
*/				
				ifs.Seek(lenElement, SeekOrigin.Current);
				streamPosition += lenElement;
				
				// Parse Block
//				ParseBlock(tmpBuffer);
				
			}
			
		}
		
		
		
/*		
		private void ParseBlock(byte[] block)
		{
			// for future features
			
		}
*/		
		
		
		
		
		
		
		
		/// <summary>
		/// Read Tracks entity
		/// </summary>
		/// <param name="tracksLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void ReadTracks(long tracksLen)
		{
			string mkvElement = "";
			long lenElement = 0;
			
			// Set end of Tracks
			long endTracks = streamPosition + tracksLen - 1;
			
			// Loop for all tracks sub sections
			while (streamPosition < endTracks)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				if (mkvElement == MkvConstants.Header_TRACKENTRY)
				{
					// Founded Track
					ReadTrack(lenElement);
				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
				}
			}
		}
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Read Track entity
		/// </summary>
		/// <param name="trackLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		public void ReadTrack(long trackLen)
		{
			string mkvElement = "";
			long lenElement = 0;
			long frameDuration = 0;
			byte[] tmpBuffer;
			
			// Create instance of new MkvTrackEntity			
			MkvTrackEntity mte = new MkvTrackEntity();
			
			// Set end of Track
			long endTrack = streamPosition + trackLen - 1;
			
			// Loop for all track sub sections
			while (streamPosition < endTrack)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				
				if (mkvElement == MkvConstants.Header_TRACK_NUMBER)
				{
					// NUMBER
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					// Track Number
					mte.Number=  int.Parse(utils.GetHexFromBytes(tmpBuffer), 
					          			   System.Globalization.NumberStyles.AllowHexSpecifier);
										
				}
				else if (mkvElement == MkvConstants.Header_TRACK_TYPE)
				{
					// TYPE
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					// Should be 1 byte len
					mte.Type = Convert.ToInt32(tmpBuffer[0]);
										
				}
				else if (mkvElement == MkvConstants.Header_TRACK_LANG)
				{
					// LANGUAGE
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					// Convert Byte in Ascii
					mte.Lang = utils.GetAsciiFromBytes(tmpBuffer);

				}
				else if (mkvElement == MkvConstants.Header_TRACK_CODEC)
				{
					// CODEC
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					// Convert Byte in Ascii
					mte.Codec = utils.GetAsciiFromBytes(tmpBuffer);

				}
				else if (mkvElement == MkvConstants.Header_TRACK_VIDEO)
				{
					// VIDEO
					ReadVideo(lenElement, ref mte);

				}
				else if (mkvElement == MkvConstants.Header_TRACK_AUDIO)
				{
					// AUDIO
					ReadAudio(lenElement, ref mte);

				}
				else if (mkvElement == MkvConstants.Header_TRACK_DEFDURATION)
				{
					// FRAME DURATION
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					frameDuration =  long.Parse(utils.GetHexFromBytes(tmpBuffer), 
					                            System.Globalization.NumberStyles.AllowHexSpecifier);
					
					mte.FrameRate = (float)(Math.Pow(10, 9) / frameDuration);

				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
				}
			}
			
			_mkvContainer.Add(mte);
		}

		
		
		
		
		
		
		
		
		/// <summary>
		/// Read Video track and update TrackEntity
		/// </summary>
		/// <param name="videoLen">
		/// A <see cref="System.Int64"/>
		/// </param>
		/// <param name="trackEntity">
		/// A <see cref="MkvTrackEntity"/>
		/// </param>
		public void ReadVideo(long videoLen, ref MkvTrackEntity trackEntity)
		{
			string mkvElement = "";
			long lenElement = 0;
			byte[] tmpBuffer;
			
			MkvVideoEntity mve = new MkvVideoEntity();
			
			// Set end of Video
			long endVideo = streamPosition + videoLen - 1;
			
			// Loop for all video sub sections
			while (streamPosition < endVideo)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				
				if (mkvElement == MkvConstants.Header_VIDEO_PIXW)
				{
					// WIDTH
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					mve.PixelWidth = int.Parse(utils.GetHexFromBytes(tmpBuffer), System.Globalization.NumberStyles.AllowHexSpecifier);
					
				}
				else if (mkvElement == MkvConstants.Header_VIDEO_PIXH)
				{
					// HEIGHT
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					mve.PixelHeight = int.Parse(utils.GetHexFromBytes(tmpBuffer), System.Globalization.NumberStyles.AllowHexSpecifier);

				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
					
				}
			}
			
			trackEntity.Video.Add(mve);

		}

		
		
		
		
		
		
		
		
		
		
		public void ReadAudio(long audioLen, ref MkvTrackEntity trackEntity)
		{
			string mkvElement = "";
			long lenElement = 0;
			byte[] tmpBuffer;
			
			MkvAudioEntity mae = new MkvAudioEntity();
			
			// Set end of Video
			long endAudio = streamPosition + audioLen - 1;
			
			// Loop for all audio sub sections
			while (streamPosition < endAudio)
			{
				
				// Read elememt and len
				mkvElement = ReadElement();
				lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
				
				// Debug
				utils.DebugWrite(mkvElement, lenElement, streamPosition);
				
				
				if (mkvElement == MkvConstants.Header_AUDIO_FREQ)
				{
					// FREQUENCY
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					tmpBuffer = utils.swapByte(tmpBuffer);
					mae.Frequency = BitConverter.ToSingle(tmpBuffer,0); 
					
				}
				else if (mkvElement == MkvConstants.Header_AUDIO_CHAN)
				{
					// CHANNELS
					tmpBuffer = new byte[Convert.ToInt32(lenElement)];
					ifs.Read(tmpBuffer, 0, Convert.ToInt32(lenElement));
					streamPosition += lenElement;
					
					mae.Channels = Convert.ToInt32(tmpBuffer[0]);

				}
				else
				{
					// Skip section
					streamPosition += lenElement;
					ifs.Seek(lenElement, SeekOrigin.Current);
					
				}
			}
			
			trackEntity.Audio.Add(mae);
		}

		
		
		
		
		
		
		
		
		
		
	}
}
