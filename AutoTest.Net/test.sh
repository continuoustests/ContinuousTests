#!/bin/bash
# stty -echo

# ::Project UppercuT - http://uppercut.googlecode.com
# ::No edits to this file are required - http://uppercut.pbwiki.com

function usage
{
	echo ""
	echo "Usage: test.sh"
	echo "Usage: test.sh all - to run all tests"
	exit
}

function displayUsage
{
	case $1 in
		"/?"|"-?"|"?"|"/help") usage ;;
	esac
}

displayUsage $1

mono ./lib/NAnt/NAnt.exe /f:$(cd $(dirname "$0"); pwd)/build/compile.step -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config

mono ./lib/NAnt/NAnt.exe /f:$(cd $(dirname "$0"); pwd)/build/analyzers/test.step $1 -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config
mono ./lib/NAnt/NAnt.exe /f:$(cd $(dirname "$0"); pwd)/build/analyzers/test.step open_results -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config
