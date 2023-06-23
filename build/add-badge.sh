#!/usr/bin/env bash

pushd "`dirname $0`"

bundle install

# REQURED pre-install giversion: brew install gitversion
# Append 'beta:true' to generate beta release icon
gitversion /nofetch /output buildServer /exec bundle /execArgs "exec fastlane icon $*"

popd