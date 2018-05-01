#!/bin/bash
if [ -d PEngine.Core.Web/bin/Release/netcoreapp2.0/publish ]
then
  rm -rf PEngine.Core.Web/bin/Release/netcoreapp2.0/publish
fi
dotnet restore
if [ -f pengine_current.tgz ]
then
  rm pengine_current.tgz
fi
cd PEngine.Core.Tests
dotnet clean
dotnet test
cd ../PEngine.Core.Web
rm -rf node_modules
npm install
dotnet clean
dotnet publish -c Release
cd ..
tar -C PEngine.Core.Web/bin/Release/netcoreapp2.0/publish -czvf pengine_current.tgz ./
