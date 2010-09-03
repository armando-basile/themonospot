
using System;

namespace ThemonospotPluginMpeg
{


	public class MpegAudioStream
	{

	        public double Version {get; set;}
	
	        public int Layer  {get; set;}
	
	        public bool Protected  {get; set;}
	
	        public int BitRate  {get; set;}
	
	        public float ByteRate  {get; set;}
	
	        public int SamplingRate  {get; set;}
	
	        public bool Padding  {get; set;}
	
	        public int ModeCode  {get; set;}
	
	        public int ModeXt  {get; set;}
	
	        public bool Copyright  {get; set;}
	
	        public int EmphasisIndex  {get; set;}
	
	        public bool Original  {get; set;}
	
	        public int FrameLength  {get; set;}
	
	        // seconds; duration = file size / bit rate but from what i understand there is a timestamp
	
	        // for initial and one for when video ends, so accurate duration = timestamp end - timestamp start
	
	        public double Duration  {get; set;}
	
	        public int Frames  {get; set;}
		
	}
}
