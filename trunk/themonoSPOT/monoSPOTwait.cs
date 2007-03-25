using System;
using System.Reflection;
using System.Threading;
using System.Collections;
using Gtk;
using GLib;
using Glade;
using Pango;
using System.IO;
using Utility;


namespace monoSpotMain
{
	
	/// <summary>
	/// monoSPOTwait
	/// </summary>
	public class monoSPOTwait
	{

		// Form Objects
		[Glade.Widget] Gtk.Window topWindow;
		[Glade.Widget] Gtk.Label lblInfo;
		[Glade.Widget] Gtk.ProgressBar myPBar;
		[Glade.Widget] Gtk.Button cmdCancel;

		public System.Threading.Thread new_Thread;
		bool flagAbort = false;		
		bool firstDC = false;
		string dcValue = "";		
		string filename;
		
		byte[] moviBuffer;
		string[] udReturn;
				
		long lenMovi;
		long offSetMovi;
		
		FileStream moviStreamReader;
		cEncoding myEnc = new cEncoding();
		
		public delegate void callBackValues(string[] infos);
		public event callBackValues callBackFunction;
		
		
		/// <summary>
		/// <p> Create new instance of monoSPOTwait.</p>
		/// </summary>
		/// <param name="args">parameters passed to the application</param>
		public monoSPOTwait(string file_name, long offSet_Movi, long len_Movi)
		{
			// Update local variables
			filename = file_name;
			offSetMovi = offSet_Movi;
			lenMovi = len_Movi;
				
			Glade.XML gxml = new Glade.XML(null, "waitWindow.glade", "topWindow", null);
			gxml.Autoconnect(this);
			
			// configure objects on window
			// configWinObjects();
			
			
		}

		
		/// <summary>
		/// <p> Create new instance of monoSPOT.</p>
		/// </summary>		
		public void configWinObjects()
		{	
			// Window
			topWindow.DeleteEvent += On_topWindow_Delete;
			topWindow.SetSizeRequest(625,115);
			topWindow.Resizable=false;
			topWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
			                  " v" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			topWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			
			// Button to exit
			cmdCancel.Released += On_cmdCancel_Clicked;
			
			topWindow.ShowAll();
			
			scanningLoop();

		}
		


		/// <summary>
		/// <p>The On_topWindow_Delete method is used when we want terminate.</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_topWindow_Delete(object sender,EventArgs a)
    	{
        	flagAbort=true;
        }


		/// <summary>
		/// <p>The On_cmdCancel_Clicked method is used when we want exit</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_cmdCancel_Clicked(object sender,EventArgs a)
    	{
			flagAbort=true;
      	}			

	    
	    private void scanningLoop()
	    {
	    	long j = 0;
	    	long byteOfRead=0;	    	
	    	long dcOffSet=0;
	    	long dcByteOfRead=0;
	    	long tagLen=0;
	    	int readDCBytes=0;
	    	int FourCC=0;    	
	    	
	    	string sFourCC="";
	    	string hexFourCC="";
	    	
	    	
	    	
    		// Update GUI and ProgressBar    		
	    	myPBar.Adjustment.Lower=0;
    		myPBar.Adjustment.Value=0;
    		myPBar.Adjustment.Upper=lenMovi;	    	
	    	while (GLib.MainContext.Iteration(false));
	    	
	    	moviStreamReader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
	    	moviStreamReader.Seek(offSetMovi, SeekOrigin.Current);
	    	
	    	while (j<lenMovi)
	    	{
	    		FourCC = readDWord();
				sFourCC = Utility.cEncoding.FromFourCC(FourCC);
				hexFourCC = FourCC.ToString("X8");
				byteOfRead = readDWord();
				
				// Adjust bytes to read (no odd)
				if ((byteOfRead % 2) != 0 )
					byteOfRead++;				
				
				j += 8;
	    		
				
				
	    		// Update GUI and ProgressBar
	    		if ( (j/myPBar.Adjustment.Upper) - myPBar.Fraction > 0.01 )
	    		{
					myPBar.Adjustment.Value=j;
    				myPBar.Fraction = (myPBar.Adjustment.Value / myPBar.Adjustment.Upper);
    				myPBar.Text = (myPBar.Fraction * 100).ToString("###");
    				// wait gui update
	    			while (GLib.MainContext.Iteration(false));
	    		}
				
	    		
	    		// Verify Cancel button
	    		if(flagAbort==true)
	    		{
	    			moviStreamReader.Close(); moviStreamReader=null;
					topWindow.Destroy();
					topWindow.Dispose();
					topWindow = null;
					this.callBackFunction(udReturn);
					return;
	    		}
	    		
				
	    		// Verify FourCC
	    		if (sFourCC.Substring(2,2) == "dc" || sFourCC.Substring(2,2) == "db")
	    		{
	    			// "xxdc" Chunck founded	    			
	    			// Console.WriteLine("__dc");
	    				
	    			// Check size to read
					if ((j+byteOfRead) >lenMovi)
					{
		    			moviStreamReader.Close(); moviStreamReader=null;
						topWindow.Destroy();
						topWindow.Dispose();
						topWindow = null;
						this.callBackFunction(udReturn);
						return;	
					}

					dcByteOfRead = byteOfRead;
					dcOffSet=0;
	    			
					// Scan the section "xxdc" *************************
					// now we must read Byte to Byte (not use DWORD)
					while (dcOffSet < byteOfRead)
					{
						
						j += byteOfRead;
		    			dcOffSet = byteOfRead;
						
						if (firstDC==false)
						{
							// First xxdc chunck
							moviBuffer = new byte[(int)byteOfRead];
							readDCBytes = moviStreamReader.Read(moviBuffer,0,(int)byteOfRead);						
							process__dc();
							firstDC=true;
						}
						else
						{
							// Other time, skip data
							// moviStreamReader.Seek(byteOfRead, SeekOrigin.Current);
							
							// Exit from loop
							moviStreamReader.Close(); moviStreamReader=null;
							topWindow.Destroy();
							topWindow.Dispose();
							topWindow = null;
							this.callBackFunction(udReturn);
							return;	
							
							
						}
						
						
		    			
					}
					

	    		}
	    		else
	    		{
					
	    			// Other Chunck founded
					// Console.WriteLine(sFourCC.PadRight(20,(char)46) + " [" + byteOfRead.ToString("#,###")  + "] " + " (" + j.ToString("#,###") + ")" );

	    			// Check size to read
					if ((j+byteOfRead) >=(lenMovi - 8))
					{
		    			moviStreamReader.Close(); moviStreamReader=null;
						topWindow.Destroy();
						topWindow.Dispose();
						topWindow = null;
						this.callBackFunction(udReturn);
						return;	
					}
					
	    			
	    			// Skip bytes
	    			moviStreamReader.Seek(byteOfRead, SeekOrigin.Current);
	    			j += byteOfRead;
	    			
	    		}

	    	}
	    	
    		
			
			this.callBackFunction(udReturn);

			topWindow.Destroy();
			topWindow.Dispose();
			topWindow = null;
		    
	    }

	    
	    
	    /// <summary>
		/// Read 4 bytes (DWORD) and return the value
		/// </summary>
		/// <returns></returns>
		private int readDWord()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[4];
			readBytes = moviStreamReader.Read(tmpBuffer,0,4);
			retValue = tmpBuffer[0]+(tmpBuffer[1]<<8)+(tmpBuffer[2]<<16)+(tmpBuffer[3]<<24);
			return retValue;
		}
		
		
	    /// <summary>
		/// Read 4 bytes and return the value
		/// </summary>
		/// <returns></returns>
		private int read4Bytes()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[4];
			readBytes = moviStreamReader.Read(tmpBuffer,0,4);
			retValue = tmpBuffer[3]+(tmpBuffer[2]<<8)+(tmpBuffer[1]<<16)+(tmpBuffer[0]<<24);
			return retValue;
		}
		
		/// <summary>
		/// Read 1 bytes and return the value
		/// </summary>
		/// <returns></returns>
		private int readByte()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[1];
			readBytes = moviStreamReader.Read(tmpBuffer,0,1);
			retValue = tmpBuffer[0];
			return retValue;
		}
		
		
		/// <summary>
		/// Process bytes in [xxdc] Chunck
		/// </summary>
		private void process__dc()
		{
			// Process xxdc tag
			int dcLen = moviBuffer.Length;
			string outValue="";
			int posRet = 0;
			int sPoint = 0; 
			int ePoint = 0;
			
			sPoint = myEnc.compareBytesArray(moviBuffer, AviRiffData.UserDataBytes, 0);
			
			while (sPoint < dcLen && sPoint >= 0)
			{
				outValue="";
				
				ePoint = myEnc.compareBytesArray(moviBuffer, AviRiffData.UserDataBytes, sPoint + 3);
				if (ePoint < 0) ePoint = myEnc.compareBytesArray(moviBuffer, AviRiffData.VOLStartBytes , sPoint + 3);
				if (ePoint < 0) ePoint = myEnc.compareBytesArray(moviBuffer, AviRiffData.VOPStartBytes , sPoint + 3);
				
				if (ePoint < 0)
				{
					// from sPoint to end of Byte Array
					outValue = myEnc.getHexFromBytes(moviBuffer,sPoint+4, (dcLen - (sPoint+3)));
					addNewUData(myEnc.getAsciiFromHex(outValue));					
					return;
				}
				else
				{
					// from sPoint to ePoint
					outValue = myEnc.getHexFromBytes(moviBuffer,sPoint+4, (ePoint - (sPoint+4)));
					addNewUData(myEnc.getAsciiFromHex(outValue));					
					sPoint = myEnc.compareBytesArray(moviBuffer, AviRiffData.UserDataBytes, ePoint);					
				}
				

			}
			
			
			return;
		}
		
		
		
		
		private void addNewUData(string newData)
		{
			int j;
			if (udReturn == null)
			{
				// There isn't any elements				
				udReturn = new string[1];
				udReturn[0] = newData;
			}
			else
			{
				// There is another element
				string[] tmpASR;
				tmpASR = new string[udReturn.Length + 1];
				
				for (j=0; j<udReturn.Length; j++)
				{	tmpASR[j] = udReturn[j];	}
				
				tmpASR[udReturn.Length] = newData;
				udReturn = tmpASR;
			}
			
			return;			
		}
		
		
	}

}
