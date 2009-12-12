
using System;

namespace ThemonospotPluginAvi
{
	
	
	public static class AviConstants
	{
		
		#region AVI Constants
		
		
        // AVIMAINHEADER flags
        public static readonly int AVIF_HASINDEX = 0x00000010; // Index at end of file?
        public static readonly int AVIF_MUSTUSEINDEX = 0x00000020;
        public static readonly int AVIF_ISINTERLEAVED = 0x00000100;
        public static readonly int AVIF_TRUSTCKTYPE = 0x00000800; // Use CKType to find key frames
        public static readonly int AVIF_WASCAPTUREFILE = 0x00010000;
        public static readonly int AVIF_COPYRIGHTED = 0x00020000;

        // AVISTREAMINFO flags
        public static readonly int AVISF_DISABLED = 0x00000001;
        public static readonly int AVISF_VIDEO_PALCHANGES = 0x00010000;

        // AVIOLDINDEXENTRY flags
        public static readonly int AVIIF_LIST = 0x00000001;
        public static readonly int AVIIF_KEYFRAME = 0x00000010;
        public static readonly int AVIIF_NO_TIME = 0x00000100;
        public static readonly int AVIIF_COMPRESSOR = 0x0FFF0000;  // unused?

        // TIMECODEDATA flags
        public static readonly int TIMECODE_SMPTE_BINARY_GROUP = 0x07;
        public static readonly int TIMECODE_SMPTE_COLOR_FRAME = 0x08;

		
		
		// AVI  Constants
		public static readonly string RIFF = "RIFF";
		public static readonly string RIFX = "RIFX";
		

        // AVI stream FourCC codes (four-character code)
        public static uint StreamtypeVIDEO 
		{	get {return	new Utility().GetIntFromFourcc("vids");	}	}

		public static uint StreamtypeAUDIO
		{	get {return	new Utility().GetIntFromFourcc("auds");	}	}
		
		public static uint StreamtypeMIDI 
		{	get {return	new Utility().GetIntFromFourcc("mids");	}	}

        public static uint StreamtypeTEXT 
		{	get {return	new Utility().GetIntFromFourcc("txts");	}	}



		// AVI section FourCC codes
		public static uint IdAVIHeaderTypeAvi
		{	get {return	new Utility().GetIntFromFourcc("AVI ");	}	}

		public static uint IdAVIHeaderTypeWave
		{	get {return	new Utility().GetIntFromFourcc("WAVE");	}	}
		
		public static uint IdAVIHeaderTypeRMid
		{	get {return	new Utility().GetIntFromFourcc("RMID");	}	}
		
        public static uint IdMainAVIList 
		{	get {return	new Utility().GetIntFromFourcc("LIST");	}	}
		
		public static uint IdAVIHeaderList
		{	get {return	new Utility().GetIntFromFourcc("hdrl");	}	}
				
        public static uint IdMainAVIHeader 
		{	get {return	new Utility().GetIntFromFourcc("avih");	}	}
		
        public static uint IdODML 
		{	get {return	new Utility().GetIntFromFourcc("odml");	}	}
		
        public static uint IdAVIExtHeader 
		{	get {return	new Utility().GetIntFromFourcc("dmlh");	}	}
		
        public static uint IdAVIStreamList 
		{	get {return	new Utility().GetIntFromFourcc("strl");	}	}
		
        public static uint IdAVIStreamHeader 
		{	get {return	new Utility().GetIntFromFourcc("strh");	}	}
		
        public static uint IdAVIStreamData
		{	get {return	new Utility().GetIntFromFourcc("strd");	}	}
		
        public static uint IdAVIStreamFormat 
		{	get {return	new Utility().GetIntFromFourcc("strf");	}	}
		
        public static uint IdAVIMovieData 
		{	get {return	new Utility().GetIntFromFourcc("movi");	}	}
		
        public static uint IdAVIOldIndex
		{	get {return	new Utility().GetIntFromFourcc("idx1");	}	}
		
		public static uint IdINFOList
		{	get {return	new Utility().GetIntFromFourcc("INFO");	}	}
		
		public static uint IdJUNKTag
		{	get {return	new Utility().GetIntFromFourcc("JUNK");	}	}
		
		public static uint IdAVIISFT
		{	get {return	new Utility().GetIntFromFourcc("ISFT");	}	}
		
		public static uint IdWaveFMT
		{	get {return	new Utility().GetIntFromFourcc("fmt ");	}	}
		
		public static uint IdMovieWaveTrack 
		{	get {return	new Utility().GetIntFromFourcc("01wb");	}	}
		
		public static uint IdMovieVideoTrack
		{	get {return	new Utility().GetIntFromFourcc("00dc");	}	}
		

		
		
		
		public static readonly byte[] UserDataBytes = {0x00, 0x00, 0x01, 0xB2}; // User Data 
		public static readonly byte[] VOLStartBytes = {0x00, 0x00, 0x01, 0x20}; // Video Object Layer (VOL)
		public static readonly byte[] VOPStartBytes = {0x00, 0x00, 0x01, 0xB6}; // Video Object Plane (VOP)
		
		
		// Audio codec
		public static readonly int IdAudioMP3 = 0x0055;
		
		
		
		#endregion AVI Constants
		
		
	}
}
