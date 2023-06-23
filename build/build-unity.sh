#!/bin/bash

pushd "`dirname $0`/.."
SOLUTION_DIR="$PWD"

BUILD_VERSION=$GitVersion_MajorMinorPatch
VERSION_CODE=$GitVersion_Major$GitVersion_Minor$GitVersion_CommitsSinceVersionSourcePadded

echo ---------------------------------------------------------------------
echo Retrieving some GitVersion environment variables:
echo ---------------------------------------------------------------------
echo - GitVersion_SemVer:              $GitVersion_SemVer
echo - GitVersion_BranchName:          $GitVersion_BranchName
echo - GitVersion_AssemblySemVer:      $GitVersion_AssemblySemVer
echo - GitV-outersion_MajorMinorPatch: $GitVersion_MajorMinorPatch
echo - GitVersion_Sha:                 $GitVersion_Sha
echo BUILD_VERSION:                    $BUILD_VERSION
echo VERSION_CODE:                     $VERSION_CODE
echo ---------------------------------------------------------------------

echo "currentPath = $SOLUTION_DIR"

# Access command line arguments
echo "Argument 1: $1"
echo "Argument 2: $2"
echo "Argument 3: $3"

# Get the BUILD_TARGET from 1st command line argument (if it not empty
# [-n $1] - Check if the 1st command line argument empty or not
BUILD_TARGET="Android"
if [ -n $1 ]; then
    BUILD_TARGET=$1
fi
echo "BUILD_TARGET = $BUILD_TARGET"

# Specify some directory and file paths.
LOG_DIR="$SOLUTION_DIR/logs"
LOG_FILE="$LOG_DIR/Unity-$BUILD_TARGET.log"
OUT_DIR="$SOLUTION_DIR/bin"
PROJECT_PATH="$SOLUTION_DIR/client"

echo "SOLUTION_DIR = $SOLUTION_DIR"
echo "LOG_DIR = $LOG_DIR"
echo "OUT_DIR = $OUT_DIR"
echo "PROJECT_PATH = $PROJECT_PATH"
echo "LOG_FILE = $LOG_FILE"


# Adjust BUILD_OUTPUT path for Android
# For IOS: Build Xcode project to ./bin/ios/
# For Android: Build apk file to ./bin/GameClient.apk
BUILD_OUTPUT="$OUT_DIR/$BUILD_TARGET"
if [ "$BUILD_TARGET" = "Android" ]; then
    BUILD_OUTPUT="$BUILD_OUTPUT/CCGameClient.apk"
fi
echo "buildOutput = $BUILD_OUTPUT"

# Create bin folder to store build out put
if [ -d "$OUT_DIR" ]; then
    echo "Has folder $OUT_DIR"
else
    mkdir $OUT_DIR
fi

# Create or clean log folder
if [ -d "$LOG_DIR" ]; then
    echo "Has folder $LOG_DIR"
    rm -f $LOG_DIR#/*
else
    mkdir $LOG_DIR
fi

# Define Unity params
if [ -n $UNITY ]; then
    UNITY="/Applications/Unity/Hub/Editor/2021.3.24f1/Unity.app/Contents/MacOS/Unity"
fi
UNITY_PARAM="-batchmode -nographics -quit -silent-crashes -force-free -executeMethod Nordeus.Build.CommandLineBuild.Build -reporter Unity -logFile"
UNITY_PARAM_INPUT="-projectPath $PROJECT_PATH -buildVersion $BUILD_VERSION -buildNumber $VERSION_CODE"
echo "$UNITY $UNITY_PARAM $UNITY_PARAM_INPUT -buildTarget $BUILD_TARGET -out $BUILD_OUTPUT"

# Remove temp folder
if [ -e "$SOLUTION_DIR/client/Temp" ]; then
    rm -rf $SOLUTION_DIR/client/Temp
fi

# Remove all file in client/Library
# >/dev/null: redirects output of rm command to this file - which is special file that discard all data writen to it
# This is done to suppress any output or error messages that `rm` command might produce
if [ -e "$SOLUTION_DIR/client/Library" ]; then
    rm -f $SOLUTION_DIR/client/Library/* > /dev/null
fi

SUCCESS=0

# Redirect stdout ( > ) into a named pipe ( >() ) running "tee"
exec > >(tee -i "$LOG_FILE")

# Redirects the standard error (file descriptor 2) to the same location as the standard output (file descriptor 1).
# This means that both the standard output and standard error will be combined and displayed in the same location, such as the terminal or a log file.
# Without this, only stdout would be captured - i.e. your log file would not contain any error messages.
exec 2>&1

# Run Unity build
buildReport=$($UNITY $UNITY_PARAM $UNITY_PARAM_INPUT -buildTarget $BUILD_TARGET -out $BUILD_OUTPUT)

echo "$buildReport"

popd