#!/bin/bash
NAME=$1

./docker-stop.sh $NAME

if docker container ls -a | grep -i "$NAME"; then
  docker rm $NAME
fi