@echo off
::设置7z的exe路径
set zip7=D:\programs\7-Zip\7z.exe
::设置压缩包路径
set save=F:\_zippack
::设置当天日期，备份文件名
set curdate=%date:~0,4%-%date:~5,2%-%date:~8,2%
::设置要打包压缩的文件夹
set p8=F:\_Dev\projects\conf_a_eight

::过滤多个文件夹， .svn | Binaries
"%zip7%" a -tzip "%save%\%curdate%.zip" "%p8%" -mx0 -xr!.svn -xr!Binaries -xr!_Output -xr!DerivedDataCache -xr!Intermediate -xr!Saved -xr!Resources

pause