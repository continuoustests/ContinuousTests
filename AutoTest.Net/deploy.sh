#!/bin/bash
# stty -echo

BINARYDIR="./build_outputAnyCPU/AutoTest.NET"
BINARYDIRx86="./build_outputx86/AutoTest.TestRunner"
DEPLOYDIR="./ReleaseBinaries"
CASTLEDIR="./lib/Castle.Windsor"
GALLIODIR="./lib/Gallio"
MSPEC4DIR="./lib/MSpec"
VSADDINDIR="./addins/VisualStudio/FilesToDeploy"
RESOURCES="./src/Resources"

if [ ! -d $DEPLOYDIR ]; then
{
	mkdir $DEPLOYDIR
}
else
{
	rm -rf $DEPLOYDIR/*
}
fi

mkdir $DEPLOYDIR/Icons
mkdir $DEPLOYDIR/TestRunners
mkdir $DEPLOYDIR/TestRunners/NUnit
mkdir $DEPLOYDIR/TestRunners/XUnit
mkdir $DEPLOYDIR/TestRunners/MSTest
mkdir $DEPLOYDIR/TestRunners/MSpec
mkdir $DEPLOYDIR/TestRunners/MbUnit

cp $BINARYDIR/AutoTest.Messages.dll $DEPLOYDIR/AutoTest.Messages.dll
cp $BINARYDIR/AutoTest.Core.dll $DEPLOYDIR/AutoTest.Core.dll
cp $BINARYDIR/AutoTest.UI.dll $DEPLOYDIR/AutoTest.UI.dll
cp $BINARYDIR/AutoTest.config.template $DEPLOYDIR/AutoTest.config
sed -i "s/C:\\\Windows\\\Microsoft.NET\\\Framework\\\v4.0.30319\\\MSBuild.exe/\/usr\/bin\/xbuild/g" $DEPLOYDIR/AutoTest.config
cp ./README $DEPLOYDIR/README
cp ./LICENSE $DEPLOYDIR/AutoTest.License.txt

cp $BINARYDIR/AutoTest.TestRunner.exe $DEPLOYDIR/AutoTest.TestRunner.exe
cp $BINARYDIR/App_3.5.config $DEPLOYDIR/AutoTest.TestRunner.exe.config
cp $BINARYDIR/AutoTest.TestRunner.exe $DEPLOYDIR/AutoTest.TestRunner.v4.0.exe
cp $BINARYDIR/AutoTest.TestRunner.exe.config $DEPLOYDIR/AutoTest.TestRunner.v4.0.exe.config
cp $BINARYDIRx86/AutoTest.TestRunner.exe $DEPLOYDIR/AutoTest.TestRunner.x86.exe
cp $BINARYDIR/App_3.5.config $DEPLOYDIR/AutoTest.TestRunner.x86.exe.config
cp $BINARYDIRx86/AutoTest.TestRunner.exe $DEPLOYDIR/AutoTest.TestRunner.x86.v4.0.exe
cp $BINARYDIRx86/AutoTest.TestRunner.exe.config $DEPLOYDIR/AutoTest.TestRunner.x86.v4.0.exe.config
cp $BINARYDIR/AutoTest.TestRunners.Shared.dll $DEPLOYDIR/AutoTest.TestRunners.Shared.dll

cp $BINARYDIR/AutoTest.TestRunners.NUnit.dll $DEPLOYDIR/TestRunners/NUnit/AutoTest.TestRunners.NUnit.dll
cp $BINARYDIR/nunit.core.dll $DEPLOYDIR/TestRunners/NUnit/nunit.core.dll
cp $BINARYDIR/nunit.core.interfaces.dll $DEPLOYDIR/TestRunners/NUnit/nunit.core.interfaces.dll
cp $BINARYDIR/nunit.util.dll $DEPLOYDIR/TestRunners/NUnit/nunit.util.dll

cp $BINARYDIR/AutoTest.TestRunners.XUnit.dll $DEPLOYDIR/TestRunners/XUnit/AutoTest.TestRunners.XUnit.dll
cp $BINARYDIR/xunit.runner.utility.dll $DEPLOYDIR/TestRunners/XUnit/xunit.runner.utility.dll

cp $BINARYDIR/AutoTest.TestRunners.XUnit2.dll $DEPLOYDIR/TestRunners/XUnit2/AutoTest.TestRunners.XUnit2.dll
cp $BINARYDIR/xunit.runner.utility.desktop.dll $DEPLOYDIR/TestRunners/XUnit2/xunit.runner.utility.desktop.dll
	
cp $BINARYDIR/AutoTest.TestRunners.MSTest.dll $DEPLOYDIR/TestRunners/MSTest/AutoTest.TestRunners.MSTest.dll
cp $BINARYDIR/celer.Core.dll $DEPLOYDIR/TestRunners/MSTest/celer.Core.dll

cp $BINARYDIR/AutoTest.TestRunners.MSpec.dll $DEPLOYDIR/TestRunners/MSpec/AutoTest.TestRunners.MSpec.dll

cp $BINARYDIR/AutoTest.TestRunners.MbUnit.dll $DEPLOYDIR/TestRunners/MbUnit/AutoTest.TestRunners.MbUnit.dll
cp $BINARYDIR/Gallio.dll $DEPLOYDIR/TestRunners/MbUnit/Gallio.dll
cp $GALLIODIR/Gallio.XmlSerializers.dll $DEPLOYDIR/TestRunners/MbUnit/Gallio.XmlSerializers.dll
cp $BINARYDIR/mbunit.config $DEPLOYDIR/TestRunners/MbUnit/mbunit.config

cp $BINARYDIR/Worst.Testing.Framework.Ever.dll $DEPLOYDIR/Worst.Testing.Framework.Ever.dll
cp ./src/AutoTest.TestRunner/Plugins/Microsoft.VisualStudio.QualityTools.UnitTestFramework/Worst.Testing.Framework.Ever.License.txt $DEPLOYDIR/Worst.Testing.Framework.Ever.License.txt
cp ./src/AutoTest.TestRunner/Plugins/Microsoft.VisualStudio.QualityTools.UnitTestFramework/NUnit.License.txt $DEPLOYDIR/NUnit.License.txt

cp $BINARYDIR/FSWatcher.dll $DEPLOYDIR/FSWatcher.dll
cp $BINARYDIR/Castle.Core.dll $DEPLOYDIR/Castle.Core.dll
cp $BINARYDIR/Castle.Facilities.Logging.dll $DEPLOYDIR/Castle.Facilities.Logging.dll
cp $CASTLEDIR/Castle.license.txt $DEPLOYDIR/Castle.license.txt
cp $BINARYDIR/Castle.Windsor.dll $DEPLOYDIR/Castle.Windsor.dll
cp $BINARYDIR/Mono.Cecil.dll $DEPLOYDIR/Mono.Cecil.dll

cp ./Resources/progress.gif $DEPLOYDIR/progress.gif

cp ./$RESOURCES/* $DEPLOYDIR/Icons
