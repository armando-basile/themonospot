
using System;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace ThemonospotBase
{
	
	/// <summary>
	/// Manager for Log, Listener and Console
	/// </summary>
	public static class ThemonospotLogger
	{
		
		static StreamWriter sw;
		static DefaultTraceListener dtl = new DefaultTraceListener();
		
		
#region Properties
		
		
		private static bool _console = true;
		
		public static bool Console
		{
			get {	return _console; }
			set {	_console = value; }
		}
		

		private static bool _listener = true;
		
		public static bool Listener
		{
			get {	return _listener; }
			set {	_listener = value; }
		}

		
		
		
		private static bool _traceFile = false;
		
		public static bool TraceFile
		{
			get {	return _traceFile; }
			set {	_traceFile = value; }
		}

		
		
		private static string _traceFilePath = "";
		
		public static string TraceFilePath
		{
			get {	return _traceFilePath; }
			set
			{
				sw = new StreamWriter(value,true);
				_traceFilePath = value; 
			}
		}
		
		
		
#endregion Properties
		
		
		
		

		
		
		
		public static void Close()
		{
			if(_traceFile)
			{
				sw.Close();
				sw.Dispose();
				sw = null;
			}
		}
		
		
		
		
		public static void Append(string message)
		{
			
			if(_traceFile)
			{
				sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "  " +message);				
				sw.Flush();
			}
			
			if(_listener)
			{
				dtl.WriteLine(message);
			}
			

			if(_console)
			{
				System.Console.WriteLine(message);
			}

		
		
		}
		
		
	}
}
