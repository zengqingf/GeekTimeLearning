@echo off
set PATH=%~dp0\svntool\bin;%PATH%
set PATH=%~dp0\sqlite-tools;%PATH%

svn update ./NextGenGame/Source/ --set-depth empty --force
svn update ./NextGenGame/Source/ --set-depth infinity --force

svn update ./NextGenGame/Plugins/ --set-depth empty --force
svn update ./NextGenGame/Plugins/ --set-depth infinity --force

pause