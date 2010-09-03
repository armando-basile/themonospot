

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- -debug "-define:DEBUG" "-keyfile:themonospot-gui-qt.snk"
ASSEMBLY = bin/Debug/themonospot-gui-qt.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

THEMONOSPOT_GUI_QT_EXE_MDB_SOURCE=bin/Debug/themonospot-gui-qt.exe.mdb
THEMONOSPOT_GUI_QT_EXE_MDB=$(BUILD_DIR)/themonospot-gui-qt.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -unsafe -warn:4 -optimize- "-keyfile:themonospot-gui-qt.snk"
ASSEMBLY = bin/Release/themonospot-gui-qt.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

THEMONOSPOT_GUI_QT_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THEMONOSPOT_GUI_QT_EXE_MDB)  

BINARIES = \
	$(THEMONOSPOT_QT)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	properties/AssemblyInfo.cs \
	src/Program.cs \
	src/qt_classes/ResManager.cs \
	src/qt_classes/MainWindow.cs \
	src/gui_classes/MainWindowClass.Designer.cs \
	src/gui_classes/AboutDialogClass.cs \
	src/gui_classes/ScanningDialogClass.cs \
	src/qt_classes/AboutDialog.cs \
	src/qt_classes/ScanningDialog.cs \
	src/gui_classes/MainWindowClass.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	src \
	src/gui_classes \
	images \
	src/qt_files/MainWindow.ui \
	src/qt_files/AboutDialog.ui \
	src/qt_files/ScanningDialog.ui \
	images/themonospot_48.png \
	images/themonospot-qt.desktop \
	copying.gpl \
	themonospot-gui-qt.snk \
	themonospot-qt.in 

REFERENCES =  \
	System \
	System.Xml \
	$(THEMONOSPOT_BASE_LIBS) \
	$(QYOTO_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

THEMONOSPOT_QT = $(BUILD_DIR)/themonospot-qt

$(eval $(call emit-deploy-wrapper,THEMONOSPOT_QT,themonospot-qt,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
