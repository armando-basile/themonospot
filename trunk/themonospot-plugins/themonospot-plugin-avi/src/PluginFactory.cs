
using System;
using ThemonospotPlugins;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace ThemonospotPluginAvi
{
	/// <summary>
	/// ThemonospotBase Plugin to manage Avi file
	/// </summary>
	public class PluginFactory: MarshalByRefObject, IThemonospotPlugin
	{
		
		
		private Utility utils = new Utility();
		private double audioSize = 0;
		private AviManager am;
		
		
		#region Properties
		
		
		
		private string _managedExtensions = ".avi; .divx; .xvid;";

		/// <value>
		/// Return managed multimedia file extensions
		/// </value>
		public string ManagedExtensions 
		{
			get {return _managedExtensions;	}
		}		

		
		
		
		
		private bool _debugMode = false;
		
		/// <value>
		/// Debug mode state
		/// </value>
		public bool DebugMode
		{
			get{return _debugMode;}
			set
			{
				_debugMode = value;	
				utils.DebugMode = value;
			}
		}
		
		
		

		
		#endregion Properties
		
		
		
		
		
		
		public PluginFactory()
		{
			
		}
		
		
		
		/// <summary>
		/// Scan file and return info
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/> File path to scan
		/// </param>
		/// <param name="vInfo">
		/// A <see cref="Object"/> Output video stream info
		/// </param>
		/// <param name="aInfo">
		/// A <see cref="Object"/> Output audio stream info
		/// </param>
		public void GetFileInfo(string filePath, 
		             		ref object vInfo,
		             		ref object aInfo)
		{
			int vStreams = 0;
			int aStreams = 0;

			
			if (!IsManaged(filePath))
			{
				throw new Exception("UNMANAGED EXTENSION");
			}
			
			// Create instance of Manager class
			am = new AviManager(filePath, _debugMode);

						
			// Scan file
			am.GetInfo();
						
			
			
			
			
			
			// Parse audio streams
			for (int j=0; j<am.AviContainer.AviStreams.Count; j++)
			{

				if (am.AviContainer.AviStreams[j].AviStreamHeader.FccType == 
				    AviConstants.StreamtypeAUDIO)
				{
					// Audio Stream
					AddAudioStream(ref am, j);
					aStreams++;
					
					
					// Calc stream size and add to total audio size
					int blockPerSec = am.AviContainer.AviStreams[j].AviStreamHeader.DwRate / 
						am.AviContainer.AviStreams[j].AviStreamHeader.DwScale;
						
					double tmpAudio = am.AviContainer.AviStreams[j].AviStreamHeader.DwLength;
					tmpAudio *= am.AviContainer.AviStreams[j].AviAudioStreamHeader[0].NAvgBytesPerSec;
					tmpAudio /= blockPerSec;
					audioSize += tmpAudio;

				}				
			}
			
			
			// Parse video streams
			for (int j=0; j<am.AviContainer.AviStreams.Count; j++)
			{
				if (am.AviContainer.AviStreams[j].AviStreamHeader.FccType == 
				    AviConstants.StreamtypeVIDEO)
				{				
					// Video Stream
					AddVideoStream(ref am, j);
					vStreams ++;					
					
				}				
			}
			
			
			#region Video List update
			
			((List<string[]>)vInfo).Add(
				new string[] {"Codec name:", am.AviContainer.VideoStreams[0].CodecName} );
			((List<string[]>)vInfo).Add(
				new string[] {"Codec desc:", am.AviContainer.VideoStreams[0].CodecDesc} );			
			((List<string[]>)vInfo).Add(
				new string[] {"Frame size:", am.AviContainer.VideoStreams[0].FrameSize} );
			((List<string[]>)vInfo).Add(
				new string[] {"Average video bitrate:", am.AviContainer.VideoStreams[0].VideoBitrate} );
			((List<string[]>)vInfo).Add(
				new string[] {"File size:", ((am.AviContainer.DataSize + 8) / 1024).ToString("#,### KB")  } );
			
			
			long ttime = (long)(am.AviContainer.VideoStreams[0].TotalTime / 1000000.0);
			
			int hours = (int)(ttime / 3600);
			ttime -= (long)(hours * 3600);
			int mins = (int)(ttime / 60);
			ttime -= (long)(mins * 60);
			
			((List<string[]>)vInfo).Add(
				new string[] {"Total time:", String.Format("{0:00}:{1:00}:{2:00.00#} seconds", hours, mins, ttime)} );
			
			((List<string[]>)vInfo).Add(
				new string[] {"Frame rate:", String.Format("{0:N2} frames/sec", am.AviContainer.VideoStreams[0].FrameRate)} );

			((List<string[]>)vInfo).Add(
				new string[] {"Total frames:", am.AviContainer.VideoStreams[0].TotalFrame.ToString("#,##0") } );
			
			((List<string[]>)vInfo).Add(
				new string[] {"Video data rate:", am.AviContainer.VideoStreams[0].DataRate.ToString() + " frames/sec" } );
			((List<string[]>)vInfo).Add(
				new string[] {"Video quality:", am.AviContainer.VideoStreams[0].Quality.ToString("#,##0.##")} );
			((List<string[]>)vInfo).Add(
				new string[] {"Packet Bitstream:", am.AviContainer.VideoStreams[0].PacketBitstream.ToString()} );
			
			
			// ISFT Data
			for(int k=0; k<am.AviContainer.ISFTData.Count; k++)
			{
				((List<string[]>)vInfo).Add(
					new string[] {"ISFT data:", am.AviContainer.ISFTData[0]} );
			}
			

			// JUNK Data
			for(int k=0; k<am.AviContainer.JunkData.Count; k++)
			{
				((List<string[]>)vInfo).Add(
					new string[] {"JUNK data:", am.AviContainer.JunkData[0]} );
			}


			// USER Data
			for(int k=0; k<am.AviContainer.MoviUserData.Count; k++)
			{
				((List<string[]>)vInfo).Add(
					new string[] {"USER data:", am.AviContainer.MoviUserData[0]} );
			}

			
			
			#endregion Video List update
			
			
			
			#region Audio List update
			
			for(int k=0; k<am.AviContainer.AudioStreams.Count; k++)
			{
				AviAudioStreamEntity ase = am.AviContainer.AudioStreams[k];
				
				string streamDescInfo = "Audio " + (k+1).ToString("d2") + ":";
				string streamValueInfo = ase.StreamType + " " +
					ase.Bitrate + " - " + ase.Hz.ToString() + " Hz (" +
						ase.Channels.ToString() + " Channels)";
				
				((List<string[]>)aInfo).Add(
					new string[] {streamDescInfo, streamValueInfo} );

			}
			
			#endregion Audio List update
			
			
			
		}
		
		
		
		/// <summary>
		/// Add video stream to VideoStreams object
		/// </summary>
		/// <param name="am">
		/// A <see cref="AviManager"/>
		/// </param>
		/// <param name="j">
		/// A <see cref="System.Int32"/>
		/// </param>
		private void AddVideoStream(ref AviManager am, int j)
		{
			
			double WdH = 0;
			
			AviVideoStreamEntity vStream = new AviVideoStreamEntity();
			long totalTime = 0;
		 	
			
			vStream.CodecDesc = 
				utils.GetFourccFromInt((uint)am.AviContainer.AviStreams[j].AviStreamHeader.FccHandler);
			
			
			vStream.FrameSize = 
				am.AviContainer.AviStreams[j].AviVideoStreamHeader[0].BiWidth +
				" x " + 
				am.AviContainer.AviStreams[j].AviVideoStreamHeader[0].BiHeight;
			

			if (am.AviContainer.AviHeader.DwMicroSecPerFrame > 0)
			{
				totalTime =(long)((long)am.AviContainer.AviHeader.DwTotalFrames *
						  (long)am.AviContainer.AviHeader.DwMicroSecPerFrame);
			}

			vStream.TotalTime = totalTime;
				
			vStream.FrameRate = Convert.ToInt32(1000000.0 / am.AviContainer.AviHeader.DwMicroSecPerFrame);

			vStream.TotalFrame = (long)am.AviContainer.AviHeader.DwTotalFrames;
		
			vStream.DataRate = (long)(am.AviContainer.AviStreams[j].AviStreamHeader.DwRate /
				              am.AviContainer.AviStreams[j].AviStreamHeader.DwScale);
			
			
			
			// Calc Video Bitrate
			WdH = am.AviContainer.AviStreams[j].AviVideoStreamHeader[0].BiWidth;
			WdH /= am.AviContainer.AviStreams[j].AviVideoStreamHeader[0].BiHeight;
			
			int headerSize = am.AviContainer.AviHeader.DwTotalFrames * 8 * 
				(am.AviContainer.AudioStreams.Count + 1);
			
			long videoSize = (long)(am.AviContainer.MoviSize - headerSize - audioSize);
			
			double AverageVideoBitRate = ((double)(videoSize * vStream.DataRate * 8) /  
			    	                        (am.AviContainer.AviHeader.DwTotalFrames * 1000));
			
//			double videoQuality = (double)((0.75 * WdH) * 
//			                       (double)(AverageVideoBitRate / vStream.DataRate));
			double videoQuality = (double)((0.75 * WdH * AverageVideoBitRate) / vStream.DataRate);

			
			vStream.CodecName = 
				utils.GetFourccFromInt((uint)am.AviContainer.AviStreams[j].AviVideoStreamHeader[0].BiCompression);
			
			vStream.VideoBitrate = AverageVideoBitRate.ToString("#,##0") + " Kb/Sec";
			vStream.PacketBitstream = am.AviContainer.PBitstream;
			vStream.Quality =  (long)videoQuality;
			
			
			am.AviContainer.VideoStreams.Add(vStream);
		}
		
		
		
		
		
		/// <summary>
		/// Add audio stream to AudioStreams object
		/// </summary>
		/// <param name="am">
		/// A <see cref="AviManager"/>
		/// </param>
		/// <param name="j">
		/// A <see cref="System.Int32"/>
		/// </param>
		private void AddAudioStream(ref AviManager am, int j)
		{
			AviAudioStreamEntity aStream = new AviAudioStreamEntity();
			
			string aFormat = am.AviContainer.AviStreams[j].AviAudioStreamHeader[0].WFormatTag.ToString("X4");
			double audioRate = (8.0 * am.AviContainer.AviStreams[j].AviAudioStreamHeader[0].NAvgBytesPerSec);
			
			aStream.Bitrate = String.Format("{0:N2} Kb/Sec", audioRate / 1000.0);
			aStream.Channels = (int)am.AviContainer.AviStreams[j].AviAudioStreamHeader[0].NChannels;
			aStream.Hz = am.AviContainer.AviStreams[j].AviAudioStreamHeader[0].NSamplesPerSec;
			aStream.Code = aFormat;
			aStream.StreamType = am.GetAudioType(aFormat);

			am.AviContainer.AudioStreams.Add(aStream);
			
		}
		
		
		

		
		/// <summary>
		/// Verify if plugin manage this extension
		/// </summary>
		/// <param name="filePath">
		/// A <see cref="System.String"/> File Path
		/// </param>
		/// <returns>
		/// A <see cref="System.Boolean"/> True if extension is managed
		/// </returns>
		private bool IsManaged(string filePath)
		{
			// Check extension
			if(_managedExtensions.IndexOf(new FileInfo(filePath).Extension.ToLower() + ";") < 0)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		

		
		
	}
}
