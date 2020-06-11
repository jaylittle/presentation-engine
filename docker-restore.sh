#!/bin/bash
NAME="$1"
PORT="$2"
BACKUPDIR="$3"

./docker-rm.sh $NAME
./docker-create.sh $NAME $PORT

docker cp "$BACKUPDIR/pengine.settings.json" "$NAME:/app/"
docker cp "$BACKUPDIR/wwwroot/." "$NAME:/app/wwwroot/"
docker cp "$BACKUPDIR/data/." "$NAME:/app/data/"

./docker-refresh-app-volume.sh $NAME