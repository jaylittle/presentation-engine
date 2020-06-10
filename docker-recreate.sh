#!/bin/bash
NAME=$1
PORT=$2
TAG=penginecore:latest

./docker-rm.sh $NAME
./docker-create.sh $NAME $PORT
./docker-refresh-app-volume.sh $NAME
./docker-start.sh $NAME