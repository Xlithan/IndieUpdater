Simple updater which downloads a zip file from your webserver and extracts it to the applications directory.

All you need to do is create a .txt file with the revision number inside. The updater checks the Config.ini file in the Data Files folder for the current revision number you have,
then compares it against the number on the web server. If it doesn't match, it will download a file called Patch#.zip, where the # = the revision number on the server.

Once the file is downloaded and extracted, it will update the revision number inside the Config.ini.

Sorry for the lack of comments, it was a spur of the moment decision to share this code.
