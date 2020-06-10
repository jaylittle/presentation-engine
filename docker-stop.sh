#!/bin/bash
NAME=$1

if docker ps | grep -i "$NAME"; then
  docker stop $NAME
fi