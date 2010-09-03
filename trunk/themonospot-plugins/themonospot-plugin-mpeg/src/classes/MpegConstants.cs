
using System;

namespace ThemonospotPluginMpeg
{


	public class MpegConstants
	{


#region Constants
		
		// MPEG PROGRAM STREAM
		public static readonly string MPEGPS_START_CODE       		= "000001";
        public static readonly string MPEGPS_PACKID  		  		= "000001BA";
		public static readonly string MPEGPS_PARTIAL_HEADER	  		= "000001BB";
		
		
		
		public static readonly double[] FrameRateTable = new double[]
		{ 
			0.0d, 
			24000d/1001d, 
			24d, 
			25d,	
	        30000d/1001d, 
			30d, 
			50d, 
			60000d/1001d, 
			60d 
		};
		
		
		
		public static readonly string[] AspectRatio = new string[]
		{
	    	"Invalid",
			"1/1 (VGA)",
			"4/3 (TV)",
			"16/9 (Large TV)",
			"2.21/1 (Cinema)"
        };
		
		
		public static readonly string[] ChromaFormat = new string[]
		{
	    	"_",
			"4:2:0",
			"4:2:2",
			"4:4:4"
        };
		
		
		public static readonly string[] VideoFormat = new string[]
		{
	    	"Component",
			"PAL",
			"NTSC",
			"SECAM",
			"MAC",
			"Unspecified"
        };
		
		
		public static readonly int[, ,] BitRateIndex = new int[2, 3, 16]	
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
		
		
		
		
		public static readonly int[,] SamplingIndex = new int[3, 4]	
        {	
            {44100,48000,32000,0}, // mpeg 1
            {22050,24000,16000,0}, // mpeg 2
            {11025,12000, 8000,0}  // mpeg 2.5
        };
		
		
		
	    public static readonly string[] ModeIndex = new string[4]
        {
            "Stereo",
            "Joint Stereo",
            "Dual Channel",
            "Mono"
        };
	
	 
	
	    public static readonly string[] EmphasisIndex = new string[4]	
        {	
            "No Emphasis",	
            "50/15 Micro seconds",	
            "Unknown",	
            "CCITT J 17"
        };
		
		
		
		
		
		
		
#endregion Constants
		
		
		
		
		
		
	}
}
