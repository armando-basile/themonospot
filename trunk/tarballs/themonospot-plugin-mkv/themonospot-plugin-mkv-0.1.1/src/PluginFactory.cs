
using System;
using ThemonospotPlugins;
using System.Collections.Generic;
using System.IO;

namespace ThemonospotPluginMkv
{
	
	/// <summary>
	/// ThemonospotBase Plugin to manage Matroska file
	/// </summary>
	public class PluginFactory: MarshalByRefObject, IThemonospotPlugin
	{

		#region Properties
		
		
		private string _managedExtensions = ".mkv;";

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
			set{_debugMode = value;	}
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
			
			if (!IsManaged(filePath))
			{
				throw new Exception("UNMANAGED EXTENSION");
			}

			string outMessage = "";
			int aTrack = 0;
			int vTrack = 0;
			int sTrack = 0;

			
			
			// Create instance of MkvManager
			MkvManager mkvManager = new MkvManager(filePath, _debugMode);
			
			// Read matroska file and fill MkvContainer class
			mkvManager.GetInfo();

			
			#region Adjust output lists
			
			for(int j=0; j<mkvManager.MkvContainer.Count; j++)
			{
				MkvTrackEntity tmpTrack = mkvManager.MkvContainer[j];
				
				if (tmpTrack.Type == 2)
				{
					// AUDIO TRACK
					aTrack++;
					
					// Prepare message
					outMessage = "[" +tmpTrack.Lang + "] " +
						tmpTrack.Codec + " - " + tmpTrack.Audio[0].Frequency.ToString() + " Hz - " +
							tmpTrack.Audio[0].Channels.ToString() + " channels";
					
					// Add message to list
					((List<string[]>)aInfo).Add(
						new string[] {"Audio " + aTrack.ToString("D2") + " :", outMessage} );

				}
				else if (tmpTrack.Type == 1)
				{
					// VIDEO TRACK
					vTrack++;
					
					// Prepare message
					outMessage = tmpTrack.Codec + "  (" +
						tmpTrack.Video[0].PixelWidth.ToString() + " x " + 
							tmpTrack.Video[0].PixelHeight.ToString() + ") ";
					
					if (tmpTrack.FrameRate != 0)
					{
						outMessage += "FPS " + tmpTrack.FrameRate.ToString("#,##0.00");						
					}
					
					// Add message to list
					((List<string[]>)vInfo).Add(
						new string[] {"Video " + vTrack.ToString("D2") + " :", outMessage} );
					
					
				}
				else if (tmpTrack.Type == 0x11)
				{
					// SUBTITLE TRACK
					sTrack++;
					
					// Prepare message
					outMessage = "[" +tmpTrack.Lang + "] " +
						tmpTrack.Codec;
					
					// Add message to list
					((List<string[]>)vInfo).Add(
						new string[] {"Subtitle " + sTrack.ToString("D2") + " :", outMessage} );
					
				}
				
			} // end for
			
			
			#endregion Adjust output lists
			
			

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
