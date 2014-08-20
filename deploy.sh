#!/bin/bash
# stty -echo

DIR=$PWD
SOURCEDIR="./src"
BINARYDIR="./build_output"
BINARYDIRENDED=$DIR"/build_output/"
DEPLOYDIR="./ReleaseBinaries"

if [ ! -d $BINARYDIR ]; then
{
	mkdir $BINARYDIR
}
else
{
	rm -rf $BINARYDIR/*
}
fi

xbuild $SOURCEDIR/AutoTestExtensions.XPlatform.sln /property:OutDir=$BINARYDIRENDED;Configuration=Release

chmod +x $BINARYDIR/*.exe
chmod +x $DIR/lib/AutoTest.Net/*.exe

if [ ! -d $DEPLOYDIR ]; then
{
	mkdir $DEPLOYDIR
}
else
{
	rm -rf $DEPLOYDIR/*
}
fi

mkdir $DEPLOYDIR/en-US
mkdir $DEPLOYDIR/Installer
mkdir $DEPLOYDIR/Icons
mkdir $DEPLOYDIR/TestRunners
mkdir $DEPLOYDIR/TestRunners/NUnit
mkdir $DEPLOYDIR/TestRunners/XUnit
mkdir $DEPLOYDIR/TestRunners/MSTest
mkdir $DEPLOYDIR/TestRunners/MSpec
mkdir $DEPLOYDIR/TestRunners/MbUnit
mkdir $DEPLOYDIR/TestRunners/SimpleTesting

cp $DIR/lib/AutoTest.Net/AutoTest.Messages.dll $DEPLOYDIR/AutoTest.Messages.dll
cp $DIR/lib/AutoTest.Net/AutoTest.Core.dll $DEPLOYDIR/AutoTest.Core.dll
cp $DIR/lib/AutoTest.Net/AutoTest.UI.dll $DEPLOYDIR/AutoTest.UI.dll
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.exe  $DEPLOYDIR/AutoTest.TestRunner.exe
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.exe.config  $DEPLOYDIR/AutoTest.TestRunner.exe.config
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.v4.0.exe  $DEPLOYDIR/AutoTest.TestRunner.v4.0.exe
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.v4.0.exe.config  $DEPLOYDIR/AutoTest.TestRunner.v4.0.exe.config
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.x86.exe  $DEPLOYDIR/AutoTest.TestRunner.x86.exe
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.x86.exe.config  $DEPLOYDIR/AutoTest.TestRunner.x86.exe.config
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.x86.v4.0.exe  $DEPLOYDIR/AutoTest.TestRunner.x86.v4.0.exe
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunner.x86.v4.0.exe.config  $DEPLOYDIR/AutoTest.TestRunner.x86.v4.0.exe.config
cp $DIR/lib/AutoTest.Net/AutoTest.TestRunners.Shared.dll  $DEPLOYDIR/AutoTest.TestRunners.Shared.dll
cp $DIR/lib/AutoTest.Net/progress.gif  $DEPLOYDIR/progress.gif
cp $DIR/lib/AutoTest.Net/progress-light.gif  $DEPLOYDIR/progress-light.gif

cp $DIR/lib/AutoTest.Net/Worst.Testing.Framework.Ever.dll $DEPLOYDIR/Worst.Testing.Framework.Ever.dll
cp $DIR/lib/AutoTest.Net/Worst.Testing.Framework.Ever.License.txt $DEPLOYDIR/Worst.Testing.Framework.Ever.License.txt
cp $DIR/lib/AutoTest.Net/NUnit.License.txt $DEPLOYDIR/NUnit.License.txt

cp $DIR/lib/AutoTest.Net/TestRunners/NUnit/AutoTest.TestRunners.NUnit.dll  $DEPLOYDIR/TestRunners/NUnit/AutoTest.TestRunners.NUnit.dll
cp $DIR/lib/AutoTest.Net/TestRunners/NUnit/nunit.core.dll  $DEPLOYDIR/TestRunners/NUnit/nunit.core.dll
cp $DIR/lib/AutoTest.Net/TestRunners/NUnit/nunit.core.interfaces.dll  $DEPLOYDIR/TestRunners/NUnit/nunit.core.interfaces.dll
cp $DIR/lib/AutoTest.Net/TestRunners/NUnit/nunit.util.dll  $DEPLOYDIR/TestRunners/NUnit/nunit.util.dll

cp $DIR/lib/AutoTest.Net/TestRunners/XUnit/AutoTest.TestRunners.XUnit.dll  $DEPLOYDIR/TestRunners/XUnit/AutoTest.TestRunners.XUnit.dll
cp $DIR/lib/AutoTest.Net/TestRunners/XUnit/xunit.runner.utility.dll  $DEPLOYDIR/TestRunners/XUnit/xunit.runner.utility.dll

cp $DIR/lib/AutoTest.Net/TestRunners/MSTest/AutoTest.TestRunners.MSTest.dll  $DEPLOYDIR/TestRunners/MSTest/AutoTest.TestRunners.MSTest.dll
cp $DIR/lib/AutoTest.Net/TestRunners/MSTest/celer.Core.dll  $DEPLOYDIR/TestRunners/MSTest/celer.Core.dll

cp $DIR/lib/AutoTest.Net/TestRunners/MSpec/AutoTest.TestRunners.MSpec.dll $DEPLOYDIR/TestRunners/MSpec/AutoTest.TestRunners.MSpec.dll

cp $DIR/lib/AutoTest.Net/TestRunners/MbUnit/AutoTest.TestRunners.MbUnit.dll $DEPLOYDIR/TestRunners/MbUnit/AutoTest.TestRunners.MbUnit.dll
cp $DIR/lib/AutoTest.Net/TestRunners/MbUnit/Gallio.dll $DEPLOYDIR/TestRunners/MbUnit/Gallio.dll
cp $DIR/lib/AutoTest.Net/TestRunners/MbUnit/Gallio.XmlSerializers.dll $DEPLOYDIR/TestRunners/MbUnit/Gallio.XmlSerializers.dll
cp $DIR/lib/AutoTest.Net/TestRunners/MbUnit/mbunit.config $DEPLOYDIR/TestRunners/MbUnit/mbunit.config
cp $DIR/lib/GoDiagrams/*.* $DEPLOYDIR/

cp $DIR/lib/AutoTest.Net/TestRunners/SimpleTesting/AutoTest.TestRunners.SimpleTesting.dll $DEPLOYDIR/TestRunners/SimpleTesting/AutoTest.TestRunners.SimpleTesting.dll
cp $DIR/lib/AutoTest.Net/TestRunners/SimpleTesting/Simple.Testing.Framework.dll $DEPLOYDIR/TestRunners/SimpleTesting/Simple.Testing.Framework.dll
cp $DIR/lib/AutoTest.Net/TestRunners/SimpleTesting/Simple.Testing.ClientFramework.dll $DEPLOYDIR/TestRunners/SimpleTesting/Simple.Testing.ClientFramework.dll
cp $DIR/lib/AutoTest.Net/TestRunners/SimpleTesting/PowerAssert.dll $DEPLOYDIR/TestRunners/SimpleTesting/PowerAssert.dll

cp $DIR/lib/AutoTest.Net/FSWatcher.dll $DEPLOYDIR/FSWatcher.dll
cp $DIR/lib/AutoTest.Net/Castle.Core.dll $DEPLOYDIR/Castle.Core.dll
cp $DIR/lib/AutoTest.Net/Castle.Facilities.Logging.dll $DEPLOYDIR/Castle.Facilities.Logging.dll
cp $DIR/lib/AutoTest.Net/Castle.license.txt $DEPLOYDIR/Castle.license.txt
cp $DIR/lib/AutoTest.Net/Castle.Windsor.dll $DEPLOYDIR/Castle.Windsor.dll
cp $DIR/lib/AutoTest.Net/Icons/* $DEPLOYDIR/Icons
cp $DIR/License.txt $DEPLOYDIR/License.txt
cp $DIR"/lib/cecil deploy/Mono.Cecil.Rocks.dll" $DEPLOYDIR/Mono.Cecil.Rocks.dll
cp $DIR/lib/MSAGL/Microsoft.Msagl.GraphViewerGDI.dll $DEPLOYDIR/Microsoft.Msagl.GraphViewerGDI.dll
cp $DIR/lib/MSAGL/Microsoft.Msagl.dll $DEPLOYDIR/Microsoft.Msagl.dll
cp $DIR/lib/MSAGL/Microsoft.Msagl.Drawing.dll $DEPLOYDIR/Microsoft.Msagl.Drawing.dll
cp $BINARYDIR/AutoTest.Minimizer.dll $DEPLOYDIR/AutoTest.Minimizer.dll
cp $BINARYDIR/ContinuousTests.exe $DEPLOYDIR/ContinuousTests.exe
cp $BINARYDIR/ContinuousTests.ExtensionModel.dll $DEPLOYDIR/ContinuousTests.ExtensionModel.dll
cp $BINARYDIR/Mono.Cecil.dll $DEPLOYDIR/Mono.Cecil.dll
cp $BINARYDIR/AutoTest.VM.exe $DEPLOYDIR/AutoTest.VM.exe
cp $BINARYDIR/AutoTest.VM.Messages.dll $DEPLOYDIR/AutoTest.VM.Messages.dll
cp $BINARYDIR/AutoTest.Client.dll $DEPLOYDIR/AutoTest.Client.dll
cp $BINARYDIR/AutoTest.Graphs.dll $DEPLOYDIR/AutoTest.Graphs.dll
cp $BINARYDIR/AutoTest.Profiler.dll $DEPLOYDIR/AutoTest.Profiler.dll
cp $DIR/lib/Rhino.Licensing/acknowledgements.txt $DEPLOYDIR/Rhino.Licensing.acknowledgements.txt
cp $DIR/lib/Rhino.Licensing/license.txt $DEPLOYDIR/Rhino.Licensing.license.txt
cp $DIR/lib/Rhino.Licensing/Rhino.Licensing.dll $DEPLOYDIR/Rhino.Licensing.dll
cp $DIR/lib/Rhino.Licensing/log4net.dll $DEPLOYDIR/log4net.dll
cp $DIR/lib/Rhino.Licensing/log4net.license.txt $DEPLOYDIR/log4net.license.txt
