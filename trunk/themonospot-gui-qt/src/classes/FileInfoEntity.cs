
using System;
using System.Collections.Generic;

namespace ThemonospotGuiQt
{
	
	/// <summary>
	/// Entity to manage scan result
	/// </summary>
	public class FileInfoEntity
	{
		
		public List<string[]> Audio {get; set;}
		public List<string[]> Video {get; set;}
		public string FileName 
		{
			get
			{	return System.IO.Path.GetFileName(FilePath);	}				
		}
		public string FilePath {get; set;}
		public string Errors {get; set;}
		public string PluginUsed {get; set;}
		
		public FileInfoEntity()
		{
			Errors = "";
		}
		
		public FileInfoEntity(List<string[]> video, 
		                      List<string[]> audio, 
		                      string filepath, 
		                      string errors,
		                      string pluginused)
		{
			Audio = audio;
			Video = video;
			FilePath = filepath;
			Errors = errors;
			PluginUsed = pluginused;
		}
	}
}
