@echo off

::Project UppercuT - http://uppercut.googlecode.com
::No edits to this file are required - http://uppercut.pbwiki.com

if '%2' NEQ '' goto usage
if '%3' NEQ '' goto usage
if '%1' == '/?' goto usage
if '%1' == '-?' goto usage
if '%1' == '?' goto usage
if '%1' == '/help' goto usage

SET DIR=%~d0%~p0%
SET SOURCEDIR=%DIR%
SET BINARYDIR="%DIR%build_outputAnyCPU"
SET BINARYDIRx86="%DIR%build_outputx86"
SET DEPLOYDIR="%DIR%ReleaseBinaries"


%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %SOURCEDIR%\AutoTest.NET.sln /property:OutDir=%BINARYDIR%\AutoTest.NET;Configuration=Release /target:rebuild
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %SOURCEDIR%\AutoTest.TestRunner.sln /property:OutDir=%BINARYDIRx86%\AutoTest.TestRunner;Configuration=Release /target:rebuild /p:Platform=x86

if %ERRORLEVEL% NEQ 0 goto errors

goto finish

:usage
echo.
echo Usage: build.bat
echo.
goto finish

:errors
EXIT /B %ERRORLEVEL%

:finish