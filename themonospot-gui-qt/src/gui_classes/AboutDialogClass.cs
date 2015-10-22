
using System;
using Qyoto;
using System.Reflection;
using ThemonospotBase;

namespace ThemonospotGuiQt
{
	
	
	public class AboutDialogClass: QDialog
	{
		
		// ATTRIBUTES
		Ui_AbautDialog aboutdialog_UI;
		
		
		
		// CONSTRUCTOR
		public AboutDialogClass()
		{
			
			// Create new aboutwindow_UI object
			aboutdialog_UI = new Ui_AbautDialog();
			
			// Configure layout of this new QDialog with 
			// aboutdialog_UI objects and data			
			aboutdialog_UI.SetupUi(this);
			
			// Update Qt graphic objects
			UpdateGraphicObjects();

			
			// Update reactors (eventhandler)
			UpdateReactors();			
			
			
		}
		
		
		
		
		
		private void UpdateGraphicObjects()
		{
			
			// fill graphic objects informations
			this.WindowTitle = GlobalData.GetLanguageKeyValue("ABOUTWINTITLE");
			aboutdialog_UI.tabInfo.SetTabText(0, GlobalData.GetLanguageKeyValue("ABOUTTABINFO"));
			aboutdialog_UI.tabInfo.SetTabText(1, GlobalData.GetLanguageKeyValue("ABOUTTABCOMPONENTS"));
			aboutdialog_UI.lblName.Text = "Themonospot [Qt]";
			aboutdialog_UI.lblVersion.Text =  GlobalData.GuiRelease;
			aboutdialog_UI.txtInfo.SetText
				("<b>Themonospot Qt</b><br />\r\n" +
				 "&nbsp;&nbsp;" + GlobalData.GuiDescription + "<br /><br />\r\n" +
				 "Copyright " + GlobalData.GuiCopyright + "<br /><br />\r\n" +
				 "<b>Website</b><br />\r\n" +
				 "&nbsp;&nbsp;www.integrazioneweb.com/themonospot<br /><br />\r\n" +
				 "<b>Developers</b><br />\r\n" + 
			     "&nbsp;&nbsp;Armando Basile <i>(hmandevteam@gmail.com)</i><br />\r\n" +
				 "&nbsp;&nbsp;Giuseppe Coviello <i>(cjg@cruxppc.org)</i><br /><br />\r\n" +
				 "<b>Special thanks to</b><br />\r\n" +
				 "&nbsp;&nbsp;Moitah <i>(moitah@yahoo.com)</i><br />\r\n" + 
				 "&nbsp;&nbsp;Insomniac <i>(insomniac@slackware.it)</i><br />\r\n" + 
				 "&nbsp;&nbsp;Rigel.va<br />\r\n" + 
				 "&nbsp;&nbsp;Mubumba <i>(mubumba@gmail.com)</i><br /><br />\r\n" + 				 
				 "<b>Bugs report</b><br />\r\n" +
                 "&nbsp;&nbsp;https://github.com/armando-basile/themonospot/issues<br /><br />\r\n");
			
			
			// fill components area information
			string components = "<b>base component: </b>" + GlobalData.BaseRelease + "<br />\r\n";
			for (int j=0; j<GlobalData.BasePlugins.Count; j++)
			{
				components += "<b>" + GlobalData.BasePlugins[j].FileName + ": </b>" + 
					GlobalData.BasePlugins[j].Release + "<br />\r\n";
			}
			
			aboutdialog_UI.txtComponents.SetText(components);
			
			
			// add logo
			aboutdialog_UI.frmTop.Palette.Background().SetColor(new QColor(255,255,255));
			
		}
		
		
		
		
		private void UpdateReactors()
		{
			// Configure events reactors
			Connect( aboutdialog_UI.buttonBox, SIGNAL("clicked(QAbstractButton*)"), this, SLOT("ActionExit(QAbstractButton*)"));			
			
		}
		
		
		
		
		#region Q_SLOTS
		
		[Q_SLOT]
		public void ActionExit(QAbstractButton buttonPressed)
		{
			//Console.WriteLine("Pressed::" + buttonPressed.Text);
			this.Close();
			
		}
		
		
		
		
		#endregion Q_SLOTS
		
		
		
	}
}
