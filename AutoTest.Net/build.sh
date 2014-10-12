#!/bin/bash
# stty -echo

# ::Project UppercuT - http://uppercut.googlecode.com
# ::No edits to this file are required - http://uppercut.pbwiki.com

function usage
{
	echo ""
	echo "Usage: build.sh"
	exit
}

function displayUsage
{
	case $1 in
		"/?"|"-?"|"?"|"/help") usage ;;
	esac
}

displayUsage $1

mono ./lib/NAnt/NAnt.exe $1 /f:$(cd $(dirname "$0"); pwd)/build/default.build -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuTx86.config
mono ./lib/NAnt/NAnt.exe $1 /f:$(cd $(dirname "$0"); pwd)/build/default.build -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config