

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:3 -optimize- -debug "-define:DEBUG" "-keyfile:themonospot-plugin-avi.snk"
ASSEMBLY = bin/Debug/themonospot-plugin-avi.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

THEMONOSPOT_PLUGIN_AVI_DLL_MDB_SOURCE=bin/Debug/themonospot-plugin-avi.dll.mdb
THEMONOSPOT_PLUGIN_AVI_DLL_MDB=$(BUILD_DIR)/themonospot-plugin-avi.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- "-keyfile:themonospot-plugin-avi.snk"
ASSEMBLY = bin/Release/themonospot-plugin-avi.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

THEMONOSPOT_PLUGIN_AVI_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THEMONOSPOT_PLUGIN_AVI_DLL_MDB)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) 

FILES = \
	properties/AssemblyInfo.cs \
	src/PluginFactory.cs \
	src/classes/AviManager.cs \
	src/classes/AviContainerEntity.cs \
	src/classes/AviVideoStreamEntity.cs \
	src/classes/AviAudioStreamEntity.cs \
	src/classes/Utility.cs \
	src/classes/AviConstants.cs \
	src/classes/AviMainHeaderEntity.cs \
	src/classes/AviExtHeaderEntity.cs \
	src/classes/AviStreamHeaderEntity.cs \
	src/classes/BitmapInfoHeaderEntity.cs \
	src/classes/WaveFormatExEntity.cs \
	src/classes/AviStreamEntity.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	src \
	src/classes \
	copying.gpl \
	themonospot-plugin-avi.snk 

REFERENCES =  \
	System \
	$(THEMONOSPOT_PLUGIN_INTERFACE_LIBS)

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) 

include $(top_srcdir)/Makefile.include




$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
