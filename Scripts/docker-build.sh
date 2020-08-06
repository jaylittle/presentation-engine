#!/bin/bash
TAG=penginecore
BASE=pengine-core
TIMESTAMP=`date +%Y%m%d%H%M`
VERSION="${BASE}-${TIMESTAMP}-docker-latest"

if [ -d .git ]
then
  CONFIG=Release
  BRANCH=`git rev-parse --abbrev-ref HEAD | tr '/' '-'`
  COMMIT=`git log --pretty=format:'%h' -n 1`
  VERSION="${BASE}-${TIMESTAMP}-${BRANCH}-${COMMIT}-${CONFIG}-docker"
fi

echo $VERSION > docker-version.txt

docker build -t $TAG .

rm docker-version.txt