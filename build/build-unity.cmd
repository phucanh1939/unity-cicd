@echo off

rem SET ANDROID_HOME=W:\Android\android-sdk 
rem SET ANDROID_NDK_HOME=W:\Android\android-ndk

echo Current environment variables:
set

pushd "%~dp0.."

SET BUILD_VERSION=%GitVersion_MajorMinorPatch%
SET VERSION_CODE=%GitVersion_Major%%GitVersion_Minor%%GitVersion_CommitsSinceVersionSourcePadded%

echo ---------------------------------------------------------------------
echo Retrieving some GitVersion environment variables:
echo ---------------------------------------------------------------------
echo - GitVersion_SemVer:          %GitVersion_SemVer%
echo - GitVersion_BranchName:      %GitVersion_BranchName%
echo - GitVersion_AssemblySemVer:  %GitVersion_AssemblySemVer%
echo - GitVersion_MajorMinorPatch: %GitVersion_MajorMinorPatch%
echo - GitVersion_Sha:             %GitVersion_Sha%
echo BUILD_VERSION:                %BUILD_VERSION%
echo VERSION_CODE:                 %VERSION_CODE%
echo ---------------------------------------------------------------------

SET BUILD_TARGET=Android
if NOT "%~1"=="" SET BUILD_TARGET=%~1

SET OUT_DIR=%CD%\bin
SET LOG_DIR=%CD%\logs
SET LOG_FILE="%LOG_DIR%\Unity-%BUILD_TARGET%.log"

SET BUILD_OUTPUT=%OUT_DIR%\%BUILD_TARGET%
if "%BUILD_TARGET%"=="Android" SET BUILD_OUTPUT=%BUILD_OUTPUT%\GameClient.apk

if NOT exist "%LOG_DIR%" md "%LOG_DIR%"
if NOT EXIST "%OUT_DIR%" md "%OUT_DIR%"

SET UNITY_BUILD_CMD=tools\Unity3D\UnityProxy.exe "%ProgramFiles%\Unity\Editor\unity.exe" -batchmode -nographics -quit -silent-crashes -force-free -executeMethod Nordeus.Build.CommandLineBuild.Build -reporter TeamCity -projectPath %CD%\src\GameClient -buildVersion %BUILD_VERSION% -buildNumber %VERSION_CODE%

if exist src\GameClient\Temp del /F /Q /S src\GameClient\Temp
if exist src\GameClient\Library del /F /Q src\GameClient\Library\*

%UNITY_BUILD_CMD% -buildTarget %BUILD_TARGET% -out "%BUILD_OUTPUT%" -cleanedLogFile "%LOG_FILE%"

SET SUCCESS=%ERRORLEVEL%

popd

if "%SUCCESS%"=="0" (
  echo ---------------------------------------------------------------------
  echo Build Successful!!!
  echo ---------------------------------------------------------------------
) else (
  echo ---------------------------------------------------------------------
  echo Build failed with the following errors:
  echo ---------------------------------------------------------------------
  type "%LOG_FILE%"
  echo ---------------------------------------------------------------------
  echo Build FAILED with code: %SUCCESS%!!!
  echo ---------------------------------------------------------------------
  rem Will exit with status of last command.
  exit /b -1
)
