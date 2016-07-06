# KeePassJsonExport
A KeePass plugin which adds the ability export and import into 
json/bson formats.

To install: build the project and copy the Newtonsoft and 
project dll into the keepass plugin directory. (Only tested in 
monodevelop on arch, may require some tinkering to work on 
windows and visualstudio.)

Currently supports exporting into a somewhat limited model, 
not retaining all the fields that keepass requires. For 
example, icons and extra strings are missing. 
