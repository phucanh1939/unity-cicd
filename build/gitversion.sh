#!/bin/bash

# REQURED pre-install giversion: brew install gitversion
# echo `dirname $0`
args="$@"
info=$(gitversion /nofetch /output buildServer)
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
echo AAAA:  $info
# bash ./build/build-unity.sh $args
