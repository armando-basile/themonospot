using System;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization.Formatters.Soap;
using Gtk;
using Glade;
using System.IO;
using System.Windows.Forms;
using Utility;


namespace monoSpotMain
{
	
	
	
	
	/// <summary>
	/// monoSPOT is a content descripter for AVI files.
	/// </summary>
	public class monoSPOT
	{
		

		/// <summary>
		/// <p> monoSPOT's entry point.</p>
		/// </summary> 
		public static void Main (string[] args)
        {
			new monoSPOT (args);
        }


		// Form Objects
		[Glade.Widget] Gtk.Window MainWindow;
		[Glade.Widget] Gtk.Label lblFrame;
		[Glade.Widget] Gtk.Label lblFrameVideo;
		[Glade.Widget] Gtk.Label lblFrameAudio;
		[Glade.Widget] Gtk.Label lblFrameOptions;
		[Glade.Widget] Gtk.Frame frameFile;
		[Glade.Widget] Gtk.Frame frameVideo;
		[Glade.Widget] Gtk.Frame frameAudio;
		[Glade.Widget] Gtk.Frame frameOptions;
		[Glade.Widget] Gtk.Image imageVideo;
		[Glade.Widget] Gtk.Image imageAudio;		
		[Glade.Widget] Gtk.Label lblFileName;
		[Glade.Widget] Gtk.Entry txtFileName;
		[Glade.Widget] Gtk.Button cmdBrowse;
		[Glade.Widget] Gtk.Button cmdExport;
		[Glade.Widget] Gtk.Button cmdScanMovi;		
		[Glade.Widget] Gtk.TreeView tvVideo;		
		[Glade.Widget] Gtk.TreeView tvAudio;
		[Glade.Widget] Gtk.Fixed fixed2;

		// Serializable objects
		configurationClass myConfigClass; 
		 
		// Local variables
		string myConfigFilePath = "";
		Gtk.ListStore VinfoListStore;
		Gtk.ListStore AinfoListStore;
		monoSpotParser laProva;
		monoSpotMain.monoSPOTwait waitForm;
		
		

		/// <summary>
		/// <p> Create new instance of monoSPOT.</p>
		/// </summary>
		/// <param name="args">parameters passed to the application</param>
		public monoSPOT(string[] args)
		{
			Gtk.Application.Init ();
			
			Glade.XML gxml = new Glade.XML(null, "monoSPOT.glade", "MainWindow", null);
			gxml.Autoconnect(this);
			
			// configure objects on window
			configWinObjects();
			
			// Set Application Path Variable
			myConfigFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location.ToString();
			myConfigFilePath = myConfigFilePath.Replace(System.IO.Path.GetFileName(myConfigFilePath), "");
			Console.WriteLine("myConfigFilePath ".PadRight(20,(char)46) + myConfigFilePath);
			

			// Read Configuration file
			readConfigurationFile();

			// If there is a path to process in a command line
			if (args.Length > 0)
			{
				Console.WriteLine("Path: ".PadRight(20,(char)46) + args[0]);
				
				if (File.Exists(args[0]) == false)
				{	
					Console.WriteLine("Path not found...:");
					// No Path Founded				
					MessageDialog Dlg;
					Dlg=new MessageDialog(null ,DialogFlags.Modal,MessageType.Error,ButtonsType.Ok,
							"Path (" + args[0] + ") Not found... !!!");
					Dlg.Title = "Error detected";
					Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
					MainWindow.Visible=false;
					Dlg.Run();
					Dlg.Destroy();
					return;				

				}
				
				
				txtFileName.Text = args[0];
				goParseFile(args[0]);
			}
			
			Gtk.Application.Run ();
			
			
			
		}

		
		/// <summary>
		/// <p> Create new instance of monoSPOT.</p>
		/// </summary>		
		private void configWinObjects()
		{	
			// Window
			MainWindow.DeleteEvent += On_MainWindow_Delete;			
			MainWindow.SetSizeRequest(700,580);
			MainWindow.Resizable=false;
			MainWindow.Title = Assembly.GetExecutingAssembly().GetName().Name.ToString() + 
			                  " v" + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
			                  "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			MainWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			
			// Label for avi file
			lblFileName.TextWithMnemonic = "file name:";
			
			// Text box for avi file selected
			txtFileName.IsEditable=false;
			
			// Frames
			lblFrame.MarkupWithMnemonic = " <b>avi file to scan...</b> ";
			lblFrameVideo.MarkupWithMnemonic = " <b>video section</b> ";
			lblFrameAudio.MarkupWithMnemonic = " <b>audio section</b> ";
			lblFrameOptions.MarkupWithMnemonic = " <b>options section</b> ";
			imageVideo.FromPixbuf = Gdk.Pixbuf.LoadFromResource("video.png");
			imageAudio.FromPixbuf = Gdk.Pixbuf.LoadFromResource("sound.png");
			
			// *** ONLY FOR DEBUG PHASE ***
			// frameVideo.Visible=false;
			// frameAudio.Visible=false;
			// frameOptions.Visible=false;*/
			

			// TreeView Video & Audio...
			// Create column and header
			Gtk.TreeViewColumn VtypeInfoColumn = new Gtk.TreeViewColumn ();
			Gtk.TreeViewColumn VdescInfoColumn = new Gtk.TreeViewColumn ();
			VtypeInfoColumn.Title = "Information";			
			VdescInfoColumn.Title = "Description";
			Gtk.TreeViewColumn AtypeInfoColumn = new Gtk.TreeViewColumn ();
			Gtk.TreeViewColumn AdescInfoColumn = new Gtk.TreeViewColumn ();
			AtypeInfoColumn.Title = "Information";			
			AdescInfoColumn.Title = "Description";
			
			// add columns to TreeView object 
			tvVideo.RulesHint=true;
			tvAudio.RulesHint=true;
			tvVideo.AppendColumn(VtypeInfoColumn);
			tvVideo.AppendColumn(VdescInfoColumn);
			tvAudio.AppendColumn(AtypeInfoColumn);
			tvAudio.AppendColumn(AdescInfoColumn);
			
			// Create a model that will hold two strings
			VinfoListStore = new Gtk.ListStore (typeof (string), typeof (string));
			AinfoListStore = new Gtk.ListStore (typeof (string), typeof (string));
			
			// Add model to TreeView object			
			tvVideo.Model = VinfoListStore;
			tvVideo.Columns[0].Resizable=true;
			tvVideo.Columns[1].Resizable=true;
			
			tvAudio.Model = AinfoListStore;
			tvAudio.Columns[0].Resizable=true;
			tvAudio.Columns[1].Resizable=true;

			// Create the text cells that will display the info
  			Gtk.CellRendererText VtypeInfoCell = new Gtk.CellRendererText ();		
  			Gtk.CellRendererText VdescInfoCell = new Gtk.CellRendererText ();
  			Gtk.CellRendererText AtypeInfoCell = new Gtk.CellRendererText ();		
  			Gtk.CellRendererText AdescInfoCell = new Gtk.CellRendererText ();  			
  			VtypeInfoColumn.PackStart (VtypeInfoCell, true);
  			VdescInfoColumn.PackStart (VdescInfoCell, true);
  			AtypeInfoColumn.PackStart (AtypeInfoCell, true);
  			AdescInfoColumn.PackStart (AdescInfoCell, true);
  			
  			// Tell the Cell Renderers which items in the model to display
  			VtypeInfoColumn.AddAttribute (VtypeInfoCell, "text", 0);
  			VdescInfoColumn.AddAttribute (VdescInfoCell, "text", 1);
  			AtypeInfoColumn.AddAttribute (AtypeInfoCell, "text", 0);
  			AdescInfoColumn.AddAttribute (AdescInfoCell, "text", 1);
  			
			// Button to browse file
			cmdBrowse.Released += On_cmdBrowse_Clicked;

			// Button to export info			
			cmdExport.Label=" Export";
			Widget cmdExport_Imgage = new Image(Gdk.Pixbuf.LoadFromResource("textfile.png"));
			cmdExport.Image = cmdExport_Imgage;
			cmdExport.Image.Visible=true;
			cmdExport.Released += On_cmdExport_Clicked;
			
			// Button to scan movi
			cmdScanMovi.Label = "Scan movie";
			Widget cmdScanMovi_Imgage = new Image(Gdk.Pixbuf.LoadFromResource("video_mini.png"));
			cmdScanMovi.Image = cmdScanMovi_Imgage;
			cmdScanMovi.Image.Visible=true;			
			cmdScanMovi.Released += On_cmdMoviScan_Clicked;
			cmdScanMovi.Visible=true;
			
			
			
			MainWindow.Show();
		}
		


		/// <summary>
		/// <p>The On_MainWindow_Delete method is used when we want quit.</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_MainWindow_Delete(object sender,EventArgs a)
    	{
        	Gtk.Application.Quit();
        }


		/// <summary>
		/// <p>The On_cmdBrowse_Clicked method is used is pressed the button to select
		/// the .avi file to scan.</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_cmdBrowse_Clicked(object sender,EventArgs a)
    	{
			// New dialog for select avi file 
			Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Select .avi file to scan...", 
			                                MainWindow,
			                                FileChooserAction.Open, 
			                                "Cancel", Gtk.ResponseType.Cancel,
                                            "Open", Gtk.ResponseType.Accept);
			
			if (myConfigClass.defaultPath.ToString() != "")
			{	
				if ( Directory.Exists(myConfigClass.defaultPath.ToString()) )
				{	
					FileBox.SetCurrentFolder(myConfigClass.defaultPath.ToString());	
				}
			}
			
			// Filter for useing only avi files
			Gtk.FileFilter myFilter = new Gtk.FileFilter(); 
			myFilter.AddPattern("*.avi");
			myFilter.AddPattern("*.xvid");
			myFilter.AddPattern("*.divx");			
			myFilter.Name = "avi, divx, xvid files";
			FileBox.AddFilter(myFilter);
			
			// Manage result of dialog box
			FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			int retFileBox = FileBox.Run();
			if (retFileBox == Gtk.ResponseType.Accept.value__)
			{	
				// path of a right file returned
				string tmpFName = FileBox.Filename.ToString();
				
				myConfigClass.defaultPath = FileBox.Filename.ToString();
				myConfigClass.defaultPath = myConfigClass.defaultPath.Replace(System.IO.Path.GetFileName(myConfigClass.defaultPath), "");
				
				FileBox.Destroy();
				FileBox.Dispose();
				
				this.txtFileName.Text = tmpFName;
				
			}
			else
			{
				// nothing returned
				FileBox.Destroy();
				FileBox.Dispose();
				return;
			}
			

			writeConfigurationFile();
			
			
			// Start Parse Process
			goParseFile(this.txtFileName.Text.ToString());
			
      	}			

	    
	    
	    
	    
	    /// <summary>
		/// <p>The On_cmdExport_Clicked method is used when pressed the button to export
		/// result of scan.</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_cmdExport_Clicked(object sender,EventArgs a)
    	{

			// If there isn't any info
	    	if (VinfoListStore.IterNChildren() == 0 )
	    		return;
	    	
	    	
	    	// New dialog to save file 
			Gtk.FileChooserDialog FileBox = new Gtk.FileChooserDialog("Insert file path to export scan result...", 
			                                MainWindow,
			                                FileChooserAction.Save, 
			                                "Cancel", Gtk.ResponseType.Cancel,
                                            "Accept", Gtk.ResponseType.Accept);
			
			// Manage result of dialog box
			FileBox.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
			int retFileBox = FileBox.Run();
			
			if (retFileBox == Gtk.ResponseType.Accept.value__)
			{	
	    		// Save File
	    		DateTime Oggi = DateTime.Now; 
	    		StreamWriter outputStream = new StreamWriter(FileBox.Filename , false);
	    		outputStream.WriteLine(("Avi File:").PadRight(25)+ laProva.m_shortname);
	    		outputStream.WriteLine(("Analisys Date:").PadRight(25)+ Oggi.ToString("dd-MM-yyyy HH:mm:ss") );
	    		outputStream.WriteLine("-----------------------------------------------------------------------------");
		    	
		    	
		    	// Video Stream
		    	TreeIter tmpIter = new TreeIter();
				VinfoListStore.GetIterFirst(out tmpIter);
				string tmpItem1 = (string) VinfoListStore.GetValue(tmpIter,0);
				string tmpItem2 = (string) VinfoListStore.GetValue(tmpIter,1);
				outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
				
				while(VinfoListStore.IterNext(ref tmpIter)) 
				{
	  				tmpItem1 = (string) VinfoListStore.GetValue(tmpIter,0);
	  				tmpItem2 = (string) VinfoListStore.GetValue(tmpIter,1);
					outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
				}
	    		
	    		outputStream.WriteLine("");
	    		
	    		// Audio Stream
	    		AinfoListStore.GetIterFirst(out tmpIter);
	    		tmpItem1 = (string) AinfoListStore.GetValue(tmpIter,0);
	  			tmpItem2 = (string) AinfoListStore.GetValue(tmpIter,1);
	  			outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);

				while(AinfoListStore.IterNext(ref tmpIter)) 
				{
	  				tmpItem1 = (string) AinfoListStore.GetValue(tmpIter,0);
	  				tmpItem2 = (string) AinfoListStore.GetValue(tmpIter,1);
					outputStream.WriteLine(tmpItem1.PadRight(25)+ tmpItem2);
				}
	  			
	  			outputStream.WriteLine(""); outputStream.WriteLine(""); outputStream.WriteLine("");
	  			outputStream.WriteLine("Report generated by themonoSPOT v" + 
	  									Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
			                  			"." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
			                  			"." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString());
				outputStream.WriteLine("project web site: http://code.google.com/p/themonoproject");
	  			
	    		outputStream.Close();
	    		outputStream.Dispose();
			}

			
			FileBox.Destroy();
			FileBox.Dispose();
			return;
	    	
	    }
	    
	    
		public void moviScanCallBack(string[] retInfos)
		{	
				
			int j;
			
			// Set old last Row index
			int itersBefore = VinfoListStore.IterNChildren();
			
			// Update list
			if (retInfos.Length >0)
			{
				for (j=0; j<retInfos.Length; j++)
				{
					VinfoListStore.AppendValues("User Data:", retInfos[j]);
				}
			}

			// Scan old rows			
			TreeIter tmpIter;
			string tmpItem1;			
			tmpIter = new TreeIter();
			for (j=0; j<itersBefore; j++)
			{
				VinfoListStore.IterNthChild(out tmpIter, j);
				tmpItem1 = (string) VinfoListStore.GetValue(tmpIter,0);
				if (tmpItem1 == "User Data:")	VinfoListStore.Remove(ref tmpIter);
			}
			
			
		}
		
		/// <summary>
		/// <p>The On_cmdMoviScan_Clicked method is used is pressed the button to scan 
		/// the MOVI chunk in .avi file.</p>
		/// </summary>
		/// <param name="o">The sender objet</param>
		/// <param name="args">Arguments to be passed</param>	
	    private void On_cmdMoviScan_Clicked(object sender,EventArgs a)
	    {
			
				/* MessageDialog Dlg;
				Dlg=new MessageDialog(MainWindow ,DialogFlags.Modal,MessageType.Info ,ButtonsType.Ok,
						"Function not yet implemented ");
				Dlg.Title = "monoSpotParser Information";
				Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				Dlg.Run();
				Dlg.Destroy();
				return;	*/
				
	    	System.Threading.Thread new_Thread = new System.Threading.Thread(new System.Threading.ThreadStart(startMOVI));
	    	new_Thread.Start();
    	
	    }
	    

		public void startMOVI()
		{
	    	Gtk.Application.Invoke(delegate {
	    		MainWindow.Visible=false;
	    		waitForm = new monoSpotMain.monoSPOTwait(laProva.m_filename, laProva.m_MoviStart, laProva.m_MoviSize );	    		
	    		waitForm.callBackFunction += moviScanCallBack;
	    		waitForm.configWinObjects();
	    		MainWindow.Visible=true;
	    		});
		
		}


		/// <summary>
		/// <p>This method write the parameters in a config file.</p>
		/// </summary>
		private void writeConfigurationFile()
		{
			string filename = myConfigFilePath + "configuration.xml";
			
			SoapFormatter formatter = new SoapFormatter();
			using (FileStream file = new FileStream(filename, FileMode.Create, FileAccess.Write))
  			{
    			// Serializaion...

    			formatter.Serialize(file, myConfigClass);
			}
		}
		


		/// <summary>
		/// <p>This method read the parameters in a config file.</p>
		/// </summary>
		private void readConfigurationFile()
		{
			string filename = myConfigFilePath + "configuration.xml";
			
			if (File.Exists(filename) == false)
			{	
				// If config file is missing
				myConfigClass = new configurationClass();
				
				// Create new parameters
				myConfigClass.defaultPath = "";
				return;			
			}
			
			SoapFormatter formatter = new SoapFormatter();
			using (FileStream file = new FileStream(filename, FileMode.Open, FileAccess.Read))
  			{
    			// Deserializaion...
    			myConfigClass = formatter.Deserialize(file) as configurationClass;
			}
		}
		
		
		
		
		
		/// <summary>
		/// Function to process .Avi file
		/// </summary>
		/// <param name="pathOfFile">Path of file to process</param>
		void goParseFile (string pathOfFile)
		{
			// Clear two collections
			VinfoListStore.Clear();
			AinfoListStore.Clear();

			try
			{
				// Create the parser object and scan the avi file
				laProva = new monoSpotParser();
				laProva.OpenAviFile(pathOfFile);
			}
			catch (monoSpotParserException e)
			{
				// Exception of monoParser
				MessageDialog Dlg;
				Dlg=new MessageDialog(MainWindow ,DialogFlags.Modal,MessageType.Warning,ButtonsType.Ok,
						"Error: " + e.Message);
				Dlg.Title = "monoSpotParser Error";
				Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				Dlg.Run();
				Dlg.Destroy();
				return;					
			}
			catch (Exception e)
			{
				// Generic Exception			
				MessageDialog Dlg;
				Dlg=new MessageDialog(MainWindow ,DialogFlags.Modal,MessageType.Warning,ButtonsType.Ok,
						"Error: " + e.Message);
				Dlg.Title = "Generic Error";
				Dlg.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT.png");
				Dlg.Run();
				Dlg.Destroy();
				return;	
			}
			
			// Explain readed infos
			DecodeInfos();
			
			return;
			
			
		}

		/// <summary>
		/// Explain info readed
		/// </summary>		
		void DecodeInfos()
		{
			
			int k, l;
			int iV=0;
			int iA=0;
			double sizOfAudio=0;
			double sizOfVideo=0;
			double sizOfHeader=0;
			double videoQuality=0;
			double WdH=0;
			int framePerSec=0;
			int blockPerSec=0;
			int AverageVideoBitRate = 0;
			string Frame_Size = "";
			string Total_Time = "";
			string Frame_Rate = "";
			string Total_Frames = "";
			string Video_Data_Rate = "";
			
			for (k=0; k<laProva.headerStreams.Length; k++ )
			{
				// Streams present
				if (cEncoding.FromFourCC(laProva.headerStreams[k].fccType) == "vids" )
				{
					// Video Stream
					long totalTime = 0;
					if (laProva.headerFile.dwMicroSecPerFrame > 0)
						totalTime = (long)((long)laProva.headerFile.dwTotalFrames * (long)laProva.headerFile.dwMicroSecPerFrame);
					totalTime = (long)(totalTime / 1000000.0);					
					int hours = (int)(totalTime / 3600);
					totalTime -= (long)(hours * 3600);
					int mins = (int)(totalTime / 60);
					totalTime -= (long)(mins * 60);
					framePerSec = laProva.headerStreams[k].dwRate / laProva.headerStreams[k].dwScale;
					WdH = laProva.videoStreams[0].biWidth;
					WdH /= laProva.videoStreams[0].biHeight;
					
					
					
					Frame_Size = laProva.videoStreams[0].biWidth.ToString() +
           						 " x " +
						         laProva.videoStreams[0].biHeight.ToString();
					Total_Time = String.Format("{0:00}:{1:00}:{2:00.00#} seconds", hours, mins, totalTime);
					Frame_Rate = String.Format("{0:N2} Frames/Sec", (1000000.0 /laProva.headerFile.dwMicroSecPerFrame));
					Total_Frames = String.Format("{0:G}", laProva.headerFile.dwTotalFrames  );
					Video_Data_Rate = String.Format("{0:N2} frames/Sec", framePerSec );
					
					
					iV++;
				}
				else
				{
					// Audio Stream
					string aFormat = laProva.audioStreams[iA].wFormatTag.ToString("X4");
					string CVBR = "";
					
					// Audio Rate Calc
					double audioRate = (8.0 * laProva.audioStreams[iA].nAvgBytesPerSec) ;
					if (laProva.headerStreams[k].dwSampleSize > 0 ) audioRate /= (double)laProva.headerStreams[k].dwSampleSize;
					
					if(aFormat == "0055")						
					{
						CVBR = "";
						
						// MP3 CODEC
						AinfoListStore.AppendValues("Audio " + (iA+1).ToString() + ":",
					    	                        laProva.parseAudioType(aFormat) + " " + CVBR + " " +
					        	                    String.Format("{0:N2} Kb/Sec", audioRate / 1000.0) + " " + 
					            	                "- " + laProva.audioStreams[iA].nSamplesPerSec + " Hz (" +
					                	            laProva.audioStreams[iA].nChannels.ToString() + " Channels)");
					}
					else
					{
						// Other codec
						AinfoListStore.AppendValues("Audio " + (iA+1).ToString() + ":",
					    	                        laProva.parseAudioType(aFormat) + " " +
					        	                    String.Format("{0:N2} Kb/Sec", audioRate / 1000.0) + " " + 
					            	                "- " + laProva.audioStreams[iA].nSamplesPerSec + " Hz (" +
					                	            laProva.audioStreams[iA].nChannels.ToString() + " Channels)");
					}
										
					// Calc Data for AVBitrate
					blockPerSec = laProva.headerStreams[k].dwRate / laProva.headerStreams[k].dwScale;
					
					double tmpAudio = laProva.headerStreams[k].dwLength;
					tmpAudio *= laProva.audioStreams[iA].nAvgBytesPerSec;
					tmpAudio /= blockPerSec;
					sizOfAudio += tmpAudio;
					
					// increment total audio streams
					iA++;
					
				}
				
				
			}
			
			
			sizOfHeader = laProva.headerFile.dwTotalFrames * 8 * (iA+1); 
			sizOfVideo = laProva.m_MoviSize - sizOfHeader - sizOfAudio;
			AverageVideoBitRate = (int)((sizOfVideo * framePerSec * 8) /  (laProva.headerFile.dwTotalFrames * 1000));
			videoQuality = (0.75 * WdH) * (AverageVideoBitRate / framePerSec);
			
			VinfoListStore.AppendValues("Video:", cEncoding.FromFourCC(laProva.videoStreams[0].biCompression));
			VinfoListStore.AppendValues("Frame Size:", Frame_Size );
			VinfoListStore.AppendValues("Average Video Bitrate:", AverageVideoBitRate.ToString() + " Kb/Sec");			
			VinfoListStore.AppendValues("Avi file size:", ( (laProva.m_filesize / 1024).ToString("#,### KB")));
			VinfoListStore.AppendValues("Total Time:", Total_Time );
			VinfoListStore.AppendValues("Frame Rate:", Frame_Rate) ;
			VinfoListStore.AppendValues("Total Frames:", Total_Frames);
			VinfoListStore.AppendValues("Video Data Rate:", Video_Data_Rate );
			VinfoListStore.AppendValues("Video Quality:", videoQuality.ToString("#,###.##") );
			
			if (laProva.userData.Length >0 )
				for (l=0; l<laProva.userData.Length;l++)
					VinfoListStore.AppendValues("Info Data:", laProva.userData[l].ToString());

			
			Console.WriteLine("MOVI start ".PadRight(20,(char)46) +laProva.m_MoviStart.ToString());
			Console.WriteLine("MOVI size ".PadRight(20,(char)46) +laProva.m_MoviSize.ToString());
			
			return;
			

			
		}
		

		
		

		
		

		
		
		
		
		
		
		
	}

}
