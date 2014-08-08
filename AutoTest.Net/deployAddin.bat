@ECHO OFF

SET DIR=%~d0%~p0%
SET BIDDIR="%DIR%build_output"
SET DEPDIR="%DIR%..\lib\AutoTest.Net"
SET VSADDINDIR="%DIR%addins\VisualStudio"

copy %BIDDIR%\AutoTest.VS.Util.dll %DEPDIR%\AutoTest.VS.Util.dll
copy %BIDDIR%\VSMenuKiller.exe %DEPDIR%\VSMenuKiller.exe
