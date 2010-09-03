
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:3 -optimize- -debug "-define:DEBUG" "-keyfile:themonospot-base.snk"
ASSEMBLY = bin/Debug/themonospot-base.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../themonospot-plugin-interface/bin/Debug/themonospot-plugin-interface.dll \
	../themonospot-plugin-manager/bin/Debug/themonospot-plugin-manager.dll
BUILD_DIR = bin/Debug

THEMONOSPOT_PLUGIN_INTERFACE_DLL_SOURCE=../themonospot-plugin-interface/bin/Debug/themonospot-plugin-interface.dll
THEMONOSPOT_PLUGIN_MANAGER_DLL_SOURCE=../themonospot-plugin-manager/bin/Debug/themonospot-plugin-manager.dll
THEMONOSPOT_BASE_DLL_MDB_SOURCE=bin/Debug/themonospot-base.dll.mdb
THEMONOSPOT_BASE_DLL_MDB=$(BUILD_DIR)/themonospot-base.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:3 -optimize- "-keyfile:themonospot-base.snk"
ASSEMBLY = bin/Release/themonospot-base.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES =  \
	../themonospot-plugin-interface/bin/Release/themonospot-plugin-interface.dll \
	../themonospot-plugin-manager/bin/Release/themonospot-plugin-manager.dll
BUILD_DIR = bin/Release

THEMONOSPOT_PLUGIN_INTERFACE_DLL_SOURCE=../themonospot-plugin-interface/bin/Release/themonospot-plugin-interface.dll
THEMONOSPOT_PLUGIN_MANAGER_DLL_SOURCE=../themonospot-plugin-manager/bin/Release/themonospot-plugin-manager.dll
THEMONOSPOT_BASE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THEMONOSPOT_PLUGIN_INTERFACE_DLL) \
	$(THEMONOSPOT_PLUGIN_MANAGER_DLL) \
	$(THEMONOSPOT_BASE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(THEMONOSPOT_BASE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	properties/AssemblyInfo.cs \
	src/BaseFactory.cs \
	src/exceptions/ThemonospotBaseException.cs \
	src/ThemonospotSettings.cs \
	src/classes/SettingsManager.cs \
	src/classes/ThemonospotLogger.cs \
	src/classes/CancelScanEventHandler.cs \
	src/classes/FileInfoEntity.cs \
	src/classes/GlobalData.cs \
	src/classes/LanguageManager.cs \
	src/plugins/PluginManager.cs \
	src/plugins/ThemonospotPluginEntity.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	src/exceptions \
	src/classes \
	src/plugins \
	languages/english.xml \
	languages/italiano.xml \
	languages/spanish.xml \
	copying.gpl \
	readme \
	themonospot-base.snk \
	themonospot-base.pc.in 

REFERENCES =  \
	System \
	System.Xml

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

THEMONOSPOT_PLUGIN_INTERFACE_DLL = $(BUILD_DIR)/themonospot-plugin-interface.dll
THEMONOSPOT_PLUGIN_MANAGER_DLL = $(BUILD_DIR)/themonospot-plugin-manager.dll
THEMONOSPOT_BASE_PC = $(BUILD_DIR)/themonospot-base.pc

$(eval $(call emit-deploy-target,THEMONOSPOT_PLUGIN_INTERFACE_DLL))
$(eval $(call emit-deploy-target,THEMONOSPOT_PLUGIN_MANAGER_DLL))
$(eval $(call emit-deploy-wrapper,THEMONOSPOT_BASE_PC,themonospot-base.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
