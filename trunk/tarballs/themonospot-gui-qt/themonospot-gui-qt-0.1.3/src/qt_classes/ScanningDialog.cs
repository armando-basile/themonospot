/********************************************************************************
** Form generated from reading ui file 'ScanningDialog.ui'
**
** Created: mer dic 9 16:05:37 2009
**      by: Qt User Interface Compiler for C# version 4.5.3
**
** WARNING! All changes made in this file will be lost when recompiling ui file!
********************************************************************************/


using Qyoto;

public class Ui_ScanningDialog
{
    public QVBoxLayout vboxLayout;
    public QLabel lblOperation;
    public QLabel lblFileName;
    public QDialogButtonBox buttonBox;

    public void SetupUi(QDialog ScanningDialog)
    {
    if (ScanningDialog.ObjectName == "")
        ScanningDialog.ObjectName = "ScanningDialog";
    QSize Size = new QSize(750, 100);
    Size = Size.ExpandedTo(ScanningDialog.MinimumSizeHint());
    ScanningDialog.Size = Size;
    ScanningDialog.MinimumSize = new QSize(750, 100);
    ScanningDialog.MaximumSize = new QSize(750, 100);
    ScanningDialog.WindowIcon = new QIcon(":/main/themonospot_48.png");
    ScanningDialog.Modal = true;
    vboxLayout = new QVBoxLayout(ScanningDialog);
    vboxLayout.ObjectName = "vboxLayout";
    lblOperation = new QLabel(ScanningDialog);
    lblOperation.ObjectName = "lblOperation";
    lblOperation.Alignment = Qyoto.Qyoto.GetCPPEnumValue("Qt", "AlignCenter");
    lblOperation.Margin = 1;

    vboxLayout.AddWidget(lblOperation);

    lblFileName = new QLabel(ScanningDialog);
    lblFileName.ObjectName = "lblFileName";
    lblFileName.Alignment = Qyoto.Qyoto.GetCPPEnumValue("Qt", "AlignCenter");
    lblFileName.WordWrap = true;
    lblFileName.Margin = 1;

    vboxLayout.AddWidget(lblFileName);

    buttonBox = new QDialogButtonBox(ScanningDialog);
    buttonBox.ObjectName = "buttonBox";
    buttonBox.Orientation = Qt.Orientation.Horizontal;
    buttonBox.StandardButtons = Qyoto.Qyoto.GetCPPEnumValue("QDialogButtonBox", "Cancel");
    buttonBox.CenterButtons = true;

    vboxLayout.AddWidget(buttonBox);


    RetranslateUi(ScanningDialog);

    QMetaObject.ConnectSlotsByName(ScanningDialog);
    } // SetupUi

    public void RetranslateUi(QDialog ScanningDialog)
    {
    ScanningDialog.WindowTitle = QApplication.Translate("ScanningDialog", "Dialog", null, QApplication.Encoding.UnicodeUTF8);
    lblOperation.Text = QApplication.Translate("ScanningDialog", "TextLabel", null, QApplication.Encoding.UnicodeUTF8);
    lblFileName.Text = QApplication.Translate("ScanningDialog", "TextLabel", null, QApplication.Encoding.UnicodeUTF8);
    } // RetranslateUi

}

namespace Ui {
    public class ScanningDialog : Ui_ScanningDialog {}
} // namespace Ui

