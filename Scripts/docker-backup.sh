#!/bin/bash
NAME="$1"
BACKUPDIR="$2"

if [ -d $BACKUPDIR ]
then
  rm -rf $BACKUPDIR
fi
mkdir $BACKUPDIR

docker cp "$NAME:/app/." "$BACKUPDIR"