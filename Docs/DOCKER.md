# Docker Integration

Presentation Engine now officially supports the use of Docker for hosting.  The base distribution includes several scripts designed to ease the process of creating, managing, updating, backing up and restoring docker containers.

## Initial PEngine Docker Container Setup

If you already know Docker, then this will be very easy and you likely don't need any of these scripts.  If you want to deploy this application with docker then you need to follow these steps:

1. Install Docker

2. Open a shell

3. git clone https://github.com/jaylittle/presentation-engine.git

4. cd presentation-engine

5. ./Scripts/docker-build.sh

6. ./Scripts/docker-create.sh penginecore.sample 5002
   
    Note: feel free to substitute whatever container name you want for the first parameter "penginecore.sample" and whatever port name you want for the second parameter "5002".

7. ./Scripts/docker-start.sh penginecore.sample

8. Open a browser and go to http://localhost:5002

## Updating PEngine Docker Containers with the latest Presentation Engine release

Per docker standard operation, containers are disposable and should be deleted and recreated in order to update them.  Using our scripts to create docker containers will also create a volume associated with each container for the /app directory within the container.  Due to the design of this application that volume will contain a mixture of user generated and system generated content.  So with that in mind, we have created a simple script to assist in the process of destroying a container, recreating the container and updating the system files in the volume.

1. Open a shell

2. Navigate to the presentation-engine git directory

3. git fetch && git pull --ff-only

4. ./Scripts/docker-build.sh

5. ./Scripts/docker-recreate.sh penginecore.sample 5002

    Note: feel free to substitute whatever container name you want for the first parameter "penginecore.sample" and whatever port name you want for the second parameter "5002".

## Backing up and restoring data from PEngine Docker Containers

Two scripts have been provided to facilitate this activity.  

1. ./Scripts/docker-backup.sh penginecore.sample /home/pengine/backups/sample

    Note: feel free to substitute whatever container name you want for the first parameter "penginecore.sample" and whatever directory you want to backup the data to for the third parameter.

2. ./Scripts/docker-restore.sh penginecore.sample 5002 /home/pengine/backups/sample

    Note: feel free to substitute whatever container name you want for the first parameter "penginecore.sample" and whatever port name you want for the second parameter "5002" and whatever directory you want to restore data from for the third parameter.

    It's also worth noting that if you want to transition from a traditional install to a container install of PEngine, the restore script is the one you'll want to use to facilitate that.

## Final Notes

It's worth noting that there are some other scripts, but their purpose ought to be fairly self explanatory especially if you go over the other scripts we've already mentioned here.  In addition all of these scripts are designed to function in a Linux bash environment, just like our previous build scripts.  In theory with a proper docker installation on Windows along with git bash these scripts could perhaps work for you in Windows, but I personally have no intention of testing that as support of proprietary platforms isn't something I care that much about.