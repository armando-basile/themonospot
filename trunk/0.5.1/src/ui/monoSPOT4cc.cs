using System;
using Gtk;
using System.Reflection;

namespace monoSpotMain 
{

	public class monoSPOT4cc
	{
	
		[Glade.Widget] Gtk.Dialog 		FourCCWindow;
		[Glade.Widget] Gtk.Entry		txtFourCCdesc;
		[Glade.Widget] Gtk.Entry		txtFourCCcode;
		
		
		public monoSPOT4cc(Gtk.Window w, DialogFlags df, ref string fCC_Code, ref string fCC_Desc, out int retValue)
		{
			Glade.XML rxml = new Glade.XML (null, "ChangeFourCCwin.glade", "FourCCWindow", null);
			rxml.Autoconnect (this);
			
			this.FourCCWindow.Icon = Gdk.Pixbuf.LoadFromResource("monoSPOT_16.png");			
			
			this.FourCCWindow.TransientFor = w;
			//this.infoWindow.Modal=true;
			this.FourCCWindow.Title = "FourCC Change...";
			txtFourCCcode.Text = fCC_Code;
			txtFourCCdesc.Text = fCC_Desc;
			retValue = FourCCWindow.Run();
			fCC_Code = txtFourCCcode.Text;
			fCC_Desc = txtFourCCdesc.Text;			
			FourCCWindow.Destroy();		
			return;		
		}
	
	


	}
	
}
