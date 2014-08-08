#!/bin/bash 

# Script parameters
#	Param 1: Script run location
#	Param 2: global profile name
#	Param 3: local profile name
#	Param 4-: Any passed argument
#
# When calling oi use the --profile=PROFILE_NAME and 
# --global-profile=PROFILE_NAME argument to ensure calling scripts
# with the right profile.
#
# To post back oi commands print command prefixed by command| to standard output
# To post a comment print to std output prefixed by comment|
# To post an error print to std output prefixed by error|

if [ "$2" = "get-command-definitions" ]; then
	# Definition format usually represented as a single line:

	# Script description|
	# command1|"Command1 description"
	# 	param|"Param description" end
	# end
	# command2|"Command2 description"
	# 	param|"Param description" end
	# end

	echo "Update AutoTest.Net from at release binaries"
	exit
fi

AT="../AutoTest.Net/ReleaseBinaries"
ATLib="lib/AutoTest.Net"

if [ ! -d $ATLib ]; then
{
	mkdir $ATLib
	mkdir $ATLib/TestRunners
	mkdir $ATLib/Icons
}
fi


cp -R $AT/TestRunners/* $ATLib/TestRunners
cp -R $AT/Icons/* $ATLib/Icons
cp $AT/AutoTest.Core.dll $ATLib
cp $AT/AutoTest.License.txt $ATLib
cp $AT/AutoTest.Messages.dll $ATLib
cp $AT/AutoTest.TestRunner.exe $ATLib
cp $AT/AutoTest.TestRunner.exe.config $ATLib
cp $AT/AutoTest.TestRunners.Shared.dll $ATLib
cp $AT/AutoTest.TestRunner.v4.0.exe $ATLib
cp $AT/AutoTest.TestRunner.v4.0.exe.config $ATLib
cp $AT/AutoTest.TestRunner.x86.exe $ATLib
cp $AT/AutoTest.TestRunner.x86.exe.config $ATLib
cp $AT/AutoTest.TestRunner.x86.v4.0.exe $ATLib
cp $AT/AutoTest.TestRunner.x86.v4.0.exe.config $ATLib
cp $AT/AutoTest.UI.dll $ATLib
#cp $AT/AutoTest.VS.Util.dll $ATLib
cp $AT/Castle.Core.dll $ATLib
cp $AT/Castle.Facilities.Logging.dll $ATLib
cp $AT/Castle.license.txt $ATLib
cp $AT/Castle.Windsor.dll $ATLib
cp $AT/FSWatcher.dll $ATLib
cp $AT/Mono.Cecil.dll $ATLib
cp $AT/NUnit.License.txt $ATLib
cp $AT/progress.gif $ATLib
#cp $AT/progress-light.gif $ATLib
cp $AT/Worst.Testing.Framework.Ever.dll $ATLib
cp $AT/Worst.Testing.Framework.Ever.License.txt $ATLib