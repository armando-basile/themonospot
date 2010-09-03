
using System;
using System.IO;
using System.Collections.Generic;
using log4net;
using ThemonospotComponents;

namespace ThemonospotPluginMpeg
{
	
	/// <summary>
	/// Parse Matroska file
	/// </summary>
	public class MpegManager
	{
		
		private MpegStreamManager streamManager;
		
		private MpegVideoParser mvp = new MpegVideoParser();
		private MpegAudioParser map = new MpegAudioParser();
		
		private string mpegFilePath = "";
		private string mpegFileName = "";
		private long mpegFileSize = 0;
		private MpegVideoStream vStream = new MpegVideoStream();
		private MpegAudioStream aStream = new MpegAudioStream();
		
		// Log4Net object
		private static readonly ILog log = LogManager.GetLogger(typeof(MpegManager));
		
		
		#region Properties
		
		public MpegVideoStream VStream {get {return vStream;}}
		public MpegAudioStream AStream {get {return aStream;}}
		
		
		#endregion Properties
		
		
		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="mkvFilePath"> Matroska file path
		/// A <see cref="System.String"/>
		/// </param>
		public MpegManager(string mpegfilePath)
		{
			mpegFilePath = mpegfilePath;
			mpegFileName = Path.GetFileNameWithoutExtension(mpegFilePath);	
			mpegFileSize = new FileInfo(mpegFilePath).Length;
			
		}
		
		
		
		
		
		/// <summary>
		/// Scan matroska file and fill MkvTracks
		/// </summary>
		public void GetInfo()
		{
			
			
			// Debug
			log.Info("\r\nFile path: " + mpegFilePath + "\r\n" +
			         "File name: " + mpegFileName + "\r\n" +
					 "File size: " + mpegFileSize.ToString("#,##0"));
			
			streamManager = new MpegStreamManager(mpegFilePath);
			vStream = new MpegVideoStream();
			aStream = new MpegAudioStream();
			
			bool isVideo = mvp.Parse(ref streamManager);
			
			if (isVideo)
			{
				vStream = mvp.VideoStream;
			}
			
			
			bool isAudio = map.Parse(ref streamManager);
			
			if (isAudio)
			{
				aStream = map.AudioStream;
			}
			
			streamManager.Close();
			
			return;	
			
			
			
		} // end void GetInfo()
		
		
		
		
		
		
/*
		
		private void ReadStream_BA()
		{
			// DEBUG
			//log.Info(utils.GetRow(MpegConstants.MPEGPS_PACKID, 0, streamPosition));
			
			// Detect Mpeg type
			byte[] buffBytes = ReadBytes(1);
			
			if ((buffBytes[0] & 0xC0) == 0x40)
			{
				// MPEG 2
				mpegType = 2;
			}
			else if ((buffBytes[0] & 0xF0) == 0x20)
			{
				// MPEG 1
				mpegType = 1;
			}
			else
			{
				// MPEG unknow
				ifs.Close();
				ifs.Dispose();
				ifs = null;
				throw new Exception("Not a supported mpeg file type (not 1 or 2)");
			}
			
			//log.Info("MPEG " + mpegType.ToString() + " founded");
			
			if (mpegType == 2)
			{
				// MPEG 2... Skip 8 bytes
				ifs.Seek(8, SeekOrigin.Current);
				streamPosition += 8;
				
				// Read last byte
				buffBytes = ReadBytes(1);
				if( (buffBytes[0] & 7) > 0)
				{
					// Skip other header bytes
					ifs.Seek((buffBytes[0] & 7), SeekOrigin.Current);
					streamPosition += (buffBytes[0] & 7);
				}				
				
			}
			else if (mpegType == 1)
			{
				// MPEG 1... Skip only 8 bytes
				ifs.Seek(7, SeekOrigin.Current);
				streamPosition += 7;
			}
			
		}
		
		
		
*/
		
		
		
		
		
		
		
		
	}
}
