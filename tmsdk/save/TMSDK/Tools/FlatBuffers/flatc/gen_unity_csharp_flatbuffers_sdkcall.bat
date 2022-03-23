@echo off

echo "gen code start..."

"./flatc.exe" --csharp -o ../code/csharp/ ../fbs/SDKCall.fbs

echo "current dir : %~dp0"

cd ../../../UnitySDK/Assets/Scripts/protocol/

echo "current dir : %cd%"

del /S /Q "."

REM rd /S /Q "."

cd %~dp0/../

echo "current dir : %cd%"

REM notice ： path need split by "\"
xcopy .\code\csharp\ ..\..\UnitySDK\Assets\Scripts\protocol\ /y /s
   
echo "gen code end..."

pause