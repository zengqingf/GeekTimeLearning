@echo off

echo "gen code start..."

"./flatc.exe" --java -o ../code/java/ ../fbs/SDKCall.fbs

echo "current dir : %~dp0"

cd ../../../AndroidSDK/flatbuffers/src/main/java/

echo "current dir : %cd%"

del /S /Q "."

REM rd /S /Q "."

cd %~dp0/../

echo "current dir : %cd%"

REM notice ： path need split by "\"
xcopy .\code\java\ ..\..\AndroidSDK\flatbuffers\src\main\java\ /y /s   

echo "gen code end..."

pause