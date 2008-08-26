using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;
using themonospot_Base_Main;

namespace themonospot_Base_Main
{
	public class clsThemonospotBase
	{

		private string myConfigFilePath = "";
		private clsConfiguration settingsClass = null;

		private clsParserAVI tmsParserAVI = null;
		private clsParserMKV tmsParserMKV = null;
		
		private long ASH_offset = 0;
		private long VSH_offset = 0;
		private long moviOffset = 0;
		private long moviSize = 0;
		private int moviFrames = 0;
		private long idxSize = 0;
		private long idxStart = 0;
		private bool _rec_ix = false;
		private string _userDataToChange = "";
		private string _newAviFileName = "";
		private double _saveStatus = 0;
		private string _saveError = "";
		private bool _saveFlag = false;
		private double _totProgressItems = 0;
		private bool _redrawInfo = false;
		private string _saveInfo = "";
		
		
		public double totProgressItems
		{   get {   return _totProgressItems; }   }
		
		
		public bool redrawInfo
		{   get {   return _redrawInfo; }
		    set {   _redrawInfo = value;    } 
		}
		
		
		public bool saveFlag
		{   
			get { return _saveFlag; }
			set { _saveFlag = value; }
		}
		
		
		public double saveStatus
		{   get { return _saveStatus; }   }
		
		
		public string newAviFileName
		{   
			get { return _newAviFileName; }
			set { _newAviFileName = value; }
		}
		
		public string saveInfo
		{   get { return _saveInfo; }   }
		
		
		public string saveError
		{   get { return _saveError; }   }
		
		
		public string userDataToChange
		{   get { return _userDataToChange; }   }

		
		public string defaultPath
		{	get	{	return settingsClass.defaultPath;}	}
		
		
		public bool rec_ix
		{   get { return _rec_ix; }   }
		

		/// <summary>
		/// Create an instance of object
		/// </summary>
		public clsThemonospotBase()
		{
			if (IsWindows() == true)
				myConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + Path.DirectorySeparatorChar.ToString();				
			else
				myConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar.ToString();
			
            settingsClass = new clsConfiguration();
			
			readConfigurationFile(ref settingsClass);		
		}
		
		
		public string Release()
		{
			return Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." + 
				   Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + "." + 
				   Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
		}
				
		
		private bool IsWindows()
		{
		    PlatformID platform = Environment.OSVersion.Platform;	    
		    return (platform == PlatformID.Win32NT | platform == PlatformID.Win32Windows |
		            platform == PlatformID.Win32S | platform == PlatformID.WinCE);    
		}
			
		
		// Write the parameters in a config file.
		private void writeConfigurationFile(ref clsConfiguration theConfigClass)
		{
			Console.WriteLine("myConfigFilePath = " + myConfigFilePath);			
			string filename = myConfigFilePath + ".themonospot";
			
			XmlSerializer formatter = new XmlSerializer(typeof(clsConfiguration));
			using (FileStream file = File.OpenWrite(filename))
  			{
    			// Serializaion...
    			try
    			{
    				formatter.Serialize(file, theConfigClass);    				
    			}
    			catch (Exception e)
    			{
    				Console.WriteLine("Serialization error: " + e.Message);
    			}
    			file.Close();
			}			
			
			File.SetAttributes(filename, FileAttributes.Hidden);
		}
		

		// Read the parameters in a config file
		private void readConfigurationFile(ref clsConfiguration theConfigClass)
		{
			bool toDelete = false;
			
			Console.WriteLine("myConfigFilePath = " + myConfigFilePath);
			string filename = myConfigFilePath + ".themonospot";
			
			if (File.Exists(filename) == false)
			{	
				// If config file is missing
				theConfigClass = new clsConfiguration();
				
				// Create new parameters
				theConfigClass.defaultPath = "";
				return;
			}
			
			 
			XmlSerializer formatter = new XmlSerializer(typeof(clsConfiguration));
			using (FileStream fileconf = new FileStream(filename, FileMode.Open, FileAccess.Read))
  			{
    			// Deserializaion...
    			try
    			{
    				theConfigClass = formatter.Deserialize(fileconf) as clsConfiguration;
    			}
    			catch (Exception e)
    			{
    				Console.WriteLine("Deserialization of config file error: " + e.Message);
    				fileconf.Close();    				
    				fileconf.Dispose();
    				toDelete = true;
    			}
			}

			if (toDelete == true)
			{
				StreamWriter clearer = new StreamWriter(filename,false);
				clearer.Write("");
				clearer.Close(); clearer.Dispose(); clearer = null;
			}
				
		}
		
		
		
		
		public bool parseMovieFile(string filename, 
		                         ref string retError,
		                         ref List<clsInfoItem> VideoInfo, 
		                         ref List<clsInfoItem> AudioInfo, 
		                         ref string ASHinfo, 
		                         ref string VSHinfo)
		{
		    
		    
		    string file_extention = filename.ToLower().Substring(filename.Length - 3);
		    VideoInfo = new List<clsInfoItem>();
		    AudioInfo = new List<clsInfoItem>();
		    retError = "";
		    _userDataToChange = "";
		    
		    Console.WriteLine("file ext = " + file_extention);
		    
		    if (file_extention == "avi")
		        tmsParserAVI = new clsParserAVI();
		    else if (file_extention == "mkv")
		        tmsParserMKV = new clsParserMKV();
		    else
		    {
		        retError = "File extention not supported !";
				return false;
		    }
		    
		    if (File.Exists(filename) != true)
			{
			    retError = "File not found !";
				return false;
			}
		    
		    
		    try 
			{
				
				
				if (file_extention == "avi")
				{
				    tmsParserAVI.OpenFile(filename);
				    ASH_offset = tmsParserAVI.fourCC_AVISTREAMHEADER_offset;
    				VSH_offset = tmsParserAVI.fourCC_AVIVIDEOHEADER_offset;
    				moviOffset = tmsParserAVI.m_MoviStart;
    				moviSize = tmsParserAVI.m_MoviSize;
    				moviFrames = tmsParserAVI.headerFile.dwTotalFrames;
    				idxSize = tmsParserAVI.m_IdxSize;
    				idxStart = tmsParserAVI.m_IdxStart;
    				_rec_ix = tmsParserAVI.rec_ix_presence;
    				VSHinfo = tmsParserAVI.VideoItems[0].ItemValue;
    				ASHinfo = tmsParserAVI.VideoItems[1].ItemValue;			        
					
					VideoInfo = tmsParserAVI.VideoItems;
					AudioInfo = tmsParserAVI.AudioItems;
					_userDataToChange = tmsParserAVI.udToChange;
					
				}
				else if (file_extention == "mkv")
				{
				    tmsParserMKV.OpenFile(filename);
					
					VideoInfo = tmsParserMKV.VideoItems;
					AudioInfo = tmsParserMKV.AudioItems;
				}
				    
				
		    }
		    catch (ParserException e)
		    {
		        retError = e.Message;
		        return false;		   
		    }
		    catch (Exception e)
		    {
		        retError = e.Message;
				return false;		   
		    }
			

			settingsClass.defaultPath = Path.GetDirectoryName(filename);
			writeConfigurationFile(ref settingsClass);
			
		    return true;
		    
		}
		
		
		public void updateFourCC(string ASH, string VSH)
		{
		    tmsParserAVI.change4CC(ASH, VSH, ASH_offset, VSH_offset);
		    return;
		}
		
		
		public void resaveAviFile()
		{
			tmsParserAVI.udToChange = _userDataToChange;			
			tmsParserAVI.saveNewAvi(_newAviFileName, 
			                 ref _redrawInfo, 
			                 ref _saveError, 
			                 ref _saveStatus,
			                 ref _saveFlag, 
			                 ref _totProgressItems, 
			                 ref _saveInfo);
			return;
		}
		
		
		
	}
}		
