
using System;

namespace ThemonospotPluginMpeg
{


	public class MpegVideoStream
	{

		public int Width {get; set;}
		public int Height {get; set;}
		public int BitRate {get; set;}
		public int ChromaFormat {get; set;}
		public int VideoFormat {get; set;}
		public int Version {get; set;}
		public string ChromaFormatText {get; set;}
		public string VideoFormatText {get; set;}		
		public double Duration {get; set;}
		public string AspectRatio {get; set;}
		public double FrameRate {get; set;}
		public int Frames {get; set;}
		
		
		public MpegVideoStream()
		{
			Version = 1;
		}
		
	}
	
}
