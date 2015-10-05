[![](http://www.integrazioneweb.com/themonospot/images/themonospot_48.png)](http://www.integrazioneweb.com/themonospot/)

# Use language files #

From release 0.6.4 Themonospot support language files. Are xml files containing rows of follow type:
> 

&lt;KEY&gt;

Value

&lt;/KEY&gt;


their names are equal to _(language\_name)_.**xml** and you can find them:
  * **On Linux**: in the folder /usr/share/themonospot/languages
  * **On Windows**: in the folder _(application\_path)\languages_
Themonospot select automatically language file to use by user language configured. If there isn't a language file named as user language configured, select english.xml language file as default.

#  #
# Modify language file #
To modify an existent language file you need only write/update access at _**languages**_ folder and can do it with a text editor (gedit, kwrite, kate, notepad, wordpad, etc.).

#  #
# Add new language file #
To add a language file for your user language configured you need:
  1. write/update access at _**languages**_ folder
  1. know your user language exact name (_English_ is different from _english_) on your system

To detect fastly your user language name you can lunch Themonospot from a shell in _DEBUG MODE_:
```
mono --debug (application_path)/themonospot.exe
```
so you can read the shell message that report this information:
```
System language = english; Available language = english
```

If you have a _(system language).ls_ file in your Themonospot _languages_ folder, the application already select it as language file.

Otherwise you can create a new text file with name _(system language).xml_ in your Themonospot _languages_ folder.
After copy and paste _KEYS NAME_ in this file from other language file and complete it with _KEYS VALUE_ in your language.