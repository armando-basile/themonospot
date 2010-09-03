
using System;
using System.Collections.Generic;

namespace ThemonospotPlugins
{
	
	/// <summary>
	/// Interface to implement in plugin
	/// </summary>
	public interface IThemonospotPlugin
	{
		
		/// <value>
		/// Get/Set if debug is on
		/// </value>
		bool DebugMode
		{
			get;
			set;
		}
		
		
		
		
		
		/// <value>
		/// File extensions that plugin manage
		/// </value>
		string ManagedExtensions
		{
			get;
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
		void GetFileInfo(string filePath, 
		             ref object vInfo,
		             ref object aInfo);
		
		
		
		
		
	}
}