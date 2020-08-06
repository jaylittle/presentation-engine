#!/bin/bash
NAME=$1

if docker ps | grep -i "$NAME"; then
  exit 0
else
  docker start $NAME
fi