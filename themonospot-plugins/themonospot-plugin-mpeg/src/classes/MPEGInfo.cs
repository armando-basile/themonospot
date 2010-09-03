
using System;
using System.IO;
using System.Text;

namespace mpeg
{

	
	public  class MPEGInfo
	
	{
	
	    #region index structures from mpgtx source
	
	    int[, ,] BitRateIndex = new int[2, 3, 16]
	
	            {
	
	                { // MPEG 1
	
	                    {0,32,64,96,128,160,192,224,256,288,320,352,384,416,448,0}, // layer 1
	
	                    {0,32,48,56, 64, 80, 96,112,128,160,192,224,256,320,384,0}, // layer 2
	
	                    {0,32,40,48, 56, 64, 80, 96,112,128,160,192,224,256,320,0}  // layer 3
	
	                },
	
	                {// MPEG 2 or 2.5
	
	                    {0,32,48,56, 64, 80, 96,112,128,144,160,176,192,224,256,0}, // layer 1
	
	                    {0, 8,16,24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}, // layer 2
	
	                    {0, 8,16,24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}  // layer 3
	
	                }
	
	            };
	
	 
	
	    int[,] SamplingIndex = new int[3, 4]
	
	            {
	
	                {44100,48000,32000,0}, // mpeg 1
	
	                {22050,24000,16000,0}, // mpeg 2
	
	                {11025,12000, 8000,0}  // mpeg 2.5
	
	            };
	
	 
	
	    double[] FrameRateIndex = new double[9]
	
	        {
	
	            0d, 24000d/1001d, 24d, 25d,
	
	            30000d/1001d, 30d, 50d,
	
	            60000d/1001d, 60d
	
	        };
	
	 
	
	    string[] AspectRatioIndex = new string[5]
	
	            {
	
	                "Invalid",
	
	                "1/1 (VGA)",
	
	                "4/3 (TV)",
	
	                "16/9 (Large TV)",
	
	                "2.21/1 (Cinema)"
	
	            };
	
	 
	
	    public string[] ModeIndex = new string[4] // these can change anytime specs change so be careful
	
	            {
	
	                "Stereo",
	
	                "Joint Stereo",
	
	                "Dual Channel",
	
	                "Mono",
	
	            };
	
	 
	
	    public string[] EmphasisIndex = new string[4] // same as above
	
	            {
	
	                "No Emphasis",
	
	                "50/15 Micro seconds",
	
	                "Unknown",
	
	                "CCITT J 17",
	
	            };
	
	    #endregion
	
	 
	
	    #region properties
	
	 
	
	    public class Video
	
	    {
	
	        public int GopHeaders;
	
	        public int Height; // px
	
	        public int Width;
	
	        public double FrameRate; // fps
	
	        public int AspectRatioCode;
	
	        public string AspectRatio;
	
	        public int BitRate;
	
	        public double Duration; // seconds
	
	        public double MuxRate;
	
	        public int ChromaFormat;
	
	        public string ChromaFormatText;
	
	        public int Format;
	
	        public string FormatText;
	
	        public int Frames;
	
	        public int Version = 1; // by default its 1
	
	    }
	
	    public Video VideoInfo;
	
	 
	
	    public class Audio
	
	    {
	
	        public double Version;
	
	        public int Layer;
	
	        public bool Protected;
	
	        public int BitRate;
	
	        public float ByteRate;
	
	        public int SamplingRate;
	
	        public bool Padding;
	
	        public int ModeCode;
	
	        public int ModeXt;
	
	        public bool Copyright;
	
	        public int EmphasisIndex;
	
	        public bool Original;
	
	        public int FrameLength;
	
	        // seconds; duration = file size / bit rate but from what i understand there is a timestamp
	
	        // for initial and one for when video ends, so accurate duration = timestamp end - timestamp start
	
	        public double Duration;
	
	        public int Frames;
	
	    }
	
	    public Audio AudioInfo;
	
	    #endregion
	
	 
	
	    public bool EnableTrace; // debugging purposes
	
	 
	
	    const byte PADDING_PACKET = 0xBE; // use these to compare instead of bytes every time
	
	    const byte VIDEO_PACKET = 0xE0;
	
	    const byte AUDIO_PACKET = 0xC0;
	
	    const byte SYSTEM_PACKET = 0xBB;
	
	 
	
	    const double FLOAT_0x10000 = (double)((UInt32)1 << 16); // unsigned long in c++ is UInt32 in c#
	
	    const UInt32 STD_SYSTEM_CLOCK_FREQ = (UInt32)90000;
	
	 
	
	    const int BUFFER_SIZE = 8192; // 8K buffer
	
	 
	
	    int _mpegVersion = 1;
	
	    long _fileSize;
	
	    double _initialTS; // needed to fix video/audio duration
	
	    bool _mpeg2Found; // switch needed to fix video duration
	
	 
	
	    byte[] _backwardBuffer;
	
	    byte[] _forwardBuffer;
	
	 
	
	    FileStream _fs;
	
	    BinaryReader _br;
	
	 
	
	    public MPEGInfo(string file)
	
	    {
	
	        if (!File.Exists(file))
	
	        {
	
	            return;
	
	        }
	
	        _fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
	
	        _br = new BinaryReader(_fs);
	
	    }
	
	 
	
	    public void Parse()
	
	    {
	
	        if (_br != null)
	
	        {
	
	            _backwardBuffer = new byte[BUFFER_SIZE];
	
	            _forwardBuffer = new byte[BUFFER_SIZE];
	
	            _fileSize = _fs.Length;
	
	            VideoInfo = new Video();
	
	            AudioInfo = new Audio();
	
	 
	
	            ParseVideo(0);
	
	            //_br.BaseStream.Seek(0, SeekOrigin.Begin); // go back to the start (redundant now)
	
	            ParseAudio();
	
	            //_br.BaseStream.Seek(0, SeekOrigin.Begin); // go back to the start
	
	            ParseSystem(0);
	
	        }
	
	        _br.Close();
	
	        _fs.Close();
	
	    }
	
	 
	
	    int _backwardBufferStart = 0;
	
	    int _backwardBufferEnd = 0;
	
	 
	
	    byte GetByteBackwards(int offset)
	
	    {
	
	        int pos = -1;
	
	        if (offset >= _backwardBufferEnd || offset < _backwardBufferStart) // need new buffer
	
	        {
	
	            //Trace(string.Format("-debug- new buffer needed; offset: {0}, buffer start: {1}, buffer end: {2}", offset,
	
	            //    _backwardBufferStart, _backwardBufferEnd));
	
	 
	
	            pos = offset - BUFFER_SIZE + 1;
	
	            if (pos < 0) { pos = 0; }
	
	 
	
	            //Trace(string.Format("-debug- position: {0}", pos));
	
	 
	
	            _backwardBufferStart = pos;
	
	            _backwardBufferEnd = offset;
	
	 
	
	            //Trace(string.Format("-debug- buffer start: {0}, buffer end: {1}",
	
	            //    _backwardBufferStart, _backwardBufferEnd));
	
	 
	
	            _br.BaseStream.Seek(pos, SeekOrigin.Begin);
	
	            _br.Read(_backwardBuffer, 0, BUFFER_SIZE);
	
	        }
	
	 
	
	        //Trace(string.Format("-debug- offset: {0}, buffer start: {1}",
	
	        //        offset, _backwardBufferStart));
	
	 
	
	        // got new buffer or still  in range
	
	        return _backwardBuffer[offset - _backwardBufferStart];
	
	    }
	
	 
	
	    int _forwardBufferStart = 0;
	
	    int _forwardBufferEnd = 0;
	
	 
	
	    byte GetByte(int offset)
	
	    {
	
	        int pos = -1;
	
	        if (offset >= _forwardBufferEnd || offset < _forwardBufferStart) // need new buffer
	
	        {
	
	            //Trace(string.Format("-debug- new f buffer needed; offset: {0}, buffer start: {1}, buffer end: {2}", offset,
	
	            //    _forwardBufferStart, _forwardBufferEnd));
	
	 
	
	            pos = offset + BUFFER_SIZE;
	
	            if (pos > _fileSize) { pos = (int)_fileSize; }
	
	 
	
	            //Trace(string.Format("-debug- f position: {0}", pos));
	
	 
	
	            _forwardBufferStart = offset;
	
	            _forwardBufferEnd = pos;
	
	 
	
	            //Trace(string.Format("-debug- f buffer start: {0}, buffer end: {1}",
	
	            //    _forwardBufferStart, _forwardBufferEnd));
	
	 
	
	            _br.BaseStream.Seek(offset, SeekOrigin.Begin);
	
	            _br.Read(_forwardBuffer, 0, BUFFER_SIZE);
	
	        }
	
	 
	
	        //Trace(string.Format("-debug- f offset after creating/if statements: {0}, buffer start: {1}",
	
	        //        offset, _forwardBufferStart));
	
	 
	
	        // got new buffer or still  in range
	
	        return _forwardBuffer[offset - _forwardBufferStart];
	
	    }
	
	 
	
	    int GetSize(int offset)
	
	    {
	
	        return GetByte(offset) * 256 + GetByte(offset + 1);
	
	    }
	
	 
	
	    /// <summary>
	
	    /// Find the specified marker given a starting position
	
	    /// </summary>
	
	    /// <param name="offset">Position to start searching from</param>
	
	    /// <param name="marker">Marker to search for</param>
	
	    /// <returns>True and the offset if the maker is found</returns>
	
	    bool EnsureMPEG(out int offset, byte marker)
	
	    {
	
	        offset = 0;
	
	        for (int i = 0; i < _fileSize - 4; i++)
	
	        {
	
	            if (GetByte(i) == 0x00
	
	                && GetByte(i + 1) == 0x00
	
	                && GetByte(i + 2) == 0x01
	
	                && GetByte(i + 3) == marker)
	
	            {
	
	                offset = i;
	
	                return true;
	
	            }
	
	        }
	
	        return false;
	
	    }
	
	 
	
	    /// <summary>
	
	    /// Find a specific marker by going through the file in reverse
	
	    /// TODO: Improve this (GetByte() is slow when using this method because it seeks from the start
	
	    /// </summary>
	
	    /// <param name="offset">Position to start searching from</param>
	
	    /// <param name="marker">Marker to search for</param>
	
	    /// <returns>Position of the marker</returns>
	
	    int FindMarkerBackwards(int offset, byte marker)
	
	    {
	
	 
	
	        for (int i = offset; i > 0; i--)
	
	        {
	
	            if (GetByteBackwards(i) == 0x00
	
	                && GetByteBackwards(i + 1) == 0x00
	
	                && GetByteBackwards(i + 2) == 0x01
	
	                && GetByteBackwards(i + 3) == marker)
	
	            {
	
	                return i;
	
	            }
	
	        }
	
	        return -1;
	
	    }
	
	 
	
	    bool MarkerExistsAt(out int offset, byte marker, int from) // self explanatory / see above
	
	    {
	
	        offset = -1;
	
	        for (int i = from; i < _fileSize - 4; i++)
	
	        {
	
	            if (GetByte(i) == 0x00
	
	                && GetByte(i + 1) == 0x00
	
	                && GetByte(i + 2) == 0x01
	
	                && GetByte(i + 3) == marker)
	
	            {
	
	                offset = i;
	
	                return true;
	
	            }
	
	        }
	
	        return false;
	
	    }
	
	 
	
	    int FindNextMarker(int from, byte marker) // overloaded method / see above
	
	    {
	
	        int offset = from;
	
	        while (from >= 0 && from < _fileSize - 4)
	
	        {
	
	            offset = FindNextMarker(from);
	
	 
	
	            if (offset == -1)
	
	            {
	
	                return -1;
	
	            }
	
	            else if (MarkerExistsAt(out offset, marker, from))
	
	            {
	
	                return offset;
	
	            }
	
	            else
	
	            {
	
	                from++;
	
	            }
	
	        }
	
	        return -1;
	
	    }
	
	 
	
	    int FindNextMarker(int from, ref byte marker) // see above
	
	    {
	
	        int offset = FindNextMarker(from);
	
	        if (offset > -1)
	
	        {
	
	            marker = GetByte(offset + 3);
	
	            return offset;
	
	        }
	
	        return -1;
	
	    }
	
	 
	
	    int FindNextMarker(int from) // see above
	
	    {
	
	        int offset;
	
	        for (offset = from; offset < (_fileSize - 4); offset++)
	
	        {
	
	            if (
	
	                (GetByte(offset + 0) == 0x00) &&
	
	                (GetByte(offset + 1) == 0x00) &&
	
	                (GetByte(offset + 2) == 0x01))
	
	            {
	
	                return offset;
	
	            }
	
	        }
	
	        return -1;
	
	    }
	
	 
	
	    bool hasAudio = false;
	
	    void ParseSystem(int offset)
	
	    {
	
	        bool keepGoing = true;
	
	        byte mark = 0x00;
	
	 
	
	        if (EnsureMPEG(out offset, 0xBA)) // make sure we'r parsing a mpeg header
	
	        {
	
	            //FindMuxRate(offset + 4);
	
	 
	
	            int packLength = 0;
	
	            int packetSize = 0;
	
	            byte packetType = new byte();
	
	 
	
	            while (keepGoing)
	
	            {
	
	                offset = FindNextMarker(offset, ref mark);
	
	 
	
	                if (offset == -1)
	
	                {
	
	                    break; // valid marker not found
	
	                }
	
	 
	
	                if (mark == VIDEO_PACKET || mark == AUDIO_PACKET)
	
	                {
	
	                    break; // end of system header
	
	                }
	
	 
	
	                if (mark == PADDING_PACKET)
	
	                {
	
	                    offset += GetSize(offset + 4);
	
	                    continue;
	
	                }
	
	 
	
	                if (mark == 0xBA)
	
	                {
	
	                    FindMuxRate(offset + 4);
	
	                    offset += 12;
	
	                    continue;
	
	                }
	
	 
	
	                if (mark != SYSTEM_PACKET)
	
	                {
	
	                    offset += 4;
	
	                    continue;
	
	                }
	
	 
	
	                int startOfPack = FindNextMarker(offset, 0xBA);
	
	                if (startOfPack != -1)
	
	                {
	
	                    if ((GetByte(startOfPack + 4) & 0xF0) == 0x20)
	
	                    {
	
	                        packLength = 12; // mpeg 1
	
	                    }
	
	                    else
	
	                    {
	
	                        if ((GetByte(startOfPack + 4) & 0xC0) == 0x40)
	
	                        {
	
	                            // mpeg 2 pack + stuffing
	
	                            packLength = 14 + (GetByte(startOfPack + 13) & 0x07);
	
	                        }
	
	                        else
	
	                        {
	
	                            packLength = 12; // somethings not right
	
	                        }
	
	                    }
	
	                }
	
	 
	
	                if (startOfPack == -1 || (startOfPack + packLength != offset))
	
	                {
	
	                    //Trace(string.Format("-debug- startOfPack: {0}, packLength: {1}, offset: {2}",
	
	                    //   startOfPack, packLength, offset));
	
	 
	
	                    startOfPack = offset;
	
	                    //return; keep going anyway
	
	                }
	
	 
	
	                // TODO: Implement ParseSystemPacket
	
	 
	
	                packetSize = GetSize(offset + 4);
	
	                packetType = GetByte(offset + 12);
	
	 
	
	                //Trace(string.Format("-debug- packetSize: {0}, packetType: {1}, packLength: {2}, offset: {3}",
	
	                //    packetSize, packetType, packLength, offset));
	
	 
	
	                if (GetByte(offset + 15) == AUDIO_PACKET || GetByte(offset + 15) == VIDEO_PACKET)
	
	                {
	
	                    packetType = VIDEO_PACKET;
	
	                }
	
	 
	
	                if (packetType == AUDIO_PACKET)
	
	                {
	
	                    // TODO: Implement this if needed
	
	                }
	
	                else if (packetType == VIDEO_PACKET)
	
	                {
	
	                    // here we are finally!, this part will fix up the durations
	
	                    if (packLength == 12)
	
	                    {
	
	                        _initialTS = ReadTS(offset - packLength); // find the initial timestamp
	
	                        _mpeg2Found = false; // pack 12 so its not mpeg 2
	
	                    }
	
	                    else
	
	                    {
	
	                        _initialTS = ReadTSMpeg2(offset - packLength);
	
	                        _mpeg2Found = true;
	
	                    }
	
	 
	
	                    //Trace(string.Format("-debug- initial ts: {0}", _initialTS));
	
	                }
	
	                offset += 4; // keep going
	
	            }
	
	        }
	
	 
	
	        /* hopefully it gets to here without crashing :P
	
	        * start searching from the end of the file and -12 because a pack is usually 12 bytes long
	
	        */
	
	        int lastPack = FindMarkerBackwards((int)_fileSize - 13, 0xBA);
	
	 
	
	        //Trace(string.Format("-debug- lastPack: {0}", lastPack));
	
	 
	
	        double duration;
	
	        if ((GetByte(lastPack + 4) & 0xF0) == 0x20)
	
	        {
	
	            duration = ReadTS(lastPack + 4); // mpeg 1
	
	        }
	
	        else
	
	        {
	
	            lastPack = FindMarkerBackwards((int)_fileSize - 8, 0xB8);
	
	 
	
	            //Trace(string.Format("-debug- lastPack 2: {0}", lastPack));
	
	 
	
	            if (((GetByte(lastPack + 4) & 0xC0) == 0x40) || _mpeg2Found)
	
	            {
	
	                lastPack = FindMarkerBackwards((int)_fileSize - 8, 0xBA);
	
	 
	
	                //Trace(string.Format("-debug- lastPack 3: {0}", lastPack));
	
	 
	
	                duration = ReadTSMpeg2(lastPack + 4);
	
	            }
	
	            else
	
	            {
	
	                duration = ReadTSMpeg2(lastPack + 4);
	
	            }
	
	        }
	
	 
	
	        //Trace(string.Format("-debug- duration before: {0}", duration));
	
	        duration -= _initialTS; // fix up the duration
	
	        //Trace(string.Format("-debug- duration after: {0}", duration));
	
	 
	
	        if (this.VideoInfo.Duration > 0)
	
	        {
	
	            if (this.VideoInfo.Duration > duration)
	
	            {
	
	                this.VideoInfo.Duration = duration; // only update if existing is more than current
	
	            }
	
	        }
	
	 
	
	        //Trace(string.Format("-debug- video duration: {0}", SecondsToHMS(this.VideoInfo.Duration)));
	
	    }
	
	 
	
	    double ReadTS(int offset)
	
	    {
	
	        byte highBit;
	
	        UInt32 low4Bytes; // needs to be UInt32 otherwise won't work
	
	        double ts;
	
	 
	
	        highBit = (byte)((GetByte(offset) >> 3) & 0x01);
	
	 
	
	        low4Bytes = (UInt32)(((GetByte(offset) >> 1) & 0x03) << 30);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 1) << 22);
	
	        low4Bytes |= (UInt32)((GetByte(offset + 2) >> 1) << 15);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 3) << 7);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 4) >> 1);
	
	 
	
	        ts = (double)(highBit * FLOAT_0x10000 * FLOAT_0x10000);
	
	        ts += (double)(low4Bytes);
	
	        ts /= (double)STD_SYSTEM_CLOCK_FREQ;
	
	 
	
	        return ts;
	
	    }
	
	 
	
	    double ReadTSMpeg2(int offset)
	
	    {
	
	        byte highBit;
	
	        UInt32 low4Bytes;
	
	        UInt32 sysClockRef;
	
	        double ts;
	
	 
	
	        highBit = (byte)((GetByte(offset) & 0x20) >> 5);
	
	 
	
	        low4Bytes = (UInt32)(((GetByte(offset) & 0x18) >> 3) << 30);
	
	        low4Bytes |= (UInt32)((GetByte(offset) & 0x03) << 28);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 1) << 20);
	
	        low4Bytes |= (UInt32)((GetByte(offset + 2) & 0xF8) << 12);
	
	        low4Bytes |= (UInt32)((GetByte(offset + 2) & 0x03) << 13);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 3) << 5);
	
	        low4Bytes |= (UInt32)(GetByte(offset + 4) >> 3);
	
	 
	
	        sysClockRef = (UInt32)((GetByte(offset + 4) & 0x3) << 7);
	
	        sysClockRef |= (UInt32)((GetByte(offset + 5) >> 1));
	
	 
	
	        ts = (double)(highBit * FLOAT_0x10000 * FLOAT_0x10000);
	
	        ts += (double)low4Bytes;
	
	 
	
	        // TODO: fix this up (confirm with mpgtx first)
	
	        if (sysClockRef == 0)
	
	        {
	
	            ts /= (double)STD_SYSTEM_CLOCK_FREQ;
	
	        }
	
	        else
	
	        {
	
	            ts /= (double)STD_SYSTEM_CLOCK_FREQ;
	
	        }
	
	 
	
	        return ts;
	
	    }
	
	 
	
	    void FindMuxRate(int offset)
	
	    {
	
	        int muxrate = 0;
	
	        if ((GetByte(offset) & 0xC0) == 0x40)
	
	        {
	
	            muxrate = GetByte(offset + 6) << 14;
	
	            muxrate |= GetByte(offset + 7) << 6;
	
	            muxrate |= GetByte(offset + 8) >> 2;
	
	 
	
	        }
	
	        else
	
	        {
	
	            if ((GetByte(offset) & 0xF0) != 0x20)
	
	            {
	
	                //Trace("-debug- invalid offset for mux rate");
	
	            }
	
	            muxrate = (GetByte(offset + 5) & 0x7F) << 15;
	
	            muxrate |= (GetByte(offset + 6) << 7);
	
	            muxrate |= (GetByte(offset + 7) >> 1);
	
	        }
	
	        muxrate *= 50;
	
	        this.VideoInfo.MuxRate = (double)((muxrate * 8.0) / 1000000.0);
	
	 
	
	        //Trace(string.Format("system mux rate: {0} mbps", this.VideoInfo.MuxRate.ToString("n2")));
	
	    }
	
	 
	
	    void ParseAudio()
	
	    {
	
	        int offset = 0;
	
	        hasAudio = false;
	
	 
	
	        //            Trace("starting to parse audio");
	
	 
	
	        offset = FindNextMarker(0, 0xC0); // find the audio marker
	
	 
	
	        if (offset <= -1)
	
	        {
	
	            return;
	
	        }
	
	 
	
	        if (offset > -1)
	
	        {
	
	            offset += 13;
	
	        }
	
	 
	
	        //            Trace(string.Format("offset of audio: {0}", offset));
	
	 
	
	        if (!ParseAudio(offset))
	
	        {
	
	            //                Trace("failed to parse audio");
	
	 
	
	            while ((offset < _fileSize - 10) && !hasAudio) // go through the file until we find an audio marker
	
	            {
	
	                //                    Trace(string.Format("looping to find audio offset: {0}", offset));
	
	 
	
	                if ((GetByte(offset) == 0xFF) && (GetByte(offset + 1) & 0xF0) == 0xF0) // found the proper sequence
	
	                {
	
	                    if (ParseAudio(offset))
	
	                    {
	
	                        //                            Trace("found audio");
	
	                        //this.AudioInfo.Frames++; this doesn't work right
	
	                        hasAudio = true;
	
	                    }
	
	                }
	
	                offset++; // try next offset
	
	            }
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio frames: {0}", this.AudioInfo.Frames));
	
	    }
	
	 
	
	    bool ParseAudio(int offset)
	
	    {
	
	        hasAudio = false;
	
	 
	
	        bool mpeg25 = false;
	
	        if ((GetByte(offset + 0) != 0xFF) || ((GetByte(offset + 1) & 0xF0) != 0xF0)) // see if this has mpeg 2.5
	
	        {
	
	            if ((GetByte(offset + 0) != 0xFF) || ((GetByte(offset + 1) & 0xE0) != 0xE0))
	
	            {
	
	                return false; // invalid sequence exit and try a different offset
	
	            }
	
	            else
	
	            {
	
	                mpeg25 = true;
	
	            }
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- offset: {0}", offset));
	
	        //            Trace(string.Format("-debug- mpeg25: {0}", mpeg25));
	
	 
	
	        if (Convert.ToBoolean((GetByte(offset + 1) & 0x08)))
	
	        {
	
	            if (!mpeg25)
	
	            {
	
	                this.AudioInfo.Version = 1.0d;
	
	            }
	
	            else
	
	            {
	
	                //                    Trace(string.Format("-debug- invalid 01"));
	
	                return false; // invalid version exit and try a different offset
	
	            }
	
	        }
	
	        else
	
	        {
	
	            if (!mpeg25)
	
	            {
	
	                this.AudioInfo.Version = 2.0d;
	
	            }
	
	            else
	
	            {
	
	                this.AudioInfo.Version = 3.0d; // for 2.5
	
	            }
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio version: {0}", this.AudioInfo.Version));
	
	 
	
	        this.AudioInfo.Layer = (GetByte(offset + 1) & 0x06) >> 1;
	
	 
	
	        switch (this.AudioInfo.Layer)
	
	        {
	
	            case 0:
	
	                this.AudioInfo.Layer = -1;
	
	                return false; // invalid layer exit and try a different offset
	
	                break;
	
	            case 1:
	
	                this.AudioInfo.Layer = 3;
	
	                break;
	
	            case 2:
	
	                this.AudioInfo.Layer = 2;
	
	                break;
	
	            case 3:
	
	                this.AudioInfo.Layer = 1;
	
	                break;
	
	            default:
	
	                this.AudioInfo.Layer = -1;
	
	                return false;
	
	                break;
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio layer: {0}", this.AudioInfo.Layer));
	
	 
	
	        // TODO: confirm if 0 is true or 1 is true, mpgtx has 0 is false and 1 is true?
	
	        this.AudioInfo.Protected = Convert.ToBoolean(GetByte(offset + 1) & 0x01);
	
	 
	
	        //            Trace(string.Format("-debug- audio protection: {0}", this.AudioInfo.Protected));
	
	 
	
	        int bitrateIndex = GetByte(offset + 2) >> 4;
	
	        int samplingIndex = (GetByte(offset + 2) & 0x0F) >> 2;
	
	 
	
	        if (samplingIndex >= 3) { return false; } // invalid indexes, try another offset
	
	        if (bitrateIndex == 15) { return false; }
	
	 
	
	        this.AudioInfo.BitRate = BitRateIndex[(int)this.AudioInfo.Version - 1, this.AudioInfo.Layer - 1, bitrateIndex];
	
	        this.AudioInfo.ByteRate = (float)((this.AudioInfo.BitRate * 1000) / 8.0d);
	
	        this.AudioInfo.SamplingRate = SamplingIndex[(int)this.AudioInfo.Version - 1, samplingIndex];
	
	 
	
	        if (this.AudioInfo.BitRate <= 0
	
	            || this.AudioInfo.ByteRate <= 0
	
	            || this.AudioInfo.SamplingRate <= 0) { return false; }
	
	 
	
	        //Trace(string.Format("-debug- audio bit rate: {0}", this.AudioInfo.BitRate));
	
	        //Trace(string.Format("-debug- audio byte rate: {0}", this.AudioInfo.ByteRate));
	
	        //Trace(string.Format("-debug- audio sampling rate: {0}", this.AudioInfo.SamplingRate));
	
	 
	
	        if (Convert.ToBoolean(GetByte(offset + 2) & 0x02))
	
	        {
	
	            this.AudioInfo.Padding = true;
	
	        }
	
	        else
	
	        {
	
	            this.AudioInfo.Padding = false;
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio padding: {0}", this.AudioInfo.Padding));
	
	 
	
	        this.AudioInfo.ModeCode = GetByte(offset + 3) >> 6;
	
	 
	
	        //            Trace(string.Format("-debug- audio mode: {0}", ModeIndex[this.AudioInfo.ModeCode]));
	
	 
	
	        //TODO: add matching mode extension text
	
	        int modeExt = (GetByte(offset + 3) >> 4) & 0x03;
	
	 
	
	        //Trace(string.Format("-debug- audio modeext: {0}", modeExt));
	
	 
	
	        if (Convert.ToBoolean(GetByte(offset + 3) & 0x08))
	
	        {
	
	            this.AudioInfo.Copyright = true;
	
	        }
	
	        else
	
	        {
	
	            this.AudioInfo.Copyright = false;
	
	        }
	
	 
	
	        //Trace(string.Format("-debug- audio copyright: {0}", this.AudioInfo.Copyright));
	
	 
	
	        if (Convert.ToBoolean(GetByte(offset + 3) & 0x04))
	
	        {
	
	            this.AudioInfo.Original = true;
	
	        }
	
	        else
	
	        {
	
	            this.AudioInfo.Original = false;
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio original: {0}", this.AudioInfo.Original));
	
	 
	
	        this.AudioInfo.EmphasisIndex = GetByte(offset + 3) & 0x03;
	
	 
	
	        //            Trace(string.Format("-debug- audio emphasis index: {0}",
	
	        //EmphasisIndex[this.AudioInfo.EmphasisIndex]));
	
	 
	
	        if (this.AudioInfo.Version == 1)
	
	        {
	
	            if (this.AudioInfo.Layer == 1)
	
	            {
	
	                this.AudioInfo.FrameLength =
	
	                    ((48000 * this.AudioInfo.BitRate) / this.AudioInfo.SamplingRate)
	
	                + 4 * Convert.ToInt32(this.AudioInfo.Padding);
	
	            }
	
	            else
	
	            {
	
	                this.AudioInfo.FrameLength =
	
	                   ((144000 * this.AudioInfo.BitRate) / this.AudioInfo.SamplingRate)
	
	               + Convert.ToInt32(this.AudioInfo.Padding);
	
	            }
	
	        }
	
	        else if (this.AudioInfo.Version == 2)
	
	        {
	
	            if (this.AudioInfo.Layer == 1)
	
	            {
	
	                this.AudioInfo.FrameLength =
	
	                    ((24000 * this.AudioInfo.BitRate) / this.AudioInfo.SamplingRate)
	
	                + 4 * Convert.ToInt32(this.AudioInfo.Padding);
	
	            }
	
	            else
	
	            {
	
	                this.AudioInfo.FrameLength =
	
	                   ((72000 * this.AudioInfo.BitRate) / this.AudioInfo.SamplingRate)
	
	               + Convert.ToInt32(this.AudioInfo.Padding);
	
	            }
	
	        }
	
	        else
	
	        {
	
	            //                Trace(string.Format("-debug- audio layer is invalid"));
	
	            return false;
	
	        }
	
	 
	
	        if (this.AudioInfo.Protected)
	
	        {
	
	            // frame length sometimes gets offset by +- 2, got to find out why its happening, most likely its the protection switch (see above)
	
	            this.AudioInfo.FrameLength += 2;
	
	        }
	
	 
	
	        //            Trace(string.Format("-debug- audio frame length: {0}", this.AudioInfo.FrameLength));
	
	 
	
	        this.AudioInfo.Duration = ((_fileSize * 1.0d) / this.AudioInfo.BitRate) * 0.008d;
	
	 
	
	        //            Trace(string.Format("-debug- audio duration: {0}", SecondsToHMS(this.AudioInfo.Duration)));
	
	 
	
	        //CountAudioFrames();
	
	 
	
	        hasAudio = true;
	
	        return hasAudio;
	
	    }
	
	 
	
	    bool ParseVideo(int offset)
	
	    {
	
	        bool foundVideo = false;
	
	 
	
	        //            Trace(string.Format("-debug- file size: {0}", _fileSize));
	
	 
	
	        if (EnsureMPEG(out offset, 0xB3))
	
	        {
	
	            foundVideo = true;
	
	        }
	
	 
	
	        if (foundVideo)
	
	        {
	
	            //                Trace(string.Format("-debug- offset: {0}", offset));
	
	            offset += 4; // move to the header location
	
	 
	
	            this.VideoInfo.Width = GetSize(offset) >> 4;
	
	            this.VideoInfo.Height = GetSize(offset + 1) & 0x0FFF;
	
	 
	
	            //                Trace(string.Format("-debug- video height: {0}", this.VideoInfo.Height));
	
	            //                Trace(string.Format("-debug- video width: {0}", this.VideoInfo.Width));
	
	 
	
	            offset += 3;
	
	            int frameRateIndex = GetByte(offset) & 0x0F;
	
	            if (frameRateIndex > 8)
	
	            {
	
	                this.VideoInfo.FrameRate = 0.0d;
	
	            }
	
	            else
	
	            {
	
	                this.VideoInfo.FrameRate = FrameRateIndex[frameRateIndex];
	
	            }
	
	 
	
	            //                Trace(string.Format("-debug- video frame rate: {0}", this.VideoInfo.FrameRate));
	
	 
	
	            this.VideoInfo.AspectRatioCode = (GetByte(offset) & 0xF0) >> 4;
	
	            if (this.VideoInfo.AspectRatioCode <= 4)
	
	            {
	
	                this.VideoInfo.AspectRatio = AspectRatioIndex[this.VideoInfo.AspectRatioCode];
	
	            }
	
	            else
	
	            {
	
	                this.VideoInfo.AspectRatio = "Unknown";
	
	            }
	
	 
	
	            //                Trace(string.Format("-debug- video aspect ratio code: {0}, string: {1}",
	
	            //this.VideoInfo.AspectRatioCode, this.VideoInfo.AspectRatio));
	
	 
	
	 
	
	            offset += 1;
	
	 
	
	            this.VideoInfo.BitRate = GetSize(offset);
	
	            //Trace(string.Format("-debug- video bit rate: {0}", this.VideoInfo.BitRate));
	
	            this.VideoInfo.BitRate <<= 2;
	
	            //Trace(string.Format("-debug- video bit rate: {0}", this.VideoInfo.BitRate));
	
	            byte lastTwo = GetByte(offset + 2);
	
	            //Trace(string.Format("-debug- video lastTwo: {0}", lastTwo));
	
	            lastTwo >>= 6;
	
	            //Trace(string.Format("-debug- video lastTwo: {0}", lastTwo));
	
	            this.VideoInfo.BitRate |= lastTwo;
	
	 
	
	            //Trace(string.Format("-debug- video bit rate: {0}", this.VideoInfo.BitRate));
	
	 
	
	            this.VideoInfo.Duration = (_fileSize) /
	
	                ((this.VideoInfo.BitRate * 400) / 8.0); // this gets fixed later when ParseSystem runs
	
	 
	
	            //Trace(string.Format("-debug- video duration: {0}", SecondsToHMS(this.VideoInfo.Duration)));
	
	 
	
	            //Trace(string.Format("-debug- video speed: {0} Mbps",
	
	            //(this.VideoInfo.BitRate / 2500).ToString("n2"))); // always 15mb/s?
	
	 
	
	            // this part is needed to find video format and chroma format
	
	            byte mark = new byte();
	
	            while (true)
	
	            {
	
	                offset = FindNextMarker(offset, ref mark);
	
	                if (offset > -1)
	
	                {
	
	                    if (mark == 0xB8) { break; }
	
	                    switch (GetByte(offset + 3))
	
	                    {
	
	                        case 0xB5:
	
	                            ParseExtension(offset);
	
	                            break;
	
	                        default:
	
	                            break;
	
	                    }
	
	                }
	
	                else
	
	                {
	
	                    //Trace(string.Format("-debug- video offset is -1"));
	
	                    break;
	
	                }
	
	                offset++;
	
	            }
	
	 
	
	            switch (this.VideoInfo.ChromaFormat)
	
	            {
	
	                case 1:
	
	                    this.VideoInfo.ChromaFormatText = "4:2:0";
	
	                    break;
	
	                case 2:
	
	                    this.VideoInfo.ChromaFormatText = "4:2:2";
	
	                    break;
	
	                case 3:
	
	                    this.VideoInfo.ChromaFormatText = "4:4:4";
	
	                    break;
	
	                default:
	
	                    this.VideoInfo.ChromaFormatText = "Unknown";
	
	                    break;
	
	            }
	
	 
	
	            switch (this.VideoInfo.Format)
	
	            {
	
	                case 0:
	
	                    this.VideoInfo.FormatText = "Component";
	
	                    break;
	
	                case 1:
	
	                    this.VideoInfo.FormatText = "PAL";
	
	                    break;
	
	                case 2:
	
	                    this.VideoInfo.FormatText = "NTSC";
	
	                    break;
	
	                case 3:
	
	                    this.VideoInfo.FormatText = "SECAM";
	
	                    break;
	
	                case 4:
	
	                    this.VideoInfo.FormatText = "MAC";
	
	                    break;
	
	                case 5:
	
	                    this.VideoInfo.FormatText = "Unspecified";
	
	                    break;
	
	                default:
	
	                    this.VideoInfo.FormatText = "Unknown";
	
	                    break;
	
	            }
	
	 
	
	            //Trace(string.Format("-debug- video chroma format: {0} is {1}",
	
	            //    this.VideoInfo.ChromaFormat, this.VideoInfo.ChromaFormatText));
	
	            //Trace(string.Format("-debug- video format: {0} is {1}",
	
	            //    this.VideoInfo.Format, this.VideoInfo.FormatText));
	
	 
	
	            //CountVideoFrames();// slows down the routine
	
	 
	
	            // Trace(string.Format("-debug- video frames: {0}", this.VideoInfo.Frames));
	
	        }
	
	 
	
	        return true;
	
	    }
	
	 
	
	    // remove this and use TimeSpan
	
	    string SecondsToHMS(double duration)
	
	    {
	
	        int hours = (int)(duration / 3600);
	
	        int mins = (int)((duration / 60) - (hours * 60));
	
	        double seconds = duration - 60 * mins - 3600 * hours;
	
	 
	
	        if (hours != 0)
	
	        {
	
	            return string.Format("{0:n0}h {1:n0}m {2:n2}s", hours, mins, seconds);
	
	        }
	
	        if (mins != 0)
	
	        {
	
	            return string.Format("{0:n0}m {1:n2}s", mins, seconds);
	
	        }
	
	        return string.Format("{0:n2}s", seconds);
	
	    }
	
	 
	
	    void CountVideoFrames() // slow, find a better way of doing this
	
	    {
	
	        byte[] buffer = new byte[4];
	
	        _br.BaseStream.Seek(0, SeekOrigin.Begin);
	
	 
	
	        int read = 0;
	
	        while ((read = _br.Read(buffer, 0, 4)) != 0)
	
	        {
	
	            if (buffer[0] == 0x00
	
	                && buffer[1] == 0x00
	
	                && buffer[2] == 0x01
	
	                && buffer[3] == 0x00)
	
	            {
	
	                this.VideoInfo.Frames++;
	
	            }
	
	        }
	
	    }
	
	 
	
	    void CountVideoGopHeaders() // slow, find a better way of doing this
	
	    {
	
	        byte[] buffer = new byte[4];
	
	        _br.BaseStream.Seek(0, SeekOrigin.Begin);
	
	 
	
	        int read = 0;
	
	        while ((read = _br.Read(buffer, 0, 4)) != 0)
	
	        {
	
	            if (buffer[0] == 0x00
	
	                && buffer[1] == 0x00
	
	                && buffer[2] == 0x01
	
	                && buffer[3] == 0xB8)
	
	            {
	
	                this.VideoInfo.GopHeaders++;
	
	            }
	
	        }
	
	    }
	
	 
	
	    void ParseExtension(int offset)
	
	    {
	
	        offset += 4;
	
	        switch (GetByte(offset) >> 4)
	
	        {
	
	            case 1: ParseSequenceExt(offset);
	
	                break;
	
	            case 2:
	
	                ParseSequenceDisplayExt(offset);
	
	                break;
	
	            default:
	
	                break;
	
	        }
	
	    }
	
	 
	
	    void ParseSequenceExt(int offset)
	
	    {
	
	        _mpegVersion = 2;
	
	        this.VideoInfo.Version = 2;
	
	        this.VideoInfo.ChromaFormat = (GetByte(offset + 1) & 0x06) >> 1;
	
	    }
	
	 
	
	    void ParseSequenceDisplayExt(int offset)
	
	    {
	
	        this.VideoInfo.Format = (GetByte(offset) & 0x0E) >> 1;
	
	    }
	
	 
	
	    int SkipPacketHeader(int offset)
	
	    {
	
	        byte mark = new byte();
	
	        if (_mpegVersion == 1)
	
	        {
	
	            offset += 6;
	
	            mark = GetByte(offset);
	
	            while (Convert.ToBoolean(mark & 0x80))
	
	            {
	
	                mark = GetByte(++offset);
	
	            }
	
	 
	
	            if ((mark & 0xC0) == 0x40)
	
	            {
	
	                offset += 2;
	
	            }
	
	 
	
	            mark = GetByte(offset);
	
	            if ((mark & 0xF0) == 0x20) { offset += 5; }
	
	            else if ((mark & 0xF0) == 0x30) { offset += 10; }
	
	            else offset++;
	
	 
	
	            return offset;
	
	        }
	
	        else if (_mpegVersion == 2)
	
	        {
	
	            return (offset + 9 + GetByte(offset + 8));
	
	        }
	
	        else return (offset + 10);
	
	    }
	
	 
	
	    void Trace(string msg)
	
	    {
	
	        if (this.EnableTrace)
	
	        {
	
	            Console.WriteLine(msg);
	
	        }
	
	    }
	
	}

}
