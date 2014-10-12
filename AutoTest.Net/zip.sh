#!/bin/bash
# stty -echo

# ::Project UppercuT - http://uppercut.googlecode.com
# ::No edits to this file are required - http://uppercut.pbwiki.com

./build.sh

mono ./lib/NAnt/NAnt.exe $1 /f:$(cd $(dirname "$0"); pwd)/build/zip.build -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config
