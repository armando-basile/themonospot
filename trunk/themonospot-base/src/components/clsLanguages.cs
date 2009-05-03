// clsLanguages.cs created with MonoDevelop
// User: hman at 16:22 26/12/2007
//

using System;
using System.IO;
using System.Reflection;

namespace themonospot_Base_Main
{
	
	/// <summary>
	/// Internationalization ...
	/// </summary>
	public static class clsLanguages
	{

		private static string ExePath = "";					// Application path
		
		private static string _DESCFILE = "";
		private static string _LBLFILE = "";
		private static string _DESCVIDEO = "";
		private static string _VCOL1NAME = "";
		private static string _VCOL2NAME = "";
		private static string _DESCAUDIO = "";
		private static string _ACOL1NAME = "";
		private static string _ACOL2NAME = "";
		private static string _ABOUTBT = "";
		private static string _EXPORTBT = "";
		private static string _FOURCCBT = "";
		private static string _UDATABT = "";
		private static string _RESCANBT = "";
		private static string _FCTITLE = "";
		private static string _TTTEXPORT = "";
		private static string _TTTFOURCC = "";
		private static string _TTTUDATA = "";
		private static string _FOURCCSURE = "";
		private static string _FOURCCTITLE = "";
		private static string _UDATAERROR = "";
		private static string _UDATATITLE = "";
		private static string _EXPORTTITLE = "";
		private static string _BTCANCEL = "";
		private static string _BTACCEPT = "";
		private static string _EXPMESSAGE1 = "";
		private static string _EXPMESSAGE2 = "";
		private static string _EXPMESSAGE3 = "";
		private static string _EXPMESSAGE4 = "";
		private static string _WAITTITLE = "";
		private static string _SAVEAS = "";
		private static string _4CCTITLE = "";
		private static string _4CCINFO = "";
		private static string _4CCCODE = "";
		private static string _4CCDESC = "";
				
		public static string DESCFILE 		{	get	{ return _DESCFILE;			} }
		public static string LBLFILE 		{	get	{ return _LBLFILE;			} }
		public static string DESCVIDEO 		{	get	{ return _DESCVIDEO;		} }
		public static string VCOL1NAME 		{	get	{ return _VCOL1NAME;		} }
		public static string VCOL2NAME 		{	get	{ return _VCOL2NAME;		} }
		public static string DESCAUDIO 		{	get	{ return _DESCAUDIO;		} }
		public static string ACOL1NAME 		{	get	{ return _ACOL1NAME;		} }
		public static string ACOL2NAME 		{	get	{ return _ACOL2NAME;		} }
		public static string ABOUTBT 		{	get	{ return _ABOUTBT;			} }
		public static string EXPORTBT 		{	get	{ return _EXPORTBT;			} }
		public static string FOURCCBT 		{	get	{ return _FOURCCBT;			} }
		public static string UDATABT 		{	get	{ return _UDATABT;			} }
		public static string RESCANBT 		{	get	{ return _RESCANBT;			} }
		public static string FCTITLE 		{	get	{ return _FCTITLE;			} }
		public static string TTTEXPORT 		{	get	{ return _TTTEXPORT;		} }
		public static string TTTFOURCC 		{	get	{ return _TTTFOURCC;		} }
		public static string TTTUDATA 		{	get	{ return _TTTUDATA;			} }		
		public static string FOURCCSURE 	{	get	{ return _FOURCCSURE;		} }
		public static string FOURCCTITLE 	{	get	{ return _FOURCCTITLE;		} }
		public static string UDATAERROR 	{	get	{ return _UDATAERROR;		} }
		public static string UDATATITLE 	{	get	{ return _UDATATITLE;		} }
		public static string EXPORTTITLE 	{	get	{ return _EXPORTTITLE;		} }
		public static string BTCANCEL 		{	get	{ return _BTCANCEL;			} }
		public static string BTACCEPT 		{	get	{ return _BTACCEPT;			} }
		public static string EXPMESSAGE1 	{	get	{ return _EXPMESSAGE1;		} }
		public static string EXPMESSAGE2 	{	get	{ return _EXPMESSAGE2;		} }
		public static string EXPMESSAGE3 	{	get	{ return _EXPMESSAGE3;		} }
		public static string EXPMESSAGE4 	{	get	{ return _EXPMESSAGE4;		} }
		public static string WAITTITLE 		{	get	{ return _WAITTITLE;		} }
		public static string SAVEAS 		{	get	{ return _SAVEAS;			} }
		public static string FCCTITLE 		{	get	{ return _4CCTITLE;			} }
		public static string FCCINFO 		{	get	{ return _4CCINFO;			} }
		public static string FCCCODE 		{	get	{ return _4CCCODE;			} }
		public static string FCCDESC 		{	get	{ return _4CCDESC;			} }
		
		public static string LanguageName = "";				// System Language
		public static string LanguageSet = "";				// Available Language

		/// <summary>
		/// Init component and load infos
		/// </summary>
		/// <returns>
		/// Error message
		/// </returns>
		public static string Init()
		{
		
			ExePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location.ToString());
			StreamReader mySR;
			string theLine = "";
			
			if (System.IO.File.Exists(ExePath + Path.DirectorySeparatorChar + "languages" + Path.DirectorySeparatorChar + LanguageName + ".lf") == true)
				LanguageSet = LanguageName;
			else
				LanguageSet = "English";
			
			
			try
			{
				mySR = new StreamReader(ExePath + Path.DirectorySeparatorChar + "languages" + Path.DirectorySeparatorChar + LanguageSet + ".lf");
				
				while (mySR.EndOfStream == false)
				{
					theLine = mySR.ReadLine().Trim();
					
					if (theLine.IndexOf("#") != 0)
					{
					
						if (theLine.IndexOf("DESCFILE=") == 0)
						{	_DESCFILE = theLine.Substring(9);	}						
						else if (theLine.IndexOf("LBLFILE=") == 0)
						{	_LBLFILE = theLine.Substring(8);	}
						else if (theLine.IndexOf("DESCVIDEO=") == 0)
						{	_DESCVIDEO = theLine.Substring(10);	}
						else if (theLine.IndexOf("VCOL1NAME=") == 0)
						{	_VCOL1NAME = theLine.Substring(10);	}
						else if (theLine.IndexOf("VCOL2NAME=") == 0)
						{	_VCOL2NAME = theLine.Substring(10);	}
						else if (theLine.IndexOf("DESCAUDIO=") == 0)
						{	_DESCAUDIO = theLine.Substring(10);	}
						else if (theLine.IndexOf("ACOL1NAME=") == 0)
						{	_ACOL1NAME = theLine.Substring(10);	}
						else if (theLine.IndexOf("ACOL2NAME=") == 0)
						{	_ACOL2NAME = theLine.Substring(10);	}
						else if (theLine.IndexOf("ABOUTBT=") == 0)
						{	_ABOUTBT = theLine.Substring(8);	}
						else if (theLine.IndexOf("EXPORTBT=") == 0)
						{	_EXPORTBT = theLine.Substring(9);	}
						else if (theLine.IndexOf("FOURCCBT=") == 0)
						{	_FOURCCBT = theLine.Substring(9);	}
						else if (theLine.IndexOf("UDATABT=") == 0)
						{	_UDATABT = theLine.Substring(8);	}
						else if (theLine.IndexOf("RESCANBT=") == 0)
						{	_RESCANBT = theLine.Substring(9);	}
						else if (theLine.IndexOf("FCTITLE=") == 0)
						{	_FCTITLE = theLine.Substring(8);	}
						else if (theLine.IndexOf("TTTEXPORT=") == 0)
						{	_TTTEXPORT = theLine.Substring(10);	}
						else if (theLine.IndexOf("TTTFOURCC=") == 0)
						{	_TTTFOURCC = theLine.Substring(10);	}
						else if (theLine.IndexOf("TTTUDATA=") == 0)
						{	_TTTUDATA = theLine.Substring(9);	}
						else if (theLine.IndexOf("FOURCCSURE=") == 0)
						{	_FOURCCSURE = theLine.Substring(11);	}
						else if (theLine.IndexOf("FOURCCTITLE=") == 0)
						{	_FOURCCTITLE = theLine.Substring(12);	}
						else if (theLine.IndexOf("UDATAERROR=") == 0)
						{	_UDATAERROR = theLine.Substring(11);	}
						else if (theLine.IndexOf("UDATATITLE=") == 0)
						{	_UDATATITLE = theLine.Substring(11);	}
						else if (theLine.IndexOf("EXPORTTITLE=") == 0)
						{	_EXPORTTITLE = theLine.Substring(12);	}
						else if (theLine.IndexOf("BTCANCEL=") == 0)
						{	_BTCANCEL = theLine.Substring(9);	}
						else if (theLine.IndexOf("BTACCEPT=") == 0)
						{	_BTACCEPT = theLine.Substring(9);	}
						else if (theLine.IndexOf("EXPMESSAGE1=") == 0)
						{	_EXPMESSAGE1 = theLine.Substring(12);	}
						else if (theLine.IndexOf("EXPMESSAGE2=") == 0)
						{	_EXPMESSAGE2 = theLine.Substring(12);	}
						else if (theLine.IndexOf("EXPMESSAGE3=") == 0)
						{	_EXPMESSAGE3 = theLine.Substring(12);	}
						else if (theLine.IndexOf("EXPMESSAGE4=") == 0)
						{	_EXPMESSAGE4 = theLine.Substring(12);	}
						else if (theLine.IndexOf("WAITTITLE=") == 0)
						{	_WAITTITLE = theLine.Substring(10);	}
						else if (theLine.IndexOf("SAVEAS=") == 0)
						{	_SAVEAS = theLine.Substring(7);	}
						else if (theLine.IndexOf("4CCTITLE=") == 0)
						{	_4CCTITLE = theLine.Substring(9);	}
						else if (theLine.IndexOf("4CCINFO=") == 0)
						{	_4CCINFO = theLine.Substring(8);	}
						else if (theLine.IndexOf("4CCCODE=") == 0)
						{	_4CCCODE = theLine.Substring(8);	}
						else if (theLine.IndexOf("4CCDESC=") == 0)
						{	_4CCDESC = theLine.Substring(8);	}

						
					}
					
				}

				mySR.Close();
				mySR.Dispose();
				mySR = null;
					
				return "";
				
			}
			catch (Exception Ex)
			{
				return Ex.Message;
			}
			
		}
		
		
	}
}
