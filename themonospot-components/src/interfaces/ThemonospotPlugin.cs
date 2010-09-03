
using System;
using System.Collections.Generic;

namespace ThemonospotPlugins
{
	
	/// <summary>
	/// Interface to implement in plugin
	/// </summary>
	public interface IThemonospotPlugin
	{
/*		
		/// <value>
		/// Get/Set if debug is active in console
		/// </value>
		bool IsConsole	{ get; set; }
		
		/// <value>
		/// Get/Set if debug is active in trace file
		/// </value>
		bool IsTrace	{ get; set; }
		
		
		/// <value>
		/// Get/Set if debug is active in trace file
		/// </value>
		string TraceFolderPath	{ get; set; }
*/
		
		
		/// <value>
		/// File extensions that plugin manage
		/// </value>
		string ManagedExtensions { get; }
		
		
		/// <value>
		/// Release
		/// </value>
		string Release { get; }

		/// <value>
		/// Description
		/// </value>
		string Description { get; }
		

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