#!/bin/bash

#NOTE: This script is only meant to be run as part of a docker container build process
#      For non-containerized builds, run linux_build.sh

BASE=pengine-core
CONFIG=Release
PLATFORM=netcoreapp8.0
PUBDIR="/app"

echo Publishing from $PUBDIR to $FILENAME

#Clean up leftovers from previous builds
if [ -d $PUBDIR ]
then
  rm -rf $PUBDIR
fi

#Run Unit Tests (if applicable)
cd PEngine.Core.Tests
dotnet clean -c $CONFIG
dotnet test
if [ $? -ne 0 ]; then
  exit 1
fi

#Clean the build output
cd ../PEngine.Core.Web
dotnet clean -c $CONFIG
if [ $? -ne 0 ]; then
  exit 1
fi

#Publish Release Build
dotnet publish -c $CONFIG -o $PUBDIR --no-restore
if [ $? -ne 0 ]; then
  exit 1
fi

cd ..

#Create version.test file
cp ./docker-version.txt "${PUBDIR}/version.txt"
