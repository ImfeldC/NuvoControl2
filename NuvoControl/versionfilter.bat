@echo off
rem svn stat -v %1% | sed -n 's/^[ A-Z?\*|!]\{1,15\}/r/;s/ \{1,15\}/\/r/;s/ .*//p'
svn stat -v %1% 
