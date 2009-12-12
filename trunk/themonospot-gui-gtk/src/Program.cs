
using System;
using ThemonospotBase;
using System.Reflection;
using System.Collections.Generic;

namespace ThemonospotGuiGtk
{
	
	
	public class Program
	{
		
		BaseFactory bFactory;
		
		
		[STAThread]
		public static void Main(string[] args)
		{
			Gtk.Application.Init();
			
			bool retStart = false;
			
			GtkWinMain mainwindow = new GtkWinMain(args, ref retStart);
			
			if (!retStart)
			{
				return;
			}
			
			Gtk.Application.Run();
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
	}
}
