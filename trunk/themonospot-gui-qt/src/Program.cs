
using System;
using Qyoto;

namespace ThemonospotGuiQt
{
	
	
	public class Program: Qt
	{
		[STAThread]
		public static void Main(string[] args)
		{
			
			// Init resource class manager						
			Q_INIT_RESOURCE("ResManager");
			
			// Create new Qyoto Application
			new QApplication(args);
			
			// Create new Qyoto Desktop Object
			QDesktopWidget qdw = new QDesktopWidget();
			
			// Create MainWindow class manager
			MainWindowClass mwc = new MainWindowClass();
			
			int wWidth = Convert.ToInt32(mwc.Width() / 2);
			int wHeight = Convert.ToInt32(mwc.Height() / 2);
			int dWidth = Convert.ToInt32(qdw.Width() / 2);
			int dHeight = Convert.ToInt32(qdw.Height() / 2);
			
			mwc.Move(dWidth - wWidth, dHeight - wHeight - 20);
			
			mwc.ParseAguments(args);
			
			mwc.Show();
			
			// Run Qyoto Application
			QApplication.Exec();			
			
		}
	}
}
