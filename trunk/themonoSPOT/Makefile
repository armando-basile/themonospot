CC=gmcs
SOURCES=monoSPOT.cs \
        AssemblyInfo.cs \
        monoSPOTwait.cs \
        ./settings/configurationClass.cs \
	./components/cEncoding.cs \
	./components/cAviStructures.cs \
	./components/cMonoSpotParser.cs 
ASSEMBLIES= 	-pkg:gtk-sharp-2.0 \
                -pkg:glade-sharp-2.0 \
                -pkg:art-sharp-2.0 \
                -pkg:glib-sharp-2.0
RESOURCES=  	-resource:./monoSPOT.glade,monoSPOT.glade \
		-resource:./images/monoSPOT.png,monoSPOT.png \
		-resource:./waitWindow.glade,waitWindow.glade \
		-resource:./images/video.png,video.png \
		-resource:./images/video_mini.png,video_mini.png \
		-resource:./images/textfile.png,textfile.png \
		-resource:./images/sound.png,sound.png
TARGET=themonoSPOT.exe
OPTIONS= -target:winexe 
LIBDIR=-lib:/usr/lib/mono/2.0 
DLLUSED=/r:System.Windows.Forms.dll \
	/r:System.Runtime.Serialization.Formatters.Soap.dll



clean:
	rm -f ./bin/Makefile_Release/*.exe ./bin/Makefile_Release/*.dll

all:
	$(CC) $(OPTIONS) -out:./bin/Makefile_Release/$(TARGET) $(ASSEMBLIES) $(SOURCES) $(RESOURCES) $(LIBDIR) $(DLLUSED) 

deb:
	$(CC) -target:exe -out:./bin/Makefile_Release/$(TARGET) $(ASSEMBLIES) $(SOURCES) $(RESOURCES) $(LIBDIR) $(DLLUSED) 

run:
	mono --debug ./bin/Makefile_Release/$(TARGET)
