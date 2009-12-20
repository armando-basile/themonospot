
using System;
using Gtk;
using Gdk;
using ThemonospotBase;

namespace ThemonospotGuiGtk
{
	
	
	public class AboutDialogClass
	{
		
		[Glade.Widget]	Gtk.Window			AboutDialog;
		[Glade.Widget]	Gtk.Image			imgLogo;
		[Glade.Widget]	Gtk.Label			lblTitle;
		[Glade.Widget]	Gtk.Notebook		tabInfo;
		[Glade.Widget]	Gtk.Button			btnOk;
		[Glade.Widget]	Gtk.Viewport		vpTitle;
		[Glade.Widget]	Gtk.Viewport		vpLogo;
		
		
		
		
		/// <summary>
		/// Constructor
		/// </summary>
		public AboutDialogClass(ref Gtk.Window parent)
		{
			
			// Instance glade xml object using glade file
			Glade.XML gxml =  new Glade.XML("AboutDialog.glade", "AboutDialog");
			
			// Aonnect glade xml object to this Gtk.Dialog
			gxml.Autoconnect(this);
			
			// Update Gtk graphic objects
			UpdateGraphicObjects();
			
			// Ipdate Event Handlers
			UpdateReactors();			
			
			AboutDialog.TransientFor = parent;
			AboutDialog.DestroyWithParent = true;
			AboutDialog.Show();
		}
		
		
		
		
		private void UpdateGraphicObjects()
		{
			// Set dialog icon
			AboutDialog.Icon = Gdk.Pixbuf.LoadFromResource("themonospot.png");
			vpTitle.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
			AboutDialog.Title = GlobalData.GetLanguageKeyValue("ABOUTWINTITLE");
			
			ScrolledWindow sw = new ScrolledWindow();
			Viewport vp = new Viewport();
			sw.Add(vp);
			Label lblContent = new Label();
			lblContent.SetPadding(4,4);
			lblContent.SetAlignment((float)0, (float)0);
			vp.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
			lblContent.Markup =
				"<b>Themonospot Gtk</b>\r\n" +
				"  " + GlobalData.GuiDescription + "\r\n\r\n" +
				"Copyright " + GlobalData.GuiCopyright + "\r\n\r\n" +
				"<b>Website</b>\r\n" +
				"  www.integrazioneweb.com/themonospot\r\n\r\n" +
				"<b>Developers</b>\r\n" + 
				"  Armando Basile <i>(hmandevteam@gmail.com)</i>\r\n" +
				"  Giuseppe Coviello <i>(cjg@cruxppc.org)</i>\r\n\r\n" +
				"<b>Special thanks to</b>\r\n" +
				"  Moitah <i>(moitah@yahoo.com)</i>\r\n" + 
				"  Insomniac <i>(insomniac@slackware.it)</i>\r\n" + 
				"  Rigel.va\r\n" + 
				"  Mubumba <i>(mubumba@gmail.com)</i>\r\n\r\n" + 				 
				"<b>Bugs report</b>\r\n" +
				"  code.google.com/p/monosim/issues/list\r\n\r\n" +
				"<b>Irc contact</b>\r\n" +
				"  irc.eu.azzurra.net   channel <i>#mono</i>\r\n";
			
			vp.Add(lblContent);
			
			tabInfo.AppendPage(sw, new Label(GlobalData.GetLanguageKeyValue("ABOUTTABINFO")));
			
			
			string components = "<b>base component: </b>" + GlobalData.BaseRelease + "\r\n";
			for (int j=0; j<GlobalData.BasePlugins.Count; j++)
			{
				components += "<b>" + GlobalData.BasePlugins[j].FileName + ": </b>" + 
					GlobalData.BasePlugins[j].Release + "\r\n";
			}
			
			
			sw = new ScrolledWindow();
			vp = new Viewport();
			sw.Add(vp);
			lblContent = new Label();
			lblContent.SetPadding(4,4);
			lblContent.SetAlignment((float)0, (float)0);
			lblContent.Markup = components;
			vp.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
			
			vp.Add(lblContent);
			
			tabInfo.AppendPage(sw, new Label(GlobalData.GetLanguageKeyValue("ABOUTTABCOMPONENTS")));
			
			tabInfo.ShowAll();
			
			
			
			imgLogo.Pixbuf = Gdk.Pixbuf.LoadFromResource("themonospot.png");
			vpLogo.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
			
			lblTitle.Markup = "<b>Themonospot [Gtk]</b>\r\n" +
				GlobalData.GuiRelease;
			lblTitle.ModifyBg(StateType.Normal, new Gdk.Color(255,255,255));
			
			Gdk.Geometry geo = new Gdk.Geometry();
			geo.MinHeight = 380;
			geo.MinWidth = 500;
			AboutDialog.SetGeometryHints(tabInfo, geo, Gdk.WindowHints.MinSize);
			
			
			// wait for gui processes
			while (Gtk.Application.EventsPending ())
            	Gtk.Application.RunIteration ();
			
		}
		
		
		
		
		
		private void UpdateReactors()
		{
			// Configure events reactors
			
			AboutDialog.DeleteEvent += ActionCancel;
			btnOk.Clicked += ActionCancel;
		}
		
		
		
		
		private void ActionCancel(object sender, EventArgs args)
		{
			//When click on Ok, raise CancelScan Event
			AboutDialog.Destroy();
			AboutDialog.Dispose();
		}
		

		
		

	}
}
