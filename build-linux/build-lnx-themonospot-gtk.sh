#! /bin/bash

TARGET="Debug"
DIRNAME=`dirname $0`
PRJNAME="themonospot-gui-gtk"

# detect if there is a target specified
if [ $# -gt 0 ] ; then
  TARGET="$1"
fi

# Clean and Build
xbuild /t:Rebuild /p:Configuration=$TARGET  ../solutions/themonospot-solution-gtk.sln

# move plugins into subfolders
rm -rf $DIRNAME/../$PRJNAME/bin/$TARGET/plugins
sleep 1
mkdir -p $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-avi
mkdir -p $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-mkv
mkdir -p $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-mpeg
mv $DIRNAME/../$PRJNAME/bin/$TARGET/themonospot-plugin-avi.dll* $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-avi/
mv $DIRNAME/../$PRJNAME/bin/$TARGET/themonospot-plugin-mkv.dll* $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-mkv/
mv $DIRNAME/../$PRJNAME/bin/$TARGET/themonospot-plugin-mpeg.dll* $DIRNAME/../$PRJNAME/bin/$TARGET/plugins/themonospot-plugin-mpeg/

