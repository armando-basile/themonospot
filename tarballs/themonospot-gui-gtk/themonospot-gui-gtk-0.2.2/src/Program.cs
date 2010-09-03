
using System;
using ThemonospotBase;
using System.Reflection;
using System.Collections.Generic;

namespace ThemonospotGuiGtk
{
	
	
	public class Program
	{
		
		
		[STAThread]
		public static void Main(string[] args)
		{
			// Init Gtk Application
			Gtk.Application.Init();

			// Create a new MainWindowClass instance
			MainWindowClass mainWindow = new MainWindowClass(args);
			
			mainWindow.ParseArguments();
			
			
			// Run Gtk Application			
			Gtk.Application.Run();
			
		}
		
		
		
		
		
		
		
		
		
		
		
		
	}
}
