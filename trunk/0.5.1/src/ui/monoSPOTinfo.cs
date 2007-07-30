using System;
using Gtk;
using System.Reflection;

namespace monoSpotMain 
{

	public class monoSPOTinfo
	{
	
		[Glade.Widget] Gtk.Dialog 		infoWindow;
		[Glade.Widget] Gtk.Button	 	logoApp;
		[Glade.Widget] Gtk.Image 		monologo;
		[Glade.Widget] Gtk.Label    	lblName;
		[Glade.Widget] Gtk.Label    	lblInfo;	
		[Glade.Widget] Gtk.TextView		txtInfo;
		[Glade.Widget] Gtk.Button   	cmdExit;
		[Glade.Widget] Gtk.Button	 	linkmono;
		
		
		
		public monoSPOTinfo(Gtk.Window w, DialogFlags df)
		{
			Glade.XML rxml = new Glade.XML (null, "infowin.glade", "infoWindow", null);
			rxml.Autoconnect (this);
			
			this.infoWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT_16.png");			
			Image monoL = new Image(Gdk.Pixbuf.LoadFromResource("monologo.png"));
			Image simL = new Image(Gdk.Pixbuf.LoadFromResource("monoSPOT_48.png"));
			linkmono.Image = monoL;
			
			logoApp.Image = simL;
			
			this.infoWindow.TransientFor = w;
			//this.infoWindow.Modal=true;
			this.infoWindow.Title = "Information about...";
			this.lblName.Markup = "<b><big><big>" + Assembly.GetExecutingAssembly().GetName().Name.ToString() + "</big></big></b>   " + 
			                      Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
		                  		  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
		                          "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			
			//cmdExit.Label = myTranslator.readTranslatedString(36);
			lblInfo.Markup = "realized by hman (hmandevteam@gmail.com)" + Environment.NewLine + 
			                 "and cjg (cjg@cruxppc.org) to scan avi files" + Environment.NewLine + 
			                 "and extract audio and video informations. " + Environment.NewLine + 
				             "special thanks to moitah (moitah@yahoo.com)";
			infoWindow.Run();		 
			infoWindow.Destroy();		
			return;		
		}
	
	
		public void On_Linkmono_Press(object sender, EventArgs a)
		{
			System.Diagnostics.Process.Start("http://www.mono-project.com");
		}
	
		public void On_Logoapp_Press(object sender, EventArgs a)
		{
			System.Diagnostics.Process.Start("http://www.integrazioneweb.com/themonospot");
		}

	}
	
}
