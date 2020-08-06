#!/bin/bash

#NOTE: This script creates a traditional versioned build of pengine meant for non-container use

if [ $# -eq 0 ]; then
    echo "Must provide version number as an argument"
    exit 1
fi

./Scripts/linux_build.sh

RELEASE=pengine-v$1
RELEASE_DIR=$RELEASE
RELEASE_ARCHIVE=$RELEASE.tgz

if [ -d $RELEASE_DIR ]
then
  rm -rf $RELEASE_DIR
fi

if [ -f $RELEASE_ARCHIVE ]
then
  rm $RELEASE_ARCHIVE
fi

mkdir Builds/$RELEASE_DIR
cd Builds
tar -C $RELEASE_DIR -xf pengine_current.tgz
tar -czvf $RELEASE_ARCHIVE $RELEASE_DIR
cd ..
