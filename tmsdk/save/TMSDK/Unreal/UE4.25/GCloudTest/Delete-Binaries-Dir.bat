@echo off
REM set /p folder=Binaries
set folder=Binaries
set rootPath=F:\_Dev\tmsdk\save\TMSDK\Unreal\UE4.25\GCloudTest
for /f "delims=" %%a in ('dir /ad /b /s %rootPath%') do (
    echo %%a | find "%folder%" >nul && (rd /s /q "%%~a") 
)
pause
