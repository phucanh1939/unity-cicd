#!/usr/bin/env bash

pushd "`dirname $0`"

bundle install

echo "+-------------------------------------------------------------------+"
echo "|   Build fastlane environment                                      |"
echo "+-------------------------------------------------------------------+"
set

echo "+-------------------------------------------------------------------+"
echo "|   Ruby Gem environment                                            |"
echo "+-------------------------------------------------------------------+"
echo " - System: `which ruby`"
echo " - Homebrew:"
brew info ruby
gem env

echo "+-------------------------------------------------------------------+"
echo "|   OpenSSL version                                                 |"
echo "+-------------------------------------------------------------------+"
echo " - System: `which openssl`"
echo " - Homebrew:"
brew info openssl

openssl version -a

bundle exec fastlane build $*

popd