using System;
using System.Text;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Xml.Serialization;
using themonospot_Base_Main;

namespace themonospot_Base_Main
{
	public class clsThemonospotBase
	{

		private string myConfigFilePath = "";
		private clsConfiguration settingsClass = null;
				
		private themonospotParser tmsParser = null;

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
				// myConfigFilePath = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName + Path.DirectorySeparatorChar.ToString();
			else
				myConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar.ToString();
			
            settingsClass = new clsConfiguration();
			
			readConfigurationFile(ref settingsClass);
			
			//if (Directory.Exists(settingsClass.defaultPath) == true)
			//	defaultPath = settingsClass.defaultPath;
		
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
		
		
		
		
		public bool parseAviFile(string avifilename, 
		                         ref string retError,
		                         ref Hashtable VideoInfo, 
		                         ref Hashtable AudioInfo, 
		                         ref string ASHinfo, 
		                         ref string VSHinfo)
		{
		    Hashtable outInformations = null;
		    
		    VideoInfo = null;
		    AudioInfo = null;
		    retError = "";
		    _userDataToChange = "";
		    
		    tmsParser = new themonospotParser();
		    
		    if (File.Exists(avifilename) != true)
			{
			    retError = "File not found !";
				return false;
			}
		    
		    
		    try 
			{
				
				tmsParser.OpenAviFile(avifilename);
				
				ASH_offset = tmsParser.fourCC_AVISTREAMHEADER_offset;
				VSH_offset = tmsParser.fourCC_AVIVIDEOHEADER_offset;
				moviOffset = tmsParser.m_MoviStart;
				moviSize = tmsParser. m_MoviSize;
				moviFrames = tmsParser.headerFile.dwTotalFrames;
				idxSize = tmsParser.m_IdxSize;
				idxStart = tmsParser.m_IdxStart;
				_rec_ix = tmsParser.rec_ix_presence;
		    }
		    catch (themonospotParserException e)
		    {
		        retError = e.Message;
		        return false;		   
		    }
		    catch (Exception e)
		    {
		        retError = e.Message;
				return false;		   
		    }
		    		    
		    outInformations = new Hashtable();
		    
		    Hashtable tmpInformations = tmsParser.GetVideoInformations();
		    outInformations.Add((object)"Video:", (object)(tmpInformations["Video codec:"].ToString() + " (" + tmpInformations["Codec descr:"].ToString() + ")"));
		    outInformations.Add((object)"Frame Size:", (object)(tmpInformations["Frame Size:"].ToString()));
		    outInformations.Add((object)"Average Video Bitrate:", (object)(tmpInformations["Average Video Bitrate:"].ToString()));
		    outInformations.Add((object)"Avi file size:", (object)(tmpInformations["Avi file size:"].ToString()));
		    outInformations.Add((object)"Total Time:", (object)(tmpInformations["Total Time:"].ToString()));
		    outInformations.Add((object)"Frame Rate:", (object)(tmpInformations["Frame Rate:"].ToString()));
		    outInformations.Add((object)"Total Frames:", (object)(tmpInformations["Total Frames:"].ToString()));
		    outInformations.Add((object)"Video Data Rate:", (object)(tmpInformations["Video Data Rate:"].ToString()));
		    outInformations.Add((object)"Video Quality:", (object)(tmpInformations["Video Quality:"].ToString()));
		    outInformations.Add((object)"Packet Bitstream:", (object)(tmpInformations["Packet Bitstream:"].ToString()));
		    
		    ASHinfo = tmpInformations["Codec descr:"].ToString();
			VSHinfo = tmpInformations["Video codec:"].ToString();
		    
		           
		    
		    for (int k=0; k<8; k++)
				if (tmpInformations["Info Data[" + k + "]:"] != null)
					outInformations.Add((object)("Info Data[" + k + "]:"), (object)(tmpInformations["Info Data[" + k + "]:"].ToString()));

			for (int k=0; k<8; k++)
			{
				if (tmpInformations["User Data[" + k + "]:"] != null)
				{
					outInformations.Add((object)("User Data[" + k + "]:"), (object)(tmpInformations["User Data[" + k + "]:"].ToString()));
					
					if ( tmpInformations["User Data[" + k + "]:"].ToString().IndexOf("DivX") == 0 &&					   
					     tmpInformations["User Data[" + k + "]:"].ToString().IndexOf("DivX999b000") < 0)
					{
						_userDataToChange = tmpInformations["User Data[" + k + "]:"].ToString();
					}
				}
			}
		    
		    VideoInfo = outInformations;
		    		    
		    AudioInfo = tmsParser.GetAudioInformations();
			
			settingsClass.defaultPath = Path.GetDirectoryName(avifilename);
			writeConfigurationFile(ref settingsClass);
			
		    return true;
		    
		}
		
		
		public void updateFourCC(string ASH, string VSH)
		{
		    tmsParser.change4CC(ASH, VSH, ASH_offset, VSH_offset);
		    return;
		}
		
		
		public void resaveAviFile()
		{
			tmsParser.udToChange = _userDataToChange;			
			tmsParser.saveNewAvi(_newAviFileName, 
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
