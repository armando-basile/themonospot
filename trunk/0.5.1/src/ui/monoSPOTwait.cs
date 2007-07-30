using System;
using System.Reflection;
using System.Collections;
using Gtk;
using GLib;
using System.Text;
using Glade;
using Pango;
using System.IO;
using Utility;


namespace monoSpotMain
{
	
	public delegate void callBackValues();
	
	/// <summary>
	/// monoSPOTwait
	/// </summary>
	public class monoSPOTwait
	{

		// Form Objects
		[Glade.Widget] Gtk.Window topWindow;
		[Glade.Widget] Gtk.ProgressBar myPBar;
		[Glade.Widget] Gtk.Button cmdCancel;
		[Glade.Widget] Gtk.Label lblInfo;

		bool flagAbort = false;
		
		string filename = "";
		string _udToChange ="";
		byte[] moviBuffer;

		long _moviSize;		
		long _moviOffset;
		long _idxSize;
		long _idxOffset;
		long _moviSizeNew;
		int _totalFrames;
		int stdBufferSize = 4096;

		Window pWin;
		cEncoding myEnc = new cEncoding();
		
		FileStream inFile = null;
		FileStream outFile = null;
		
		long[] framesOffset = new long[0];
		long[] framesSize = new long[0];
		
		public event callBackValues callBackFunction;
		
		/// <summary>
		/// <p> Create new instance of monoSPOTwait.</p>
		/// </summary>
		/// <param name="args">parameters passed to the application</param>
		public monoSPOTwait(ref Gtk.Window pW, 
		                    string file_name, 
		                    long offSet_Movi, 
		                    long len_Movi, 
		                    int totFrames, 
		                    long offset_Idx, 
		                    long len_Idx, 
		                    string dataToChange)
		{
			// Update local variables
			filename = file_name;
			_moviOffset = offSet_Movi;
			_moviSize = len_Movi;
			_totalFrames = totFrames;
			_idxSize = len_Idx;
			_idxOffset = offset_Idx;
			_udToChange = dataToChange;
			pWin = pW;
			
			
			Glade.XML gxml = new Glade.XML(null, "waitWindow.glade", "topWindow", null);
			gxml.Autoconnect(this);
			this.topWindow.TransientFor = pW;
			this.topWindow.Parent = pW;
			this.topWindow.Modal=true;
			configWinObjects();
		}

		
		/// <summary>
		/// <p> Create new instance of monoSPOT.</p>
		/// </summary>		
		public void configWinObjects()
		{	
			// Window
			//topWindow.SetSizeRequest(625,115);
			topWindow.Resizable=false;
			topWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
			                  " v" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString() + 
							  " - Save file";
			topWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");		
		}
		
		private void on_cmdCancel_clicked(object sender, EventArgs a)
		{
			Console.WriteLine("on_cmdCancel_clicked");			
			flagAbort = true;
		}
		
		static private void on_topWindow_delete_event(object sender, DeleteEventArgs args)
		{
			Console.WriteLine("on_topWindow_delete_event");			
		}
		
		public void saveAvi()
		{
			string tmpFName = "";
			int newTotalBytes = 0;
			
			Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Save file as...", 
			                                topWindow,
			                                FileChooserAction.Save, 
			                                "Cancel", Gtk.ResponseType.Cancel,
                                            "Accept", Gtk.ResponseType.Accept);
			
			// Filter for useing only avi files
			// Gtk.FileFilter myFilter = new Gtk.FileFilter(); 
			// myFilter.AddPattern("*.monosim");
			// myFilter.Name = "monosim files";
			// FileBox.AddFilter(myFilter);			
			FileBox.WindowPosition= WindowPosition.CenterAlways;
				
			// Manage result of dialog box
			FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT_16.png");
			int retFileBox = FileBox.Run();
			if (retFileBox == Gtk.ResponseType.Accept.value__)
			{	
				// path of a right file returned
				tmpFName = FileBox.Filename.ToString();				
				FileBox.Destroy();
				FileBox.Dispose();
			
			}
			else
			{
				// nothing returned
				FileBox.Destroy();
				FileBox.Dispose();
				this.topWindow.Dispose();
				this.topWindow.Destroy();
				this.topWindow=null;
				this.callBackFunction() ;
				return;
			}
			
			this.topWindow.WindowPosition= WindowPosition.CenterAlways;
			this.topWindow.ShowAll();
			
			while (Gtk.Application.EventsPending ())
        		Gtk.Application.RunIteration ();
			
			// Begin Write new avi file
			inFile = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
			outFile = new FileStream(tmpFName, FileMode.Create,FileAccess.Write, FileShare.None);
			
			
			FileInfo fi = new FileInfo(filename);
			long filmsize = fi.Length;
			long filmoffset = 0;
			long diffBytes = 0;
			byte[] tmpBytes;
			int totMoviItems = Convert.ToInt32(_idxSize / 16) + 3000;
			
			framesOffset = new long[ totMoviItems ];
			framesSize = new long[ totMoviItems ];
			int bufferSize = stdBufferSize;
			
			// Write data before MOVI chunk
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)_moviOffset;
			flagAbort=false;
			
			Console.WriteLine("Write Header START");
			lblInfo.Markup = "update file header... wait please";
			while (filmoffset < _moviOffset)
			{
				if ((filmoffset + bufferSize) > _moviOffset)
					bufferSize = (int)(_moviOffset - filmoffset);
				
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			Console.WriteLine("Write Header END");
			
			
			// Write new MOVI chunk from old
			if (writeMoviChunk() != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
			}
			
			
			
			// Write data before IDX1 chunk
			diffBytes = (_idxOffset - inFile.Position);
			bufferSize = stdBufferSize;
			
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT_1 START");
			lblInfo.Markup = "update file content... wait please";
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 1 END");
			
			
			
			// Write new IDX1 chunk from new MOVI created
			if (writeIdx1Chunk() != 0)
			{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
			}
			
			
			// Write data after IDX1 chunk			
			diffBytes = (filmsize - inFile.Position);
			bufferSize = stdBufferSize;
			
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)diffBytes;
			filmoffset = 0;
			
			Console.WriteLine("Write CONTENT 2 START");
			lblInfo.Markup = "update file content... wait please";
			while (filmoffset < diffBytes)
			{
				if ((filmoffset + bufferSize) > diffBytes)
					bufferSize = (int)(diffBytes - filmoffset);
						
				tmpBytes = new byte[bufferSize];
				inFile.Read(tmpBytes, 0, bufferSize);
				outFile.Write(tmpBytes, 0, bufferSize);
				
				myPBar.Adjustment.Value = (double)filmoffset;
				myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
				while (Gtk.Application.EventsPending ())
        			Gtk.Application.RunIteration ();
				
				if (flagAbort == true)
				{
					outFile.Close();
					outFile.Dispose();
					outFile = null;
					inFile.Close();
					inFile.Dispose();
					inFile = null;
					this.topWindow.Dispose();
					this.topWindow.Destroy();
					this.topWindow=null;
					this.callBackFunction() ;
					return;
				}
				
				filmoffset += bufferSize;				
			}
			
			Console.WriteLine("Write CONTENT 2 END");
			
			// Close output
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			
			// Read File Infos
			fi = new FileInfo(tmpFName);
			newTotalBytes = (int)(fi.Length - 8);
			fi = null;
			
			// Update MOVI Size and File Size
			outFile = new FileStream(tmpFName, FileMode.Open,FileAccess.Write, FileShare.None);
			outFile.Seek(4,SeekOrigin.Begin);
			outFile.Write(intToByteArray(newTotalBytes),0,4);
			outFile.Seek(_moviOffset - 8,SeekOrigin.Begin);
			outFile.Write(intToByteArray((int)(_moviSizeNew + 4)),0,4);
						
			// Close all streams and return to Main Window
			outFile.Close();
			outFile.Dispose();
			outFile = null;
			inFile.Close();
			inFile.Dispose();
			inFile = null;
			
			this.topWindow.Destroy();
			this.topWindow.Dispose();			
			this.callBackFunction();
			
		}
		
		
		
		
		
		
		
		
		
		
		
		// write MOVI Chunk frames
		private int writeMoviChunk()
		{
			long tmpMoviPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			int sizeOfFrame = 0;
			int newByteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;		
			int lenOfFrame = 0;
			string hexFourCC = "";
			byte[] tmpByteArray = new byte[0];
			int stepGuiUpdate = 1024;
			int stepFrame = 0;

			// Write data before MOVI chunk
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)_moviSize;

			lblInfo.Markup = "update MOVI chunk... wait please";
			Console.WriteLine("Write MOVI START (" + _moviOffset.ToString() + " SIZE " + _moviSize.ToString() + ")");
			_moviSizeNew = 0;
			
			while (tmpMoviPointer < _moviSize)
			{
				// Exit if Cancel button was pressed
				if (flagAbort == true)
					return 1;
				
				FourCC = readDWord();
				hexFourCC = FourCC.ToString("X8");
				sFourCC = Utility.cEncoding.FromFourCC(FourCC);
				byteOfRead = readDWord();
				
				tmpMoviPointer += 8;
				_moviSizeNew += 8;
				
				// Adjust bytes to read (no odd)
				sizeOfFrame = byteOfRead; 
				if ((byteOfRead % 2) != 0 )
					byteOfRead++;				
				
				tmpByteArray = new byte[byteOfRead];
				inFile.Read(tmpByteArray, 0, byteOfRead);
				
				stepFrame ++;
				
				// Verify frame type
				if (sFourCC.Substring(2,2) == "dc" || sFourCC.Substring(2,2) == "db")
	    		{
	    			// 
	    			tmpByteArray = processFrame(tmpByteArray, ref sizeOfFrame);
	    			newByteOfRead = tmpByteArray.Length;
	    			
	    			framesOffset[frameCount] = outFile.Position;
	    			framesSize[frameCount] = (long)newByteOfRead;
	    			outFile.Write(intToByteArray(FourCC),0, 4);
	    			outFile.Write(intToByteArray(sizeOfFrame),0, 4);
	    			outFile.Write(tmpByteArray, 0, newByteOfRead);
	    			
	    			tmpMoviPointer += byteOfRead;
	    			_moviSizeNew += newByteOfRead;
					
				}
	    		else
	    		{
	    			framesOffset[frameCount] = outFile.Position;	    			
	    			framesSize[frameCount] = (long)byteOfRead;
	    			outFile.Write(intToByteArray(FourCC),0, 4);
	    			outFile.Write(intToByteArray(sizeOfFrame),0, 4);
	    			outFile.Write(tmpByteArray, 0, byteOfRead);
	    			
	    			tmpMoviPointer += byteOfRead;
	    			_moviSizeNew += byteOfRead;
	    		}

	    		frameCount ++;
	    		
	    		if (stepFrame >= stepGuiUpdate)
	    		{
		    		// Update progressbar
		    		myPBar.Adjustment.Value = (double)tmpMoviPointer;
					myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
					while (Gtk.Application.EventsPending ())
	        			Gtk.Application.RunIteration ();
					
					stepFrame = 0;
				}
	    		
	    		
			}
			Console.WriteLine("Tot Frames: " + frameCount.ToString());
			Console.WriteLine("Write MOVI END");
			
			return 0;
			
		}
		
		// Extract UserData info from DC subarea
		private byte[] processFrame(byte[] inByteArray, ref int frameLength)
		{
			cEncoding myEnc = new cEncoding();			
			byte[] tmpByteArray = null;
			byte[] userdataOld = null;
			byte[] outByteArray = null;
			ASCIIEncoding TextEncoding = new ASCIIEncoding();
			
			tmpByteArray = inByteArray;
			userdataOld = TextEncoding.GetBytes(_udToChange);

			int startPos = myEnc.compareBytesArray(tmpByteArray, userdataOld,0);
			
			// int totalFrameBytes = 0;
			int totalFrameBytes, newFrameLength;
			
			if (startPos >= 0)
			{
				// totalFrameBytes = tmpByteArray.Length - _udToChange.Length + 12;
				newFrameLength = frameLength - _udToChange.Length + 12;
				
				// Padded to an even number of bytes but make sure the padding isn't included
                // in the size written to the chunk header or index
				totalFrameBytes = newFrameLength;
				
				if ((totalFrameBytes % 2) != 0)
					totalFrameBytes ++;
				
				// frameLength = totalFrameBytes;
				frameLength = newFrameLength;
				
				outByteArray = new byte[totalFrameBytes];
				Array.Copy(tmpByteArray, outByteArray, startPos);
				Array.Copy(TextEncoding.GetBytes("DivX999b000p"), 0, outByteArray, startPos, 12);
				Array.Copy(tmpByteArray, startPos + _udToChange.Length , outByteArray, startPos + 12, frameLength - _udToChange.Length - startPos);
				
			}
			else
				outByteArray = tmpByteArray;
			
			
			
			return outByteArray;			
		}
		
		
		// Write new Idx1 Chunk
		private int writeIdx1Chunk()
		{
			long tmpIdxPointer = 0;
			int FourCC = 0;
			int byteOfRead = 0;
			string sFourCC = "";
			int frameCount = 0;
			string hexFourCC = "";
			byte[] tmpByteArray = new byte[16];
			byte[] tmpDWordArray = new byte[4];
			int stepGuiUpdate = 256;
			int stepFrame = 0;
			
			// Write data before MOVI chunk
			myPBar.Adjustment.Value = 0;
			myPBar.Adjustment.Lower = 0;
			myPBar.Adjustment.Upper = (double)_idxSize;

			lblInfo.Markup = "update IDX chunk... wait please";
			Console.WriteLine("Write IDX START (" + _idxOffset.ToString() + " SIZE " + _idxSize.ToString() + ")");
			
			while (tmpIdxPointer < _idxSize)
			{
				// Exit if Cancel button was pressed
				if (flagAbort == true)
					return 1;
				
				inFile.Read(tmpByteArray,0,16);
				
				// Offsets are relative to the start of the 'movi' list type
				tmpDWordArray = intToByteArray((int)(framesOffset[frameCount] - (_moviOffset - 4)));
				for (int j=0; j<4; j++)
					tmpByteArray[8+j]=tmpDWordArray[j];

				tmpDWordArray = intToByteArray((int)framesSize[frameCount]);
				for (int j=0; j<4; j++)
					tmpByteArray[12+j]=tmpDWordArray[j];
				
				outFile.Write(tmpByteArray,0,16);
				
				tmpIdxPointer += 16;
				
				frameCount ++;
				stepFrame++;
	    		
	    		if (stepFrame >= stepGuiUpdate)
	    		{
		    		// Update progressbar
		    		myPBar.Adjustment.Value = (double)tmpIdxPointer;
					myPBar.Text = Convert.ToInt32(myPBar.Fraction * 100).ToString("D3") + "%";
					while (Gtk.Application.EventsPending ())
	        			Gtk.Application.RunIteration ();
					
					stepFrame = 0;
				}
				
			}
			
			
			Console.WriteLine("Tot Frames: " + frameCount.ToString());
			Console.WriteLine("Write IDX END");
			
			return 0;
		}
		
		
		
		
		
		
		
		
		// returns a byte array of length 4
		private byte[] intToByteArray(int i) 
		{
			byte[] dword = new byte[4];
			dword[0] = (byte) (i & 0x00FF);
			dword[1] = (byte) ((i >> 8) & 0x000000FF);
			dword[2] = (byte) ((i >> 16) & 0x000000FF);
			dword[3] = (byte) ((i >> 24) & 0x000000FF);
			return dword;
		}
		
		
		
		/// <summary>
		/// Read 4 bytes and return the value
		/// </summary>
		/// <returns></returns>
		private int readDWord()
		{	
			int retValue;
			int readBytes;
			byte[] tmpBuffer = new byte[4];
			try
			{
				readBytes = inFile.Read(tmpBuffer,0,4);
			}
			catch (Exception e)
			{
				Console.WriteLine("error: " + e.Message);
				Gtk.Application.Quit();
			}
			retValue = tmpBuffer[0]+(tmpBuffer[1]<<8)+(tmpBuffer[2]<<16)+(tmpBuffer[3]<<24);
			return retValue;
		}
		
		
		
		
		
		
		
		

	}
}
