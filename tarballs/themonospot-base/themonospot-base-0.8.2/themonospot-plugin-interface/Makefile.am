
EXTRA_DIST =  

# Warning: This is an automatically generated file, do not edit!

if ENABLE_DEBUG
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- -debug "-define:DEBUG" "-keyfile:themonospot-plugin-interface.snk"
ASSEMBLY = bin/Debug/themonospot-plugin-interface.dll
ASSEMBLY_MDB = $(ASSEMBLY).mdb
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Debug

THEMONOSPOT_PLUGIN_INTERFACE_DLL_MDB_SOURCE=bin/Debug/themonospot-plugin-interface.dll.mdb
THEMONOSPOT_PLUGIN_INTERFACE_DLL_MDB=$(BUILD_DIR)/themonospot-plugin-interface.dll.mdb

endif

if ENABLE_RELEASE
ASSEMBLY_COMPILER_COMMAND = gmcs
ASSEMBLY_COMPILER_FLAGS =  -noconfig -codepage:utf8 -warn:4 -optimize- "-keyfile:themonospot-plugin-interface.snk"
ASSEMBLY = bin/Release/themonospot-plugin-interface.dll
ASSEMBLY_MDB = 
COMPILE_TARGET = library
PROJECT_REFERENCES = 
BUILD_DIR = bin/Release

THEMONOSPOT_PLUGIN_INTERFACE_DLL_MDB=

endif

AL=al2
SATELLITE_ASSEMBLY_NAME=$(notdir $(basename $(ASSEMBLY))).resources.dll

PROGRAMFILES = \
	$(THEMONOSPOT_PLUGIN_INTERFACE_DLL_MDB)  

LINUX_PKGCONFIG = \
	$(THEMONOSPOT_PLUGIN_INTERFACE_PC)  


RESGEN=resgen2
	
all: $(ASSEMBLY) $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

FILES = \
	ThemonospotPlugin.cs \
	properties/AssemblyInfo.cs 

DATA_FILES = 

RESOURCES = 

EXTRAS = \
	properties \
	themonospot-plugin-interface.snk \
	themonospot-plugin-interface.pc.in 

REFERENCES =  \
	System

DLL_REFERENCES = 

CLEANFILES = $(PROGRAMFILES) $(LINUX_PKGCONFIG) 

include $(top_srcdir)/Makefile.include

THEMONOSPOT_PLUGIN_INTERFACE_PC = $(BUILD_DIR)/themonospot-plugin-interface.pc

$(eval $(call emit-deploy-wrapper,THEMONOSPOT_PLUGIN_INTERFACE_PC,themonospot-plugin-interface.pc))


$(eval $(call emit_resgen_targets))
$(build_xamlg_list): %.xaml.g.cs: %.xaml
	xamlg '$<'

$(ASSEMBLY_MDB): $(ASSEMBLY)

$(ASSEMBLY): $(build_sources) $(build_resources) $(build_datafiles) $(DLL_REFERENCES) $(PROJECT_REFERENCES) $(build_xamlg_list) $(build_satellite_assembly_list)
	mkdir -p $(shell dirname $(ASSEMBLY))
	$(ASSEMBLY_COMPILER_COMMAND) $(ASSEMBLY_COMPILER_FLAGS) -out:$(ASSEMBLY) -target:$(COMPILE_TARGET) $(build_sources_embed) $(build_resources_embed) $(build_references_ref)
