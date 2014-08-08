#!/bin/bash
# stty -echo

# ::Project UppercuT - http://uppercut.googlecode.com
# ::No edits to this file are required - http://uppercut.pbwiki.com

mono ./lib/NAnt/NAnt.exe $1 /f:$(cd $(dirname "$0"); pwd)/build/open.build -D:build.config.settings=$(cd $(dirname "$0"); pwd)/Settings/UppercuT.config
