@echo off
set PATH=%~dp0\svntool\bin;%PATH%
set PATH=%~dp0\svntool\sqlite-tools;%PATH%

for /f "delims=" %%a in ('svn info --show-item wc-root') do set "var=%%a"

@echo on

cd %var%

sqlite3 ./.svn/wc.db "select * from work_queue"
sqlite3 ./.svn/wc.db "delete from work_queue"	

svn cleanup

pause