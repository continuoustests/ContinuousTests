#!/bin/bash 
# First parameter is the execution location of this script instance

if [ "$2" = "get-command-definitions" ]; then
	# Definition format usually represented as a single line:

	# Script description|
	# command1|"Command1 description"
	# 	param|"Param description" end
	# end
	# command2|"Command2 description"
	# 	param|"Param description" end
	# end

	echo "Deploys and launches the AutoTest.Net testrunner (AutoTest.TestRunner.exe)"
	exit
fi

RUNDIR=$(pwd)/src/AutoTest.TestRunner/AutoTest.TestRunners/bin/RunTests
ATDIR=$(pwd)/src/AutoTest.TestRunner/AutoTest.TestRunners/bin/AutoTest.Net

NUNIT_DIR=$(pwd)/src/AutoTest.TestRunner/Plugins/AutoTest.TestRunners.NUnit/bin/AutoTest.Net
NUNIT_TESTS_DIR=$(pwd)/src/AutoTest.TestRunner/Plugins/AutoTest.TestRunners.NUnit.Tests/bin/AutoTest.Net

if [ ! -d $RUNDIR ]; then
	mkdir $RUNDIR
fi
rm -r $RUNDIR/*
mkdir $RUNDIR/TestAssembly
mkdir $RUNDIR/TestRunners
mkdir $RUNDIR/TestRunners/NUnit

cp $ATDIR/*.* $RUNDIR
cp $NUNIT_DIR/AutoTest.TestRunners.NUnit* $RUNDIR/TestRunners/NUnit
rm $RUNDIR/TestRunners/NUnit/*.mm.dll
cp $NUNIT_DIR/nunit* $RUNDIR/TestRunners/NUnit
cp $NUNIT_TESTS_DIR/* $RUNDIR/TestAssembly

cd $RUNDIR
echo "<?xml version=\"1.0\" encoding=\"utf-8\" ?><run><runner id=\"NUnit\"><test_assembly name=\""$RUNDIR"/TestAssembly/AutoTest.TestRunners.NUnit.Tests.dll\" /></runner></run>" > $RUNDIR/input.xml

mono AutoTest.TestRunner.exe --input=input.xml --output=output.xml --logging
