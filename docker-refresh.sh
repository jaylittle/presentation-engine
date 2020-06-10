#!/bin/bash
NAME="$1"
TMPNAME=penginecore.base
TMPDIR="/tmp/$TMPNAME"
TAG="penginecore:latest"

echo NAME=$NAME
echo TMPDIR=$TMPDIR

if [ -d $TMPDIR ]
then
  rm -rf $TMPDIR
fi
mkdir $TMPDIR

if docker container ls -a | grep -i "$TMPNAME"; then
  docker rm $TMPNAME
fi

docker create \
  --name $TMPNAME $TAG

docker cp "$TMPNAME:/app/." "$TMPDIR/"
docker cp "$TMPDIR/." "$NAME:/app/"

docker restart $NAME