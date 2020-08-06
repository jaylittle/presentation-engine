#!/bin/bash
NAME=$1
PORT=$2
TAG=penginecore:latest

./Scripts/docker-rm.sh $NAME
./Scripts/docker-create.sh $NAME $PORT
./Scripts/docker-refresh-app-volume.sh $NAME
./Scripts/docker-start.sh $NAME