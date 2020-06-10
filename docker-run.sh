#!/bin/bash
NAME=$1
VOLUME=$2
PORT=$3
TAG=penginecore:latest

docker run \
  -d \
  -p "$PORT:80" --mount "source=$VOLUME,destination=/app" --name $NAME $TAG