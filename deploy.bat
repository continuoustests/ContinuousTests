@echo off

SET DIR=%~d0%~p0%
SET SOURCEDIR="%DIR%src"
SET BINARYDIR="%DIR%build_output"
SET DEPLOYDIR="%DIR%ReleaseBinaries"

IF NOT EXIST %BINARYDIR% (
  mkdir %BINARYDIR%
) ELSE (
  del %BINARYDIR%\* /Q
)

SET MSBUILD="%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe"
IF NOT EXIST %MSBUILD% SET MSBUILD="%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe"
IF NOT EXIST %MSBUILD% SET MSBUILD="%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"

%MSBUILD% %SOURCEDIR%\AutoTest.VS.RiskClassifier\RiskClassifier.sln /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild
%MSBUILD% %SOURCEDIR%\AutoTestExtensions.sln /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

IF NOT EXIST %DEPLOYDIR% (
  mkdir %DEPLOYDIR%
) ELSE (
  del %DEPLOYDIR%\* /Q
)

IF NOT EXIST %DEPLOYDIR%\en-US (
  mkdir %DEPLOYDIR%\en-US
) ELSE (
  del %DEPLOYDIR%\en-US\* /Q
)

IF NOT EXIST %DEPLOYDIR%\Installer (
  mkdir %DEPLOYDIR%\Installer
) ELSE (
  del %DEPLOYDIR%\Installer\* /Q
)

IF NOT EXIST %DEPLOYDIR%\site (
  mkdir %DEPLOYDIR%\site
) ELSE (
  del %DEPLOYDIR%\site\* /Q
)

IF NOT EXIST %DEPLOYDIR%\site\css (
  mkdir %DEPLOYDIR%\site\css
) ELSE (
  del %DEPLOYDIR%\site\css\* /Q
)

IF NOT EXIST %DEPLOYDIR%\site\graphics (
  mkdir %DEPLOYDIR%\site\graphics
) ELSE (
  del %DEPLOYDIR%\site\graphics\* /Q
)

IF NOT EXIST %DEPLOYDIR%\site\js (
  mkdir %DEPLOYDIR%\site\js
) ELSE (
  del %DEPLOYDIR%\site\js\* /Q
)

IF NOT EXIST %DEPLOYDIR%\Icons (
  mkdir %DEPLOYDIR%\Icons
) ELSE (
  del %DEPLOYDIR%\Icons\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners (
  mkdir %DEPLOYDIR%\TestRunners
) ELSE (
  del %DEPLOYDIR%\TestRunners\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners\NUnit (
	mkdir %DEPLOYDIR%\TestRunners\NUnit
) ELSE (
	del %DEPLOYDIR%\TestRunners\NUnit\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners\XUnit (
	mkdir %DEPLOYDIR%\TestRunners\XUnit
) ELSE (
	del %DEPLOYDIR%\TestRunners\XUnit\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners\MSTest (
	mkdir %DEPLOYDIR%\TestRunners\MSTest
) ELSE (
	del %DEPLOYDIR%\TestRunners\MSTest\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners\MSpec (
	mkdir %DEPLOYDIR%\TestRunners\MSpec
) ELSE (
	del %DEPLOYDIR%\TestRunners\MSpec\* /Q
)

IF NOT EXIST %DEPLOYDIR%\TestRunners\MbUnit (
	mkdir %DEPLOYDIR%\TestRunners\MbUnit
) ELSE (
	del %DEPLOYDIR%\TestRunners\MbUnit\* /Q
)
IF NOT EXIST %DEPLOYDIR%\TestRunners\SimpleTesting (
	mkdir %DEPLOYDIR%\TestRunners\SimpleTesting
) ELSE (
	del %DEPLOYDIR%\TestRunners\SimpleTesting\* /Q
)



echo --------------------------------------------------------------------------------------------
echo                     Copying AutoTest.Net folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.Messages.dll ^
AutoTest.Core.dll ^
AutoTest.UI.dll ^
AutoTest.TestRunner.exe ^
AutoTest.TestRunner.exe.config ^
AutoTest.TestRunner.v4.0.exe ^
AutoTest.TestRunner.v4.0.exe ^
AutoTest.TestRunner.v4.0.exe.config ^
AutoTest.TestRunner.x86.exe ^
AutoTest.TestRunner.x86.exe.config ^
AutoTest.TestRunner.x86.v4.0.exe ^
AutoTest.TestRunner.x86.v4.0.exe.config ^
AutoTest.TestRunners.Shared.dll ^
progress.gif ^
progress-light.gif ^
Worst.Testing.Framework.Ever.dll ^
Worst.Testing.Framework.Ever.License.txt ^
NUnit.License.txt ^
FSWatcher.dll ^
Castle.Core.dll ^
Castle.Facilities.Logging.dll ^
Castle.license.txt ^
Castle.Windsor.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\%%f" %DEPLOYDIR%\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\NUnit folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.NUnit.dll ^
nunit.core.dll ^
nunit.core.interfaces.dll ^
nunit.util.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\NUnit\%%f"  %DEPLOYDIR%\TestRunners\NUnit\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\XUnit folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.XUnit.dll ^
xunit.runner.utility.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\XUnit\%%f"  %DEPLOYDIR%\TestRunners\XUnit\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\MSTest folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.MSTest.dll ^
celer.Core.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\MSTest\%%f"  %DEPLOYDIR%\TestRunners\MSTest\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\MSpec folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.MSpec.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\MSpec\%%f" %DEPLOYDIR%\TestRunners\MSpec\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\MbUnit folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.MbUnit.dll ^
Gallio.dll ^
Gallio.XmlSerializers.dll ^
mbunit.config ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\MbUnit\%%f" %DEPLOYDIR%\TestRunners\MbUnit\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\TestRunners\SimpleTesting folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
AutoTest.TestRunners.SimpleTesting.dll ^
Simple.Testing.Framework.dll ^
Simple.Testing.ClientFramework.dll ^
PowerAssert.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\AutoTest.Net\TestRunners\SimpleTesting\%%f" %DEPLOYDIR%\TestRunners\SimpleTesting\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying cecil deploy folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
Mono.Cecil.pdb.dll ^
Mono.Cecil.mdb.dll ^
Mono.Cecil.Rocks.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%lib\cecil deploy\%%f" %DEPLOYDIR%\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying binaries folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
Microsoft.msagl.GraphViewerGDI.dll ^
Microsoft.msagl.dll ^
Microsoft.msagl.drawing.dll ^
AutoTest.Minimizer.dll ^
ContinuousTests.exe ^
ContinuousTests.ExtensionModel.dll ^
BellyRub.dll ^
Nancy.dll ^
Nancy.Hosting.Self.dll ^
Newtonsoft.Json.dll ^
websocket-sharp.dll ^
Mono.Cecil.dll ^
AutoTest.VM.exe ^
AutoTest.VM.Messages.dll ^
AutoTest.Client.dll ^
AutoTest.Graphs.dll ^
AutoTest.VS.dll ^
AutoTest.VS.Util.dll ^
AutoTest.VS.RiskClassifier.dll ^
AutoTest.VS.2008.Addin ^
AutoTest.VS.2010.Addin ^
AutoTest.VS.2012.Addin ^
AutoTest.VS.2013.Addin ^
AutoTest.Profiler.dll ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy %BINARYDIR%\%%f %DEPLOYDIR%\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying site folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
index.html ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy %BINARYDIR%\site\%%f %DEPLOYDIR%\site\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying site\css folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
main.css ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy %BINARYDIR%\site\css\%%f %DEPLOYDIR%\site\css\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying site\graphics folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
circleAbort.png ^
circleFAIL.png ^
circleWAIL.png ^
circleWIN.png ^
progress.gif ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy %BINARYDIR%\site\graphics\%%f %DEPLOYDIR%\site\graphics\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying site\js folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
autotest-client.js ^
handlebars.js ^
jquery.min.js ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy %BINARYDIR%\site\js\%%f %DEPLOYDIR%\site\js\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying Rhino.Licensing folder files
echo --------------------------------------------------------------------------------------------
SET file_list=( ^
acknowledgements.txt ^
license.txt ^
Rhino.Licensing.dll ^
log4net.dll ^
log4net.license.txt ^
)

FOR %%f in %file_list% do (
  echo %%f
  copy "%DIR%\lib\Rhino.Licensing\%%f" %DEPLOYDIR%\%%f
)

echo --------------------------------------------------------------------------------------------
echo                 Copying GoDiagrams folder files
echo --------------------------------------------------------------------------------------------
copy "%DIR%lib\GoDiagrams"\*.* %DEPLOYDIR%\

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.Net\Icons folder files
echo --------------------------------------------------------------------------------------------
copy "%DIR%lib\AutoTest.Net\Icons\"* %DEPLOYDIR%\Icons

echo --------------------------------------------------------------------------------------------
echo                 Copying License.txt
echo --------------------------------------------------------------------------------------------
copy "%DIR%License.txt" %DEPLOYDIR%\License.txt

echo --------------------------------------------------------------------------------------------
echo                 Copying AutoTest.VS.RiskClassifier\RiskClassifier folder files
echo --------------------------------------------------------------------------------------------
copy "%DIR%src\AutoTest.VS.RiskClassifier\RiskClassifier\source.extension.vsixmanifest" %DEPLOYDIR%\extension.vsixmanifest

echo --------------------------------------------------------------------------------------------
echo                 Building resource file
echo --------------------------------------------------------------------------------------------
cd %SOURCEDIR%\AutoTest.VS
IF EXIST "C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" (
     echo Using x64 Program Files Resgen.exe
	"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" Graphics.resx
	"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Al.exe" /embed:Graphics.resources /culture:en-US /out:%DEPLOYDIR%\en-US\AutoTest.VS.resources.dll
) ELSE (
	echo Using x86 Program Files Resgen.exe
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" Graphics.resx
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\Al.exe" /embed:Graphics.resources /culture:en-US /out:%DEPLOYDIR%\en-US\AutoTest.VS.resources.dll
)
cd %DIR%

IF EXIST ldeploy.bat (
  ldeploy.bat
)
