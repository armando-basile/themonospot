using System;
using Gtk;
using System.Diagnostics;
using System.Reflection;

namespace themonospot_Gui_Main 
{

	public class tmsINFO
	{
	
		[Glade.Widget] Gtk.Dialog 		infoWindow;
		[Glade.Widget] Gtk.Button	 	logoApp;
		[Glade.Widget] Gtk.Image 		monologo;
		[Glade.Widget] Gtk.Label    	lblName;
		[Glade.Widget] Gtk.Label    	lblGuiName;
		[Glade.Widget] Gtk.Label    	lblInfo;	
		[Glade.Widget] Gtk.TextView		txtInfo;
		[Glade.Widget] Gtk.Button   	cmdExit;
		[Glade.Widget] Gtk.Button	 	linkmono;
		
		
		
		public tmsINFO(Gtk.Window w, DialogFlags df, string BaseRel)
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
			this.lblName.Markup = "<b><big><big>" + Assembly.GetExecutingAssembly().GetName().Name.ToString() + "</big></big></b> " + BaseRel;
            
            lblGuiName.Markup = "<b><big>gtk# gui</big></b> " + Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + 
		                  		  "." + Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString() + 
		                          "." + Assembly.GetExecutingAssembly().GetName().Version.Build.ToString();
			

			//cmdExit.Label = myTranslator.readTranslatedString(36);
			lblInfo.Markup = "realized by hman (hmandevteam@gmail.com)" + Environment.NewLine + 
			                 "and cjg (cjg@cruxppc.org) to scan avi files" + Environment.NewLine + 
			                 "and extract audio and video informations. " + Environment.NewLine + 
				             "special thanks to moitah (moitah@yahoo.com)" + Environment.NewLine +
				             "---" + Environment.NewLine +
				             "thanks also to:" + Environment.NewLine +
				             "rayden (raydenbest@gmail.com)" + Environment.NewLine +
				             "insomniac (insomniac@slackware.it)" + Environment.NewLine +
					         "rigel.va - spanish language file" + Environment.NewLine +
				             "mubumba (mubumba@gmail.com)" + Environment.NewLine  + Environment.NewLine +
				             "irc contact" + Environment.NewLine +
				             "irc.eu.azzurra.net   #mono" + Environment.NewLine ;
			//infoWindow.HeightRequest = 360;
			infoWindow.Run();		 
			infoWindow.Destroy();		
			return;		
		}
	
	
		public void On_Linkmono_Press(object sender, EventArgs a)
		{
			System.Diagnostics.Process.Start("http://www.mono-project.com/");
		}
	
		public void On_Logoapp_Press(object sender, EventArgs a)
		{
			System.Diagnostics.Process.Start("http://www.integrazioneweb.com/themonospot/");
		}

	}
	
}
