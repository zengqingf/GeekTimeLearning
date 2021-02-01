REM adb version > 1.0.32   otherwise use adb -a -P 5037 fork-server server

adb nodaemon server -a -P 5037

pause