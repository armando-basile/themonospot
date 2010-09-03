
using System;
using System.Reflection;
using ThemonospotPlugins;
using System.Collections.Generic;
using System.IO;
using ThemonospotComponents;
using log4net;

namespace ThemonospotPluginMpeg
{
	
	/// <summary>
	/// ThemonospotBase Plugin to manage Matroska file
	/// </summary>
	public class PluginFactory: MarshalByRefObject, IThemonospotPlugin
	{

		// Log4Net object
		//private static readonly ILog log = LogManager.GetLogger(typeof(PluginFactory));
		
		
		#region Properties
		
		
		private string _managedExtensions = ".mpg;.mpeg;";

		/// <value>
		/// Return managed multimedia file extensions
		/// </value>
		public string ManagedExtensions 
		{
			get {return _managedExtensions;	}
		}		

		
		
		
		public string Release
		{
			get 
			{
				Version rel = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
				return rel.Major.ToString() + "." +
					   rel.Minor.ToString() + "." +
					   rel.Build.ToString();	
			}
		}		

		
		
		public string Description
		{
			get 
			{
				object[] assemblyAttributes = 
					Assembly.GetExecutingAssembly().GetCustomAttributes(true);
				foreach (Attribute attr in assemblyAttributes) 
				{
					if (attr is AssemblyDescriptionAttribute)
					{
						return ((AssemblyDescriptionAttribute)attr).Description;
					}
				}
				return "";
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
			
			if (!IsManaged(filePath))
			{
				throw new Exception("UNMANAGED EXTENSION");
			}

			string outMessage = "";
			int aTrack = 0;
			int vTrack = 0;
			int sTrack = 0;

			
			
			// Create instance of MpegManager
			MpegManager mpegManager = new MpegManager(filePath);
			
			// Read matroska file and fill MkvContainer class
			mpegManager.GetInfo();

			
			#region Adjust output lists
			
			// VIDEO
			
			((List<string[]>)vInfo).Add(
						new string[] {"Video mpeg type :", mpegManager.VStream.Version.ToString()} );
			
			((List<string[]>)vInfo).Add(
						new string[] {"Video size :", 
				                      mpegManager.VStream.Width.ToString() +
				                      " x " + 
				                      mpegManager.VStream.Height.ToString()} );

			((List<string[]>)vInfo).Add(
						new string[] {"Video format :", mpegManager.VStream.VideoFormatText} );
			
			((List<string[]>)vInfo).Add(
						new string[] {"Aspect ratio :", mpegManager.VStream.AspectRatio} );

			((List<string[]>)vInfo).Add(
						new string[] {"Bitrate :", mpegManager.VStream.BitRate.ToString("#,##0")} );
			
			((List<string[]>)vInfo).Add(
						new string[] {"Duration :", mpegManager.VStream.Duration.ToString("#,##0.00")} );
			
			((List<string[]>)vInfo).Add(
						new string[] {"Frame rate :", mpegManager.VStream.FrameRate.ToString("#,##0.00")} );
			
			((List<string[]>)vInfo).Add(
						new string[] {"Chroma format :", mpegManager.VStream.ChromaFormatText} );

			
			// AUDIO
			
			((List<string[]>)aInfo).Add(
						new string[] {"Audio version :", mpegManager.AStream.Version.ToString()} );
			
			((List<string[]>)aInfo).Add(
						new string[] {"Audio layer :", mpegManager.AStream.Layer.ToString()} );			
			
			((List<string[]>)aInfo).Add(
						new string[] {"Bitrate :", mpegManager.AStream.BitRate.ToString()} );
			
			((List<string[]>)aInfo).Add(
						new string[] {"Duration :", mpegManager.AStream.Duration.ToString("#,##0.00")} );
			
			((List<string[]>)aInfo).Add(
						new string[] {"Sampling rate :", mpegManager.AStream.SamplingRate.ToString("#,##0.00")} );
			
			
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
