
using System;
using ThemonospotComponents;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace ThemonospotPluginMpeg
{

	/// <summary>
	/// Parser for video stream on mpeg file stream
	/// </summary>
	public class MpegVideoParser
	{
	
		// ATTRIBUTES
		private MpegVideoStream videoStream;

		
		// PROPERTIES
		public MpegVideoStream VideoStream { get{ return videoStream; } }
		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MpegVideoParser ()
		{

		}
		
		
		
		
		/// <summary>
		/// Search video stream using passed MpegStreamManager
		/// </summary>
		public bool Parse(ref MpegStreamManager msManager)
		{
			videoStream = new MpegVideoStream();
			long offset = 0;
			
			// Search 0x00 0x00 0x01 0xB3 marker [Sequence header]			
			bool isSeqHeader = msManager.SearchMarker(ref offset,
			                                          new byte[4]{0x00, 0x00, 0x01, 0xB3});
			
			if (!isSeqHeader)
			{
				// Sequence header not founded
				return false;
			}
			
			// skip marker bytes
			offset += 4;
			
			
			
			// read height and width
			videoStream.Width = msManager.GetSize(offset) >> 4;
			videoStream.Height = msManager.GetSize(offset + 1) & 0x0FFF;
			offset += 3;
			
			
			// read framerate
			int frameRateIndex = msManager.GetByte(offset) & 0x0F;
			
			if (frameRateIndex > 8)
			{
				// Reserved
				videoStream.FrameRate = 0.0d;
			}
			else
			{
				// Retrieve value from table
				videoStream.FrameRate = MpegConstants.FrameRateTable[frameRateIndex];
			}
			
			
			
			// read aspectratio
			int aspectRatioIndex = (msManager.GetByte(offset) & 0xF0) >> 4;
			
			if (aspectRatioIndex <=4)
			{
				videoStream.AspectRatio = MpegConstants.AspectRatio[aspectRatioIndex];
			}
			else
			{
				videoStream.AspectRatio = "Unknow";
			}
			
			offset += 1;
			
			
			// read BitRate
			int bitRate = ((msManager.GetByte(offset) * 0x10000) +
			               (msManager.GetByte(offset + 1) * 0x100) +
			                msManager.GetByte(offset + 2) ) >> 6;
			
			videoStream.BitRate = bitRate;
			
			// calculate Duration
			videoStream.Duration = msManager.StreamSize / ((bitRate * 400) / 8.0);
			
			// Extract video format and chroma format
			GetVideoFormat(ref msManager, offset);
			
			return true;

		}
		
		
		
		
		
		
		
		
		
		
		
		
		/// <summary>
		/// Extract Video format and Chroma format
		/// </summary>
		private void GetVideoFormat(ref MpegStreamManager msManager, long offset)
		{
			byte marker = new byte();
			bool isFounded = false;
			long _offset = offset;
			
			// search marker 0xB5 or 0xB8 was founded
			isFounded = msManager.FindNextMarker(ref _offset, ref marker);
				
			if (isFounded)
			{
				// founded one marker
//				if (marker == 0xB8)
//				{
//					// [Group of pictures] founded... 
//					return;
//				}
				
				if (marker == 0xB5)
				{
					// [extension]
					ParseExtension(_offset, ref msManager);					
				}
				else
				{
					// in all other cases
					return;
				}
				
			}
			else
			{
				// marker not founded
				return;
			}
			
			
			// Get Chroma Format Text
			
			if (videoStream.ChromaFormat >= 1 &&
			    videoStream.ChromaFormat <= 3)
			{
				// Get chroma format text from chroma format table
				videoStream.ChromaFormatText = MpegConstants.ChromaFormat[videoStream.ChromaFormat];
			}
			else
			{
				// Unknow chroma format
				videoStream.ChromaFormatText = "Unknow";
			}
			
			
			// Get Video Format Text
			
			if (videoStream.VideoFormat >= 0 &&
			    videoStream.VideoFormat <= 5)
			{
				// Get video format text from video format table
				videoStream.VideoFormatText = MpegConstants.VideoFormat[videoStream.VideoFormat];
			}
			else
			{
				// Unknow video format
				videoStream.VideoFormatText = "Unknow";
			}

			
			
		}
		
		
		
		
		
		
		private void ParseExtension(long offset, ref MpegStreamManager msManager)
		{
			offset += 4;
			
			int ext = msManager.GetByte(offset) >> 4;
			
			if (ext == 1)
			{
				videoStream.Version = 2;
				videoStream.VideoFormat = (msManager.GetByte(offset + 1) & 0x06) >> 1;
				
			}
			else if (ext == 2)
			{
				
				videoStream.VideoFormat = (msManager.GetByte(offset) & 0x0E) >> 1;
				
			}

			
			
		}
		
		
		
		
		
		
		
	}
}
