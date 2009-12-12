using System;
using System.IO;
using System.Text;
using System.Threading;
using Utility;
using themonospot_Base_Main;
using System.Collections;
using System.Collections.Generic;

namespace themonospot_Base_Main
{
    
   
    
    /// <summary>
    /// Matroska Parser for Themonospot
    /// </summary>
    public class clsParserMKV : IParser
    {

        public List<clsInfoItem>    VideoItems
        {
            get {return _VideoItems;    }
        }
        
        public List<clsInfoItem>    AudioItems
        {
            get {return _AudioItems;    }
        }

        private List<clsInfoItem>   _VideoItems;
        private List<clsInfoItem>   _AudioItems;
        private List<clsMkvTrack>   _MkvTrackItems;
        private clsMkvTrack         _MkvTrackItem;
        private clsEncoding         EncObject       = new clsEncoding();
        
        private readonly string Header_EBML         = "1A45DFA3";
        private readonly string Header_SEGMENT      = "18538067";
        private readonly string Header_TRACK        = "1654AE6B";
        private readonly string Header_TRACKENTRY   = "AE";
        private readonly string Header_TRACK_TYPE   = "83";
        private readonly string Header_TRACK_LANG   = "22B59C";
        private readonly string Header_TRACK_CODEC  = "86";
        private readonly string Header_TRACK_VIDEO  = "E0";
        private readonly string Header_TRACK_AUDIO  = "E1";
        private readonly string Header_VIDEO_PIXW   = "B0";
        private readonly string Header_VIDEO_PIXH   = "BA";
        private readonly string Header_AUDIO_FREQ   = "B5";
        private readonly string Header_AUDIO_CHAN   = "9F";
        

		private FileStream  mkvStreamReader = null;
		private string      _m_filename     = "";
		private string      _m_shortname    = "";
		private long        _m_filesize     = 0;
		private long        _m_posStream    = 0;
		private string      strElement      = "";
		private long        lenElement      = 0;
		private byte[]      ReadBuffer;
		
		
        public clsParserMKV()
        {
            
        }
        
        /// <summary>
		/// Parse the selected file 
		/// </summary>
		public void OpenFile(string FileName)
		{	
			
			// File Not Found...
			if (File.Exists(FileName) != true)
				throw new ParserException("File (" + FileName + ") Not Found...");
			
			_VideoItems = new List<clsInfoItem>();
			_AudioItems = new List<clsInfoItem>();
			_MkvTrackItems = new List<clsMkvTrack>();
			
			// Read File Infos
			FileInfo fi = new FileInfo(FileName);
			_m_filename = fi.FullName;
			_m_shortname = fi.Name;
			_m_filesize = fi.Length;
			fi = null;
			
			// DEBUG
/*			Console.WriteLine(""); Console.WriteLine(""); Console.WriteLine("");
			Console.WriteLine("_m_filename  = " + _m_filename);
			Console.WriteLine("_m_shortname = " + _m_shortname);
			Console.WriteLine("_m_filesize  = " + _m_filesize.ToString("#,###.##")); */
			
			// Open the streamer
			mkvStreamReader = new FileStream(_m_filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			_m_posStream = 0;
			
			
			strElement = ReadElement();
			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
/*			Console.WriteLine("**************************************************");    			
			Console.WriteLine("Element ID = " + strElement);
			Console.WriteLine("Length     = " + lenElement.ToString("#,###.##") + " bytes");
			Console.WriteLine("Offset     = " + _m_posStream.ToString()); */

			if (strElement != Header_EBML)
			{
			    mkvStreamReader.Close(); mkvStreamReader = null;
				throw new ParserException("Error. Not a valid MATROSKA file");
			}
			
   			// Skip Element Length    			
   			_m_posStream += lenElement;
   			mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
			
			
			while (_m_posStream < _m_filesize)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
/*    			Console.WriteLine("**************************************************");    			
    			Console.WriteLine("Element ID = " + strElement);
    			Console.WriteLine("Length     = " + lenElement.ToString("#,###.##") + " bytes");
    			Console.WriteLine("Offset     = " + _m_posStream.ToString()); */
                
    			
    			if (strElement != Header_SEGMENT)
    			{
        			// Skip Element Length if not SEGMENT			
        			_m_posStream += lenElement;
    	    		mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
    			}
    			else
    			    ReadSegment(lenElement);
    			
    			
			}
			
			
			mkvStreamReader.Close();
			
			// Console.WriteLine("_MkvTrackItems[" + k.ToString() + "].Track_Codec = " + _MkvTrackItems[k].Track_Codec );
			int Atrack = 0, Vtrack = 0, Otrack = 0;
			
			for (int k=0; k<_MkvTrackItems.Count; k++)
			{
    			if (_MkvTrackItems[k].Track_Type == 2)
    			{
    			    // Audio Track
    			    Atrack++;
    			    _AudioItems.Add(new clsInfoItem("AudioTrack " + Atrack.ToString("D2"), 
    			        "[" + _MkvTrackItems[k].Track_Lang + "] " +
    			        _MkvTrackItems[k].Track_Codec + " - " +  _MkvTrackItems[k].Track_Audio.Freq.ToString() + " Hz - " + 
    			        _MkvTrackItems[k].Track_Audio.Channels.ToString() + " channels"));
    			    
    			}
    			else if (_MkvTrackItems[k].Track_Type == 1)
    			{
    			    // Video Track
    			    Vtrack++;
    			    _VideoItems.Add(new clsInfoItem("VideoTrack " + Vtrack.ToString("D2"), 
    			        _MkvTrackItems[k].Track_Codec + "  (" +
    			        _MkvTrackItems[k].Track_Video.PixelWidth.ToString() + "x" + _MkvTrackItems[k].Track_Video.PixelHeight.ToString() + ")"));

    			    
    			}
    			else if (_MkvTrackItems[k].Track_Type == 0x11)
                {
                    // Subtitle track types
                    Otrack++;
                    _VideoItems.Add(new clsInfoItem("SubtitleTrack " + Otrack.ToString("D2"), 
    			        "[" + _MkvTrackItems[k].Track_Lang + "] " +
    			        _MkvTrackItems[k].Track_Codec ));
                    
                    
                    
                }
			
			}

			    
			
			return;
		}
        
		
		private void ReadSegment(long lenSegment)
		{
		    long _m_endSegment = _m_posStream + lenSegment - 1;
		    
		    while (_m_posStream < _m_endSegment)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
    			
    			if (strElement != Header_TRACK)
    			{
       			    // Skip Element Length if not a TRACKS
       			    _m_posStream += lenElement;   			    
       			    mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
    			}
                else
                {
/*    			    Console.WriteLine("**************************************************");
    			    Console.WriteLine("Tracks ID  = " + strElement);
    			    Console.WriteLine("Length     = " + lenElement.ToString("#,###.##") + " bytes");
    			    Console.WriteLine("Offset     = " + _m_posStream.ToString()); */
    			    
    			    ReadTracks(lenElement);
                }
                
    			
			}
		    
		    return;
		    
		}
		
		
		
		private void ReadTracks(long lenTracks)
		{
		    long _m_endSegment = _m_posStream + lenTracks - 1;
		    
		    while (_m_posStream < _m_endSegment)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
    			
    			if (strElement != Header_TRACKENTRY)
    			{
       			    // Skip Element Length if not a TRACK
       			    _m_posStream += lenElement;   			    
       			    mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
    			}
                else
                {
/*    			    Console.WriteLine("**************************************************");
    			    Console.WriteLine("Track ID   = " + strElement);
    			    Console.WriteLine("Length     = " + lenElement.ToString("#,###.##") + " bytes");
    			    Console.WriteLine("Offset     = " + _m_posStream.ToString()); */
    			 
    			    _MkvTrackItem = new clsMkvTrack();
    			    ReadTrack(lenElement);
    			    _MkvTrackItems.Add(_MkvTrackItem);
    			    
                }
                
    			
			}
		    
		    return;
		    
		}

		
		
		private void ReadTrack(long lenTrack)
		{
		    long _m_endSegment = _m_posStream + lenTrack - 1;
		    
		    while (_m_posStream < _m_endSegment)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
    			
/*			    Console.WriteLine("**************************************************");
			    Console.WriteLine("  Track Elem = " + strElement);
			    Console.WriteLine("  Length     = " + lenElement.ToString("#,###.##") + " bytes");
			    Console.WriteLine("  Offset     = " + _m_posStream.ToString()); */

    			if (strElement == Header_TRACK_TYPE)
    			{
    			    ReadBuffer = new byte[lenElement];
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			    
    			    // should be 1 byte
    			    _MkvTrackItem.Track_Type = Convert.ToInt32(ReadBuffer[0]);
    			    
    			}
    			else if (strElement == Header_TRACK_LANG)
    			{
       			    ReadBuffer = new byte[lenElement];
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			    
    			    _MkvTrackItem.Track_Lang = EncObject.getAsciiFromArray(ReadBuffer);
    			    
    			}
    			else if (strElement == Header_TRACK_VIDEO)
    			{
    			    ReadVideo(lenElement);
    			    
    			}
    			else if (strElement == Header_TRACK_AUDIO)
    			{
       			    ReadAudio(lenElement);
       			    
    			}
    			else if (strElement == Header_TRACK_CODEC)
    			{
       			    ReadBuffer = new byte[lenElement];
       			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement) );
    			    _m_posStream += lenElement;
    			    
    			    _MkvTrackItem.Track_Codec = EncObject.getAsciiFromArray(ReadBuffer);
    			    
    			}
                else
                {   
                    // Skip Element Length if not a significative element
       			    _m_posStream += lenElement;   			    
       			    mkvStreamReader.Seek(lenElement, SeekOrigin.Current);       			    
                }
                
    			
			}
		    
		    return;
		    
		}

	
		
		
		
		
		private void ReadVideo(long lenVideo)
		{
		    long _m_endSegment = _m_posStream + lenVideo - 1;
		    int tmpVal = 0;
		    
		    while (_m_posStream < _m_endSegment)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
    			
    			
    			if (strElement == Header_VIDEO_PIXW)
    			{
                    ReadBuffer = new byte[lenElement];
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			    
                    _MkvTrackItem.Track_Video.PixelWidth = int.Parse(EncObject.getHexFromBytes(ReadBuffer),System.Globalization.NumberStyles.AllowHexSpecifier);    			    
    			    
    			}
    			else if (strElement == Header_VIDEO_PIXH)
    			{
                    ReadBuffer = new byte[lenElement];
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			    
    			    _MkvTrackItem.Track_Video.PixelHeight = int.Parse(EncObject.getHexFromBytes(ReadBuffer),System.Globalization.NumberStyles.AllowHexSpecifier);    			    
    			    
    			}
                else
                {
       			    // Skip Element Length if not a TRACK
       			    _m_posStream += lenElement;   			    
       			    mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
    			    
                }
                
    			
			}
		    
		    return;
		    
		}
		
		
		
				
		private void ReadAudio(long lenAudio)
		{
		    long _m_endSegment = _m_posStream + lenAudio - 1;
		    
		    while (_m_posStream < _m_endSegment)
			{
    			// Read ElementID and Length
    			strElement = ReadElement();
    			lenElement = long.Parse(GetVInt(ReadElement()),System.Globalization.NumberStyles.AllowHexSpecifier);
    			
    			
    			if (strElement == Header_AUDIO_FREQ)
    			{
                    ReadBuffer = new byte[lenElement];
                    
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			        			    
    			    ReadBuffer = EncObject.swapByte(ReadBuffer);    			    
    			    _MkvTrackItem.Track_Audio.Freq = BitConverter.ToSingle(ReadBuffer,0);    			        			    
    			    
    			}
    			else if (strElement == Header_AUDIO_CHAN)
    			{
                    ReadBuffer = new byte[lenElement];
    			    mkvStreamReader.Read(ReadBuffer, 0, Convert.ToInt32(lenElement));
    			    _m_posStream += lenElement;
    			    
    			    // should be 1 byte
    			    _MkvTrackItem.Track_Audio.Channels = Convert.ToInt32(ReadBuffer[0]);    			    
    			    
    			}
                else
                {
       			    // Skip Element Length if not a TRACK
       			    _m_posStream += lenElement;   			    
       			    mkvStreamReader.Seek(lenElement, SeekOrigin.Current);
    			    
                }
                
    			
			}
		    
		    return;
		    
		}

		
		
		
		
		
		
    
		private int BytesToRead(int FirstByteInt)
		{
			if ( (FirstByteInt & 0x80) > 0)
				return 0;
			else if ( (FirstByteInt & 0x40) > 0)
				return 1;
			else if ( (FirstByteInt & 0x20) > 0)
				return 2;
			else if ( (FirstByteInt & 0x10) > 0)
				return 3;
			else if ( (FirstByteInt & 0x08) > 0)
				return 4;
			else if ( (FirstByteInt & 0x04) > 0)
				return 5;
			else if ( (FirstByteInt & 0x02) > 0)
				return 6;
			else if ( (FirstByteInt & 0x01) > 0)
				return 7;
			else
				return 8;
		}
		

		private string ReadElement()
		{
		    int OneByte = mkvStreamReader.ReadByte();
		    string tmpElement = OneByte.ToString("X2");
		    
		    int NextBytes = BytesToRead(OneByte);
		    
		    // Update File OffSet
		    _m_posStream += NextBytes+1;
		    
		    for (int j=0; j<NextBytes; j++)
		    {
		        OneByte = mkvStreamReader.ReadByte();
		        tmpElement += OneByte.ToString("X2");
		    }
		    
		    return tmpElement;
		}
	
	
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
	

	
	
	}
        
    
}




 
