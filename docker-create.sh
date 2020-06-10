#!/bin/bash
NAME=$1
PORT=$2
VOLUME="$1.app"
TAG=penginecore:latest

docker create \
  -p "$PORT:80" \
  --mount "source=$VOLUME,destination=/app" \
  --restart unless-stopped \
  --name $NAME $TAG