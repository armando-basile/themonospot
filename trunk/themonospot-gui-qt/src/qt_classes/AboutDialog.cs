/********************************************************************************
** Form generated from reading ui file 'AboutDialog.ui'
**
** Created: lun mag 31 10:51:58 2010
**      by: Qt User Interface Compiler for C# version 4.5.3
**
** WARNING! All changes made in this file will be lost when recompiling ui file!
********************************************************************************/


using Qyoto;

public class Ui_AbautDialog
{
    public QGridLayout gridLayout;
    public QVBoxLayout vboxLayout;
    public QFrame frmTop;
    public QGridLayout gridLayout1;
    public QWidget logo;
    public QVBoxLayout vboxLayout1;
    public QLabel lblName;
    public QLabel lblVersion;
    public QTabWidget tabInfo;
    public QWidget Informations;
    public QGridLayout gridLayout2;
    public QTextEdit txtInfo;
    public QWidget Components;
    public QGridLayout gridLayout3;
    public QTextEdit txtComponents;
    public QDialogButtonBox buttonBox;

    public void SetupUi(QDialog AbautDialog)
    {
    if (AbautDialog.ObjectName == "")
        AbautDialog.ObjectName = "AbautDialog";
    AbautDialog.WindowModality = Qt.WindowModality.NonModal;
    QSize Size = new QSize(400, 370);
    Size = Size.ExpandedTo(AbautDialog.MinimumSizeHint());
    AbautDialog.Size = Size;
    AbautDialog.MinimumSize = new QSize(400, 370);
    AbautDialog.MaximumSize = new QSize(400, 370);
    AbautDialog.WindowIcon = new QIcon(":/main/themonospot_48.png");
    AbautDialog.Modal = true;
    gridLayout = new QGridLayout(AbautDialog);
    gridLayout.ObjectName = "gridLayout";
    gridLayout.sizeConstraint = QLayout.SizeConstraint.SetDefaultConstraint;
    vboxLayout = new QVBoxLayout();
    vboxLayout.ObjectName = "vboxLayout";
    frmTop = new QFrame(AbautDialog);
    frmTop.ObjectName = "frmTop";
    frmTop.MinimumSize = new QSize(0, 64);
    frmTop.AutoFillBackground = false;
    frmTop.FrameShape = QFrame.Shape.StyledPanel;
    frmTop.FrameShadow = QFrame.Shadow.Raised;
    gridLayout1 = new QGridLayout(frmTop);
    gridLayout1.ObjectName = "gridLayout1";
    logo = new QWidget(frmTop);
    logo.ObjectName = "logo";
    logo.MinimumSize = new QSize(48, 48);
    logo.MaximumSize = new QSize(48, 48);

    gridLayout1.AddWidget(logo, 0, 0, 1, 1);

    vboxLayout1 = new QVBoxLayout();
    vboxLayout1.ObjectName = "vboxLayout1";
    lblName = new QLabel(frmTop);
    lblName.ObjectName = "lblName";
    QFont font = new QFont();
    font.SetBold(true);
    font.SetWeight(75);
    lblName.Font = font;
    lblName.Margin = 1;

    vboxLayout1.AddWidget(lblName);

    lblVersion = new QLabel(frmTop);
    lblVersion.ObjectName = "lblVersion";
    lblVersion.Margin = 1;

    vboxLayout1.AddWidget(lblVersion);


    gridLayout1.AddLayout(vboxLayout1, 0, 1, 1, 1);


    vboxLayout.AddWidget(frmTop);

    tabInfo = new QTabWidget(AbautDialog);
    tabInfo.ObjectName = "tabInfo";
    tabInfo.AutoFillBackground = false;
    Informations = new QWidget();
    Informations.ObjectName = "Informations";
    gridLayout2 = new QGridLayout(Informations);
    gridLayout2.ObjectName = "gridLayout2";
    txtInfo = new QTextEdit(Informations);
    txtInfo.ObjectName = "txtInfo";
    txtInfo.ReadOnly = true;

    gridLayout2.AddWidget(txtInfo, 0, 0, 1, 1);

    tabInfo.AddTab(Informations, QApplication.Translate("AbautDialog", "Informazioni su", null, QApplication.Encoding.UnicodeUTF8));
    Components = new QWidget();
    Components.ObjectName = "Components";
    gridLayout3 = new QGridLayout(Components);
    gridLayout3.ObjectName = "gridLayout3";
    txtComponents = new QTextEdit(Components);
    txtComponents.ObjectName = "txtComponents";
    txtComponents.ReadOnly = true;

    gridLayout3.AddWidget(txtComponents, 0, 0, 1, 1);

    tabInfo.AddTab(Components, QApplication.Translate("AbautDialog", "Componenti", null, QApplication.Encoding.UnicodeUTF8));

    vboxLayout.AddWidget(tabInfo);

    buttonBox = new QDialogButtonBox(AbautDialog);
    buttonBox.ObjectName = "buttonBox";
    buttonBox.StandardButtons = Qyoto.Qyoto.GetCPPEnumValue("QDialogButtonBox", "Ok");

    vboxLayout.AddWidget(buttonBox);


    gridLayout.AddLayout(vboxLayout, 0, 0, 1, 1);


    RetranslateUi(AbautDialog);

    tabInfo.CurrentIndex = 0;


    QMetaObject.ConnectSlotsByName(AbautDialog);
    } // SetupUi

    public void RetranslateUi(QDialog AbautDialog)
    {
    AbautDialog.WindowTitle = QApplication.Translate("AbautDialog", "About Themonospot", null, QApplication.Encoding.UnicodeUTF8);
    frmTop.StyleSheet = QApplication.Translate("AbautDialog", "background-color: rgb(255, 255, 255);", null, QApplication.Encoding.UnicodeUTF8);
    logo.StyleSheet = QApplication.Translate("AbautDialog", "background-image: url(:/main/themonospot_48.png);", null, QApplication.Encoding.UnicodeUTF8);
    tabInfo.SetTabText(tabInfo.IndexOf(Informations), QApplication.Translate("AbautDialog", "Informazioni su", null, QApplication.Encoding.UnicodeUTF8));
    tabInfo.SetTabText(tabInfo.IndexOf(Components), QApplication.Translate("AbautDialog", "Componenti", null, QApplication.Encoding.UnicodeUTF8));
    } // RetranslateUi

}

namespace Ui {
    public class AbautDialog : Ui_AbautDialog {}
} // namespace Ui

