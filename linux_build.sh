#!/bin/bash
if [ -d PEngine.Core.Web/bin/Release/netcoreapp2.1/publish ]
then
  rm -rf PEngine.Core.Web/bin/Release/netcoreapp2.1/publish
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
if [ -d wwwroot/dist ]
then
  rm -rf wwwroot/dist
  mkdir wwwroot/dist
fi
npm install
dotnet clean
NODE_ENV=production dotnet publish -c Release
cd ..
tar -C PEngine.Core.Web/bin/Release/netcoreapp2.1/publish -czvf pengine_current.tgz ./
