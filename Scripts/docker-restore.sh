#!/bin/bash
NAME="$1"
PORT="$2"
BACKUPDIR="$3"

./Scripts/docker-rm.sh $NAME
./Scripts/docker-create.sh $NAME $PORT

docker cp "$BACKUPDIR/pengine.settings.json" "$NAME:/app/"
docker cp "$BACKUPDIR/wwwroot/." "$NAME:/app/wwwroot/"
docker cp "$BACKUPDIR/data/." "$NAME:/app/data/"

./Scripts/docker-refresh-app-volume.sh $NAME