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

%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %SOURCEDIR%\AutoTest.VS.RiskClassifier\RiskClassifier.sln /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe %SOURCEDIR%\AutoTestExtensions.sln /property:OutDir=%BINARYDIR%\;Configuration=Release /target:rebuild

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
copy %DIR%lib\AutoTest.Net\AutoTest.Messages.dll %DEPLOYDIR%\AutoTest.Messages.dll
copy %DIR%lib\AutoTest.Net\AutoTest.Core.dll %DEPLOYDIR%\AutoTest.Core.dll
copy %DIR%lib\AutoTest.Net\AutoTest.UI.dll %DEPLOYDIR%\AutoTest.UI.dll
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.exe  %DEPLOYDIR%\AutoTest.TestRunner.exe
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.exe.config  %DEPLOYDIR%\AutoTest.TestRunner.exe.config
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.v4.0.exe  %DEPLOYDIR%\AutoTest.TestRunner.v4.0.exe
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.v4.0.exe.config  %DEPLOYDIR%\AutoTest.TestRunner.v4.0.exe.config
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.x86.exe  %DEPLOYDIR%\AutoTest.TestRunner.x86.exe
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.x86.exe.config  %DEPLOYDIR%\AutoTest.TestRunner.x86.exe.config
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.x86.v4.0.exe  %DEPLOYDIR%\AutoTest.TestRunner.x86.v4.0.exe
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunner.x86.v4.0.exe.config  %DEPLOYDIR%\AutoTest.TestRunner.x86.v4.0.exe.config
copy %DIR%lib\AutoTest.Net\AutoTest.TestRunners.Shared.dll  %DEPLOYDIR%\AutoTest.TestRunners.Shared.dll
copy %DIR%lib\AutoTest.Net\progress.gif  %DEPLOYDIR%\progress.gif
copy %DIR%lib\AutoTest.Net\progress-light.gif  %DEPLOYDIR%\progress-light.gif

copy %DIR%lib\AutoTest.Net\Worst.Testing.Framework.Ever.dll %DEPLOYDIR%\Worst.Testing.Framework.Ever.dll
copy %DIR%lib\AutoTest.Net\Worst.Testing.Framework.Ever.License.txt %DEPLOYDIR%\Worst.Testing.Framework.Ever.License.txt
copy %DIR%lib\AutoTest.Net\NUnit.License.txt %DEPLOYDIR%\NUnit.License.txt

copy %DIR%lib\AutoTest.Net\TestRunners\NUnit\AutoTest.TestRunners.NUnit.dll  %DEPLOYDIR%\TestRunners\NUnit\AutoTest.TestRunners.NUnit.dll
copy %DIR%lib\AutoTest.Net\TestRunners\NUnit\nunit.core.dll  %DEPLOYDIR%\TestRunners\NUnit\nunit.core.dll
copy %DIR%lib\AutoTest.Net\TestRunners\NUnit\nunit.core.interfaces.dll  %DEPLOYDIR%\TestRunners\NUnit\nunit.core.interfaces.dll
copy %DIR%lib\AutoTest.Net\TestRunners\NUnit\nunit.util.dll  %DEPLOYDIR%\TestRunners\NUnit\nunit.util.dll

copy %DIR%lib\AutoTest.Net\TestRunners\XUnit\AutoTest.TestRunners.XUnit.dll  %DEPLOYDIR%\TestRunners\XUnit\AutoTest.TestRunners.XUnit.dll
copy %DIR%lib\AutoTest.Net\TestRunners\XUnit\xunit.runner.utility.dll  %DEPLOYDIR%\TestRunners\XUnit\xunit.runner.utility.dll

copy %DIR%lib\AutoTest.Net\TestRunners\MSTest\AutoTest.TestRunners.MSTest.dll  %DEPLOYDIR%\TestRunners\MSTest\AutoTest.TestRunners.MSTest.dll
copy %DIR%lib\AutoTest.Net\TestRunners\MSTest\celer.Core.dll  %DEPLOYDIR%\TestRunners\MSTest\celer.Core.dll

copy %DIR%lib\AutoTest.Net\TestRunners\MSpec\AutoTest.TestRunners.MSpec.dll %DEPLOYDIR%\TestRunners\MSpec\AutoTest.TestRunners.MSpec.dll

copy %DIR%lib\AutoTest.Net\TestRunners\MbUnit\AutoTest.TestRunners.MbUnit.dll %DEPLOYDIR%\TestRunners\MbUnit\AutoTest.TestRunners.MbUnit.dll
copy %DIR%lib\AutoTest.Net\TestRunners\MbUnit\Gallio.dll %DEPLOYDIR%\TestRunners\MbUnit\Gallio.dll
copy %DIR%lib\AutoTest.Net\TestRunners\MbUnit\Gallio.XmlSerializers.dll %DEPLOYDIR%\TestRunners\MbUnit\Gallio.XmlSerializers.dll
copy %DIR%lib\AutoTest.Net\TestRunners\MbUnit\mbunit.config %DEPLOYDIR%\TestRunners\MbUnit\mbunit.config

copy %DIR%lib\AutoTest.Net\TestRunners\SimpleTesting\AutoTest.TestRunners.SimpleTesting.dll %DEPLOYDIR%\TestRunners\SimpleTesting\AutoTest.TestRunners.SimpleTesting.dll
copy %DIR%lib\AutoTest.Net\TestRunners\SimpleTesting\Simple.Testing.Framework.dll %DEPLOYDIR%\TestRunners\SimpleTesting\Simple.Testing.Framework.dll
copy %DIR%lib\AutoTest.Net\TestRunners\SimpleTesting\Simple.Testing.ClientFramework.dll %DEPLOYDIR%\TestRunners\SimpleTesting\Simple.Testing.ClientFramework.dll
copy %DIR%lib\AutoTest.Net\TestRunners\SimpleTesting\PowerAssert.dll %DEPLOYDIR%\TestRunners\SimpleTesting\PowerAssert.dll


copy %DIR%lib\GoDiagrams\*.* %DEPLOYDIR%\

copy "%DIR%lib\cecil deploy\Mono.Cecil.pdb.dll" %DEPLOYDIR%\Mono.Cecil.pdb.dll
copy "%DIR%lib\cecil deploy\Mono.Cecil.mdb.dll" %DEPLOYDIR%\Mono.Cecil.mdb.dll

copy %DIR%lib\AutoTest.Net\FSWatcher.dll %DEPLOYDIR%\FSWatcher.dll
copy %DIR%lib\AutoTest.Net\Castle.Core.dll %DEPLOYDIR%\Castle.Core.dll
copy %DIR%lib\AutoTest.Net\Castle.Facilities.Logging.dll %DEPLOYDIR%\Castle.Facilities.Logging.dll
copy %DIR%lib\AutoTest.Net\Castle.license.txt %DEPLOYDIR%\Castle.license.txt
copy %DIR%lib\AutoTest.Net\Castle.Windsor.dll %DEPLOYDIR%\Castle.Windsor.dll
copy %DIR%lib\AutoTest.Net\Icons\* %DEPLOYDIR%\Icons
copy %DIR%License.txt %DEPLOYDIR%\License.txt
copy "%DIR%lib\cecil deploy\Mono.Cecil.Rocks.dll" %DEPLOYDIR%\Mono.Cecil.Rocks.dll
copy %BINARYDIR%\Microsoft.msagl.GraphViewerGDI.dll %DEPLOYDIR%\Microsoft.msagl.GraphViewerGDI.dll
copy %BINARYDIR%\Microsoft.msagl.dll %DEPLOYDIR%\Microsoft.msagl.dll
copy %BINARYDIR%\Microsoft.msagl.drawing.dll %DEPLOYDIR%\Microsoft.msagl.drawing.dll
copy %BINARYDIR%\AutoTest.Minimizer.dll %DEPLOYDIR%\AutoTest.Minimizer.dll
copy %BINARYDIR%\ContinuousTests.exe %DEPLOYDIR%\ContinuousTests.exe
copy %BINARYDIR%\ContinuousTests.ExtensionModel.dll %DEPLOYDIR%\ContinuousTests.ExtensionModel.dll
copy %BINARYDIR%\Mono.Cecil.dll %DEPLOYDIR%\Mono.Cecil.dll
copy %BINARYDIR%\AutoTest.VM.exe %DEPLOYDIR%\AutoTest.VM.exe
copy %BINARYDIR%\AutoTest.VM.Messages.dll %DEPLOYDIR%\AutoTest.VM.Messages.dll
copy %BINARYDIR%\AutoTest.Client.dll %DEPLOYDIR%\AutoTest.Client.dll
copy %BINARYDIR%\AutoTest.Graphs.dll %DEPLOYDIR%\AutoTest.Graphs.dll
copy %BINARYDIR%\AutoTest.VS.dll %DEPLOYDIR%\AutoTest.VS.dll
copy %BINARYDIR%\AutoTest.VS.Util.dll %DEPLOYDIR%\AutoTest.VS.Util.dll
copy %BINARYDIR%\AutoTest.VS.RiskClassifier.dll %DEPLOYDIR%\AutoTest.VS.RiskClassifier.dll
copy %BINARYDIR%\AutoTest.VS.2008.Addin %DEPLOYDIR%\AutoTest.VS.2008.Addin
copy %BINARYDIR%\AutoTest.VS.2010.Addin %DEPLOYDIR%\AutoTest.VS.2010.Addin
copy %BINARYDIR%\AutoTest.VS.2012.Addin %DEPLOYDIR%\AutoTest.VS.2012.Addin
copy %BINARYDIR%\AutoTest.VS.2013.Addin %DEPLOYDIR%\AutoTest.VS.2013.Addin
copy %DIR%src\AutoTest.VS.RiskClassifier\RiskClassifier\source.extension.vsixmanifest %DEPLOYDIR%\extension.vsixmanifest
copy %BINARYDIR%\AutoTest.Profiler.dll %DEPLOYDIR%\AutoTest.Profiler.dll
copy %DIR%\lib\Rhino.Licensing\acknowledgements.txt %DEPLOYDIR%\Rhino.Licensing.acknowledgements.txt
copy %DIR%\lib\Rhino.Licensing\license.txt %DEPLOYDIR%\Rhino.Licensing.license.txt
copy %DIR%\lib\Rhino.Licensing\Rhino.Licensing.dll %DEPLOYDIR%\Rhino.Licensing.dll
copy %DIR%\lib\Rhino.Licensing\log4net.dll %DEPLOYDIR%\log4net.dll
copy %DIR%\lib\Rhino.Licensing\log4net.license.txt %DEPLOYDIR%\log4net.license.txt
cd %SOURCEDIR%\AutoTest.VS

IF EXIST "C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" (
	"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" Graphics.resx
	"C:\Program Files\Microsoft SDKs\Windows\v7.0A\bin\Al.exe" /embed:Graphics.resources /culture:en-US /out:"%DEPLOYDIR%\en-US\AutoTest.VS.resources.dll"
) ELSE (
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\Resgen.exe" Graphics.resx
	"C:\Program Files (x86)\Microsoft SDKs\Windows\v7.0A\bin\Al.exe" /embed:Graphics.resources /culture:en-US /out:"%DEPLOYDIR%\en-US\AutoTest.VS.resources.dll"
)
cd %DIR%

IF EXIST ldeploy.bat (
  ldeploy.bat
)
