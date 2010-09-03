

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:3 -optimize- -debug "-define:DEBUG"
ASSEMBLY = bin/Debug/themonospot-gui-gtk.exe
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

THEMONOSPOT_GUI_GTK_EXE_MDB_SOURCE=bin/Debug/themonospot-gui-gtk.exe.mdb
THEMONOSPOT_GUI_GTK_EXE_MDB=$(BUILD_DIR)/themonospot-gui-gtk.exe.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize-
ASSEMBLY = bin/Release/themonospot-gui-gtk.exe
ASSEMBLY_MDB = 
COMPILE_TARGET = exe
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

THEMONOSPOT_GUI_GTK_EXE_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THEMONOSPOT_GUI_GTK_EXE_MDB)  

BINARIES = \
	$(THEMONOSPOT_GTK)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(BINARIES) 

FILES = \
	properties/AssemblyInfo.cs \
	src/Program.cs \
	src/gtk_classes/MainWindowClass.Designer.cs \
	src/gtk_classes/MainWindowClass.cs \
	src/gtk_classes/ScanningDialogClass.cs \
	src/gtk_classes/AboutDialogClass.cs 

DATA_FILES = 

RESOURCES = \
	src/resources/monologo.png \
	src/resources/sound.png \
	src/resources/themonospot.png \
	src/resources/video.png \
	src/glade_files/MainWindow.glade \
	src/glade_files/ScanningDialog.glade \
	src/glade_files/AboutDialog.glade 

EXTRAS = \
	src \
	src/resources \
	src/gtk_classes \
	src/resources/themonospot-gtk.desktop \
	copying.gpl \
	themonospot-gtk.in 

REFERENCES =  \
	System \
	System.Xml \
	$(GTK_SHARP_20_LIBS) \
	$(GLADE_SHARP_20_LIBS) \
	$(GLIB_SHARP_20_LIBS) \
	$(THEMONOSPOT_BASE_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(BINARIES) 

include $(top_srcdir)/Makefile.include

THEMONOSPOT_GTK = $(BUILD_DIR)/themonospot-gtk

$(eval $(call emit-deploy-wrapper,THEMONOSPOT_GTK,themonospot-gtk,x))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
