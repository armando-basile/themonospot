/********************************************************************************
** Form generated from reading ui file 'MainWindow.ui'
**
** Created: mer dic 9 16:05:37 2009
**      by: Qt User Interface Compiler for C# version 4.5.3
**
** WARNING! All changes made in this file will be lost when recompiling ui file!
********************************************************************************/


using Qyoto;

public class Ui_MainWindow
{
    public QAction actionScanFile;
    public QAction actionScanFolder;
    public QAction actionScanFolderSubfolders;
    public QAction actionSaveReport;
    public QAction actionExit;
    public QAction actionAutoReport;
    public QAction actionInfoAbout;
    public QWidget centralwidget;
    public QGridLayout gridLayout;
    public QGroupBox grpResult;
    public QGridLayout gridLayout1;
    public QTabWidget tabContainer;
    public QWidget tab;
    public QGridLayout gridLayout2;
    public QListWidget listWidget;
    public QMenuBar menubar;
    public QMenu menu_File;
    public QMenu menu_Options;
    public QMenu menu_About;
    public QStatusBar statusbar;
    public QToolBar toolBar;

    public void SetupUi(QMainWindow MainWindow)
    {
    if (MainWindow.ObjectName == "")
        MainWindow.ObjectName = "MainWindow";
    QSize Size = new QSize(600, 550);
    Size = Size.ExpandedTo(MainWindow.MinimumSizeHint());
    MainWindow.Size = Size;
    MainWindow.MinimumSize = new QSize(600, 550);
    MainWindow.WindowIcon = new QIcon(":/main/themonospot_48.png");
    actionScanFile = new QAction(MainWindow);
    actionScanFile.ObjectName = "actionScanFile";
    actionScanFile.icon = new QIcon(":/main/document-open.png");
    actionScanFolder = new QAction(MainWindow);
    actionScanFolder.ObjectName = "actionScanFolder";
    actionScanFolder.icon = new QIcon(":/main/document-open.png");
    actionScanFolderSubfolders = new QAction(MainWindow);
    actionScanFolderSubfolders.ObjectName = "actionScanFolderSubfolders";
    actionScanFolderSubfolders.icon = new QIcon(":/main/document-open.png");
    actionSaveReport = new QAction(MainWindow);
    actionSaveReport.ObjectName = "actionSaveReport";
    actionSaveReport.icon = new QIcon(":/main/document-save-as.png");
    actionExit = new QAction(MainWindow);
    actionExit.ObjectName = "actionExit";
    actionExit.icon = new QIcon(":/main/application-exit.png");
    actionAutoReport = new QAction(MainWindow);
    actionAutoReport.ObjectName = "actionAutoReport";
    actionAutoReport.Checkable = true;
    actionAutoReport.icon = new QIcon(":/main/export.png");
    actionInfoAbout = new QAction(MainWindow);
    actionInfoAbout.ObjectName = "actionInfoAbout";
    actionInfoAbout.icon = new QIcon(":/main/themonospot_48.png");
    centralwidget = new QWidget(MainWindow);
    centralwidget.ObjectName = "centralwidget";
    gridLayout = new QGridLayout(centralwidget);
    gridLayout.ObjectName = "gridLayout";
    grpResult = new QGroupBox(centralwidget);
    grpResult.ObjectName = "grpResult";
    grpResult.Flat = false;
    gridLayout1 = new QGridLayout(grpResult);
    gridLayout1.ObjectName = "gridLayout1";
    tabContainer = new QTabWidget(grpResult);
    tabContainer.ObjectName = "tabContainer";
    tab = new QWidget();
    tab.ObjectName = "tab";
    gridLayout2 = new QGridLayout(tab);
    gridLayout2.ObjectName = "gridLayout2";
    listWidget = new QListWidget(tab);
    listWidget.ObjectName = "listWidget";

    gridLayout2.AddWidget(listWidget, 0, 0, 1, 1);

    tabContainer.AddTab(tab, QApplication.Translate("MainWindow", "Tab 1", null, QApplication.Encoding.UnicodeUTF8));

    gridLayout1.AddWidget(tabContainer, 0, 0, 1, 1);


    gridLayout.AddWidget(grpResult, 0, 0, 1, 1);

    MainWindow.SetCentralWidget(centralwidget);
    menubar = new QMenuBar(MainWindow);
    menubar.ObjectName = "menubar";
    menubar.Geometry = new QRect(0, 0, 600, 24);
    menu_File = new QMenu(menubar);
    menu_File.ObjectName = "menu_File";
    menu_Options = new QMenu(menubar);
    menu_Options.ObjectName = "menu_Options";
    menu_About = new QMenu(menubar);
    menu_About.ObjectName = "menu_About";
    MainWindow.SetMenuBar(menubar);
    statusbar = new QStatusBar(MainWindow);
    statusbar.ObjectName = "statusbar";
    MainWindow.SetStatusBar(statusbar);
    toolBar = new QToolBar(MainWindow);
    toolBar.ObjectName = "toolBar";
    toolBar.ContextMenuPolicy = Qt.ContextMenuPolicy.DefaultContextMenu;
    toolBar.Movable = false;
    toolBar.ToolButtonStyle = Qt.ToolButtonStyle.ToolButtonTextBesideIcon;
    toolBar.Floatable = false;
    MainWindow.AddToolBar(Qt.ToolBarArea.TopToolBarArea, toolBar);

    menubar.AddAction(menu_File.MenuAction());
    menubar.AddAction(menu_Options.MenuAction());
    menubar.AddAction(menu_About.MenuAction());
    menu_File.AddAction(actionScanFile);
    menu_File.AddAction(actionScanFolder);
    menu_File.AddAction(actionScanFolderSubfolders);
    menu_File.AddSeparator();
    menu_File.AddAction(actionSaveReport);
    menu_File.AddSeparator();
    menu_File.AddAction(actionExit);
    menu_Options.AddAction(actionAutoReport);
    menu_About.AddAction(actionInfoAbout);
    toolBar.AddAction(actionScanFile);
    toolBar.AddAction(actionScanFolder);
    toolBar.AddAction(actionScanFolderSubfolders);
    toolBar.AddSeparator();
    toolBar.AddAction(actionSaveReport);
    toolBar.AddSeparator();
    toolBar.AddAction(actionExit);

    RetranslateUi(MainWindow);

    tabContainer.CurrentIndex = 0;


    QMetaObject.ConnectSlotsByName(MainWindow);
    } // SetupUi

    public void RetranslateUi(QMainWindow MainWindow)
    {
    MainWindow.WindowTitle = QApplication.Translate("MainWindow", "MainWindow", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFile.Text = QApplication.Translate("MainWindow", "F&ile", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFile.StatusTip = QApplication.Translate("MainWindow", "Scan media file to extract info", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFile.Shortcut = QApplication.Translate("MainWindow", "Ctrl+1", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolder.Text = QApplication.Translate("MainWindow", "F&older", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolder.StatusTip = QApplication.Translate("MainWindow", "Scan media files contained in a specific folder to extract info", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolder.Shortcut = QApplication.Translate("MainWindow", "Ctrl+2", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolderSubfolders.Text = QApplication.Translate("MainWindow", "Folder &and subfolders", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolderSubfolders.ToolTip = QApplication.Translate("MainWindow", "Scan Folder and subfolders", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolderSubfolders.StatusTip = QApplication.Translate("MainWindow", "Scan media files contained in a specific folder and her subfolders to extract info", null, QApplication.Encoding.UnicodeUTF8);
    actionScanFolderSubfolders.Shortcut = QApplication.Translate("MainWindow", "Ctrl+3", null, QApplication.Encoding.UnicodeUTF8);
    actionSaveReport.Text = QApplication.Translate("MainWindow", "&Save report", null, QApplication.Encoding.UnicodeUTF8);
    actionSaveReport.StatusTip = QApplication.Translate("MainWindow", "Save report of selected file in a text file", null, QApplication.Encoding.UnicodeUTF8);
    actionSaveReport.Shortcut = QApplication.Translate("MainWindow", "Ctrl+S", null, QApplication.Encoding.UnicodeUTF8);
    actionExit.Text = QApplication.Translate("MainWindow", "&Exit", null, QApplication.Encoding.UnicodeUTF8);
    actionExit.StatusTip = QApplication.Translate("MainWindow", "Close application", null, QApplication.Encoding.UnicodeUTF8);
    actionExit.Shortcut = QApplication.Translate("MainWindow", "Ctrl+Q", null, QApplication.Encoding.UnicodeUTF8);
    actionAutoReport.Text = QApplication.Translate("MainWindow", "&Auto save report", null, QApplication.Encoding.UnicodeUTF8);
    actionAutoReport.StatusTip = QApplication.Translate("MainWindow", "For each file scanned, generate a report file in the same folder", null, QApplication.Encoding.UnicodeUTF8);
    actionInfoAbout.Text = QApplication.Translate("MainWindow", "&Information about Themonospot", null, QApplication.Encoding.UnicodeUTF8);
    actionInfoAbout.StatusTip = QApplication.Translate("MainWindow", "Information about Themonospot", null, QApplication.Encoding.UnicodeUTF8);
    grpResult.Title = QApplication.Translate("MainWindow", "Scan result", null, QApplication.Encoding.UnicodeUTF8);
    tabContainer.SetTabText(tabContainer.IndexOf(tab), QApplication.Translate("MainWindow", "Tab 1", null, QApplication.Encoding.UnicodeUTF8));
    menu_File.Title = QApplication.Translate("MainWindow", "&File", null, QApplication.Encoding.UnicodeUTF8);
    menu_Options.Title = QApplication.Translate("MainWindow", "&Options", null, QApplication.Encoding.UnicodeUTF8);
    menu_About.Title = QApplication.Translate("MainWindow", "&About", null, QApplication.Encoding.UnicodeUTF8);
    toolBar.WindowTitle = QApplication.Translate("MainWindow", "toolBar", null, QApplication.Encoding.UnicodeUTF8);
    } // RetranslateUi

}

namespace Ui {
    public class MainWindow : Ui_MainWindow {}
} // namespace Ui

