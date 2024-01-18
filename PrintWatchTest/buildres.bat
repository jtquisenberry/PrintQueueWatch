echo Creating .resources file
RESGEN.exe PrintWatchTest.txt PrintWatchTest.resources
echo Linking resources file
al /out:PrintWatchTest.Resources.dll /embedresource:PrintWatchTest.resources,PrintWatchTest.resources,Private /keyfile:resources.snk
echo Done
