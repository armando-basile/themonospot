
using System;
using Qyoto;

namespace ThemonospotGuiQt
{
	
	
	public class ScanningDialogClass: QDialog
	{
		
		// ATTRIBUTES
		Ui_ScanningDialog scanningdialog_UI;
		
		
		
		// EVENTS
		public event CancelScanEventHandler CancelScan;
		
		
		
		
		// PROPERTIES
		public string Operation 
		{
			set 
			{
				scanningdialog_UI.lblOperation.Text = value;
			}
			get
			{
				return scanningdialog_UI.lblOperation.Text;
			}
		}

		public string FileName 
		{
			set 
			{
				scanningdialog_UI.lblFileName.Text = value;
			}
			get
			{
				return scanningdialog_UI.lblFileName.Text;
			}
		}		
		
		
		
		// CONSTRUCTOR
		public ScanningDialogClass()
		{
			
			// Create new scanningdialog_UI object
			scanningdialog_UI = new Ui_ScanningDialog();
			
			// Configure layout of this new QDialog with 
			// scanningdialog_UI objects and data			
			scanningdialog_UI.SetupUi(this);
			
			// Update Qt graphic objects
			UpdateGraphicObjects();

			
			// Update reactors (eventhandler)
			UpdateReactors();					
			
		}
		
		
		
		
		private void UpdateGraphicObjects()
		{
			this.WindowTitle = GlobalData.GetLanguageKeyValue("SCANWINTITLE");
			scanningdialog_UI.lblOperation.Text = GlobalData.GetLanguageKeyValue("SCANDESC");
			scanningdialog_UI.lblFileName.Text = "...";
			
			QApplication.ProcessEvents();
			
		}
		
		
		
		
		
		private void UpdateReactors()
		{
			// Configure events reactors
			Connect( scanningdialog_UI.buttonBox, SIGNAL("clicked(QAbstractButton*)"), this, SLOT("ActionCancel(QAbstractButton*)"));
			Connect( this, SIGNAL("rejected()"), this, SLOT("ActionDestroyed()"));
		}
		
		
		
		
		
		
		#region Q_SLOTS
		
		
		[Q_SLOT]
		public void ActionCancel(QAbstractButton buttonPressed)
		{
			// When click on Cancel, raise CancelScan Event
			CancelScan(this, new EventArgs());
		}
		
		[Q_SLOT]
		public void ActionDestroyed()
		{
			// When close QDialog, raise CancelScan Event
			CancelScan(this, new EventArgs());
		}
		
		
		
		#endregion Q_SLOTS
		
		
		
		
		
	}
}
