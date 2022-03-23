@echo off

call pyinstaller -h > nul 2>&1
if NOT %errorlevel%==0 (
    goto PYINSTALLER_ERR
)
call virtualenv -h > nul 2>&1
if NOT %errorlevel%==0 (
    goto PYINSTALLER_ERR
)
if NOT EXIST venv (
    goto MAKE_VENV
)

:MAIN
call venv\Scripts\activate.bat

call pyinstaller -y -F main.py --name gamerecord -i icon.ico --distpath prepack
if EXIST prepack\bin (
    rmdir /S /Q prepack\bin
)
if EXIST prepack\airtest (
    rmdir /S /Q prepack\airtest
)
echo D | xcopy bin prepack\bin /Y /Q
echo D | xcopy venv\Lib\site-packages\airtest prepack\airtest /S /Y /Q
@REM 拷贝修复了小米miui11无法截图的minicap.so
@REM echo D | xcopy fix\airtest prepack\airtest /S /Y /Q
@REM 拷贝设备配置
echo F | xcopy devices_setting.json prepack\devices_setting.json /Y /Q
call venv\Scripts\deactivate.bat

goto EXIT

:MAKE_VENV
call virtualenv venv
call venv\Scripts\activate.bat
call pip3 install -r requirements.txt
call py airtestmod.py venv\Lib\site-packages
call venv\Scripts\deactivate.bat

goto MAIN


:PYINSTALLER_ERR
echo pyinstaller not installed
goto EXIT
:VIRTUALENV_ERR
echo virtualenv not installed
goto EXIT
:EXIT
