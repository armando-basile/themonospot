using System;
using ThemonospotComponents;
using System.IO;


namespace ThemonospotPluginMpeg
{


	public class MpegAudioParser
	{
		
		// ATTRIBUTES
		private MpegAudioStream audioStream;

		
		// PROPERTIES
		public MpegAudioStream AudioStream { get{ return audioStream; } }
		

		public MpegAudioParser ()
		{
		}
		
		
		
		
		/// <summary>
		/// Search audio stream using passed MpegStreamManager
		/// </summary>
		public bool Parse(ref MpegStreamManager msManager)
		{
			
			audioStream = new MpegAudioStream();
			long offset = 0;
			
			// Search 0x00 0x00 0x01 0xC0 marker [Audio Stream]		
			bool isAudioContains = 
				msManager.SearchMarker(ref offset, new byte[4]{0x00, 0x00, 0x01, 0xC0});
			
			if (!isAudioContains)
			{
				// there isn't audio
				return false;
			}
			
			
			offset += 13;
			
			
			bool isAudio = false;
			
			while (!isAudio && offset < (msManager.StreamSize - 10))
			{
				if ((msManager.GetByte(offset) == 0xFF) && 
				    (msManager.GetByte(offset + 1) & 0xF0) == 0xF0)
				{
					isAudio = ParseAudioStream(ref msManager, offset);					
				}
				offset ++;
			}
			
			
			if (isAudio)
			{
				// Audio information added
				return true;
			}
			
			
			
			
			
			return false;
			
		}
		
		
		
		
		
		
		
		private bool ParseAudioStream(ref MpegStreamManager msManager, long offset)
		{
			
			if (Convert.ToBoolean((msManager.GetByte(offset + 1) & 0x08)))
			{
				audioStream.Version = 1.0d;
			}
			else
			{
				audioStream.Version = 2.0d;
			}
			
			audioStream.Layer = (msManager.GetByte(offset + 1) & 0x06) >> 1;
			
			if (audioStream.Layer < 1 || audioStream.Layer > 3)
			{
				return false;
			}
			
			audioStream.Protected = Convert.ToBoolean(msManager.GetByte(offset + 1) & 0x01);
			
			
			int bitrateIndex = msManager.GetByte(offset + 2) >> 4;	
	        int samplingIndex = (msManager.GetByte(offset + 2) & 0x0F) >> 2;
	
	        if (samplingIndex >= 3 || bitrateIndex == 15) 
			{ 
				return false; 
			}
	

			
			audioStream.BitRate = MpegConstants.BitRateIndex[(int)audioStream.Version -1,
			                                                 audioStream.Layer -1,
			                                                 bitrateIndex];
			audioStream.ByteRate = (float)((audioStream.BitRate * 1000) / 8.0d);
			
			audioStream.SamplingRate = 
				MpegConstants.SamplingIndex[(int)audioStream.Version - 1, samplingIndex];
			
			if (audioStream.BitRate <= 0 ||
			    audioStream.ByteRate <= 0 ||
			    audioStream.SamplingRate <= 0)
			{
				return false;
			}
			
			
			
			if (Convert.ToBoolean(msManager.GetByte(offset + 2) & 0x02))	
	        {	
	            audioStream.Padding = true;	
	        }
			else
			{
				audioStream.Padding = false;	
			}
			
			
			audioStream.ModeCode = msManager.GetByte(offset + 3) >> 6;
			
			// int modeExt = (msManager.GetByte(offset + 3) >> 4) & 0x03;
			
			if (Convert.ToBoolean(msManager.GetByte(offset + 3) & 0x08))	
	        {	
	            audioStream.Copyright = true;	
	        }	
	        else	
	        {	
	            audioStream.Copyright = false;	
	        }
			
			
			if (Convert.ToBoolean(msManager.GetByte(offset + 3) & 0x04))	
	        {	
	            audioStream.Original = true;	
	        }	
	        else	
	        {	
	            audioStream.Original = false;	
	        }
			
			audioStream.EmphasisIndex = msManager.GetByte(offset + 3) & 0x03;
			
			
			if (audioStream.Version == 1)
			{
				if (audioStream.Layer == 1)
				{
					audioStream.FrameLength =
	                    ((48000 * audioStream.BitRate) / audioStream.SamplingRate)	
	                    + 4 * Convert.ToInt32(audioStream.Padding);
				}
				else
				{
					audioStream.FrameLength =
						((144000 * audioStream.BitRate) / audioStream.SamplingRate)	
	               		+ Convert.ToInt32(audioStream.Padding);
				}
				
			}
			else if (audioStream.Version == 2)
			{
				if (audioStream.Layer == 1)
				{
					audioStream.FrameLength =
						((24000 * audioStream.BitRate) / audioStream.SamplingRate)	
	                	+ 4 * Convert.ToInt32(audioStream.Padding);
				}
				else
				{
					audioStream.FrameLength =
						((72000 * audioStream.BitRate) / audioStream.SamplingRate)	
	               		+ Convert.ToInt32(audioStream.Padding);
				}
				
			}
			else
			{
				return false;
			}
			
			
			if (audioStream.Protected)	
	        {	
	            // frame length sometimes gets offset by +- 2, 
				// got to find out why its happening, 
				// most likely its the protection switch (see above)	
	            audioStream.FrameLength += 2;	
	        }
			
			
			audioStream.Duration = ((msManager.StreamSize * 1.0d) / audioStream.BitRate) * 0.008d;
			
			
			return true;
		}
		
		
		
		
		
		
		
		
	}
}
