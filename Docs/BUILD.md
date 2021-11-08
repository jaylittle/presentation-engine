# How to Build

If you choose to utilize this project for a more traditional build and deployment process instead of making use of docker, then this document is for you.

## Requirements

1. Git
2. DotNet SDK 6.0
3. NodeJS and NPM
4. Yarn installed via NPM globally

## Instructions

This is pretty simple:

1. Open a shell

2. git clone https://github.com/jaylittle/presentation-engine.git

3. cd presentation-engine

4. ./Scripts/linux_build.sh

This will produce a tar file called pengine_current.tgz as it's output.  This is a fully deployable version of the application and can be deployed on top of an existing installation as it will not overwrite any database files or additional files that users have uploaded to the website.

In addition, if you want to deploy a versioned build (which is what I do when uploading a new release to github, you'll want to replace step 4 with the following command):

4. ./Scripts/linux_release.sh 5.1.100

The first parameter is the version number of the release you are building.  The release script calls linux_build.sh and produces both a pengine_current.tgz file as well as a pengine-v5.1.100.tgz file.

## Docker Caveats

If you want to use docker at all, you should probably [read this document instead](DOCKER.md) as these instructions will not produce anything that can be directly utilized via docker.  These instructions are meant only to facilitate a traditional build and deployment process.  Docker is an entirely different beast and should be treated as such.